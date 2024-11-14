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
public class GetFullDepthChartHandlerTests
{
    private Mock<IDbHandler> _dbHandlerMock;
    private IMapper _mapper;

    [TestInitialize]
    public void Setup()
    {
        _dbHandlerMock = new Mock<IDbHandler>();

        var configuration = new MapperConfiguration(cfg => cfg.AddProfile(new EntityMappingProfile()));
        _mapper = new Mapper(configuration);

        _dbHandlerMock.Setup(s => s.GetGameId(It.IsAny<string>())).ReturnsAsync(1);
    }

    [TestMethod]
    public void Handle_WhenInvalidGameNameProvided_MustThrowError()
    {
        // Arrange
        var request = new AddPlayerRequest { PlayerId = 1, GameId = 2 };
        _dbHandlerMock.Setup(s => s.GetGameId(It.IsAny<string>())).ReturnsAsync((int?)null);

        // Act
        var sut = new AddPlayerHandler(_dbHandlerMock.Object, _mapper);
        var call = async () => await sut.Handle(request, CancellationToken.None);

        // Assert
        call.Should().ThrowAsync<NotFoundException>().WithMessage("Game not found");
    }

    [TestMethod]
    public async Task Handle_WhenMulitplePlayersPresentForAPositionDepth_MustPickDistinctPostions()
    {
        // Arrange
        var request = new GetDepthChartRequest { gameName ="test" };
        var dbResult = new PlayerDepthChart();
        dbResult.Positions = [new Position { Depth = "XB" }, new Position { Depth = "YB" }, new Position { Depth = "XB" } ];
        _dbHandlerMock.Setup(s => s.GetPositionChartDetails(It.IsAny<int>())).ReturnsAsync(dbResult);

        // Act
        var sut = new GetFullDepthChartHandler(_dbHandlerMock.Object, _mapper);
        var result = await sut.Handle(request, CancellationToken.None);

        // Assert
        result.Positions.Should().HaveCount(2);
        result.Positions[0].PositionName.Should().Be("XB");
        result.Positions[1].PositionName.Should().Be("YB");
    }
}
