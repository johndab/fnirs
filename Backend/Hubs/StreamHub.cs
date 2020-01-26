using Microsoft.AspNetCore.SignalR;
using fNIRS.Hardware;

namespace fNIRS.Hubs
{
    public partial class MainHub
    {
        public void StartDataStream()
        {
            messageParser.PacketAction = (y) =>
            {
                if(store.graph == null) return;


                store.hubContext.Clients.All.SendAsync("Debug", y);
                var viewModel = y.ToModel(store.graph, store.freq);
                store.hubContext.Clients.All.SendAsync("NewDataPacket", viewModel);
            };

            adapter.StartStreaming();
            var conn = adapter.IsStreaming();
            Clients.Caller.SendAsync("IsStreaming", conn);
        }

        public void StopDataStream()
        {
            messageParser.PacketAction = null;
            adapter.StopStreaming();

            messageParser.CollectStop();
            Clients.Caller.SendAsync("IsCollecting", messageParser.IsCollecting());

            var conn = adapter.IsStreaming();
            Clients.Caller.SendAsync("IsStreaming", conn);
        }

        public void IsStreaming()
        {
            var conn = adapter.IsStreaming();
            Clients.Caller.SendAsync("IsStreaming", conn);
        }

        public void SetCyclesNum(int x)
        {
            var num = adapter.SetCycleNum(x);
            Clients.Caller.SendAsync("SetCyclesNum", num);
        }

        public void SetSwitch(int x)
        {
            var num = adapter.SetSwitch(x);
            Clients.Caller.SendAsync("SetSwitch", num);
        }
    }
}
