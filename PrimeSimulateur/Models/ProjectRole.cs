using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrimeSimulateur.Models
{
    [Authorize(Roles = "Admin")]
    public class ProjectRole
    { public int Id { get; set; }
        public string RoleName { get; set; } 
    }
}
