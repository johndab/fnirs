using System;
using System.Collections.Generic;
using fNIRS.Hardware.Models;

namespace fNIRS.Hardware 
{
    public interface IAdapter : IDisposable
    {
        void Connect();
        ICollection<Frequency> GetFrequencies();
    }
}