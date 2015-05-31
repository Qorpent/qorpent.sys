using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Xml.Linq;
using Qorpent;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.messaging.senders {
    [ContainerComponent(Lifestyle.Singleton,"smtp.sender",ServiceType=typeof(IMessageSender))]
    public class SmtpMessageSender : ServiceBase,IMessageSender {
        public SmtpMessageSender() {
            Registry = new Dictionary<string, SmtpConfig>();
        }

        [Inject]
        public IConfigProvider ConfigProvider { get; set; }

        public IDictionary<string, SmtpConfig> Registry { get; private set; }

        public override void OnContainerCreateInstanceFinished() {
            base.OnContainerCreateInstanceFinished();
            Initialize();
        }

        public void Initialize() {
            if (null != ConfigProvider) {
                var e = ConfigProvider.GetConfig();
                InitializeFromXml(e);
            }
        }

        public void InitializeFromXml(XElement e) {
            if (null != e) {
                e = e.Element("messaging");
            }
            if (null != e) {
                foreach (var element in e.Elements("smtp")) {
                    var conf = new SmtpConfig {
                        Code = element.Attr("code"),
                        From = element.Attr("name"),
                        Host = element.Attr("host"),
                        Port = element.Attr("port").ToInt(),
                        SslRequired = element.Attr("ssl").ToBool(),
                        User = element.Attr("user")
                    };

                    var _password = element.Attr("pass");
                    var p = new SecureString();
                    foreach (var c in _password) {
                        p.AppendChar(c);
                    }
                    conf.Password = p;
                    Registry[conf.Code] = conf;
                    Registry[conf.From] = conf;
                    if (e.Attr("default").ToBool()) {
                        Registry["default"] = conf;
                    }
                    
                }
            }
        }

        public void Send(PostMessage message) {
            lock (this) {
                SmtpConfig conf = null;
                if (!Registry.ContainsKey(message.From)) {
                    if (message.CanUseDefault && Registry.ContainsKey("default")) {
                        conf = Registry["default"];
                    }
                    else {
                        throw new Exception(message.From + " not configured");
                    }
                }
                else {
                    conf = Registry[message.From];
                }
                var mail = BuildMessage(message, conf);
                conf.SmtpClient.Send(mail);
            }
        }

        private static MailMessage BuildMessage(PostMessage message, SmtpConfig conf) {
            var m = new MailMessage {
                From = new MailAddress(conf.From),
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true,
                Body = message.Message,
                SubjectEncoding =  Encoding.UTF8,
                Subject = message.Subject
            };
            foreach (var address in message.Addresses) {
                m.Bcc.Add(new MailAddress(address));
            }
            
            return m;
        }
    }
}