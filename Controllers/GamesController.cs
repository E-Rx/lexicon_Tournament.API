using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Dtos;

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
        public async Task<ActionResult<IEnumerable<GameDto>>> GetGame()
        {
            try
            {
                var games = await _unitOfWork.GameRepository.GetAllAsync();

                if (games == null || !games.Any())
                {
                    return NotFound("No games found.");
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
