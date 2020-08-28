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
    public class SituationsController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public SituationsController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Situations
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
                var myDbContext = (from S in _context.Situations

                                   join C in _context.Clients on S.ClientId equals C.ClientId
                                   where C.ClientId == clientId
                                   select S);
                return View(await myDbContext.ToListAsync());
            }
            else
            {
                var myDbContext = _context.Situations.Include(s => s.Client);
                return View(await myDbContext.ToListAsync());
            }
        }


        // GET: Situations/Details/5
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



            var situation = await _context.Situations
                .Include(s => s.Client)
                
                .FirstOrDefaultAsync(m => m.SituationId == id && m.ClientId == request.ClientId);
            if (situation == null)
            {
                return NotFound();
            }

            return View(situation);
        }

        // GET: Situations/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email");
            return View();
        }

        // POST: Situations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SituationId,Nombredepersonne,Revenumenage,ClientId")] Situation situation)
        {
            if (ModelState.IsValid)
            { 
                var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
                var ClientId = (from C in _context.Clients
                               where C.email == user_.Email
                               select C.ClientId).FirstOrDefault();
                situation.ClientId = ClientId;
                _context.Add(situation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
           // ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", situation.ClientId);
            return View(situation);
        }

        // GET: Situations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var situation = await _context.Situations.FindAsync(id);
            if (situation == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", situation.ClientId);
            return View(situation);
        }

        // POST: Situations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SituationId,Nombredepersonne,Revenumenage,ClientId")] Situation situation)
        {
            if (id != situation.SituationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(situation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SituationExists(situation.SituationId))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", situation.ClientId);
            return View(situation);
        }

        // GET: Situations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var situation = await _context.Situations
                .Include(s => s.Client)
                .FirstOrDefaultAsync(m => m.SituationId == id);
            if (situation == null)
            {
                return NotFound();
            }

            return View(situation);
        }

        // POST: Situations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var situation = await _context.Situations.FindAsync(id);
            _context.Situations.Remove(situation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SituationExists(int id)
        {
            return _context.Situations.Any(e => e.SituationId == id);
        }
    }
}
