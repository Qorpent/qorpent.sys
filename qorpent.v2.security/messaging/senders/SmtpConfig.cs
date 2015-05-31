using System.Net;
using System.Net.Mail;
using System.Security;

namespace qorpent.v2.security.messaging.senders {
    public class SmtpConfig {
        private SmtpClient _smtpClient;
        public string Code { get; set; }
        public string From { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public SecureString Password { get; set; }
        public bool SslRequired { get; set; }

        public SmtpClient SmtpClient {
            get { return _smtpClient ?? (_smtpClient = BuildSmtpClient()); }
            set { _smtpClient = value; }
        }

        private SmtpClient BuildSmtpClient() {
            var result = new SmtpClient {
                Host = Host,
                Port = Port,
                EnableSsl = SslRequired,
                Credentials = new NetworkCredential(User, Password)
            };
            return result;
        }
    }
}