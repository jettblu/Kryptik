using CrypticPay.Data;
using CrypticPay.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Twilio;
using Microsoft.WindowsAzure.Storage;
using CrypticPay.Areas.Identity.Data;
using Tatum;


namespace CrypticPay
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<CrypticPayFriendshipContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("CrypticPayFriendsContextConnection")));

            services.AddDbContext<CrypticPayCoinContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("CrypticPayCoinContextConnection")));


            services.AddTransient<WalletHandler>( i =>
               new WalletHandler(
                   baseUrl: Configuration["TatumAccount:BaseUrl"],
                   apiKey: Configuration["TatumAccount:Key"],
                   encryptKeyPub: Configuration["Encryption:Public"],
                   encryptKeyPriv: Configuration["Encryption:Private"])
           );


            services.AddRazorPages();

            
            
            services.AddTransient<IEmailSender, EmailSender>(i =>
               new EmailSender(
                   Configuration["SendGrid:Key"],
                   Configuration["SendGrid:FromEmail"],
                   Configuration["SendGrid:FromName"]
               )
           );

            // uncomment below for gmail smtp

            /* services.AddTransient<IEmailSender, EmailSender>(i =>
                 new EmailSender(
                     Configuration["EmailSender:Host"],
                     Configuration.GetValue<int>("EmailSender:Port"),
                     Configuration.GetValue<bool>("EmailSender:EnableSSL"),
                     Configuration["EmailSender:UserName"],
                     Configuration["EmailSender:Password"]
                 )
             );*/

            services.AddSingleton<CountryService>();

            var accountSid = Configuration["TwilioAccountDetails:AccountSID"];
            var authToken = Configuration["TwilioAccountDetails:AuthToken"];
            var verificationSid = Configuration["TwilioAccountDetails:VerificationServiceSID"];
            var sendNumber = Configuration["TwilioAccountDetails:SendNumber"];

            TwilioClient.Init(accountSid, authToken);


            services.AddTransient<ISmsSender, SmsSender>(implementationFactory =>
            new SmsSender(
                verificationSid: verificationSid,
                accountSid:accountSid,
                authToken:authToken,
                sendNumber:sendNumber
                )
            );


            services.Configure<TwilioVerifySettings>(Configuration.GetSection("TwilioAccountDetails"));
            
            services.Configure<StorageAccountOptions>(Configuration.GetSection("StorageAccount"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }

    }
}
