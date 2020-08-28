
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrimeSimulateur.Models;

namespace PrimeSimulateur.Controllers
{
    [Authorize]
    public class TravailsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TravailsController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Travails
        public async Task<IActionResult> Index()
      
             {
            var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
        var claimsList = User.Claims.ToList();
        var role = claimsList[3].Value;
        var clientId = (from C in _context.Clients
                        where C.email == user_.Email
                        select C.ClientId).FirstOrDefault();

            if (role == "User")
            {
                var myDbContext = (from T in _context.Travails
                                   join L in _context.Logements on T.LogementId equals L.LogementId
                                   join C in _context.Clients on L.ClientId equals C.ClientId
                                   where C.ClientId == clientId
                                   select T);
                return View(await myDbContext.ToListAsync());
    }
            else
            {
                var myDbContext = _context.Travails.Include(t => t.Logement);
                return View(await myDbContext.ToListAsync());
}
        }

        // GET: Travails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travail = await _context.Travails
                .Include(t => t.Logement)
                .FirstOrDefaultAsync(m => m.TravailId == id);
            if (travail == null)
            {
                return NotFound();
            }

            return View(travail);
        }

        // GET: Travails/Create
        public async Task<IActionResult> CreateAsync()
        {
            var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
            var logements = (from T in _context.Travails
                             join L in _context.Logements on T.LogementId equals L.LogementId
                             join C in _context.Clients on L.ClientId equals C.ClientId
                             where C.email == user_.Email
                             select new { T.LogementId, L.adresse}) ;
           // ViewData["LogementId"] = new SelectList(_context.Logements, "LogementId", "adresse");
            ViewData["LogementId"] = new SelectList(logements, "LogementId", "adresse");
            return View();
        }

        // POST: Travails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TravailId,Name,surface,LogementId")] Travail travail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(travail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LogementId"] = new SelectList(_context.Logements, "LogementId", "adresse", travail.LogementId);
            return View(travail);
        }

        // GET: Travails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travail = await _context.Travails.FindAsync(id);
            if (travail == null)
            {
                return NotFound();
            }
            ViewData["LogementId"] = new SelectList(_context.Logements, "LogementId", "adresse", travail.LogementId);
            return View(travail);
        }

        // POST: Travails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TravailId,Name,surface,LogementId")] Travail travail)
        {
            if (id != travail.TravailId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(travail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TravailExists(travail.TravailId))
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
            ViewData["LogementId"] = new SelectList(_context.Logements, "LogementId", "adresse", travail.LogementId);
            return View(travail);
        }

        // GET: Travails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var travail = await _context.Travails
                .Include(t => t.Logement)
                .FirstOrDefaultAsync(m => m.TravailId == id);
            if (travail == null)
            {
                return NotFound();
            }

            return View(travail);
        }

        // POST: Travails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var travail = await _context.Travails.FindAsync(id);
            _context.Travails.Remove(travail);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TravailExists(int id)
        {
            return _context.Travails.Any(e => e.TravailId == id);
        }
    }
}
