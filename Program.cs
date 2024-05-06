using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Web;
using System.Globalization;
using WebAPP.Controllers;
using WebAPP.Extensions;
using WebAPP.Infrastructure.DBContext;
using WebAPP.Infrastructure.GlobalParameters;
using WebAPP.Infrastructure.Models.Validation;
using WebAPP.MiddlewareFactory;

namespace WebAPP;
// Preconditions: 1) Il browser deve essere configurato per visualizzare le date nel formato italiano (gg/mm/YYYY).
public class Program
{
    protected Program() { }

    public static void Main(string[] args)
    {
        Logger logger = LogManager.Setup()
                                  .LoadConfigurationFromAppSettings()
                                  .GetCurrentClassLogger();
        try
        {
            logger.Info("Esecuzione avviata");   // Solo su file log

            // Before the app is configured and started, a host is configured and launched. The host is responsible for app startup and lifetime management.
            // Startup Profile IISExpress in development environment (when the app execution is by command prompt. To set globally, in production environment in Advanced System Settings set an environment variable ASPNETCORE_ENVIRONMENT.
            // launchSettings.json override values set in the system environmen (but is not deployed).
            var builder = WebApplication.CreateBuilder(args);

            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
            builder.Services.AddAntiforgery(options =>
                                                {
                                                    options.Cookie.Name = "XSRF-TOKEN";
                                                    options.HeaderName = "X-XSRF-TOKEN";
                                                })
                            .AddAutoMapper(typeof(Program))
                            .AddScoped<Microsoft.Extensions.Logging.ILogger>(s => s.GetRequiredService<ILogger<HomeController>>())
                            .AddScoped<HttpClientFactory>()
                            .AddDbContext<GDAContext>(options => options.UseSqlServer(GlobalParameters.Config.GetConnectionString("GDA")))
                            .AddLocalization(options => options.ResourcesPath = "Resources")
                            .Configure<RequestLocalizationOptions>(options =>
                            {
                                var cultures = new List<CultureInfo> {
                                    new ("en"),
                                    new ("it")
                                };
                                options.DefaultRequestCulture = new RequestCulture("en");
                                options.SupportedCultures = cultures;
                                options.SupportedUICultures = cultures;
                            })
                            .ValidationServices()
                            .AddControllersWithViews();
            builder.Services.AddMvc()
                            .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                            .AddDataAnnotationsLocalization();
            // Per accedere ai coocky
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection()
               .UseStaticFiles()
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value)
               .UseCustomMiddlewareExtension();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
        catch (Exception exception)
        {
            logger.Error(exception, "Stopped program because of exception");
            throw;
        }
        finally
        {
            // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            LogManager.Shutdown();
        }
    }
}