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
using Microsoft.AspNetCore.Http;

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
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddSignalR()
                .AddNewtonsoftJsonProtocol();

            services.AddSpaStaticFiles(configuration =>
            {
               configuration.RootPath = "./ui";
            });

            services.AddSingleton<HubStore>();
            services.AddSingleton<MessageParser>();
            var demo = Configuration.GetValue("ISSAdapter:demo", false);
            if (demo)
                services.AddSingleton<IAdapter, DemoAdapter>();
            else
                services.AddSingleton<IAdapter, ISSAdapter>();

        }

        public void Configure(IApplicationBuilder app)
        {

            app.UseCors("CorsPolicy");
            app.UseRouting();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<MainHub>("/fnirs");
            });

            app.UseSpa(spa =>
            {
            //    spa.Options.SourcePath = "UI";
            //    spa.UseProxyToSpaDevelopmentServer("http://localhost:8080");
            });

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
                });

            // browserWindow.OnReadyToShow += () => browserWindow.Show();
            browserWindow.SetTitle("fNIRS Monitoring");
        }
    }
}
