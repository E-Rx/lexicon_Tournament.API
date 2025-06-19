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
            _mapper=mapper;
        }

        // GET: api/TournamentDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDetailsDto>>> GetTournamentDetails()
        {
            var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAllAsync();
            if (tournamentDetails == null || !tournamentDetails.Any())
            {
                return NotFound();
            }
            var tournamentDetailsDtos = _mapper.Map<IEnumerable<TournamentDetailsDto>>(tournamentDetails);
            return Ok(tournamentDetailsDtos);
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetailsDto>> GetTournamentDetails(int id)
        {
            var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAsync(id);

            if (tournamentDetails == null)
            {
                return NotFound();
            }

            var tournamentDetailsDto = _mapper.Map<TournamentDetailsDto>(tournamentDetails);
            return Ok(tournamentDetailsDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentDetailsDto tournamentDetailsDto)
        {
            if (id != tournamentDetailsDto.Id)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/TournamentDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDetailsDto>> PostTournamentDetails(TournamentDetailsDto tournamentDetailsDto)
        {
            var tournamentDetails = _mapper.Map<TournamentDetails>(tournamentDetailsDto);
            _unitOfWork.TournamentDetailsRepository.Add(tournamentDetails);
            await _unitOfWork.CompleteAsync();

            var createdTournamentDetailsDto = _mapper.Map<TournamentDetailsDto>(tournamentDetails);
            return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournamentDetails.Id }, createdTournamentDetailsDto);
        }

        // DELETE: api/TournamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTournamentDetails(int id)
        {
            var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAsync(id);

            if (tournamentDetails == null)
            {
                return NotFound();
            }

            _unitOfWork.TournamentDetailsRepository.Remove(tournamentDetails);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        private async Task<bool> TournamentDetailsExists(int id)
        {
            return await _unitOfWork.TournamentDetailsRepository.AnyAsync(id);
        }
    }
    
}
