using System;
using System.Collections.Generic;
using qorpent.v2.security.encryption;
using Qorpent;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.security.authentication {
    [ContainerComponent(Lifestyle.Singleton,"token.encryptor",ServiceType=typeof(ITokenEncryptor))]
    public class TokenEncryptor:ServiceBase,ITokenEncryptor {
        private Encryptor _encryptor;

        [Inject]
        public IConfigProvider ConfigProvider { get; set; }

        /// <summary>
        /// 
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

        private Encryptor GetEncryptor() {
            string key = "";
            bool setmachine = true;
            bool setdate = true;
            if (null != ConfigProvider) {
                var e = ConfigProvider.GetConfig();
                if (null != e) {
                    e = e.Element("logon");
                }
                if (null != e) {
                    e = e.Element("token");
                }
                if (null != e) {
                    key = e.Attr("key");
                    var defval = string.IsNullOrWhiteSpace(key) ? "true" : "false";
                    setmachine = e.Attr("machine", defval).ToBool();
                    setdate = e.Attr("date", defval).ToBool();
                }
            }
            var strings = new List<string>();
            if (!string.IsNullOrWhiteSpace(key)) {
                strings.Add(key);
            }
            if (setmachine) {
                strings.Add(Environment.MachineName);
            }
            if (setdate) {
                strings.Add(DateTime.Today.ToString("yyyyMMdd"));
            }
            var fullkey = string.Join("-", strings);
            return new Encryptor(fullkey);
        }

        public Token Decrypt(string srctoken) {
            string json = "";
            try {
                json = Encryptor.Decrypt(srctoken);
                var j = json.jsonify();
                var result = new Token {
                    User = j.str("User"),
                    ImUser = j.str("ImUser"),
                    Created = j.date("Created"),
                    Expire = j.date("Expire"),
                    Metrics = j.str("Metrics")
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

        
    }
}