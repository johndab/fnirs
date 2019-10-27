using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using fNIRS.Services.Helpers;
using fNIRS.Hardware;

namespace fNIRS.Services
{
    public class ConnectionService : IService
    {
        private readonly IAdapter adapter;

        public ConnectionService(IAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void Register()
        {
            Electron.IpcMain.Upon("hardwareConnect", (x) =>
            {
                adapter.Connect();
                var conn = adapter.IsConnected();
                Electron.IpcMain.SendMain("isConnected", conn);
            });

            Electron.IpcMain.Upon("hardwareDisconnect", async (x) =>
            {
                await adapter.Disconnect();
                var conn = adapter.IsConnected();
                Electron.IpcMain.SendMain("isConnected", conn);
            });

            Electron.IpcMain.Upon("isConnected", (x) =>
            {
                var conn = adapter.IsConnected();
                Electron.IpcMain.SendMain("isConnected", conn);
            });
        }
    }
}
