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
        public void Connect()
        {
            this.connected = true;
        }
        public void Disconnect()
        {
            this.connected = false;
        }
        public bool IsConnected()
        {
            return this.connected;
        }
#endregion
# region STREAMING
        public void StartStreaming()
        {
            this.streaming = true;
            reader.Start();
        }
        public void StopStreaming()
        {
            this.streaming = false;
            reader.Stop();
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

        public int GetFrequency()
        {
            return state.Frequency;
        }

        public void SetFrequency(int f)
        {
            state.Frequency = f;
        }

        public ICollection<Frequency> GetFrequencies()
        {
            return Constants.DMC_FREQUENCY_LIST;
        }

        #endregion
        #region cycle
        public int SetCycleNum(int num)
        {
            state.CycleNum = num;
            return num;
        }
        public int SetSwitch(int num)
        {
            state.Switch = num;
            return num;
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
            this.Disconnect();
        }
    }
}
