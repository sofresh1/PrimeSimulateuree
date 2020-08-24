using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeSimulateur.Models
{
    [Authorize]
    public class Situation
    {
       
        [Key]
        public int SituationId { get; set; }
        [Required]
        public int Nombredepersonne { get; set; }
        [Required]
        public float Revenumenage { get; set; }
        public int ClientId { get; set; }
        public virtual Client Client { get; set; }
    }
}


    