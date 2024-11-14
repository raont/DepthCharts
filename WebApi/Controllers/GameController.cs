using Application.Command;
using Application.Models;
using AutoMapper;
using Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace WebApi.Controllers
{
    /// <summary>
    /// Handles Games related funtionalities 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public GameController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Creates a new game to manage the player depth chart
        /// </summary>
        /// <param name="name">Unique name of the game</param>
        /// <returns>Returns game Id after successful creation</returns>
        [HttpPost("create-game/{name}")]
        public async Task<ActionResult<int>> CreateGame(string name)
        {
            return Ok(await _mediator.Send(new CreateGameHandlerRequest { Name = name }));
        }

        /// <summary>
        /// Add a new play to exisiting game
        /// </summary>
        /// <param name="gameId">Game to which the player needs to be added</param>
        /// <param name="player">Player details</param>
        [HttpPost("{gameId}/add-new-player")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task AddNewPlayer(int gameId, [FromBody] PlayerDto player)
        {
            var newPlayer = _mapper.Map<AddPlayerRequest>(player);
            newPlayer.GameId = gameId;
            await _mediator.Send(newPlayer);
            Ok();
        }

        /// <summary>
        /// Gets full depth chart for a game.
        /// Specify name of the game not Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("FullDepthChart/{Game}")]
        public async Task<ContentResult> GetFullDepthChart(string Game)
        {
            var depthRequest = new GetDepthChartRequest() { gameName = Game.ToUpper() };
            
            var depthDetails = await _mediator.Send(depthRequest);

            StringBuilder outPut = new StringBuilder();

            foreach (var positionDepth in depthDetails.Positions)
            {
                outPut.Append($"{positionDepth.PositionName} - ");
                foreach (var player in positionDepth.PlayerList)
                {
                    outPut.Append($"(#{player.Value.PlayerId}, {player.Value.FirstName}{player.Value.LastName}), ");
                }
                outPut.Append("\r\n");
            }
            
            return base.Content(outPut.ToString());
        }
    }
}
