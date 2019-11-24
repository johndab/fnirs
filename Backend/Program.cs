using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using fNIRS.Hardware.ISS;

namespace fNIRS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Test();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                });
                // .UseElectron(args);


        // public static void Test()
        // {
        //     var adapter = GetIISAdapter();
        //     adapter.Connect().Wait();

        //     adapter.RegisterStreamListener((packet) =>
        //     {
        //         Console.WriteLine(packet.Index);
        //         Console.WriteLine("Missed cycles: " + packet.Header.Value.missed_cycles);
        //         Console.WriteLine("Cycles: " + packet.Cycles.Count);
        //         // Console.WriteLine("DC data length: " + packet.Cycles[0].DCData.Length);
        //         return;
        //     });
        
                
        //     adapter.StartStreaming().Wait();

        //     Console.ReadLine();
        //     adapter.Join();
        //     adapter.StopStreaming().Wait();
        //     Console.WriteLine("Streaming stopped");
        //     //}
        //     adapter.Disconnect().Wait();
        //     Console.WriteLine("Disconnected");
        // }

        // public static ISSAdapter GetIISAdapter()
        // {
        //     var config = new ConfigurationBuilder()
        //         .SetBasePath(Directory.GetCurrentDirectory())
        //         .AddJsonFile("appsettings.json", true)
        //         .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
        //         .AddEnvironmentVariables()
        //         .Build();

        //     var loggerFactory = LoggerFactory.Create(builder =>
        //     {
        //         builder.AddFilter("Microsoft", LogLevel.Warning)
        //             .AddFilter("System", LogLevel.Warning)
        //             .AddConsole()
        //             .AddEventLog();
        //     });
        //     ILogger<ISSAdapter> logger = loggerFactory.CreateLogger<ISSAdapter>();
        //     return new ISSAdapter(config, logger);
        // }
    }
}
