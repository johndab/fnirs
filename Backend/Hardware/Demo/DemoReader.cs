using fNIRS.Hardware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace fNIRS.Hardware.Demo
{
    public class DemoReader
    {
        private Thread thread;
        private Action<DataPacket> action;
        private bool isRunning = false;

        private int index = 0;

        public void Start()
        {
            thread = new Thread(Run);
            isRunning = true;
            thread.Start();
        }

        public void Stop()
        {
            isRunning = false;
            thread.Join();
            thread = null;
        }

        public void Run() 
        {
            try
            {
                while (this.isRunning)
                {
                    var dataPacket = new DataPacket()
                    {
                        Index = index++,
                        Size = 1,
                        // Detectors = FillDetectors(),
                    };

                    if(action != null)
                        action.Invoke(dataPacket);

                    Thread.Sleep(500);
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("Reader stopped");
            }
        }

        // public ICollection<Detector> FillDetectors()
        // {
        //     var list = new List<Detector>();
        //     Random random = new Random();

        //     for(int i=0; i<100; i++)
        //     {
        //         list.Add(new Detector()
        //         {
        //             Address = i,
        //             Value = random.Next(100),
        //         });
        //     }

        //     return list;
        // }

        public void RegisterStreamListener(Action<DataPacket> action)
        {
            this.action = action;
        }
        public void RemoveStreamListener()
        {
            this.action = null;
        }
 
    }
}
