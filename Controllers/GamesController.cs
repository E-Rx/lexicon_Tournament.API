using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Dtos;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;

namespace Tournament.API.Controllers
{
    [Route("api/Games")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GamesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/Games
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGame([FromQuery] string? sortBy)
        {
            try
            {
                var games = await _unitOfWork.GameRepository.GetAllAsync();

                if (games == null || !games.Any())
                {
                    return NotFound("No games found.");
                }

                // Sort games if sortBy parameter is provided
                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    games = sortBy.ToLower() switch
                    {
                        "title" => games.OrderBy(g => g.Title),
                        "time" => games.OrderBy(g => g.Time),
                        _ => games
                    };
                }

                var gamesDtos = _mapper.Map<IEnumerable<GameDto>>(games);
                return Ok(gamesDtos);
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving games.");
            }
        }

        // GET: api/Games/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GameDto>> GetGame(int id)
        {
            try
            {
                var game = await _unitOfWork.GameRepository.GetAsync(id);

                if (game == null)
                {
                    return NotFound($"Game with ID {id} not found.");
                }

                var gameDto = _mapper.Map<GameDto>(game);
                return Ok(gameDto);
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the game.");
            }
        }

        // GET: api/Games/search?title=Finale
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGameByTitle([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("You must provide a title to search.");
            }

            try
            {
                var matchingGames = await _unitOfWork.GameRepository.GetByTitleAsync(title);

                if (!matchingGames.Any())
                {
                    return NotFound($"No games found with the title '{title}'.");
                }

                var matchingGamesDto = _mapper.Map<IEnumerable<GameDto>>(matchingGames);
                return Ok(matchingGamesDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while searching for games.");
            }
        }


        // PUT: api/Games/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGame(int id, GameDto gameDto)
        {
            if (id != gameDto.Id)
            {
                return BadRequest("The ID in the URL does not match the Game ID in the body.");
            }

            try
            {
                var game = _mapper.Map<Game>(gameDto);
                _unitOfWork.GameRepository.Update(game);

                await _unitOfWork.CompleteAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await GameExists(id))
                {
                    return NotFound($"Game with ID {id} does not exist.");
                }
                return StatusCode(500, "A concurrency error occurred while updating the game.");
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the game.");
            }
        }

        // POST: api/Games
        [HttpPost]
        public async Task<ActionResult<GameDto>> PostGame(GameDto gameDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var game = _mapper.Map<Game>(gameDto);
            _unitOfWork.GameRepository.Add(game);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                return StatusCode(500, "Failed to save game to the database.");
            }

            var createdDto = _mapper.Map<GameDto>(game);
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, createdDto);
        }


        [HttpPatch("{gameId}")]
        public async Task<ActionResult> PatchGame(int gameId, JsonPatchDocument<GameUpdateDto> patchDocument)
        {
            if (patchDocument is null)
            {
                return BadRequest("No patch document provided.");
            }

            var game = await _unitOfWork.GameRepository.GetAsync(gameId);

            if (game == null)
            {
                return NotFound($"Game with ID {gameId} not found.");
            }

            // Map entity to updatable DTO
            var dtoToPatch = _mapper.Map<GameUpdateDto>(game);

            // Apply the patch
            patchDocument.ApplyTo(dtoToPatch, ModelState);
            TryValidateModel(dtoToPatch);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            // Map back to entity and save changes
            _mapper.Map(dtoToPatch, game);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }


        // DELETE: api/Games/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            try
            {
                var game = await _unitOfWork.GameRepository.GetAsync(id);

                if (game == null)
                {
                    return NotFound($"Game with ID {id} not found.");
                }

                _unitOfWork.GameRepository.Remove(game);
                await _unitOfWork.CompleteAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the game.");
            }
        }

        private async Task<bool> GameExists(int id)
        {
            return await _unitOfWork.GameRepository.AnyAsync(id);
        }
    }
}
