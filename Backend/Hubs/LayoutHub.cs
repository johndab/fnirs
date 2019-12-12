using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using fNIRS.Memory;
using System.Collections.Generic;

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
                    store.layout = File.ReadAllText(store.layoutPath + "layout.json");
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

            using (StreamWriter file = File.CreateText(store.layoutPath + "layout.json"))
            {
                file.Write(x);
            }

            Clients.Caller.SendAsync("GetLayout", store.layout);
        }

        public void SetGraph(GraphModel graph)
        {
            store.graph = graph;
        }
    }
}
