using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Data.Data;

namespace Tournament.API.Controllers
{
    public class TournamentDetailsController : Controller
    {
        private readonly TournamentAPIContext _context;

        public TournamentDetailsController(TournamentAPIContext context)
        {
            _context = context;
        }

        // GET: TournamentDetails
        public async Task<IActionResult> Index()
        {
            return View(await _context.TournamentDetails.ToListAsync());
        }

        // GET: TournamentDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentDetails = await _context.TournamentDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentDetails == null)
            {
                return NotFound();
            }

            return View(tournamentDetails);
        }

        // GET: TournamentDetails/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TournamentDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title")] TournamentDetails tournamentDetails)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tournamentDetails);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tournamentDetails);
        }

        // GET: TournamentDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentDetails = await _context.TournamentDetails.FindAsync(id);
            if (tournamentDetails == null)
            {
                return NotFound();
            }
            return View(tournamentDetails);
        }

        // POST: TournamentDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] TournamentDetails tournamentDetails)
        {
            if (id != tournamentDetails.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tournamentDetails);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TournamentDetailsExists(tournamentDetails.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tournamentDetails);
        }

        // GET: TournamentDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tournamentDetails = await _context.TournamentDetails
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tournamentDetails == null)
            {
                return NotFound();
            }

            return View(tournamentDetails);
        }

        // POST: TournamentDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tournamentDetails = await _context.TournamentDetails.FindAsync(id);
            if (tournamentDetails != null)
            {
                _context.TournamentDetails.Remove(tournamentDetails);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TournamentDetailsExists(int id)
        {
            return _context.TournamentDetails.Any(e => e.Id == id);
        }
    }
}
