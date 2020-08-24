using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeSimulateur.Models
{
    public class trace
    {
        [Key]
        [Required]
        public int traceId { get; set; }

       
        [Required]

        public int ClientId{ get; set; }
        public Client Client { get; set; }
        public string Type{ get; set; }
        public string Nom{ get; set; }
        public float Surface{ get; set; }
        public float prime { get; set; }
        public string UserId { get; set; }

        public string email { get; set; }
        //modifier requte sur fonction
        //ajouter clonne sur csv(prime simulateurdoc et categorie)


    }
}
