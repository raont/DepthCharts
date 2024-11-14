using Application.Command;
using Application.Models;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Controllers
{
    /// <summary>
    /// Handles Player Depth related funtionalities for a game.
    /// </summary>
    [ApiController]
    [Route("[controller]/{GameId}")]
    public class PlayerDepthsController : ControllerBase
    {
        [FromRoute]
        [Required]
        public int GameId { get; set; }

        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public PlayerDepthsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        /// <summary>
        /// Add player to the position depth
        /// Note: Wherever you are using position depth, it starts from 0 not 1
        /// </summary>
        /// <param name="PlayerPositionDto">Player Depth details</param>
        [HttpPost("add-player-depth")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task AddPlayerDepth([FromBody] PlayerPositionDto playerDepthDetails)
        {
            var playerDepth = _mapper.Map<PlayerPositionRequest>(playerDepthDetails);
            playerDepth.Game = GameId;
            await _mediator.Send(playerDepth);
            Ok();
        }

        /// <summary>
        /// Remove player from position 
        /// Note: Wherever you are using position depth, it starts from 0 not 1
        /// </summary>
        /// <param name="playerDepthDetails">Player Depth details</param>
        [HttpDelete("remove-player-position")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<PlayerDto>> RemovePlayerPosition([FromBody] SelectedPositionDto playerDepthDetails)
        {
            var playerDepth = _mapper.Map<RemovePlayerRequest>(playerDepthDetails);
            playerDepth.Game = GameId;
            
            var result = await _mediator.Send(playerDepth);
            return result == null ? [] : [result];
        }

        /// <summary>
        /// Get player backup from current position depth
        /// Note: Wherever you are using position depth, it starts from 0 not 1
        /// </summary>
        /// <param name="playerDepthDetails">Player Depth details</param>
        [HttpPut("Get-player-backup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<PlayerDto>> GetPlayerBackup([FromBody] SelectedPositionDto playerDepthDetails)
        {
            var playerDepth = _mapper.Map<GetBackupRequest>(playerDepthDetails);
            playerDepth.Game = GameId;

            var result = await _mediator.Send(playerDepth);
            return result;
        }
    }
}
