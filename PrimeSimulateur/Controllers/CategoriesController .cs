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
            var myDbContext = _context.Categories.Include(c => c.Travail);
            return View(await myDbContext.ToListAsync());
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
        public IActionResult Create()
        {
            ViewData["TravailId"] = new SelectList(_context.Travails, "TravailId", "TravailId");
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

            //add Id variable
            var doc = await d.Generate(1);

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
                                              where T.UserId == user_.Id
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

        public async Task<IActionResult> Simuler(int id)
        {
            ViewData["prime"] = await CalculerPrime(id);

            return View();
        }

        private async Task<float> CalculerPrime(int travailId)
        {
            var request = (from T in _context.Travails
                           join L in _context.Logements on T.LogementId equals L.LogementId
                           join C in _context.Clients on L.ClientId equals C.ClientId
                           join S in _context.Situations on C.ClientId equals S.ClientId
                           join Cat in _context.Categories on T.TravailId equals Cat.TravailId
                           where T.TravailId == travailId
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
            ViewData["email"] = client.email;
            
            float prime = 0;

            // Categorie commble et toiture
            if (travail.Name == "chaudiere")
            {
                if ((categorie.type == "Biomasseperformante") || (categorie.type == "bois "))
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 204700)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 250680)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 250680)
                        {
                            prime = 25000;
                        }
                    }
                }
                if (situation.Nombredepersonne == 2)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 300440)
                    {
                        prime = 40000;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 365720)
                    {
                        prime = 40000;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 365720)
                    {
                        prime = 25000;
                    }
                }
                if (situation.Nombredepersonne == 3)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 360800)
                    {
                        prime = 40000;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 439240)
                    {
                        prime = 40000;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 432940)
                    {
                        prime = 25000;
                    }
                }
                if (situation.Nombredepersonne == 4)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 421280)
                    {
                        prime = 40000;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 512890)
                    {
                        prime = 40000;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 512890)
                    {
                        prime = 25000;
                    }
                }
                if (situation.Nombredepersonne == 5)
                {
                    // TRES MODESTE
                    if (situation.Revenumenage <= 481980)
                    {
                        prime = 40000;
                    }

                    //MODESTE
                    if (situation.Revenumenage <= 586740)
                    {
                        prime = 40000;
                    }

                    //MOYEN ET SUPERIEUR
                    if (situation.Revenumenage > 586740)
                    {
                        prime = 25000;
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
                        if (situation.Revenumenage <= 204700)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 250680)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 250680)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 300440)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 365720)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 365720)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 360800)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 439240)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 432940)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 421280)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 512890)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 512890)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 481980)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 586740)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 586740)
                        {
                            prime = 25000;
                        }
                    }
               //    // else for (int i = 6; i < 30; i++)
               //      //   {
               //        //     var y = 0;
               //          //   if (situation.Nombredepersonne == i)
               //             //// TRES MODESTE
               //            // {
               //               //  for (i = 6; i < 30; y++)
               //                 //    y = y + 60590;
               //                 //if (situation.Revenumenage <= 481980 + y)
               //                 //{
               //                  //   prime = 40000;
               //                 //}
               //            // }
               //        // }
               //     //MODESTE
               //     //var z = 0;
                    
               //       //  for (int i = 6; i < 30; z++)
               //         //    if (situation.Nombredepersonne == i)
               //           //  {      z = z + 60590;
               //     //if (situation.Revenumenage <= 586740 + z)
               //     //{
               //      //   prime = 40000;
               //    // }

               //     //MOYEN ET SUPERIEUR
               //     //else if (situation.Revenumenage > 586740 + z)
               //    // {
               //      //   prime = 25000;
               //     //}
               
                 }

                }
            

            if (travail.Name == "appareil de chauffage")
            {
                if (categorie.type == "bois très performant")
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 204700)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 250680)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 250680)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 300440)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 365720)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 365720)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 360800)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 439240)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 432940)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 421280)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 512890)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 512890)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 481980)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 586740)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 586740)
                        {
                            prime = 25000;
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
                        if (situation.Revenumenage <= 204700)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 250680)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 250680)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 300440)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 365720)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 365720)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 360800)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 439240)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 432940)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 421280)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 512890)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 512890)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 481980)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 586740)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 586740)
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
                        if (situation.Revenumenage <= 204700)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 250680)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 250680)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 300440)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 365720)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 365720)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 360800)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 439240)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 432940)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 421280)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 512890)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 512890)
                        {
                            prime = 25000;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 481980)
                        {
                            prime = 40000;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 586740)
                        {
                            prime = 40000;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 586740)
                        {
                            prime = 25000;
                        }
                    }
                }
            }

            // Categorie comble et toiture
            if (travail.Name == "isolation")

            {
                if (categorie.type == "combleettoiture")
                {
                    if (situation.Nombredepersonne == 1)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 204700)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 250680)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 250680)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 300440)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 365720)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 365720)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 360800)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 439240)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 439240)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 421280)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 512890)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 512890)
                        {
                            prime = 30 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 481980)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 586740)
                        {
                            prime = 30 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 586740)
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
                        if (situation.Revenumenage <= 204700)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 250680)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 250680)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 2)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 300440)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 365720)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 365720)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 3)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 360800)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 439240)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 439240)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 4)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 421280)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 512890)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 512890)
                        {
                            prime = 20 * travail.surface;
                        }
                    }
                    if (situation.Nombredepersonne == 5)
                    {
                        // TRES MODESTE
                        if (situation.Revenumenage <= 481980)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MODESTE
                        if (situation.Revenumenage <= 586740)
                        {
                            prime = 20 * travail.surface;
                        }

                        //MOYEN ET SUPERIEUR
                        if (situation.Revenumenage > 586740)
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
            _context.trace.AddAsync(new trace { Nom = travail.Name, Surface = travail.surface, Type = categorie.type, ClientId = client.ClientId, prime = prime, UserId = user_.Id, email = client.email ,}) ;

            _context.SaveChangesAsync();
            return prime;
        }
    }
}



