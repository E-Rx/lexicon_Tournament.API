using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using Tournament.Data.Repositories;


namespace Tournament.API.Controllers
{
    [Route("api/TournamentDetails")]
    [ApiController]
    public class TournamentDetailsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public TournamentDetailsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: api/TournamentDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDetails>>> GetTournamentDetails()
        {
            var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAllAsync();
            return Ok(tournamentDetails);
        }

        // GET: api/TournamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDetails>> GetTournamentDetails(int id)
        {
            var tournamentDetails = await _unitOfWork.TournamentDetailsRepository.GetAsync(id);

            if (tournamentDetails == null)
            {
                return NotFound();
            }

            return Ok(tournamentDetails);
        }

        // PUT: api/TournamentDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentDetails tournamentDetails)
        {
            if (id != tournamentDetails.Id)
            {
                return BadRequest();
            }

            var tournamentExists = await _unitOfWork.TournamentDetailsRepository.AnyAsync(id);
            if (!tournamentExists)
            {
                return NotFound();
            }

            _unitOfWork.TournamentDetailsRepository.Update(tournamentDetails);

            try
            {
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;           
            }

            return NoContent();
        }


        // POST: api/TournamentDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDetails>> PostTournamentDetails(TournamentDetails tournamentDetails)
        {
            _unitOfWork.TournamentDetailsRepository.Add(tournamentDetails);
            await _unitOfWork.CompleteAsync();

            return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournamentDetails.Id }, tournamentDetails);
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
