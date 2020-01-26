using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fNIRS.Hardware.Models;

namespace fNIRS.Hardware 
{
    public interface IAdapter : IDisposable
    {
        void Connect();
        void Disconnect();
        bool IsConnected();
     
        void StartStreaming();
        void StopStreaming();
        bool IsStreaming();
        int SetCycleNum(int num);
        int SetSwitch(int num);

        
        ICollection<Frequency> GetFrequencies();
        void SetFrequency(int x);
        int GetFrequency();
    }
}