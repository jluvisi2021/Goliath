using Goliath.Data;
using Goliath.Models;
using Goliath.Repository;
using Goliath.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Goliath
{
    public class Startup
    {
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

            services.Configure<SMTPConfigModel>(_config.GetSection("SMTPConfig"));
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            });

            // Enable MVC Design
            services.AddControllersWithViews();
            // URL Settings
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddRouting(options => options.LowercaseQueryStrings = true);
            services.AddRouting(options => options.AppendTrailingSlash = true);
#if DEBUG
            services.AddRazorPages().AddRazorRuntimeCompilation();
#endif
            // Enable services to use in Controllers.
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
                app.UseHsts();
            }

            // Always require HTTPS connections.
            app.UseHttpsRedirection();

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