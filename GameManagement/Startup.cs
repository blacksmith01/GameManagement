using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using GameManagement.Areas.Identity;
using GameManagement.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using GameManagement.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Components.Server;

namespace GameManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite($"Data Source={"managementdb"}.db");
                    options.EnableSensitiveDataLogging();
                });
            services.AddDbContext<GameDbContext>(options =>
                {
                    options.UseSqlite($"Data Source={"gamedb"}.db");
                    options.EnableSensitiveDataLogging();
                });

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
            }).AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddTransient<IEmailSender, EmailSender>(isp =>
            {
                var email = isp.GetRequiredService<ApplicationDbContext>().System.FirstOrDefault().Email;
                return new EmailSender(
                    email.Host,
                    email.Port,
                    email.EnableSSL,
                    email.UserName,
                    email.Password
                );
            });
            services.AddRazorPages().AddRazorPagesOptions(optons => { optons.Conventions.AuthorizeFolder("/"); });
            services.AddServerSideBlazor();
            //services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddSingleton<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddHttpContextAccessor();
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en", "ko" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });
            services.AddSingleton<BulkActionService>();
            services.AddSingleton<IHostedService, LifeTimeEventHandler>();
            services.AddSingleton<AppUserHistoryWriter>();

            services.AddValidatorsFromAssemblyContaining<Program>();
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

            var supportedCultures = new List<CultureInfo>
            {
              new CultureInfo("en"),
              new CultureInfo("ko")
            };
            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };
            app.UseRequestLocalization(options);

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }

        public static async Task<bool> Initialize(IHost host)
        {
            Logger.Initialize();

            using (var scope = host.Services.CreateScope())
            {
                if (!await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Initialize())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
