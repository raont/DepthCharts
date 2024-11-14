using Application.Command;
using Application.Exceptions;
using Application.Mappings;
using AutoMapper;
using Core.Entities;
using FluentAssertions;
using Infrastructure.Service;
using Moq;

namespace Application.UnitTests.Commands;

[TestClass]
public class PlayerDepthHandlerTests
{
    private Mock<IDbHandler> _dbHandlerMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        _dbHandlerMock = new Mock<IDbHandler>();

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new EntityMappingProfile()));
        _mapper = new Mapper(configuration);

        _dbHandlerMock.Setup(s => s.GameExists(It.IsAny<int>())).ReturnsAsync(true);
        _dbHandlerMock.Setup(s => s.PlayerExistsInGame(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(true);
        _dbHandlerMock.Setup(s => s.PlayerExistsInPosition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(false);
    }

    [TestMethod]
    public void Handle_WhenInvalidGameProvided_MustThrowError()
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test" };
        _dbHandlerMock.Setup(s => s.GameExists(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        var call = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        call.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found, Please setup the game first then try again");
    }

    [TestMethod]
    public void Handle_WhenPlayerNotRegistered_MustThrowError()
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test" };
        _dbHandlerMock.Setup(s => s.PlayerExistsInGame(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        var call = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        call.Should().ThrowAsync<NotFoundException>().WithMessage("Player not found, Please add player to the game and then try again");
    }

    [TestMethod]
    public void Handle_WhenPlayerAlreadyExistsInAPosition_MustThrowError()
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test" };
        _dbHandlerMock.Setup(s => s.PlayerExistsInPosition(It.IsAny<int>(), It.IsAny<int>(), "Test")).ReturnsAsync(true);

        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        var call = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        call.Should().ThrowAsync<NotFoundException>().WithMessage("Player already present in the Depth Lane, Please remove first then insert again");
    }

    [TestMethod]
    public async Task Handle_WhenLowestPositionDoesntExist_NewPositionNotSpecified_MustInsertPlayerAtFirstPosition()
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test" };

        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.AddPlayerPosition(It.Is<Position>(s => s.Position1 == 0)));
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(6)]
    [DataRow(7)]
    public async Task Handle_WhenLowestPositionDoesntExist_NewPositionSpecified_MustInsertPlayerAtSpecifiedPosition(int newPosition)
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test", Position = newPosition };

        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.AddPlayerPosition(It.Is<Position>(s => s.Position1 == newPosition)));
    }

    [DataTestMethod]
    [DataRow(0, 1)]
    [DataRow(1, 2)]
    [DataRow(10, 11)]
    public async Task Handle_WhenLowestPositionExists_NewPositionNotSpecified_MustInsertPlayerByIncrementingLowestPositionByone(int lowestPosition, int newPosition)
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test" };
        _dbHandlerMock.Setup(s => s.GetLowestDepth(It.IsAny<int>(), "Test")).ReturnsAsync(lowestPosition);

        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.AddPlayerPosition(It.Is<Position>(s => s.Position1 == newPosition)));
    }

    [DataTestMethod]
    [DataRow(0, 1)]
    [DataRow(1, 2)]
    [DataRow(10, 20)]
    public async Task Handle_WhenNewPositionGreaterThenLowestPosition_InsertPlayerAtSpecifiedPosition(int lowestPosition, int newPosition)
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test", Position = newPosition };
        _dbHandlerMock.Setup(s => s.GetLowestDepth(It.IsAny<int>(), "Test")).ReturnsAsync(lowestPosition);

        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.AddPlayerPosition(It.Is<Position>(s => s.Position1 == newPosition)));
    }

    [DataTestMethod]
    [DataRow(1, 0)]
    [DataRow(10, 2)]
    [DataRow(7, 5)]
    public async Task Handle_WhenNewPositionLowerThenLowestPosition_ButNotReplacingPosition_InsertPlayerAtSpecifiedPosition(int lowestPosition, int newPosition)
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test", Position = newPosition };
        _dbHandlerMock.Setup(s => s.GetLowestDepth(It.IsAny<int>(), "Test")).ReturnsAsync(lowestPosition);
        _dbHandlerMock.Setup(s => s.PlayerExistsAtPosition(It.IsAny<int>(), It.IsAny<int>(), "Test")).ReturnsAsync(false);


        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.AddPlayerPosition(It.Is<Position>(s => s.Position1 == newPosition)));
    }

    [DataTestMethod]
    [DataRow(1, 0)]
    [DataRow(10, 2)]
    [DataRow(7, 5)]
    public async Task Handle_WhenNewPositionLowerThenLowestPosition_MustInsertByShiftingPositionsDown(int lowestPosition, int newPosition)
    {
        // Arrange
        var request = new PlayerPositionRequest { DepthChart = "Test", Position = newPosition };
        _dbHandlerMock.Setup(s => s.GetLowestDepth(It.IsAny<int>(), "Test")).ReturnsAsync(lowestPosition);
        _dbHandlerMock.Setup(s => s.PlayerExistsAtPosition(It.IsAny<int>(), It.IsAny<int>(), "Test")).ReturnsAsync(true);


        // Act
        var sut = new PlayerDepthHandler(_dbHandlerMock.Object, _mapper);
        await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.ReplacePlayerPosition(It.Is<Position>(s => s.Position1 == newPosition)));
    }
}