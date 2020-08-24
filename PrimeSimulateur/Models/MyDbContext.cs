using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PrimeSimulateur.Models;

namespace PrimeSimulateur.Models
{
    [Authorize]
    public class MyDbContext : DbContext
    
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Travail> Travails { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Situation> Situations { get; set; }
        public DbSet<Logement> Logements { get; set; }




        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            foreach (var relationship in modelbuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelbuilder);
        }




        public DbSet<PrimeSimulateur.Models.ProjectRole> ProjectRole { get; set; }




        public DbSet<PrimeSimulateur.Models.trace> trace { get; set; }

    }

}

