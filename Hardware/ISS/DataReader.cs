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

        protected void Run()
        {
            RunAsync().Wait();
        }

        public async Task<DataPacket> GetPacketData(string data)
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
                        var startRead = ReadTo(Constants.DMC_BINARY_DATA_STARTS);
                        startRead.Wait();
                        var startMessage = startRead.Result;

                        if (startMessage.Length > 0)
                        {
                            var p = GetPacketData(startMessage);
                            p.Wait();
                            currentPacket = p.Result;

                            this.blockStarted = true;
                        }
                    }
                    else
                    {
                        var endRead = ReadTo(Constants.DMC_BINARY_DATA_ENDS);
                        endRead.Wait();
                        var end = endRead.Result;
                    }
                }
            }
        }

        public async Task<string> ReadTo(string key)
        {
            var result = string.Empty;
            while(true)
            {
                var chunk = await GetChunkAsync();
                result += chunk;
                var index = result.IndexOf(key);
                if (index > -1)
                {
                    return result;
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
