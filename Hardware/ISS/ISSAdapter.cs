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

        private const int HELLO_TIMEOUT = 10000;
        private DataReader reader;


        public ISSAdapter(IConfiguration Configuration, ILogger<ISSAdapter> logger)
        {
            this.logger = logger;
            host = Configuration.GetValue<string>("ISSAdapter:host");
            port = Configuration.GetValue<int>("ISSAdapter:port");
        }

        public void Connect()
        {
            logger.LogDebug($"Connecting to ISS: {host}:{port}");

            client = new TcpClient(host, port);
            stream = client.GetStream();
            reader = new DataReader(stream);
            Hello();
        }

        public void StartReader()
        {
            reader.Start();
        }

        public void StopReader()
        {
            reader.Interrupt();
            reader.Join();
        }

        public ICollection<Frequency> GetFrequencies()
        {
            return Constants.DMC_FREQUENCY_LIST;
        }

        public void Hello()
        {
            try
            {
                var result = reader.GetChunk(HELLO_TIMEOUT);
                if (result == null)
                    throw new ConnectionException($"Server does not responded in {HELLO_TIMEOUT} ms");

                while(stream.DataAvailable)
                {
                    var read = reader.GetChunk(HELLO_TIMEOUT);
                    if (read != null)
                        result += read;
                    else
                        break;
                }

                if (result.IndexOf(Constants.DMC_SERVER_HELLO) != -1)
                    return;
            }
            catch (IOException) {}
            throw new ConnectionException("Invalid hello message");
        }

        public async Task StartStream()
        {
            this.reader.StartStreaming();
            await Send(Constants.DMC_START);
        }

        public async Task StopStream()
        {
            await Send(Constants.DMC_STOP);
            this.reader.StopStreaming();

        }

        public async Task<HardwareStatus> GetHardwareStatus()
        {
            await Send(Constants.DMC_GET_HARDWARE_STATUS);
            var result = await Read();

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
            Byte[] data = Encoding.ASCII.GetBytes(command+"\n");
            await stream.WriteAsync(data, 0, data.Length);
            await stream.FlushAsync();
        }

        private async Task<string> Read()
        {
            var result = string.Empty;

            while(stream.DataAvailable)
            {
                result += await reader.GetChunkAsync();
            }

            return result;
        }

        public void Join()
        {
            reader.Join();
        }

        public async Task Disconnect()
        {
            await Send(Constants.DMC_QUIT);
        }

        public void Dispose()
        {
            client.Close();
            stream.Close();
        }
    }
}
