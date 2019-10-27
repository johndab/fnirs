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
            services.AddMvc();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "./UI/dist";
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSpaStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseStaticFiles();

            app.UseMvc();

            app.UseSpa(spa =>
            {
                spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
            });

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }

            var services = app.ApplicationServices.GetServices<IService>();
            foreach(var service in services)
            {
                Console.WriteLine(service.GetType().FullName);
                service.Register();
            }
        }

        public async void ElectronBootstrap()
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions
                {
                    Width = 1152,
                    Height = 864,
                    Show = false,
                });

            browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("fNIRS Monitoring");
        }
    }
}
