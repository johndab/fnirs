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
        private const int BUFFER_SIZE = 1024;
        private byte[] LINE_END = new byte[] { 13, 10 };

        private Thread thread;
        private NetworkStream stream;

        private byte[] buffer = new byte[BUFFER_SIZE];
        private byte[] packetBuffer = null;
        private int dataIndex = 0;
        private int readIndex = 0;

        private DataPacket currentPacket;

        private bool streaming = false;
        private bool blockStarted = false;
        private Action<DataPacket> newPacketAction;

        private ILogger logger { get; }
        private Stopwatch sw = new Stopwatch();

        public DataReader(NetworkStream stream, ILogger logger)
        {
            this.stream = stream;
            this.logger = logger;
        }

        public void StartStreaming()
        {
            if (!this.streaming)
            {
                this.streaming = true;
                thread = new Thread(Run);
                thread.Start();
            }
        }

        public void StopStreaming()
        {
            this.streaming = false;
            this.thread.Join();
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
        
        private void SkipInvalidBlock()
        {
            if (!streaming) return;
            logger.LogCritical("Invalid block start, skipping");
            FillBuffer();
        }

        private void TimeStart()
        {
            // start stopwatch
            sw.Start();
        }

        private void TimeTick(int index)
        {
            if (index != 0 && (currentPacket.Index % 100 == 0))
            {
                sw.Stop();
                Console.WriteLine("Elapsed = {0}", sw.Elapsed.Milliseconds);
                var num = (1000.0 / (sw.Elapsed.Milliseconds / 100));
                Console.WriteLine("{0}/sec ", num);
                sw.Start();
            }
        }

        protected unsafe void Run()
        {
            int HEADER_SIZE = Marshal.SizeOf(typeof(HEADERDATA6));
            int CYCLE_SIZE = Marshal.SizeOf(typeof(CYCLEDATA6));
            TimeStart();

            byte[] dataStarts = Encoding.ASCII.GetBytes(Constants.DMC_DATA_STARTS_TAG);

            try
            {

                FillBuffer();

                while (stream.CanRead && streaming)
                {
                    if (!this.blockStarted)
                    {
                        var index = PatternAt(buffer, dataStarts);
                        if (index == -1)
                        {
                            SkipInvalidBlock();
                            continue;
                        }

                        this.currentPacket = GetPacketData(index);
                        if (currentPacket == null)
                        {
                            SkipInvalidBlock();
                            continue;
                        }

                        index = PatternAt(buffer, LINE_END, index) + 2;

                        // Reuse packet buffer
                        if (packetBuffer == null || packetBuffer.Length > currentPacket.Size)
                        {
                            this.packetBuffer = new byte[this.currentPacket.Size];
                        }
                        this.dataIndex = 0;
                        this.readIndex = 0;

                        // copy the rest of bytes to the packet data array
                        int len = buffer.Length - index;
                        Array.Copy(buffer, index, packetBuffer, 0, len);
                        dataIndex = len;
                        this.blockStarted = true;
                    }
                    else
                    {
                        var bytes = FillBuffer();
                        if (bytes == 0 && !streaming) return;
                        var len = Math.Min(currentPacket.Size - dataIndex, buffer.Length);

                        // If len > 0, there's still some packet bytes to be filled
                        if (len > 0)
                        {
                            Array.Copy(buffer, 0, packetBuffer, dataIndex, len);
                            dataIndex += len;
                        }

                        if (currentPacket.Header == null)
                        {
                            if (dataIndex >= HEADER_SIZE)
                            {
                                HEADERDATA6 header = packetBuffer.ToStructure<HEADERDATA6>(0);
                                currentPacket.Header = header.ToModel();
                                readIndex = HEADER_SIZE;
                            }
                        }
                        else
                        {
                            if (dataIndex - readIndex >= CYCLE_SIZE)
                            {
                                var cycle = packetBuffer.ToStructure<CYCLEDATA6>(readIndex);

                                var cycleData = cycle.ToModel();
                                currentPacket.Cycles.Add(cycleData);
                                readIndex += CYCLE_SIZE;
                            }
                        }

                        if (len < buffer.Length)
                        {
                            this.blockStarted = false;
                            if (newPacketAction != null)
                                newPacketAction.Invoke(this.currentPacket);

                            TimeTick(currentPacket.Index);

                            // Find line end after "Data End:" message
                            var msg = Encoding.ASCII.GetString(buffer, len, (buffer.Length - len));
                            var index = msg.IndexOf(Constants.DMC_DATA_ENDS_TAG);
                            index = PatternAt(buffer, LINE_END, index) + 2;


                            // TODO: Compare that to creating new byte array and swapping references
                            var temp = new byte[(buffer.Length - index)];
                            Array.Copy(buffer, index, temp, 0, (buffer.Length - index));
                            Array.Copy(temp, 0, buffer, 0, temp.Length);

                            FillBuffer((buffer.Length - index));
                        }
                    }
                }

                var trash = ReadAvailable();
            }
            catch(Exception e)
            {
                logger.LogError(e, "Connection broken");
            }


            this.blockStarted = false;
            this.currentPacket = null;
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
        private DataPacket GetPacketData(int start)
        {
            string data = Encoding.ASCII.GetString(buffer, start, 100);
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

        public int FillBuffer(int from = 0)
        {
            lock(this)
            {
                var read = from;
                while ((read < buffer.Length) && streaming)
                {
                    if(stream.DataAvailable)
                    {
                        read += stream.Read(buffer, read, (buffer.Length - read));
                    }
                    else
                    {
                        Thread.Sleep(10);
                    }
                }
                return read;
            }
        }

        public string GetChunk()
        {
            int bytes = stream.Read(buffer, 0, buffer.Length);
            if (bytes <= 0)
                return "";

            return Encoding.ASCII.GetString(buffer, 0, bytes);
        }

        public string ReadAvailable(int timeout = -1)
        {
            lock(this)
            {
                var currTimeout = stream.ReadTimeout;
                if(timeout != -1)
                {
                    stream.ReadTimeout = timeout;
                }

                try
                {
                    var result = GetChunk();
                    if (result == null)
                        return string.Empty;

                    while (stream.DataAvailable)
                    {
                        var read = GetChunk();
                        result += read;
                    }

                    return result;
                }
                finally
                {
                    stream.ReadTimeout = currTimeout;
                }
            }
        }
    }
}
