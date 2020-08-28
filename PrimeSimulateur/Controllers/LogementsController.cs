using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrimeSimulateur.Models;

namespace PrimeSimulateur.Controllers
{
    [Authorize]
    public class LogementsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;



        public LogementsController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Logements
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
                var myDbContext = (from L in _context.Logements
                                   join C in _context.Clients on L.ClientId equals C.ClientId
                                   where C.ClientId == clientId
                                   select L);
                return View(await myDbContext.ToListAsync());
            }
            else
            {
                var myDbContext = _context.Logements.Include(l => l.Client);
                return View(await myDbContext.ToListAsync());
            }
        }

        // GET: Logements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Client 

            var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
            var request = (from C in _context.Clients
                           where C.email == user_.Email
                           select C).FirstOrDefault();



            var logement = await _context.Logements
                .Include(l => l.Client)
                .FirstOrDefaultAsync(m => m.LogementId == id && m.ClientId == request.ClientId);
            if (logement == null)
            {
                return NotFound();
            }
        
            
           
             return View(logement);
    }



        // GET: Logements/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email");
            return View();
        }

        // POST: Logements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LogementId,adresse,Ville,TypeEnergie,surface,ClientId")] Logement logement)
        {
            if (ModelState.IsValid)
            {
                var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
                var ClientId = (from C in _context.Clients
                                where C.email == user_.Email
                                select C.ClientId).FirstOrDefault();
                logement.ClientId = ClientId;
                _context.Add(logement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
         //   ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", logement.ClientId);
            return View(logement);
        }

        // GET: Logements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logement = await _context.Logements.FindAsync(id);
            if (logement == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", logement.ClientId);
            return View(logement);
        }

        // POST: Logements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LogementId,adresse,Ville,TypeEnergie,surface,ClientId")] Logement logement)
        {
            if (id != logement.LogementId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(logement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LogementExists(logement.LogementId))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", logement.ClientId);
            return View(logement);
        }

        // GET: Logements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var logement = await _context.Logements
                .Include(l => l.Client)
                .FirstOrDefaultAsync(m => m.LogementId == id);
            if (logement == null)
            {
                return NotFound();
            }

            return View(logement);
        }

        // POST: Logements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var logement = await _context.Logements.FindAsync(id);
            _context.Logements.Remove(logement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LogementExists(int id)
        {
            return _context.Logements.Any(e => e.LogementId == id);
        }

    }
}
