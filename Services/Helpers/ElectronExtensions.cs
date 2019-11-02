using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElectronNET.API;
using Microsoft.Extensions.Logging;

namespace fNIRS.Services.Helpers
{
    public static class ElectronExtensions
    {
        public static void SendMain(this IpcMain ipc, string channel, params object[] data)
        {
            var mainWindow = Electron.WindowManager.BrowserWindows.First();
            Electron.IpcMain.Send(mainWindow, channel, data);
        }

        public static void Upon<T>(this IpcMain ipc, string channel, Action<T> listener, ILogger logger = null)
        {
            ipc.On(channel, (x) =>
            {
                try
                {
                    listener.Invoke((T) x);
                }
                catch(Exception e)
                {
                    if(logger != null)
                        logger.LogError(e.StackTrace, e);
                    else
                        Console.WriteLine(e.StackTrace);
                }
            });
        }

        public static void Upon(this IpcMain ipc, string channel, Action<object> listener, ILogger logger = null)
        {
            ipc.Upon<object>(channel, listener, logger);
        }
    }
}
