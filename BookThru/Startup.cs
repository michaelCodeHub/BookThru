using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BookThru.Models;
using BookThru.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using CourseMedic.Areas.Identity.Services;
using Microsoft.AspNetCore.Identity;

namespace BookThru
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var dataProtectionProviderType = typeof(DataProtectorTokenProvider<BookThruUser>);


            services.AddIdentity<BookThruUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<BookThruContext>()
            .AddTokenProvider(TokenOptions.DefaultProvider, dataProtectionProviderType);

            services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<BookThruContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("BookThruContext")));

            services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(1);//You can set Time
            });

            services.AddTransient<IEmailSender, EmailSender>();
            services.Configure<AuthMessageSenderOptions>(Configuration);

            services.AddScoped<RoleManager<IdentityRole>>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services,
            BookThruContext dbContext,
  RoleManager<IdentityRole> roleManager,
  UserManager<BookThruUser> userManager)
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

            app.UseAuthentication();
            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Books}/{action=Index}/{id?}");
            });
            CreateUserRoles(dbContext, roleManager, userManager).Wait();
        }

        private async Task CreateUserRoles(BookThruContext dbContext, RoleManager<IdentityRole> RoleManager, UserManager<BookThruUser> UserManager)
        {
            dbContext.Database.EnsureCreated();
            //initializing custom roles 
            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;
            //Create Role if None
            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            //Assign Admin Role To Micheal
            BookThruUser michaeljaison = await UserManager.FindByEmailAsync("michaeljaison.me@gmail.com");
            if (michaeljaison == null)
            {
                michaeljaison = new BookThruUser()
                {
                    UserName = "michaeljaison",
                    Email = "michaeljaison.me@gmail.com",
                };
                await UserManager.CreateAsync(michaeljaison, "P@ssw0rd");
            }
            await UserManager.AddToRoleAsync(michaeljaison, "Admin");

            // Admin Role To nasir
            BookThruUser nacer = await UserManager.FindByEmailAsync("syednacer@gmail.com");
            if (nacer == null)
            {
                nacer = new BookThruUser()
                {
                    UserName = "syednacer@gmail.com",
                    Email = "syednacer@gmail.com",
                };
                await UserManager.CreateAsync(nacer, "Amin@2005");
            }
            await UserManager.AddToRoleAsync(nacer, "Admin");

        }
    }
}
