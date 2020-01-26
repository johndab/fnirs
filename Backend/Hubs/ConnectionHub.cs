using Microsoft.AspNetCore.SignalR;
using fNIRS.Hardware.ISS;

namespace fNIRS.Hubs
{
    public partial class MainHub
    {
        public void HardwareConnect()
        {
            if(store.dmcStart)
                store.dmcApp = new ISSConnection(store.dmcExe);

            this.adapter.Connect();
            var conn = this.adapter.IsConnected();
            Clients.Caller.SendAsync("IsConnected", conn);
        }

        public void HardwareDisconnect()
        {
            adapter.Disconnect();
            if(store.dmcApp != null)
                store.dmcApp.Dispose();

            messageParser.CollectStop();
            Clients.Caller.SendAsync("IsCollecting", messageParser.IsCollecting());

            var conn = adapter.IsConnected();
            var str = adapter.IsStreaming();
            Clients.Caller.SendAsync("IsConnected", conn);
            Clients.Caller.SendAsync("IsStreaming", str);
        }

        public void IsConnected()
        {
            var conn = adapter.IsConnected();
            Clients.Caller.SendAsync("IsConnected", conn);
        }
    }
}
