using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeSimulateur.Models
{
    [Authorize]
    public class Travail
    {
       
        [Required]
        [Key]
        public int TravailId { get; set; }
        [Required]
        public string Name { get; set; }
        public float surface { get; set; }

        [Required]

        public int LogementId { get; set; }
        public Logement Logement { get; set; }

        public virtual List<Categorie> Categories { get; set; }
    }
}
