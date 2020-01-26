using fNIRS.Hardware.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fNIRS.Memory
{
    public class MessageParser : IDisposable
    {
        private ConcurrentQueue<DataPacket> queue;
        private Thread thread;
        private ILogger<MessageParser> logger;
        private bool collecting;
        private string collectDirectory;
        private StreamWriter Writer;
        private string delim = ",";

        public Action<DataPacket> PacketAction { get; set; }

        public MessageParser(ILogger<MessageParser> logger, IConfiguration config)
        {
            this.logger = logger;
            this.collecting = false;
            this.collectDirectory = config.GetValue("CollectDirectory", "");
            this.queue = new ConcurrentQueue<DataPacket>();
        }

        public void Parse(DataPacket packet)
        {
            // Send to UI immidiately for real-time
            if (PacketAction != null)
                PacketAction.Invoke(packet);

            if(collecting)
                queue.Enqueue(packet);
        }

        public void CollectStart()
        {
            if (string.IsNullOrEmpty(collectDirectory)) return;
            
            var date = DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss");
            Writer = new StreamWriter(File.Open(collectDirectory + "/" + date + ".csv", FileMode.Append));
            Writer.AutoFlush = false;
            SaveHeader();

            this.collecting = true;
            this.thread = new Thread(Run);
            this.thread.Start();
        }

        public void CollectStop()
        {
            if (!collecting) return;
            this.collecting = false;
            thread.Join();
            Writer.Flush();
            Writer.Close();
            thread = null;
            queue.Clear();
        }

        public bool IsCollecting()
        {
            return this.collecting;
        }

        protected unsafe void Run()
        {
            try
            {
                while(collecting)
                {
                    if(queue.TryDequeue(out var message))
                    {
                        SaveToFile(message);
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            } 
            catch(Exception e)
            {
                logger.LogError(e, "Message Parser exception");
            }
        }

        private void SaveHeader()
        {
            var row = "Detector" + delim
                + "TimeMux" + delim
                + "FreqMux" + delim 
                + "AC" + delim 
                + "Phase" + delim 
                + "DC";

            Writer.WriteLine(row);
        }

        private void SaveToFile(DataPacket packet)
        {
            foreach (var cycle in packet.Cycles)
            {
                var j = 0;
                foreach (var measurement in cycle.Values)
                {
                    for (var i = 0; i < measurement.Imag.Length; i++)
                    {
                        var imag = measurement.Imag[i];
                        var real = measurement.Real[i];
                        var ac = Math.Sqrt(Math.Pow(imag, 2) + Math.Pow(real, 2));
                        var phase = Math.Atan2(imag, real);

                        var row = measurement.Detector + delim
                            + (j + 1).ToString() + delim
                            + (i + 1).ToString() + delim
                            + ac.ToString() + delim
                            + phase.ToString() + delim
                            + measurement.DC;

                        Writer.WriteLine(row);
                    }
                    j += 1;
                }
            }
            Writer.Flush();
        }

        public void Dispose()
        {
            try
            {
                Writer.Dispose();
            }
            catch(Exception)
            {
            }
        }
    }
}
