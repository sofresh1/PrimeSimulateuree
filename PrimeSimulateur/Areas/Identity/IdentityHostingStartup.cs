using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PrimeSimulateur.Data;

[assembly: HostingStartup(typeof(PrimeSimulateur.Areas.Identity.IdentityHostingStartup))]
namespace PrimeSimulateur.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<PrimeSimulateurContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("PrimeSimulateurContextConnection")));

                services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
                    .AddRoles<IdentityRole>().AddEntityFrameworkStores<PrimeSimulateurContext>();
            });
        }
    }
}