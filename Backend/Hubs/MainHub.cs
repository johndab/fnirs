using fNIRS.Hardware;
using fNIRS.Memory;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace fNIRS.Hubs
{
    public partial class MainHub : Hub
    {
        internal readonly IAdapter adapter;
        internal readonly HubStore store;
        internal readonly MessageParser messageParser;

        public MainHub(IAdapter adapter, HubStore store, MessageParser messageParser)
        {
            this.store = store;
            this.messageParser = messageParser;
            this.adapter = adapter;
        }
    }
}
