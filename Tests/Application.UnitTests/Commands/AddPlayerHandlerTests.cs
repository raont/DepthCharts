using Application.Command;
using Application.Exceptions;
using Application.Mappings;
using AutoMapper;
using FluentAssertions;
using Infrastructure.Service;
using Moq;

namespace Application.UnitTests.Commands;

[TestClass]
public class AddPlayerHandlerTests
{
    private Mock<IDbHandler> _dbHandlerMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        _dbHandlerMock = new Mock<IDbHandler>();

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new EntityMappingProfile()));
        _mapper = new Mapper(configuration);

    }

    [TestMethod]
    public void Handle_WhenInvalidGameProvided_MustThrowError()
    {
        // Arrange
        var request = new AddPlayerRequest { PlayerId = 1, GameId = 2 };
        _dbHandlerMock.Setup(s => s.GameExists(It.IsAny<int>())).ReturnsAsync(false);

        // Act
        var sut = new AddPlayerHandler(_dbHandlerMock.Object, _mapper);
        var call = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        call.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found, Please setup the game first then try again");
    }
}
