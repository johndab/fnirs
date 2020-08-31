using fNIRS.Hardware.ISS;
using NUnit.Framework;
using System;
using fNIRS.Memory;
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
                var config = Helper.GetIConfigurationRoot();
                var messageParser = new MessageParser(Helper.GetILogger<MessageParser>(), config);
                var adapter = new ISSAdapter(config, messageParser, Helper.GetILogger<ISSAdapter>());

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