using System;
using CrypticPay.Areas.Identity.Data;
using CrypticPay.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(CrypticPay.Areas.Identity.IdentityHostingStartup))]
namespace CrypticPay.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<CrypticPayContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("CrypticPayContextConnection")));

                services.AddDefaultIdentity<CrypticPayUser>(options => options.SignIn.RequireConfirmedPhoneNumber = true)
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<CrypticPayContext>();
                services.Configure<DataProtectionTokenProviderOptions>(options => 
                {   
                    options.Name = "Sms"; 
                    options.TokenLifespan = TimeSpan.FromMinutes(30); 
                });
            });
        }
    }
}