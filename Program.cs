using MiBrain.ISS;
using MiBrain.ISS.Exceptions;
using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MiBrain
{
    class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        static async Task Run()
        {
            try
            {
                using (var adapter = new Adapter("localhost", 5556))
                {
                    Console.WriteLine("CONNECTED!");
                    //var status = await adapter.GetHardwareStatus();

                    await adapter.StartStream();

                    while (true)
                    {
                        var command = Console.ReadLine();
                        if (command.ToUpper() == "QUIT")
                        {
                            break;
                        }
                        //await adapter.Send(command);
                    }

                    await adapter.StopStream();

                    adapter.StopReader();
                    await adapter.Disconnect();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
