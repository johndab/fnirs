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
    public class FrequencyService : IService
    {
        private readonly IAdapter adapter;

        public FrequencyService(IAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void Register()
        {
            Electron.IpcMain.Upon("getFrequencies", (x) =>
            {
                var list = adapter.GetFrequencies();
                Electron.IpcMain.SendMain("getFrequencies", list);
            });

            Electron.IpcMain.Upon("setFrequency", async (x) =>
            {
                await adapter.SetFrequency((int)((long) x));
                var freq = await adapter.GetFrequency();
                Electron.IpcMain.SendMain("getFrequency", freq);
            });

            Electron.IpcMain.Upon("getFrequency", async (x) =>
            {
                var freq = await adapter.GetFrequency();
                Electron.IpcMain.SendMain("getFrequency", freq);
            });
        }
    }
}
