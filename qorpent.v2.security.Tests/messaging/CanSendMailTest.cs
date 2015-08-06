using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using qorpent.v2.security.messaging;
using qorpent.v2.security.messaging.senders;
using Qorpent;
using Qorpent.Bxl;

namespace qorpent.v2.security.Tests.messaging
{
    [TestFixture]
    [Explicit]
    public class CanSendMailTest
    {
       
        [Test]
        [Explicit]
        public void CanSendMail() {
            var smtpsender = new SmtpMessageSender();
            var conf = new BxlParser().Parse(File.ReadAllText(EnvironmentInfo.ResolvePath("@repos@/zrepos/mail.bxls")),options:BxlParserOptions.ExtractSingle);
            smtpsender.InitializeFromXml(conf);
            var message = new PostMessage();
            message.From = "ivan";
            message.Addresses = new[] {"fagim.sadykov@gmail.com"};
            message.Body = "<h1>Привет!</h1>";
            smtpsender.Send(message);
        }
    }
}
