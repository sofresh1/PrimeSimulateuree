using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeSimulateur.Models
{
    [Authorize]
    public class Categorie
    {
     
        [Key]
        [Required]
        public int CategorieId { get; set; }

        public string type { get; set; }
        [Required]
        public int TravailId { get; set; }
        public Travail Travail { get; set; }

    }
}
