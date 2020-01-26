using fNIRS.Hardware.ISS;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;

namespace Backend.Tests
{
    public class FrequencyTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestFrequencyGetSet()
        {

            using (var connection = new ISSConnection(Helper.GetDMCExeFile()))
            {
                var adapter = new ISSAdapter(
                    Helper.GetIConfigurationRoot(),
                    Helper.GetILogger<ISSAdapter>()
                );

                adapter.Connect();
                var freq = adapter.GetFrequency();
                var list = adapter.GetFrequencies();

                Thread.Sleep(1000);
                adapter.Disconnect();
                Assert.IsFalse(adapter.IsConnected());
            }
        }
    }
}