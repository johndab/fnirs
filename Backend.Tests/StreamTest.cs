using fNIRS.Hardware.ISS;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;

namespace Backend.Tests
{
    public class StreamTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void SetCycleNum()
        {
            using (var connection = new ISSConnection())
            {
                var adapter = new ISSAdapter(
                    Helper.GetIConfigurationRoot(),
                    Helper.GetILogger<ISSAdapter>()
                );

                adapter.Connect();

                var num = adapter.SetCycleNum(4);

                adapter.Disconnect();
            }
        }

        [Test]
        public void TestStreamStartStop()
        {
            using (var connection = new ISSConnection())
            {
                var adapter = new ISSAdapter(
                    Helper.GetIConfigurationRoot(),
                    Helper.GetILogger<ISSAdapter>()
                );

                adapter.Connect();

                var packets = 0;
                adapter.RegisterStreamListener((packet) =>
                {
                    packets += 1;
                });

                adapter.StartStreaming();
                Assert.IsTrue(adapter.IsStreaming());

                Thread.Sleep(1000);

                adapter.StopStreaming();
                Assert.IsFalse(adapter.IsStreaming());
                Assert.GreaterOrEqual(packets, 1);
                packets = 0;

                adapter.StartStreaming();
                Assert.IsTrue(adapter.IsStreaming());

                Thread.Sleep(1000);

                adapter.StopStreaming();
                Assert.IsFalse(adapter.IsStreaming());
                Assert.GreaterOrEqual(packets, 1);

                adapter.Disconnect();
                Assert.IsFalse(adapter.IsConnected());
            }
        }
    }
}