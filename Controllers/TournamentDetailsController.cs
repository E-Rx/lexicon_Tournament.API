using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Core.Dtos;

namespace Tournament.API.Controllers
{
    [Route("api/TournamentDetails")]
    [ApiController]
    public class TournamentDetailsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TournamentDetailsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: api/TournamentDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDetailsDto>>> GetTournamentDetails()
        {
            try
            {
                var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAllAsync();
                if (tournamentDetails == null || !tournamentDetails.Any())
                {
                    return NotFound("No tournament details found.");
                }

                var tournamentDetailsDtos = _mapper.Map<IEnumerable<TournamentDetailsDto>>(tournamentDetails);
                return Ok(tournamentDetailsDtos);
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving tournaments.");
            }
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetailsDto>> GetTournamentDetails(int id)
        {
            try
            {
                var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAsync(id);
                if (tournamentDetails == null)
                {
                    return NotFound($"Tournament with ID {id} not found.");
                }

                var tournamentDetailsDto = _mapper.Map<TournamentDetailsDto>(tournamentDetails);
                return Ok(tournamentDetailsDto);
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving the tournament.");
            }
        }

        // PUT: api/TournamentDetails/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentDetailsDto tournamentDetailsDto)
        {
            if (id != tournamentDetailsDto.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            var tournamentDetails = _mapper.Map<TournamentDetails>(tournamentDetailsDto);
            _unitOfWork.TournamentDetailsRepository.Update(tournamentDetails);

            try
            {
                await _unitOfWork.CompleteAsync();
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!await TournamentDetailsExists(id))
                {
                    return NotFound($"Tournament with ID {id} no longer exists.");
                }

                return StatusCode(500, "A concurrency error occurred while updating.");
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the tournament.");
            }

            return NoContent();
        }

        // POST: api/TournamentDetails
        [HttpPost]
        public async Task<ActionResult<TournamentDetailsDto>> PostTournamentDetails(TournamentDetailsDto tournamentDetailsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tournamentDetails = _mapper.Map<TournamentDetails>(tournamentDetailsDto);
            _unitOfWork.TournamentDetailsRepository.Add(tournamentDetails);

            try
            {
                await _unitOfWork.CompleteAsync();
            }

            catch (Exception)
            {
                return StatusCode(500, "Failed to save tournament to the database.");
            }

            var createdDto = _mapper.Map<TournamentDetailsDto>(tournamentDetails);
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournamentDetails.Id }, createdDto);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            try
            {
                var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAsync(id);
                if (tournamentDetails == null)
                {
                    return NotFound($"Tournament with ID {id} not found.");
                }

                _unitOfWork.TournamentDetailsRepository.Remove(tournamentDetails);
                await _unitOfWork.CompleteAsync();
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the tournament.");
            }

            return NoContent();
        }

        private async Task<bool> TournamentDetailsExists(int id)
        {
            return await _unitOfWork.TournamentDetailsRepository.AnyAsync(id);
        }
    }
}
