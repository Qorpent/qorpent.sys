using System;
using System.Collections.Generic;
using System.Xml.Linq;
using qorpent.v2.security.encryption;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton, "token.encryptor", ServiceType = typeof (ITokenEncryptor))]
    public class TokenEncryptor : InitializeAbleService, ITokenEncryptor {
        private Encryptor _encryptor;

        /// <summary>
        /// </summary>
        public Encryptor Encryptor {
            get {
                if (null == _encryptor) {
                    lock (this) {
                        if (null == _encryptor) {
                            _encryptor = GetEncryptor();
                        }
                    }
                }
                return _encryptor;
            }
            set { _encryptor = value; }
        }

        public bool SetDate { get; set; }
        public bool SetMachine { get; set; }
        public string BaseKey { get; set; }

        public Token Decrypt(string srctoken) {
            var json = "";
            try {
                json = Encryptor.Decrypt(srctoken);
                var j = json.jsonify();
                var result = new Token {
                    User = j.str("User"),
                    ImUser = j.str("ImUser"),
                    Created = j.date("Created"),
                    Expire = j.date("Expire"),
                    Metrics = j.str("Metrics"),
                    IsAdmin = j.bul("IsAdmin")
                };
                return result;
            }
            catch (Exception e) {
                return null;
            }
        }

        public string Encrypt(Token token) {
            return Encryptor.Encrypt(token.stringify());
        }

        public override void InitializeFromXml(XElement e) {
            base.InitializeFromXml(e);
            e = e.Element("logon");
            if (null != e) {
                e = e.Element("token");
            }
            if (null != e) {
                BaseKey = e.Attr("key");
                var defval = string.IsNullOrWhiteSpace(BaseKey) ? "true" : "false";
                SetMachine = e.Attr("machine", defval).ToBool();
                SetDate = e.Attr("date", defval).ToBool();
            }
        }

        private Encryptor GetEncryptor() {
            var strings = new List<string>();
            if (!string.IsNullOrWhiteSpace(BaseKey)) {
                strings.Add(BaseKey);
            }
            if (SetMachine) {
                strings.Add(Environment.MachineName);
            }
            if (SetDate) {
                strings.Add(DateTime.Today.ToString("yyyyMMdd"));
            }
            var fullkey = string.Join("-", strings);
            return new Encryptor(fullkey);
        }
    }
}