using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeSimulateur.Models
{
    [Authorize]
    public class Logement
    {
        [Key]
        [Required]
        public int LogementId { get; set; }
        [Required]
        public string adresse { get; set; }
        public string Ville { get; set; }

        public string TypeEnergie { get; set; }
        public string surface { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; }

        public virtual List<Travail> Travails { get; set; }
    }
}
