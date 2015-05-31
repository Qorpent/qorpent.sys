using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.messaging;
using Qorpent.Experiments;

namespace qorpent.v2.security.Tests.messaging
{
    [TestFixture]
    public class MessageQueueTest:BaseFixture
    {
        [Test]
        public void CanStoreAndGet() {
            var mq = _container.Get<IMessageQueue>();
            var message = new PostMessage {
                Addresses = new[] {"fagim.sadykov@gmail.com"},
                From = "support",
                Message = "<h1>Привет</h1>"
            };
            message = mq.PushMessage(message);
            Assert.Less(10,message.Id.Length);
            Assert.Less(1990,message.CreateTime.Year);
            Assert.Less(1990,message.StartTime.Year);

            var message2 = mq.GetMessage(message.Id);
            Console.WriteLine(message2.stringify());
            Console.WriteLine(message.stringify());
            Assert.AreEqual(message.stringify(),message2.stringify());
        }

        [Test]
        public void CanMarkSent() {
            var mq = _container.Get<IMessageQueue>();
            var message = new PostMessage
            {
                Addresses = new[] { "fagim.sadykov@gmail.com" },
                From = "support",
                Message = "<h1>Привет</h1>"
            };
            message = mq.PushMessage(message);
            mq.MarkSent(message.Id);
            var message2 = mq.GetMessage(message.Id);
            Assert.True(message2.WasSent);
            Assert.Greater(1,(DateTime.Now.ToUniversalTime()-message2.SentTime.ToUniversalTime()).TotalMinutes);
        }

        [Test]
        public void CanSearch() {
            var mq = _container.Get<IMessageQueue>();
            for (var i = 0; i < 10; i++)
            {
                var message = new PostMessage
                {
                    Id = "M" + i,
                    Addresses = new[] { "fagim.sadykov@gmail.com" },
                    From = "support",
                    Message = "<h1>Привет</h1>"
                };
                if (i%2 == 0) {
                    message.Tags = new Dictionary<string, object> {
                        {"be","cool"}
                    };
                }
                mq.PushMessage(message);
            }
            var search = mq.SearchMessages(@"{
  ""query"": {
    ""query_string"": {
      ""query"": ""cool""
    }
  }
}").ToArray();
            Assert.AreEqual(5,search.Length);
            search = mq.SearchMessages("cool").ToArray();
            Assert.AreEqual(5,search.Length);
        }

        [Test]
        public void CanDetectNotSent() {
            var mq = _container.Get<IMessageQueue>();
            for (var i = 0; i < 10; i++) {
                var message = new PostMessage
                {
                    Id = "M"+i,
                    Addresses = new[] { "fagim.sadykov@gmail.com" },
                    From = "support",
                    Message = "<h1>Привет</h1>"
                };
                if (i > 4) {
                    message.StartTime = DateTime.Now.AddDays(1).ToUniversalTime();
                }
                if (i == 2) {
                    message.WasSent = true;
                    message.SentTime = DateTime.Now.ToUniversalTime();
                }
                mq.PushMessage(message);
            }
            var needsend = mq.GetRequireSendMessages().ToArray();
            Assert.AreEqual(4,needsend.Length);
            Assert.AreEqual("M0,M1,M3,M4",string.Join(",",needsend.Select(_=>_.Id).OrderBy(_=>_)));
        }
    }
}
