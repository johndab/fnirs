using System;
using ElectronNET.API;
using fNIRS.Services.Helpers;
using fNIRS.Hardware;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace fNIRS.Services
{
    public class ConfigService : IService
    {
        private readonly IAdapter adapter;
        private readonly ILogger<ConnectionService> logger;

        private string path;
        private string config;

        public ConfigService(IAdapter adapter, 
            IConfiguration configuration,
            ILogger<ConnectionService> logger)
        {
            this.adapter = adapter;
            this.path = configuration.GetValue<string>("LayoutPath");
            this.config = string.Empty;
            this.logger = logger;
        }

        public void Register()
        {
            Electron.IpcMain.Upon("getConfig", (x) =>
            {
                if(string.IsNullOrEmpty(this.config))
                {
                    try 
                    {
                        this.config = File.ReadAllText(this.path);
                    } 
                    catch(Exception e) 
                    {
                        logger.LogError("Error while reading layout config");
                    }
                }

                Electron.IpcMain.SendMain("getConfig", this.config);
            }, logger);

            Electron.IpcMain.Upon<string>("saveConfig", (x) =>
            {
                this.config = x;

                using (StreamWriter file = File.CreateText(this.path))
                {
                    file.Write(x);
                }

                Electron.IpcMain.SendMain("getConfig", this.config);
            }, logger);
        }
    }
}
