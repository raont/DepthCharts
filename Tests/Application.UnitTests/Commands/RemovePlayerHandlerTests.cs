using Application.Command;
using Application.Exceptions;
using Application.Mappings;
using AutoMapper;
using FluentAssertions;
using Infrastructure.Service;
using Moq;

namespace Application.UnitTests.Commands;

[TestClass]
public class RemovePlayerHandlerTests
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
    }

    [TestMethod]
    public void Handle_WhenInvalidGameProvided_MustThrowError()
    {
        // Arrange
        var request = new RemovePlayerRequest { Game = 2 };
        _dbHandlerMock.Setup(s => s.GameExists(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var sut = new RemovePlayerHandler(_dbHandlerMock.Object, _mapper);
        var call = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        call.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found, Please setup the game first then try again");
    }

    [TestMethod]
    public async Task Handle_WhenDeleteFails_MustReturnNull()
    {
        // Arrange
        var request = new RemovePlayerRequest { Game = 2 };
        
        // Act
        var sut = new RemovePlayerHandler(_dbHandlerMock.Object, _mapper);
        var playerId = await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.DeletePlayerPosition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
        , times: Times.Once);
        _dbHandlerMock.Verify(s => s.GetPlayer(It.IsAny<int>(), It.IsAny<int>())
        , times: Times.Never);

        playerId.Should().BeNull();
    }

    [TestMethod]
    public async Task Handle_WhenDeleteSuccessButPlayerFetchFails_MustReturnNull()
    {
        // Arrange
        var request = new RemovePlayerRequest { Game = 2 };
        _dbHandlerMock.Setup(s => s.DeletePlayerPosition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(2);
       

        // Act
        var sut = new RemovePlayerHandler(_dbHandlerMock.Object, _mapper);
        var playerId = await sut.Handle(request, CancellationToken.None);

        // Assert
        _dbHandlerMock.Verify(s => s.DeletePlayerPosition(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())
        , times: Times.Once);
        _dbHandlerMock.Verify(s => s.GetPlayer(It.IsAny<int>(), It.IsAny<int>())
        , times: Times.Once);
        playerId.Should().BeNull();
    }
}
