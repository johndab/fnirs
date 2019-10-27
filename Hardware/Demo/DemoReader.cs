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
        Thread thread;
        Action<DataPacket> action;

        private int index = 0;

        public void Start()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public void Stop()
        {
            thread.Abort();
            thread.Join();
        }

        public void Run() 
        {
            try
            {
                while (thread.IsAlive)
                {
                    var dataPacket = new DataPacket()
                    {
                        Index = index++,
                        Size = 1,
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
