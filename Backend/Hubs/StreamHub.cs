using Microsoft.AspNetCore.SignalR;
using fNIRS.Hardware;

namespace fNIRS.Hubs
{
    public partial class MainHub
    {
        public void StartDataStream()
        {
            adapter.RegisterStreamListener((y) =>
            {
                var viewModel = y.ToModel(store.graph);
                store.hubContext.Clients.All.SendAsync("NewDataPacket", viewModel);
            });

            adapter.StartStreaming();
            var conn = adapter.IsStreaming();
            Clients.Caller.SendAsync("IsStreaming", conn);
        }

        public void StopDataStream()
        {
            adapter.RemoveStreamListener();
            adapter.StopStreaming();

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
