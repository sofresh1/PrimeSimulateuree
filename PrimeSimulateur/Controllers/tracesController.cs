using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PrimeSimulateur.Models;

namespace PrimeSimulateur.Controllers
{
    public class tracesController : Controller
    {
        private readonly MyDbContext _context;

        public tracesController(MyDbContext context)
        {
            _context = context;
        }

        // GET: traces
        public async Task<IActionResult> Index()
        {
            var myDbContext = _context.trace.Include(t => t.Client);
            return View(await myDbContext.ToListAsync());
        }

        // GET: traces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trace = await _context.trace
                .Include(t => t.Client)
                .FirstOrDefaultAsync(m => m.traceId == id);
            if (trace == null)
            {
                return NotFound();
            }

            return View(trace);
        }

        // GET: traces/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email");
            return View();
        }

        // POST: traces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("traceId,ClientId,Type,Nom,Surface,prime,UserId,email")] trace trace)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trace);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", trace.ClientId);
            return View(trace);
        }

        // GET: traces/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trace = await _context.trace.FindAsync(id);
            if (trace == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", trace.ClientId);
            return View(trace);
        }

        // POST: traces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("traceId,ClientId,Type,Nom,Surface,prime,UserId,email")] trace trace)
        {
            if (id != trace.traceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trace);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!traceExists(trace.traceId))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "ClientId", "email", trace.ClientId);
            return View(trace);
        }

        // GET: traces/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trace = await _context.trace
                .Include(t => t.Client)
                .FirstOrDefaultAsync(m => m.traceId == id);
            if (trace == null)
            {
                return NotFound();
            }

            return View(trace);
        }

        // POST: traces/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trace = await _context.trace.FindAsync(id);
            _context.trace.Remove(trace);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool traceExists(int id)
        {
            return _context.trace.Any(e => e.traceId == id);
        }
    }
}
