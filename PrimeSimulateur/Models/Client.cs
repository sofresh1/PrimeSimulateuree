using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeSimulateur.Models
{

  
    public class Client
    {
      
        [Key]
        [Required]
        public int ClientId { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string number { get; set; }

        public virtual List<Logement> Logements { get; set; }
        public virtual List<Situation> Situations { get; set; }
    }
}

 
