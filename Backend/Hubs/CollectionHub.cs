using Microsoft.AspNetCore.SignalR;
using fNIRS.Hardware.ISS;

namespace fNIRS.Hubs
{
    public partial class MainHub
    {
        public void CollectStart()
        {
            messageParser.CollectStart();
            IsCollecting();
        }

        public void CollectStop()
        {
            messageParser.CollectStop();
            IsCollecting();
        }

        public void IsCollecting()
        {
            Clients.Caller.SendAsync("IsCollecting", messageParser.IsCollecting());
        }
    }
}
