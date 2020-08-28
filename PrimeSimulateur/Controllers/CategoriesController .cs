using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using PrimeSimulateur.GenerateDocuments;
using PrimeSimulateur.Models;
using System.Text;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace PrimeSimulateur.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CategoriesController(MyDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Categories
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
                var myDbContext = (from Cat in _context.Categories
                                   join T in _context.Travails on Cat.TravailId equals T.TravailId  
                                   join L in _context.Logements on T.LogementId equals L.LogementId
                                   join C in _context.Clients on L.ClientId equals C.ClientId
                                   where C.ClientId == clientId
                                   select Cat);
                return View(await myDbContext.ToListAsync());
            }
            else
            {
                var myDbContext = _context.Categories.Include(c => c.Travail);
                return View(await myDbContext.ToListAsync());
            }
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories
                .Include(c => c.Travail)
                .FirstOrDefaultAsync(m => m.CategorieId == id);
            if (categorie == null)
            {
                return NotFound();
            }

            return View(categorie);
        }

        // GET: Categories/Create
        public async Task<IActionResult> CreateAsync()
        {

            
                var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
                var categories = (from Cat in _context.Categories
                                  join T in _context.Travails on Cat.TravailId equals T.TravailId
                                  join L in _context.Logements on T.LogementId equals L.LogementId
                                  join C in _context.Clients on L.ClientId equals C.ClientId
                                  where C.email == user_.Email

                                  select new { T.TravailId, T.Name });


                ViewData["TravailId"] = new SelectList(categories, "TravailId", "Name");
                return View();

            }
        


        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategorieId,type,TravailId")] Categorie categorie)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categorie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TravailId"] = new SelectList(_context.Travails, "TravailId", "TravailId", categorie.TravailId);
            return View(categorie);
        }



        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null)
            {
                return NotFound();
            }
            ViewData["TravailId"] = new SelectList(_context.Travails, "TravailId", "TravailId", categorie.TravailId);
            return View(categorie);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategorieId,type,TravailId")] Categorie categorie)
        {
            if (id != categorie.CategorieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categorie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategorieExists(categorie.CategorieId))
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
            ViewData["TravailId"] = new SelectList(_context.Travails, "TravailId", "TravailId", categorie.TravailId);
            return View(categorie);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories
                .Include(c => c.Travail)
                .FirstOrDefaultAsync(m => m.CategorieId == id);
            if (categorie == null)
            {
                return NotFound();
            }

            return View(categorie);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(categorie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategorieExists(int id)
        {
            return _context.Categories.Any(e => e.CategorieId == id);
        }




        private bool SituationExists(int id)
        {
            return _context.Situations.Any(e => e.SituationId == id);
        }

        public async Task<ActionResult> Generate()
        {
            var d = new PrimeSimulatorDocs(_context);
            var fileName = "test";
            var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
            //add Id variable
            var doc = await d.Generate(user_.Email);

            if (doc != null)
            {
                Response.ContentType = "application/pdf";
                Response.Headers.Add("content-disposition", $"inline;filename={HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8)}.pdf");
                Response.Headers.Add("Content-Length", doc.Length.ToString());
                Response.Body.WriteAsync(doc, 0, doc.Length);
                Response.Body.Flush();
                return new EmptyResult();
            }

            return View();
        }
        public async Task<FileResult> Export()
        {
            List<string> csvRows = new List<string>();
            StringBuilder sb = new StringBuilder();

            var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
            //header line
            csvRows.Add(PrimeSimulatorDocs.GenerateExportHeader().ToString());

            //add data from DB
            List<trace> list_travaux = await (from T in _context.trace
                                              where T.email == user_.Email
                                              select T).ToListAsync();

            var list_a_exporter = PrimeSimulatorDocs.ToDataTable(list_travaux);
            foreach (DataRow objRD in list_a_exporter.Rows)
            {
                sb = new StringBuilder();
                // Records Lines
                csvRows.Add(PrimeSimulatorDocs.GenerateExportRecordLine(objRD).ToString());
            }

            var list_CSV = string.Join("\r\n", csvRows);
            return File(Encoding.GetEncoding("ISO-8859-1").GetBytes(list_CSV.ToString()), "application/csv", $"Travaux.csv");
        }

        public async Task<IActionResult> Simuler(int id, string type)
        {
            ViewData["prime"] = await CalculerPrime(id, type);

            return View();
        }

        private async Task<float> CalculerPrime(int travailId, string type)
        {
            var request = (from T in _context.Travails
                           join L in _context.Logements on T.LogementId equals L.LogementId
                           join C in _context.Clients on L.ClientId equals C.ClientId
                           join S in _context.Situations on C.ClientId equals S.ClientId
                           join Cat in _context.Categories on T.TravailId equals Cat.TravailId
                           where T.TravailId == travailId && Cat.type == type  
                           select new { S, T, Cat, C }).FirstOrDefault();
            Client client = new Client
            {
                ClientId = request.C.ClientId
            };

            Situation situation = new Situation
            {
                SituationId = request.S.SituationId,
                Nombredepersonne = request.S.Nombredepersonne,
                Revenumenage = request.S.Revenumenage
            };

            Travail travail = new Travail
            {
                TravailId = request.T.TravailId,
                Name = request.T.Name,
                surface = request.T.surface
            };
            Categorie categorie = new Categorie
            {
                CategorieId = request.Cat.CategorieId,
                type = request.Cat.type
            };
            //pour le fichier csv

            ViewData["Name"] = travail.Name;

            ViewData["travailId"] = travail.TravailId;
            ViewData["type"] = categorie.type;
            float prime = 0;

            // Categorie commble et toiture
            if (travail.Name == "chaudiere")
            {
                {
                    if ((categorie.type == "Biomasseperformante") || (categorie.type == "bois"))
                    {
                        if (situation.Nombredepersonne == 1)
                        {
                            // TRES MODESTE
                            if (situation.Revenumenage <= 20470)
                            {
                                prime = 40000;
                            }

                            //MODESTE
                            if (situation.Revenumenage <= 25068)
                            {
                                prime = 40000;
                            }

                            //MOYEN ET SUPERIEUR
                            if (situation.Revenumenage > 25068)
                            {
                                prime = 25000;
                            }
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 30044)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 36572)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 36572)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 36080)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 43924)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 43294)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 42128)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 51289)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 51289)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 48198)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 58674)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 58674)
                        {
                            prime = 25000;
                        }
                    }
                }
                if (categorie.type == "Treshauteperformanceenergetique") 
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 20470)
                        {
                            prime = 1200;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 25068)
                        {
                            prime = 1200;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 25068)
                        {
                            prime = 600;
                        }
                    }
                }
                if (situation.Nombredepersonne == 2)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 30044)
                    {
                        prime = 1200;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 36572)
                    {
                        prime = 1200;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 36572)
                    {
                        prime = 600;
                    }
                }
                if (situation.Nombredepersonne == 3)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 36080)
                    {
                        prime =1200;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 43924)
                    {
                        prime = 1200;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 43294)
                    {
                        prime = 600;
                    }
                }
                if (situation.Nombredepersonne == 4)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 42128)
                    {
                        prime = 1200;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 51289)
                    {
                        prime = 1200;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 51289)
                    {
                        prime = 600;
                    }
                }
                if (situation.Nombredepersonne == 5)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 48198)
                    {
                        prime = 1200;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 58674)
                    {
                        prime = 1200;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 58674)
                    {
                        prime = 600;
                    }
                }
            }

                if (travail.Name == "raccordement")
            {
                if (categorie.type == "réseau de chaleur EnR&R")
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 20470)
                        {
                            prime = 700;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 25068)
                        {
                            prime = 700;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 25068)
                        {
                            prime = 450;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 30044)
                        {
                            prime = 700;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 36572)
                        {
                            prime = 700;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 36572)
                        {
                            prime = 450;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 36080)
                        {
                            prime = 700;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 43924)
                        {
                            prime = 700;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 43294)
                        {
                            prime = 450;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 42128)
                        {
                            prime = 700;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 51289)
                        {
                            prime = 700;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 51289)
                        {
                            prime = 450;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 48198)
                        {
                            prime = 700;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 58674)
                        {
                            prime = 700;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 58674)
                        {
                            prime = 450;
                        }
                    }
                    //for (i = 6; i < 30; i++)
                    //{
                    //    if (situation.Nombredepersonne == i)
                    //        // TRES MODESTE
                    //        for (y = 0; y < 30)
                    //            y = y + 60590;
                    //        if (situation.Revenumenage <= 481980+y)
                    //        {
                    //            prime = 40000;
                    //        }
                    //    for (y = 0; y < 30)
                    //        y = y + 60590;
                    //    //MODESTE
                    //    for (z = 0; z < 30)
                    //        z = z + 60590;
                    //    if (situation.Revenumenage <= 586740+z)
                    //    {
                    //        prime = 40000;
                    //    }

                    //    //MOYEN ET SUPERIEUR
                    //    if (situation.Revenumenage > 586740+z)
                    //    {
                    //        prime = 25000;
                    //    }
                    //}

                }
            }

            if (travail.Name == "appareil de chauffage")
            {
                if (categorie.type == "bois très performant")
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 20470)
                        {
                            prime = 800;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 25068)
                        {
                            prime = 800;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 25068)
                        {
                            prime = 500;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 30044)
                        {
                            prime = 800;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 36572)
                        {
                            prime = 800;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 36572)
                        {
                            prime = 500;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 36080)
                        {
                            prime = 800;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 43924)
                        {
                            prime = 800;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 43294)
                        {
                            prime = 500;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 42128)
                        {
                            prime = 800;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 51289)
                        {
                            prime = 800;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 51289)
                        {
                            prime = 500;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 48198)
                        {
                            prime = 800;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 58674)
                        {
                            prime = 800;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 58674)
                        {
                            prime = 500;
                        }
                    }
                }
            }
            if (travail.Name == "systeme solaire")
            {
                if (categorie.type == "combiné")
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 20470)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 25068)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 25068)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 30044)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 36572)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 36572)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 36080)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 43924)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 43294)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 42128)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 51289)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 51289)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 48198)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 58674)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 58674)
                        {
                            prime = 25000;
                        }
                    }
                }
            }
            if (travail.Name == "pompe a chaleur")
            {
                if ((categorie.type == "air/eau") || (categorie.type == "eau/eau") || (categorie.type == "hybride"))
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 20470)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 25068)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 25068)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 30044)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 36572)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 36572)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 36080)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 43924)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 43294)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 42128)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 51289)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 51289)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 48198)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 58674)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 58674)
                        {
                            prime = 25000;
                        }
                    }
                }
            }

            // Categorie comble et toiture
            if (travail.Name == "isolation")

            {
                if (categorie.type == "comble et toiture")
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 20470)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 25068)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 25068)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 30044)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 36572)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 36572)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 36080)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 43924)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 43924)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 42128)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 51289)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 51289)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 48198)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 58674)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 58674)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                }
                if (categorie.type == "plancher bas")
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 20470)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 25068)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 25068)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 30044)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 36572)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 36572)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 36080)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 43924)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 43924)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 42128)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 51289)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 51289)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 48198)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 58674)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 58674)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                }
            }

            var user_ = await _userManager.FindByEmailAsync(User.Identity.Name);
            var roleAdmin = User.IsInRole("Admin");
            var roleUser = User.IsInRole("User");
            var claimsList = User.Claims.ToList();
            var role = claimsList[3].Value;
            ViewData["userRole"] = role;
            //pour le fichier csv 
            _context.trace.AddAsync(new trace { Nom = travail.Name, Surface = travail.surface, Type = categorie.type, ClientId = client.ClientId, prime = prime, UserId = user_.Id, email=user_.Email });

            _context.SaveChangesAsync();
            return prime;
        }
    }
}



