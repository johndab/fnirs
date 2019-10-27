﻿using System;
using ElectronNET.API;
using fNIRS.Services.Helpers;
using fNIRS.Hardware;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace fNIRS.Services
{
    public class StreamService : IService
    {
        private readonly IAdapter adapter;

        public StreamService(IAdapter adapter)
        {
            this.adapter = adapter;
        }

        public void Register()
        {
            //Electron.IpcMain.Upon("startStream", async (x) =>
            //{
            //    //adapter.RegisterStreamListener((y) =>
            //    //{
            //    //    Electron.IpcMain.SendMain("newDataPacket", y);
            //    //});

            //    //await adapter.StartStreaming();
            //    var conn = adapter.IsStreaming();
            //    Electron.IpcMain.SendMain("isStreaming", conn);
            //});

            //Electron.IpcMain.Upon("stopStream", async (x) =>
            //{
            //    //await adapter.StopStreaming();

            //    //adapter.RemoveStreamListener();
            //    var conn = adapter.IsStreaming();
            //    Electron.IpcMain.SendMain("isStreaming", conn);
            //});

            //Electron.IpcMain.Upon("isStreaming", (x) =>
            //{
            //    var conn = adapter.IsStreaming();
            //    Electron.IpcMain.SendMain("isStreaming", conn);
            //});
        }
    }
}