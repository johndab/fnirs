using fNIRS.Hardware.ISS;
using fNIRS.Memory;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;

namespace Backend.Tests
{
    public class ConnectionTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void TestConnectionStartStop()
        {
            using (var connection = new ISSConnection(Helper.GetDMCExeFile()))
            {
                var config = Helper.GetIConfigurationRoot();
                var messageParser = new MessageParser(Helper.GetILogger<MessageParser>(), config);
                var adapter = new ISSAdapter(config, messageParser, Helper.GetILogger<ISSAdapter>());

                adapter.Connect();
                Assert.IsTrue(adapter.IsConnected());
                Thread.Sleep(1000);

                adapter.Disconnect();
                Assert.IsFalse(adapter.IsConnected());
                Thread.Sleep(1000);

                adapter.Connect();
                Assert.IsTrue(adapter.IsConnected());
                Thread.Sleep(1000);

                adapter.Disconnect();
                Assert.IsFalse(adapter.IsConnected());
            }
        }
    }
}