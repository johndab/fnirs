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
using fNIRS.Hardware.ISS.Converters;
using System.Diagnostics;

namespace fNIRS.Hardware.ISS
{
    public class DataReader
    {
        private const int BUFFER_SIZE = 256;
        private byte[] LINE_END = new byte[] { 13, 10 };

        private Thread thread;
        private NetworkStream stream;

        private byte[] buffer = new byte[BUFFER_SIZE];
        private DataPacket currentPacket;

        private bool streaming = false;
        private bool started = false;
        private bool blockStarted = false;
        private Action<DataPacket> newPacketAction;

        public bool IsAlive { get; private set; }
        private ILogger logger { get; }
        private Stopwatch sw = new Stopwatch();

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

            FillBuffer();
            
            while (thread.IsAlive && stream.CanRead)
            {
                if (!this.blockStarted)
                {
                    // Read the response from the server 
                    var startMessage = ReadTo(Constants.DMC_DATA_STARTS_TAG);
                    byte[] pattern = Encoding.ASCII.GetBytes(Constants.DMC_DATA_STARTS_TAG);
                    var index = PatternAt(buffer, pattern);
                    index = PatternAt(buffer, LINE_END, index);

                    if (index > 0)
                    {
                        index += 2;
                        // start stopwatch
                        sw.Start();

                        this.currentPacket = GetPacketData(startMessage);
                        this.currentPacket.Data = new byte[this.currentPacket.Size];
                        
                        // copy the rest of bytes to the packet data array
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
                    var bytes = FillBuffer();
                    var len = Math.Min(currentPacket.Size - currentPacket.DataIndex, buffer.Length);
                    if(len > 0) {
                        Array.Copy(buffer, 0, currentPacket.Data, currentPacket.DataIndex, len);
                        currentPacket.DataIndex += len;
                    }

                    if(currentPacket.Header == null)
                    {
                        if(currentPacket.DataIndex > HEADER_SIZE)
                        {
                            currentPacket.Header = currentPacket.Data.ToStructure<HEADERDATA6>(0);
                            currentPacket.ReadIndex = HEADER_SIZE;
                        }
                    } 
                    else 
                    {
                        if(currentPacket.DataIndex - currentPacket.ReadIndex >= CYCLE_SIZE) 
                        {
                            var cycle = currentPacket.Data.ToStructure<CYCLEDATA6>(currentPacket.ReadIndex);

                            var cycleData = cycle.ToModel();
                            currentPacket.Cycles.Add(cycleData);

                            currentPacket.ReadIndex += CYCLE_SIZE;
                        }
                    }

                    if(len < buffer.Length)
                    {
                        this.blockStarted = false;
                        if(newPacketAction != null)
                            newPacketAction.Invoke(this.currentPacket);

                        sw.Stop();
                        Console.WriteLine("Elapsed = {0}", sw.Elapsed.Milliseconds);
                        var num = (1000.0 / sw.Elapsed.Milliseconds);
                        Console.WriteLine("{0}/sec: ", num);

                        // Find line end after "Data End:" message
                        var msg = Encoding.ASCII.GetString(buffer, len, (buffer.Length-len));
                        var index = msg.IndexOf(Constants.DMC_DATA_ENDS_TAG);
                        index = PatternAt(buffer, LINE_END, index) + 2;

                        var i = 0;
                        while(index < buffer.Length)
                        {
                            buffer[i] = buffer[index];
                            i += 1;
                            index += 1;
                        }
                        FillBuffer(i);
                        var test = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
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

        /** Extract useful informations from the server response 
        *  eg.
        *  100 Data Starts:	339928 bytes, block #1  time sent=1.33000E-001
        */
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

            return new DataPacket()
            {
                Index = index,
                Size = size,
                Time = time,
            };
            
        }

        private string ReadTo(string key)
        {
            lock(this)
            {
                var result = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
                while(true)
                {
                    if (result.IndexOf(key) > -1)
                        return result;
                
                    var chunk = GetChunkAsync();
                    chunk.Wait();
                    result += chunk.Result;
                }
            }
        }

        public int FillBuffer(int from = 0)
        {
            lock(this)
            {
                var read = from;
                while (read + 1 < buffer.Length)
                {
                    read += stream.Read(buffer, read, (buffer.Length - read));
                }
                return read;
            }
        }

        public async Task<int> FillBufferAsync(int from = 0)
        {
            return await stream.ReadAsync(buffer, from, (buffer.Length-from));
        }

        public async Task<string> GetChunkAsync()
        {
            int bytes = 0;
            try
            {
                bytes = await FillBufferAsync();
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

                    //if (read.IndexOf(Constants.DMC_Virtual_Secondary_DDS_System) > -1)
                    //    return result;
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
