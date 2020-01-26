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
using fNIRS.Memory;

namespace fNIRS.Hardware.ISS
{
    public class ISSAdapter : IAdapter
    {

        private TcpClient client;
        private NetworkStream stream;
        private readonly ILogger<ISSAdapter> logger;

        private readonly string host;
        private readonly int port;
        private bool connected = false;

        private const int HELLO_TIMEOUT = 2000;
        private DataReader reader;
        private MessageParser messageParser;

        public ISSAdapter(IConfiguration Configuration, MessageParser messageParser, ILogger<ISSAdapter> logger)
        {
            this.logger = logger;
            this.messageParser = messageParser;
            host = Configuration.GetValue<string>("ISSAdapter:host");
            port = Configuration.GetValue<int>("ISSAdapter:port");
        }

#region CONNECTION

        public void Connect()
        {
            logger.LogDebug($"Connecting to ISS: {host}:{port}");

            try
            {
                this.client = new TcpClient(host, port);
                this.stream = client.GetStream();
                stream.ReadTimeout = 100;
                this.reader = new DataReader(stream, messageParser, logger);

                Thread.Sleep(100);
                Hello();
                this.connected = true;
            } 
            catch(Exception e)
            {
                logger.LogCritical(e, e.Message);
            }
        }

        public void Hello()
        {
            string result = string.Empty;
            var task = Task.Run(() =>
            {
                result = reader.ReadAvailable();
            });

            task.Wait(HELLO_TIMEOUT);

            if (task.Status != TaskStatus.RanToCompletion)
                throw new ConnectionException($"Server does not responded in {HELLO_TIMEOUT} ms");

            if (result.IndexOf(Constants.DMC_SERVER_HELLO) != -1)
                return;

            throw new ConnectionException("Invalid hello message");
        }

        public void Disconnect()
        {
            if (connected)
            {
                try
                {
                    StopStreaming();
                }
                catch 
                { }

                this.reader = null;

                try
                {
                    Send(Constants.DMC_QUIT);
                } 
                finally
                {
                    try
                    {
                        client.GetStream().Close();
                        client.Close();
                    }
                    finally
                    {
                        client = null;
                        stream = null;
                        this.connected = false;
                    }
                }
            }
        }

        public bool IsConnected()
        {
            return this.connected;
        }

#endregion
#region STREAMING
        public void StartStreaming()
        {
            Send(Constants.DMC_START);
            this.reader.StartStreaming();
        }

        public void StopStreaming()
        {
            Send(Constants.DMC_STOP);
            this.reader.StopStreaming();
        }

        public bool IsStreaming()
        {
            if(reader == null) return false;
            return reader.IsStreaming();
        }

#endregion
#region FREQUENCIES

        public int GetFrequency()
        {
            Send(Constants.DMC_GET_BASE_RF_FREQ_BY_INDEX);
            
            var result = Read();
            result = result.Replace(Constants.DMC_GET_BASE_RF_FREQ_BY_INDEX_ACK, "");
            result = result.Replace(Constants.DMC_BASE_RF_FREQ_INDEX_TAG, "");
            var value = result.Split("\t")[2];

            return int.Parse(value);
        }

        public void SetFrequency(int f)
        {
            Send(Constants.DMC_SET_BASE_RF_FREQ_BY_INDEX + " " + f.ToString());
            Read();
        }

        public ICollection<Frequency> GetFrequencies()
        {
            return Constants.DMC_FREQUENCY_LIST;
        }

        #endregion
        #region cycles

        public int SetCycleNum(int num)
        {
            Send(Constants.DMC_NUM_CYCLES_PER_DATA_BLOCK + " " + num.ToString());

            var result = Read();
            result = result.Replace(Constants.DMC_NUM_CYCLES_PER_DATA_BLOCK_ACK, "");
            var values = result.Split("\t");

            return int.Parse(values[1]);
        }

        public int SetSwitch(int num)
        {
            if (num == 8)
            {
                Send(Constants.DMC_SET_SWITCH_MODE_8);
            }
            else if (num == 16)
            {
                Send(Constants.DMC_SET_SWITCH_MODE_16);
            }
            else if (num == 32)
            {
                Send(Constants.DMC_SET_SWITCH_MODE_32);
            }

            // Read and ignore
            Read();

            return num;
        }

        #endregion


        public HardwareStatus GetHardwareStatus()
        {
            Send(Constants.DMC_GET_HARDWARE_STATUS);
            var result = Read();

            var dict = ToDictionary(result, Constants.HardwareStatusPrexies);
            var status = new HardwareStatus();
            return status.FromDictionary(dict);
        }

        private IDictionary<string, string> ToDictionary(string data, string[] prefixes)
        {
            string[] rows = data.Split('\n');
            var result = new Dictionary<string, string>();

            foreach(var row in rows)
            {
                foreach(var prefix in prefixes)
                {
                    if(row.IndexOf(prefix) != -1)
                    {
                        var value = row.Replace(prefix, "").Replace("   ", "").Trim();
                        result.Add(prefix, value);
                        break;
                    }
                }
            }

            return result;
        }

        public void Send(string command)
        {
            if (!connected)
                throw new ConnectionException("Not connected!");

            byte[] data = Encoding.ASCII.GetBytes(command+"\n");
            stream.Write(data, 0, data.Length);
            stream.Flush();
            
            // Necessary time for the response 
            Thread.Sleep(100);
        }

        private string Read(int timeout = -1)
        {
            if (!connected)
                throw new ConnectionException("Not connected!");

            return reader.ReadAvailable(timeout);
        }

        public void Dispose()
        {
            this.Disconnect();
        }

    }
}
