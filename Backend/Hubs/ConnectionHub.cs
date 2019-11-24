using Microsoft.AspNetCore.SignalR;
using fNIRS.Hardware.ISS;

namespace fNIRS.Hubs
{
    public partial class MainHub
    {
        public void HardwareConnect()
        {
            store.dmcApp = new ISSConnection();
            this.adapter.Connect();
            var conn = this.adapter.IsConnected();
            Clients.Caller.SendAsync("IsConnected", conn);
        }

        public void HardwareDisconnect()
        {
            adapter.Disconnect();
            if(store.dmcApp != null)
                store.dmcApp.Dispose();
            
            var conn = adapter.IsConnected();
            Clients.Caller.SendAsync("IsConnected", conn);
        }

        public void IsConnected()
        {
            var conn = adapter.IsConnected();
            Clients.Caller.SendAsync("IsConnected", conn);
        }
    }
}
