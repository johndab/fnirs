using fNIRS.Hardware.ISS;
using fNIRS.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fNIRS.Memory
{
    public class HubStore
    {
        public HubStore(IConfiguration config, ILogger<MainHub> logger, IHubContext<MainHub> hubContext)
        {
            this.layoutPath = config.GetValue<string>("LayoutPath");
            this.logger = logger;
            this.hubContext = hubContext;
        }

        public ILogger<MainHub> logger { get; set; }
        public IHubContext<MainHub> hubContext;

        public GraphModel graph;
        public string layoutPath { get; set; } 
        public string layout { get; set; }
        public ISSConnection dmcApp { get; set; }
    }
}
