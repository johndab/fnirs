using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using fNIRS.Hardware.Models;
using System.Runtime.InteropServices;

namespace fNIRS.Hardware.ISS
{
    public class DataReader
    {
        private Thread thread;
        private const int BUFFER_SIZE = 256;
        private NetworkStream stream;
        private Byte[] buffer = new Byte[BUFFER_SIZE];
        private DataPacket currentPacket;
        private int dataPacketSize = 0;
        private int packetIndex = 0;

        private bool streaming = false;
        private bool started = false;
        private bool blockStarted = false;
        private Action<DataPacket> newPacketAction;

        public bool IsAlive { get; private set; }
        private ILogger logger { get; }

        public DataReader(NetworkStream stream, ILogger logger)
        {
            this.stream = stream;
            this.logger = logger;
        }

        public void Start()
        {
            this.started = true;
            thread = new Thread(Run);
            thread.Start();
        }

        public void StartStreaming()
        {
            this.streaming = true;
            if (!started) Start();
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

        protected unsafe void Run()
        {
            int HEADER_SIZE = Marshal.SizeOf(typeof(HEADERDATA6));
            int CYCLE_SIZE = Marshal.SizeOf(typeof(CYCLEDATA6));
            int DC_IMAGE_SIZE = Marshal.SizeOf(typeof(DCrealimage_2D));


            while (thread.IsAlive && stream.CanRead && streaming)
            {
                if (!this.blockStarted)
                {
                    var startMessage = ReadTo(Constants.DMC_DATA_STARTS_TAG);
                    byte[] pattern = Encoding.ASCII.GetBytes(Constants.DMC_DATA_STARTS_TAG);
                    var index = PatternAt(buffer, pattern);
                    byte[] lineEnd = new byte[] { 13, 10 };
                    index = PatternAt(buffer, lineEnd, index) + 2;

                    if (startMessage.Length > 0)
                    {
                        this.currentPacket = GetPacketData(startMessage);
                        this.dataPacketSize = currentPacket.Size;
                        this.currentPacket.Data = new byte[this.dataPacketSize];
                        int len = buffer.Length - index;
                        Array.Copy(buffer, index, currentPacket.Data, 0, len);
                        currentPacket.DataIndex = len;
                        this.blockStarted = true;
                    } else {
                        logger.LogCritical("Invalid block start");
                        return;
                    }
                }
                else
                {
                    var bytes = stream.Read(buffer, 0, buffer.Length);
                    var len = Math.Min(currentPacket.Size - currentPacket.DataIndex, buffer.Length);
                    if(len > 0) {
                        Array.Copy(buffer, 0, currentPacket.Data, currentPacket.DataIndex, len);
                        currentPacket.DataIndex += len;
                    }

                    if(currentPacket.Header == null)
                    {
                        if(currentPacket.DataIndex > HEADER_SIZE)
                        {
                            currentPacket.Header = ByteArrayToStructure<HEADERDATA6>(currentPacket.Data, 0);
                            currentPacket.ReadIndex = HEADER_SIZE;
                        }
                    } 
                    else 
                    {
                        if(currentPacket.DataIndex - currentPacket.ReadIndex > CYCLE_SIZE) 
                        {
                            var cycle = ByteArrayToStructure<CYCLEDATA6>(currentPacket.Data, currentPacket.ReadIndex);
                            logger.LogCritical(
                                cycle.CycleNumber1.ToString() + " " + cycle.CycleNumber2.ToString() + "\n"
                                + cycle.MagicNumber1.ToString() + " " + cycle.MagicNumber2.ToString()
                            );
                            currentPacket.Cycles.Add(cycle);
                            currentPacket.ReadIndex += CYCLE_SIZE;
                        }
                    }

                    if(len < buffer.Length)
                    {
                        logger.LogCritical("Data packed full " + currentPacket.Index);
                        this.blockStarted = false;
                        if(newPacketAction != null)
                            newPacketAction.Invoke(this.currentPacket);

                        this.currentPacket = new DataPacket();
                        this.currentPacket.Data = new byte[this.dataPacketSize];

                        Array.Copy(buffer, len, currentPacket.Data, 0, buffer.Length - len);
                        currentPacket.DataIndex += buffer.Length - len;
                    }

                }
            }
        }

        public static int PatternAt(byte[] source, byte[] pattern, int start = 0)
        {
            for (int i = start; i < source.Length; i++)
            {
                if (source.Skip(i).Take(pattern.Length).SequenceEqual(pattern))
                {
                    return i;
                }
            }
            return -1;
        }

        private unsafe T ByteArrayToStructure<T>(byte[] bytes, int start) where T : struct
        {
            fixed (byte* ptr = &bytes[start])
            {
                return (T)Marshal.PtrToStructure((IntPtr)ptr, typeof(T));
            }
        }

        private DataPacket GetPacketData(string data)
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
                    result += chunk.Result;
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
