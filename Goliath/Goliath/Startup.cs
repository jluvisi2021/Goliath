
using DNTCaptcha.Core;
using Goliath.Data;
using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using GoogleReCaptcha.V3;
using GoogleReCaptcha.V3.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Goliath
{
    public class Startup
    {
        /// <summary>
        /// <b>appsettings.json</b> object.
        /// </summary>
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<GoliathContext>(
                options => options.UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            );
            // Setup custom Identity Core.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<GoliathContext>().AddDefaultTokenProviders();

            // Setup the SMTPConfig model to use values directly from the appsettings.
            services.Configure<SMTPConfigModel>(_config.GetSection("SMTPConfig"));
            // General Identity Core settings.
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true; // Password must have at least one digit.
                options.Password.RequiredLength = 6; // Password min length is 6.
                options.Password.RequireLowercase = true; // Password must have at least one lower case character.
                options.Password.RequireNonAlphanumeric = true; // Password must have at least one non-alphanumeric character.
                options.Password.RequireUppercase = true; // Password must have at least one upper case character.
                options.Password.RequireDigit = true; // Password must have digit
                options.User.RequireUniqueEmail = true; // All emails unique
                options.SignIn.RequireConfirmedEmail = true; // Require activated accounts.
            });

            // All tokens expire within 5 minutes.
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            // Enable MVC Design
            services.AddControllersWithViews();

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = true;
            });

            // DNT Captchas
            services.AddDNTCaptcha(options =>
               options.UseCookieStorageProvider()
                   .ShowThousandsSeparators(false)
                   .AbsoluteExpiration(minutes: 5)
            );

            

#if DEBUG
            // Allow Razor pages to update upon browser refresh.
            services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

            // Enable services to use in Controllers through DI.
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (bool.Parse(_config["General:HTTPSRedirection"]) == true)
                {
                    app.UseHsts();
                }
            }
            app.UseStatusCodePages(options =>
            {
                options.UseStatusCodePagesWithRedirects("/Errors/Index?code={0}");
            });

            if (bool.Parse(_config["General:HTTPSRedirection"]) == true)
            {
                // Always require HTTPS connections.
                app.UseHttpsRedirection();
            }

      

            // Use wwwroot folder.
            app.UseStaticFiles();

            // Enable URL routing.
            app.UseRouting();

            // Enable ASP.NET Core Identity
            app.UseAuthentication();

            // Require user accounts
            app.UseAuthorization();


            /* Starts at ~/Views/Auth/Login.cshtml */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Auth}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}