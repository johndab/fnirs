using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElectronNET.API;
using ElectronNET.API.Entities;
using fNIRS.Hardware;
using fNIRS.Hardware.ISS;
using fNIRS.Hubs;
using fNIRS.Memory;

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
            services.AddCors(options => 
                options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .WithOrigins("http://localhost:8080")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            }));

            services.AddSignalR();

            //services.AddControllers();

            //services.AddSpaStaticFiles(configuration =>
            //{
            //    configuration.RootPath = "./ui";
            //});

            services.AddSingleton<HubStore>();
            var demo = Configuration.GetValue("ISSAdapter:demo", false);
            if (demo)
                services.AddSingleton<IAdapter, DemoAdapter>();
            else
                services.AddSingleton<IAdapter, ISSAdapter>();

            //services.AddTransient

            //services.Scan(scan => scan
            //    .FromAssemblyOf<IService>()
            //        .AddClasses(classes => classes.AssignableTo<IService>())
            //        .AsImplementedInterfaces()
            //        .WithTransientLifetime()
            //);
        }

        public void Configure(IApplicationBuilder app)
        {
            //app.UseSpaStaticFiles();

            app.UseCors("CorsPolicy");
            app.UseRouting();

            //app.UseStaticFiles();

            //app.UseSpa(spa =>
            //{
            //    spa.Options.SourcePath = "UI";
            //    spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
            //});

            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<MainHub>("/MainHub");
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MainHub>("/fnirs");
            });

            //var services = app.ApplicationServices.GetServices<IService>();
            //foreach(var service in services)
            //{
            //    Console.WriteLine(service.GetType().FullName);
            //    service.Register();
            //}

            //if (HybridSupport.IsElectronActive)
            //{
            //    ElectronBootstrap();
            //}
        }

        public async void ElectronBootstrap()
        {
            var browserWindow = await Electron.WindowManager.CreateWindowAsync(
                new BrowserWindowOptions
                {
                    Width = 1152,
                    Height = 864,
                });

            // browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("fNIRS Monitoring");
        }
    }
}
