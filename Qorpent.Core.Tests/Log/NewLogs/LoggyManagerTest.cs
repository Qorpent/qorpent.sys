using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.Log.NewLogs {
    [TestFixture]
    public class LoggyManagerTest {
        private LoggyManager manager;

        [SetUp]
        public void Setup() {
            this.manager = new LoggyManager();
        }

        [Test]
        public void CanGetDefaultLogger() {
            var l = manager.Get();
            var l2 = manager.Get("default");
            var l3 = manager.Get();
            var l4 = manager.Get("default");
            Assert.True((l==l2)&&(l2==l3)&&(l3==l4));
        }

        [Test]
        public void CanGetExplicitlySetNamed() {
            var l = new DefaultLoggy();
            manager.Loggers["x"] = l;
            Assert.AreEqual(l,manager.Get("x"));

        }


        [Test]
        public void CanGetNamed() {
            var l = manager.Get("x");
            Assert.NotNull(l);
            var l2 = manager.Get("x");
            Assert.AreEqual(l,l2);
            var d = manager.Get();
            Assert.True(l.SubLoggers.Contains(d));
            Assert.True(l.SubLoggers.Count==1);

        }

    }

    [TestFixture]
    public class UdpAppenderTest
    {
        private UdpClient udpclient;
        private IPEndPoint sender;

        [SetUp]
        public void setup() {
            this.sender = new IPEndPoint(IPAddress.Any, 0);
            this.udpclient = new UdpClient(7072);
        }

        [TearDown]
        public void teardown() {
            udpclient.Close();
        }

        [Test]
        public void MainTest() {
            bool gotdata = false;
            string result = null;
            var appender = new UdpAppender("127.0.0.1", 7072) {AutoFlushSize = 1000};


                
                var task = Task.Run(() => {
                    var data = udpclient.Receive(ref sender);
                    var xml = XElement.Parse(System.Text.Encoding.UTF8.GetString(data));
                    Console.WriteLine(xml.ToString().Simplify(SimplifyOptions.SingleQuotes));
                    result = xml.ToString().Simplify(SimplifyOptions.Full);
                    gotdata = true;
                });
                appender.Write(new LoggyMessage(LogLevel.Error, "test") {LoggerName = "mylogger"});
                Thread.Sleep(100);
                Assert.False(task.IsCompleted);
                Assert.False(gotdata);
                appender.Flush();
                Thread.Sleep(300);
                Assert.True(gotdata);
                Assert.True(task.IsCompleted);
                Assert.True(result.Contains("<message>test</message>"));
                Assert.True(result.Contains("logger='mylogger'"));
                
        }

        [Test]
        public void AutoFlushTest() {
            bool gotdata = false;
            string result = null;
            var sender = new IPEndPoint(IPAddress.Any, 0);
            var appender = new UdpAppender("127.0.0.1", 7072) {AutoFlushSize = 3};


                var task = Task.Run(() => {
                    var data = udpclient.Receive(ref sender);
                    var xml = XElement.Parse(System.Text.Encoding.UTF8.GetString(data));
                    Console.WriteLine(xml.ToString().Simplify(SimplifyOptions.SingleQuotes));
                    result = xml.ToString().Simplify(SimplifyOptions.Full);
                    gotdata = true;
                });
                appender.Write(new LoggyMessage(LogLevel.Error, "test") {LoggerName = "mylogger"});
                Thread.Sleep(100);
                Assert.False(task.IsCompleted);
                Assert.False(gotdata);
                appender.Write(new LoggyMessage(LogLevel.Error, "test") {LoggerName = "mylogger"});
                Thread.Sleep(100);
                Assert.False(task.IsCompleted);
                Assert.False(gotdata);
                appender.Write(new LoggyMessage(LogLevel.Error, "test") {LoggerName = "mylogger"});
                Thread.Sleep(100);
                Assert.True(gotdata);
                Assert.True(task.IsCompleted);
             
        }
    }


}