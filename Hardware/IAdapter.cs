using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fNIRS.Hardware.Models;

namespace fNIRS.Hardware 
{
    public interface IAdapter : IDisposable
    {
        Task Connect();
        Task Disconnect();
        bool IsConnected();
     
        Task StartStreaming();
        Task StopStreaming();
        bool IsStreaming();
        void RegisterStreamListener(Action<DataPacket> action);
        void RemoveStreamListener();

        
        ICollection<Frequency> GetFrequencies();
        Task SetFrequency(int x);
        Task<int> GetFrequency();
    }
}