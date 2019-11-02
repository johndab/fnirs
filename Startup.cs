using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElectronNET.API;
using ElectronNET.API.Entities;
using VueCliMiddleware;
using System.Linq;
using fNIRS.Services;
using fNIRS.Hardware;
using fNIRS.Hardware.ISS;
using Scrutor;
using System;

namespace fNIRS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "UI/dist";
            });

            var demo = Configuration.GetValue("ISSAdapter:demo", false);
            if (demo)
                services.AddSingleton<IAdapter, DemoAdapter>();
            else
                services.AddSingleton<IAdapter, ISSAdapter>();

            services.Scan(scan => scan
                .FromAssemblyOf<IService>()
                    .AddClasses(classes => classes.AssignableTo<IService>())
                    .AsImplementedInterfaces()
                    .WithTransientLifetime()
            );
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseSpaStaticFiles();

            app.UseRouting();
            app.UseStaticFiles();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "UI";
                spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
            });

            var services = app.ApplicationServices.GetServices<IService>();
            foreach(var service in services)
            {
                Console.WriteLine(service.GetType().FullName);
                service.Register();
            }

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }
        }

        public async void ElectronBootstrap()
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions
                {
                    Width = 1152,
                    Height = 864,
                    Show = true,
                });

            // browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("fNIRS Monitoring");
        }
    }
}
