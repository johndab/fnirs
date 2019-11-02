using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using fNIRS.Hardware.Models;

namespace fNIRS.Hardware.ISS
{
    public class DataReader
    {
        private Thread thread;
        private int TIMEOUT = 1000;
        private const int BUFFER_SIZE = 256;
        private NetworkStream stream;
        private Byte[] buffer = new Byte[BUFFER_SIZE];
        private List<Byte> packet = new List<Byte>();
        private DataPacket currentPacket;

        private bool streaming = false;
        private bool started = false;
        private bool blockStarted = false;
        private Action<DataPacket> newPacketAction;

        public bool IsAlive { get; private set; }

        public DataReader(NetworkStream stream)
        {
            this.stream = stream;
        }

        public void Start()
        {
            this.started = true;
            thread = new Thread(Run);
            thread.Start();
        }

        public void StartStreaming()
        {
            if (!started) Start();
            this.streaming = true;
        }

        public void StopStreaming()
        {
            this.streaming = false;
        }
        
        public bool IsStreaming() {
            return this.streaming;
        }

        public void RegisterStreamListener(Action<DataPacket> action)
        {
            this.newPacketAction = action;
        }

        public void RemoveStreamListener()
        {
            this.newPacketAction = null;
        }

        protected void Run()
        {
            RunAsync().Wait();
        }

        protected async Task RunAsync()
        {
            while (thread.IsAlive && stream.CanRead)
            {
                if (!streaming)
                {
                    var chunk = GetChunk(TIMEOUT);
                    if (chunk != null)
                    {
                        Console.Write(chunk);
                    }
                }
                else
                {
                    if (!this.blockStarted)
                    {
                        var startMessage = ReadTo(Constants.DMC_BINARY_DATA_STARTS);

                        if (startMessage.Length > 0)
                        {
                            this.currentPacket = await GetPacketData(startMessage);
                            this.blockStarted = true;
                        }
                    }
                    else
                    {
                        var end = ReadTo(Constants.DMC_BINARY_DATA_ENDS);
                        if(newPacketAction != null)
                                newPacketAction.Invoke(this.currentPacket);
                    }
                }
            }
        }

        private async Task<DataPacket> GetPacketData(string data)
        {
            string[] rows = data.Split("\n");
            if (rows.Length == 0) return null;
            var row = rows.Where(r => r.IndexOf(Constants.DMC_DATA_STARTS_TAG) != -1).First();
            var list = row.Split(new string[] { "\n", "," }, StringSplitOptions.RemoveEmptyEntries);

            var size = int.Parse(list[0]
                .Replace(Constants.DMC_DATA_STARTS_TAG + "\t", "")
                .Replace("bytes", "")
                .Trim());

            var indexAndTime = list[1].Trim().Split(" ");
            var index = int.Parse(indexAndTime[1].Replace("#", ""));
            var time = indexAndTime[4].Replace("sent=", "");

            Console.WriteLine(row);
            return new DataPacket()
            {
                Index = index,
                Size = size,
                Time = time,
            };
            // 100 Data Starts:	339928 bytes, block #1  time sent=1.33000E-001
        }

        public string ReadTo(string key)
        {
            lock(this)
            {
                var result = string.Empty;
                while(true)
                {
                    var chunk = GetChunkAsync();
                    chunk.Wait();
                    result += chunk;
                    var index = result.IndexOf(key);
                    if (index > -1)
                    {
                        return result;
                    }
                }
            }
        }

        public async Task<string> GetChunkAsync()
        {
            int bytes = 0;
            try
            {
                bytes = await stream.ReadAsync(buffer, 0, buffer.Length);
            }
            catch (ObjectDisposedException)
            {
                return "";
            };
            return Encoding.ASCII.GetString(buffer, 0, bytes);
        }

        public string GetChunk(int timeout = -1)
        {
            var read = GetChunkAsync();
            read.Wait(timeout);
            if (read.Status == TaskStatus.RanToCompletion)
                return read.Result;

            return null;
        }

        public string ReadAvailable(int timeout = -1)
        {
            lock(this)
            {
                var result = GetChunk(timeout);
                if (result == null)
                    return string.Empty;

                while (stream.DataAvailable)
                {
                    var read = GetChunk(timeout);
                    if (read != null)
                        result += read;
                    else
                        break;
                }

                return result;
            }
        }

        public void Interrupt()
        {
            this.IsAlive = false;
        }

        public void Join()
        {
            thread.Join();
        }
    }
}
