using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

namespace fNIRS.Hubs
{
    public partial class MainHub
    {

        public void GetLayout()
        {
            if (string.IsNullOrEmpty(store.layout))
            {
                try
                {
                    store.layout = File.ReadAllText(store.layoutPath);
                }
                catch (Exception e)
                {
                    store.logger.LogError(e, "Error while reading layout config");
                }
            }

            Clients.Caller.SendAsync("GetLayout", store.layout);
        }

        public void SaveLayout(string x)
        {
            store.layout = x;

            using (StreamWriter file = File.CreateText(store.layoutPath))
            {
                file.Write(x);
            }

            Clients.Caller.SendAsync("GetLayout", store.layout);
        }
    }
}
