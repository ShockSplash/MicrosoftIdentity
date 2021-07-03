using Authorize.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using Authorize.Entities;

namespace Authorize
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddAuthorization(options => {
                options.AddPolicy("Mentor", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, "Mentor");
                });
                options.AddPolicy("Student", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, "Student");
                });

                options.AddPolicy("SuperUser", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, "SuperUser");
                });
            });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("Memory")
            ).AddIdentity<ApplicationUser, ApplicationRole>(config =>
            {
                config.Password.RequireDigit = false;
                config.Password.RequireLowercase = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.Password.RequiredLength = 3;

            }).AddEntityFrameworkStores<ApplicationDbContext>();

            services.ConfigureApplicationCookie(builder =>
            {
                builder.LoginPath = "/Admin/Login";
                builder.AccessDeniedPath = "/Home/AccessDenied";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

            });
        }
    }
}
