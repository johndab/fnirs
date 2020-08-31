using Microsoft.Extensions.Configuration;
using fNIRS.Hardware.ISS;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using fNIRS.Memory;

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
            using (var connection = new ISSConnection(Helper.GetDMCExeFile()))
            {
                var config = Helper.GetIConfigurationRoot();
                var messageParser = new MessageParser(Helper.GetILogger<MessageParser>(), config);
                var adapter = new ISSAdapter(config, messageParser, Helper.GetILogger<ISSAdapter>());

                adapter.Connect();

                var num = adapter.SetCycleNum(4);
                Assert.AreEqual(4, num);

                adapter.Disconnect();
            }
        }

        [Test]
        public void TestStreamStartStop()
        {
            using (var connection = new ISSConnection(Helper.GetDMCExeFile()))
            {
                var config = Helper.GetIConfigurationRoot();
                var messageParser = new MessageParser(Helper.GetILogger<MessageParser>(), config);
                var adapter = new ISSAdapter(config, messageParser, Helper.GetILogger<ISSAdapter>());

                adapter.Connect();

                var packets = 0;

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