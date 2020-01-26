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
            this.dmcExe = config.GetValue("DMCExe", "");
            this.dmcStart = config.GetValue("StartDMC", false);
            this.hubContext = hubContext;
            this.freq = 0;
        }

        public ILogger<MainHub> logger { get; set; }
        public IHubContext<MainHub> hubContext;

        public GraphModel graph;
        public int freq { get; set; }
        public bool dmcStart;
        public string dmcExe;
        public string layoutPath { get; set; } 
        public string layout { get; set; }
        public ISSConnection dmcApp { get; set; }
    }
}
