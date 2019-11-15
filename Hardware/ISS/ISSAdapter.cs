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

        private const int HELLO_TIMEOUT = 10000;
        private DataReader reader;


        public ISSAdapter(IConfiguration Configuration, ILogger<ISSAdapter> logger)
        {
            this.logger = logger;
            host = Configuration.GetValue<string>("ISSAdapter:host");
            port = Configuration.GetValue<int>("ISSAdapter:port");
        }

#region CONNECTION

        public Task Connect()
        {
            logger.LogDebug($"Connecting to ISS: {host}:{port}");

            try
            {
                this.client = new TcpClient(host, port);
                this.stream = client.GetStream();
                this.reader = new DataReader(stream, logger);

                Hello();
                this.connected = true;
            } 
            catch(Exception e)
            {
                logger.LogCritical(e, e.Message);
            }

            return Task.CompletedTask;
        }

        public void Hello()
        {
            var result = reader.ReadAvailable(HELLO_TIMEOUT);
            Console.WriteLine(result);
            if (string.IsNullOrEmpty(result))
                throw new ConnectionException($"Server does not responded in {HELLO_TIMEOUT} ms");

            if (result.IndexOf(Constants.DMC_SERVER_HELLO) != -1)
                return;

            throw new ConnectionException("Invalid hello message");
        }

        public async Task Disconnect()
        {
            if (connected)
            {
                try
                {
                    await Send(Constants.DMC_QUIT);
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
        public async Task StartStreaming()
        {
            this.reader.StartStreaming();
            Console.WriteLine("Streaming started");
            await Send(Constants.DMC_START);
        }

        public async Task StopStreaming()
        {
            await Send(Constants.DMC_STOP);
            this.reader.StopStreaming();
            reader.Interrupt();
            reader.Join();
        }

        public bool IsStreaming()
        {
            return reader.IsStreaming();
        }

        public void RegisterStreamListener(Action<DataPacket> action)
        {
            reader.RegisterStreamListener(action);
        }
        public void RemoveStreamListener()
        {
            reader.RemoveStreamListener();
        }
#endregion
#region FREQUENCIES

        public async Task<int> GetFrequency()
        {
            await Send(Constants.DMC_GET_BASE_RF_FREQ_BY_INDEX);
            var result = Read();
            result = result.Replace(Constants.DMC_GET_BASE_RF_FREQ_BY_INDEX_ACK, "");
            result = result.Replace(Constants.DMC_BASE_RF_FREQ_INDEX_TAG, "");
            var value = result.Split("\t")[2];

            return int.Parse(value);
        }

        public async Task SetFrequency(int f)
        {
            await Send(Constants.DMC_SET_BASE_RF_FREQ_BY_INDEX + " " + f.ToString());
            Read();
        }

        public ICollection<Frequency> GetFrequencies()
        {
            return Constants.DMC_FREQUENCY_LIST;
        }

#endregion



        public async Task<HardwareStatus> GetHardwareStatus()
        {
            await Send(Constants.DMC_GET_HARDWARE_STATUS);
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

        public async Task Send(string command)
        {
            if (!connected)
                throw new ConnectionException("Not connected!");

            Byte[] data = Encoding.ASCII.GetBytes(command+"\n");
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
        }

        private string Read()
        {
            if (!connected)
                throw new ConnectionException("Not connected!");

            return reader.ReadAvailable(10000);
        }

        public void Join()
        {
            reader.Join();
        }

        public void Dispose()
        {
            this.Disconnect().Wait();
        }

    }
}
