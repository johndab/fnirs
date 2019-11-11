using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ElectronNET.API;
using Newtonsoft.Json;
using fNIRS.Hardware.ISS;
using fNIRS.Hardware.Models;

namespace fNIRS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 
            // Console.WriteLine(size);
            Test();
            // CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseElectron(args);


        public static void Test()
        {
            var adapter = GetIISAdapter();
            adapter.Connect();

            using (StreamWriter file = new StreamWriter(@"C:\dev\fNIRS\app\data-log.json"))
            {
                adapter.RegisterStreamListener((packet) =>
                {
                    file.WriteLine("\n");

                    var json = JsonConvert.SerializeObject(packet);
                    file.WriteLine(json);
                });
                
                adapter.StartStreaming().Wait();

                Console.ReadLine();
                adapter.StopStreaming().Wait();
                Console.WriteLine("Streaming stopped");
            }
            adapter.Disconnect().Wait();
            Console.WriteLine("Disconnected");
        }

        public static ISSAdapter GetIISAdapter()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true)
                .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddConsole()
                    .AddEventLog();
            });
            ILogger<ISSAdapter> logger = loggerFactory.CreateLogger<ISSAdapter>();
            return new ISSAdapter(config, logger);
        }
    }
}
