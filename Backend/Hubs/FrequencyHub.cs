using Microsoft.AspNetCore.SignalR;

namespace fNIRS.Hubs
{
    public partial class MainHub
    {
        public void GetFrequencies()
        {
            var list = adapter.GetFrequencies();
            Clients.Caller.SendAsync("GetFrequencies", list);
        }
        
        public void SetFrequency(int x)
        {
            adapter.SetFrequency(x);
            var freq = adapter.GetFrequency();
            Clients.Caller.SendAsync("GetFrequency", freq);
        }

        public void GetFrequency()
        {
            var freq = adapter.GetFrequency();
            Clients.Caller.SendAsync("GetFrequency", freq);
        }
    }
}
