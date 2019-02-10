using System;
using BookThru.Data;
using BookThru.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(BookThru.Areas.Identity.IdentityHostingStartup))]
namespace BookThru.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<BookThruContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("BookThruContextConnection")));

                services.AddDefaultIdentity<BookThruUser>()
                    .AddEntityFrameworkStores<BookThruContext>();
            });
        }
    }
}