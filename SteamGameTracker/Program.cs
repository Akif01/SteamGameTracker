using SteamGameTracker.Components;
using SteamGameTracker.Logging.Providers;
using SteamGameTracker.Services.API;
using SteamGameTracker.Utils;

namespace SteamGameTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure logging
            builder.Logging.AddConsole();
            builder.Logging.AddProvider(new FileLoggerProvider("logs/log.txt"));

            // Add services to the container.
            builder.Services.AddMemoryCache();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddSingleton<IUrlFormatter, UrlFormatter>();
            builder.Services.AddHttpClient<IAppListService, AppListService>();
            builder.Services.AddHttpClient<IAppDetailsService, AppDetailsService>();
            builder.Services.AddHttpClient<IPlayerNumberService, PlayerNumberService>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
