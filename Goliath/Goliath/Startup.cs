using DNTCaptcha.Core;
using Goliath.Data;
using Goliath.Enums;
using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        /// <b> appsettings.json </b> object.
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
            services.AddResponseCaching();

            services.AddDbContext<GoliathContext>(
                options => options.UseSqlServer(_config.GetConnectionString("DefaultConnection"))
            );
            // Setup custom Identity Core.
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<GoliathContext>().AddDefaultTokenProviders();

            // Setup the SMTPConfig model to use values directly from the appsettings.
            services.Configure<SMTPConfigModel>(_config.GetSection("SMTPConfig"));

            #region Configuring password settings

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

            // Change password hash iteration count.
            services.Configure<PasswordHasherOptions>(option =>
            {
                option.IterationCount = int.Parse(_config["Application:PasswordHashIterations"]);
            });

            #endregion Configuring password settings

            #region Global cookie policy

            // Guarantee all cookies are secure.
            services.AddCookiePolicy(options =>
            {
                options.Secure = CookieSecurePolicy.Always;
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            });

            #endregion Global cookie policy

            #region Token lifespans

            // All tokens expire within 5 minutes.
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromMinutes(5);
            });

            #endregion Token lifespans

            // Enable MVC Design
            services.AddControllersWithViews();

            #region Routing settings

            // Change URL Settings.
            // Note: options.LowercaseQuery won't work and will interfere with DNT Captcha.
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = true;
            });

            #endregion Routing settings

            #region Captcha options

            // DNT Captchas
            services.AddDNTCaptcha(options =>
               options.UseCookieStorageProvider()
                   .ShowThousandsSeparators(false)
                   .AbsoluteExpiration(minutes: 5)
                   .WithEncryptionKey(_config["Application:CaptchaEncryptionKey"])
            );

            #endregion Captcha options

#if DEBUG
            // Allow Razor pages to update upon browser refresh.
            services.AddRazorPages().AddRazorRuntimeCompilation();
#endif

            #region Custom Goliath services

            // Enable services to use in Controllers through DI.
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IValidHumanVerifyTokensRepository, ValidHumanVerifyTokensRepository>();
            services.AddScoped<ISmsVerifyTokensRepository, SmsVerifyTokensRepository>();
            services.AddScoped<ICookieManager, CookieManager>();
            services.AddScoped<IGoliathCaptchaService, GoliathCaptchaService>();
            services.AddScoped<ITwilioService, TwilioService>();
            services.AddScoped<IUnauthorizedTimeoutsRepository, UnauthorizedTimeoutsRepository>();
            services.AddScoped<ITwoFactorAuthorizeTokenRepository, TwoFactorAuthorizeTokenRepository>();

            #endregion Custom Goliath services

            #region Additional cookie policy

            // Configure secure cookie options for .NET Identity Core.
            services.ConfigureApplicationCookie(options =>
            {
                options.SlidingExpiration = true;
                options.Cookie.Name = CookieKeys.AuthenticationCookie;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.IsEssential = true;
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(2); // Token expires every 2 days unless renewed.
            });

            // Add AntiForgery and its respective options.
            services.AddAntiforgery(options =>
            {
                // Set Cookie properties using CookieBuilder properties.
                options.HeaderName = "X-CSRF-TOKEN";
                options.Cookie.Name = CookieKeys.AntiForgeryCookie;
                options.Cookie.MaxAge = new TimeSpan(12, 0, 0); // 12 Hour Expire
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.SuppressXFrameOptionsHeader = true;
            });

            services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.Name = CookieKeys.AspNetSessionCookie;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            });

            #endregion Additional cookie policy
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IAccountRepository accountRepository)
        {
            // Create roles and super user if not created.
            accountRepository.LoadDefaultsAsync().Wait();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                if (bool.Parse(_config["General:HTTPSRedirection"]))
                {
                    app.UseHsts();
                }
            }

            app.UseExceptionHandler("/Errors/GeneralException");
            app.UseStatusCodePages(options =>
            {
                options.UseStatusCodePagesWithRedirects("/errors?code={0}");
            });

            if (bool.Parse(_config["General:HTTPSRedirection"]))
            {
                // Always require HTTPS connections.
                app.UseHttpsRedirection();
            }

            #region Static file caching

            // Use wwwroot folder. Store cached files for 7 days.
            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse =
        r =>
        {
            string path = r.File.PhysicalPath;
            if (path.EndsWith(".css") || path.EndsWith(".js") || path.EndsWith(".gif") || path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".svg"))
            {
                TimeSpan maxAge = new(0, 4, 0, 0); // 5 minutes 45 seconds for storing statics.
                r.Context.Response.Headers.Add("Cache-Control", $"public, max-age={maxAge.TotalSeconds:0}");
            }
        }
            });

            #endregion Static file caching

            // Enable URL routing.
            app.UseRouting();

            // Enable ASP.NET Core Identity
            app.UseAuthentication();

            // Require user accounts
            app.UseAuthorization();

            // Caching web pages that do not use forms.
            app.UseResponseCaching();

            // For session storage like TempData
            app.UseSession();

            /* Starts at ~/Views/Auth/Login.cshtml */
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}