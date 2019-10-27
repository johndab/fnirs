using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fNIRS.Hardware.ISS.Converters;
using fNIRS.Hardware.Models;
using fNIRS.Hardware.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using fNIRS.Hardware.Demo;

namespace fNIRS.Hardware.ISS
{
    public class DemoAdapter : IAdapter
    {
        private readonly ILogger<DemoAdapter> logger;
        private bool connected = false;
        private bool streaming = false;
        private DemoMachineState state;
        private DemoReader reader;

        public DemoAdapter(ILogger<DemoAdapter> logger)
        {
            this.logger = logger;
            this.state = new DemoMachineState();
            this.reader = new DemoReader();
        }

#region CONNECTION
        public Task Connect()
        {
            this.connected = true;
            return Task.CompletedTask;
        }
        public Task Disconnect()
        {
            this.connected = false;
            return Task.CompletedTask;
        }
        public bool IsConnected()
        {
            return this.connected;
        }
#endregion
# region STREAMING
        public Task StartStreaming()
        {
            this.streaming = true;
            reader.Start();
            return Task.CompletedTask;
        }
        public Task StopStreaming()
        {
            this.streaming = false;
            reader.Stop();
            return Task.CompletedTask;
        }
        public bool IsStreaming()
        {
            return this.streaming;
        }
        public void RegisterStreamListener(Action<DataPacket> action)
        {
            this.reader.RegisterStreamListener(action);
        }
        public void RemoveStreamListener()
        {
            this.reader.RemoveStreamListener();
        }
#endregion
#region FREQUENCIES

        public Task<int> GetFrequency()
        {
            return Task.FromResult(state.Frequency);
        }

        public Task SetFrequency(int f)
        {
            state.Frequency = f;
            return Task.CompletedTask;
        }

        public ICollection<Frequency> GetFrequencies()
        {
            return Constants.DMC_FREQUENCY_LIST;
        }

#endregion

        public Task Send(string command)
        {           
            this.logger.LogCritical(command);
            return Task.CompletedTask;
        }

        public Task<HardwareStatus> GetHardwareStatus()
        {
            return Task.FromResult(this.state.HardwareStatus);
        }

        public void Dispose()
        {
            if(streaming)
            {
                reader.Stop();
            }
            this.Disconnect().Wait();
        }
    }
}
