using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ElectronNET.API;
using fNIRS.Hardware;

namespace fNIRS.Services
{
    public class FrequencyService : IService
    {
        private readonly IAdapter adapter;

        public FrequencyService(IAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void Register()
        {
            // Electron.IpcMain.On("async-msg", (x) =>
            // {
            //     var mainWindow = Electron.WindowManager.BrowserWindows.First();
            //     Electron.IpcMain.Send(mainWindow, "asynchronous-reply", "pong");
            // });

            Electron.IpcMain.OnSync("getFrequencies", (args) =>
            {
                return adapter.GetFrequencies();
            });

        }
    }
}
