using Authorize.Data;
using Authorize.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Authorize
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            using(var scope = host.Services.CreateScope())
            {
                DatabaseInitialize.Init(scope.ServiceProvider);
            }
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

    public static class DatabaseInitialize
    {
        public static void Init(IServiceProvider scopeServiceProvider)
        {
            var userManager = scopeServiceProvider.GetService<UserManager<ApplicationUser>>();
            
            var user = new ApplicationUser
            {
                FirstName = "Artem",
                LastName = "Borisovskiy",
                UserName = "host02"
            };

            var result = userManager.CreateAsync(user, "123").GetAwaiter().GetResult();
            
            if (result.Succeeded)
            {
                var claims = new List<Claim>
                    {new Claim(ClaimTypes.Role, "Student"), new Claim(ClaimTypes.Role, "Mentor")};
                userManager.AddClaimsAsync(user, claims).GetAwaiter().GetResult();
            }
            
        }
    }
}
