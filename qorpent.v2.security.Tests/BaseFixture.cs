using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using qorpent.v2.security.logon;
using qorpent.v2.security.logon.services;
using qorpent.v2.security.Tests.user;
using qorpent.v2.security.user;
using Qorpent;
using Qorpent.Bxl;
using Qorpent.Experiments;
using Qorpent.IoC;
using Qorpent.IO.Net;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using IOException = Qorpent.IO.IOException;

namespace qorpent.v2.security.Tests
{
    public class BaseFixture {
        protected const string Fpass = "A123456$";
        protected const string FApass = "C123456$";
        protected const string Epass = "B123456$";
        protected static bool _envinitialized = false;
        protected Container _container;

        protected static string mainpwd;

        protected string esindex = "v2securetest";
        protected bool ignoreelastic = false;
        protected static XElement config = null;

        class configprovider : IConfigProvider {
            public XElement GetConfig() {
                return config;
            }
        }
        [SetUp]
        public virtual void Setup() {
            lock (this) {
                if (!_envinitialized) {
                    InitializeEnvironment();
                    _envinitialized = true;
                }
                _container = new Container();
                _container.GetLoader().LoadAssembly(typeof (ILogonService).Assembly);
                _container.Register(_container.NewComponent<IConfigProvider, configprovider>());
                Loggy.Default.Level = LogLevel.All;
                if (Loggy.Default.Appenders.Count == 0) {
                    Loggy.Default.Appenders.Add(new ConsoleAppender {
                        Format = "${Minute}:${Second} ${Message}",
                        Level = LogLevel.All
                    });
                }
            }
        }

        private void InitializeEnvironment() {
            mainpwd = Path.Combine(FileSystemHelper.ResetTemporaryDirectory(), "pwd.bxls");
            var fileuser = new User {
                Login = "fuser"
            };
            var fadm = new User
            {
                Login = "fadm"
            };
            var pwd = new PasswordManager();
            pwd.SetPassword(fileuser, Fpass, salt: "fuser_salt");
            pwd.SetPassword(fadm, FApass, salt: "fadm_salt");

            var esuser = new User {
                Login = "esuser",
                Logable = true,
                Active = true,
                Expire = new DateTime(2020, 1, 1).ToUniversalTime(),
                Id="esuser",
                CreateTime = new DateTime(2015,1,1).ToUniversalTime(),
                UpdateTime = new DateTime(2015,1,1).ToUniversalTime(),
                Roles=new []{"role1"}
            };
            pwd.SetPassword(esuser, Epass, salt: "esuser_salt");
            ExecuteCommand("http://127.0.0.1:9200/" + esindex, method: "DELETE");
            var storeesresult = ExecuteCommand("http://127.0.0.1:9200/" + esindex + "/pwd/esuser",
                esuser.ToString("store"));
            if (null == storeesresult) {
                ignoreelastic = true;
            }
            var bxl = string.Format(@"
class u1 prototype=pwd
    usr fuser salt='{0}' hash='{1}' logable active expire='2020-01-01T00:00:00+0000'
        role role2
    usr fadm salt='{2}' hash='{3}' logable isadmin active expire='2020-01-01T00:00:00+0000'
        role role2
", fileuser.Salt,fileuser.Hash,fadm.Salt,fadm.Hash);

            File.WriteAllText(mainpwd, bxl);

            var configbxl = @"
class app
    cluster
    logon
        default schema=localtrust guest 
            trusted sense local=192.168.0.3 origin='http://127.0.0.1:9200' port=3450
            trusted controller remote=192.168.0.2 local=192.168.0.3
        token cookie=testauth secure key=1234567 
        elastic index='v2securetest'
        file path='TEST'
";
            var xml = new BxlParser().Parse(configbxl, options: BxlParserOptions.ExtractSingle);
            xml.Element("logon").Element("file").SetAttributeValue("path",mainpwd);
            if (ignoreelastic) {
                xml.Element("logon").Element("elastic").SetAttributeValue("active","false");
            }

            config = xml;
        }

        public void CheckRate(Action<int> action, int count = 10000, string message="") {
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < count; i++) {
                action(i);
            }
            sw.Stop();
            Console.WriteLine(message);
            Console.WriteLine(count+" s/ds cycles gain " + sw.Elapsed);

            Console.WriteLine((count / sw.Elapsed.TotalSeconds) + " per second ");
        }

        public static User GetUser(int idx) {
            return GetUser<User>(idx);
        }
        public static T GetUser<T>(int idx)where T:User,new() {
            var pwd = new PasswordManager();
            var user = new T {
                Login = "login" + idx,
                Name = "myname" + idx,
                IsAdmin = idx%5 == 0,
                Logable = idx%3 == 0,
                IsGroup = idx%4 == 0,
                Domain = "master" + idx,
                Roles = new[] {"role1_" + idx, "role2_" + idx},
                Groups = new[] {"grp1_" + idx, "grp2_" + idx},
                Email = "email" + idx,
            };
            user.ResetExpire = user.ResetExpire.ToUniversalTime();
            pwd.SetPassword(user,"Asdf1_32",salt:"salt1");
            user.PublicKey =
                Convert.ToBase64String(
                    typeof (UserSerializationTest).Assembly.ReadManifestResourceBytes("public.cer"));
            user.Custom = new {a = "1_" + idx, b = "2_" + idx}.jsonify() as IDictionary<string, object>;
            user.Expire = new DateTime(2016, 1, 1).AddMonths(idx);
            return user;
        }

        protected string ExecuteCommand(string url, string post = null,string method = "")
        {
            string json;
            try
            {
                var cli = new HttpClient();
                json = cli.GetString(url, post, _ => {
                    if (!string.IsNullOrWhiteSpace(method)) {
                        _.Method = method;
                    }
                });
              
                return json;
            }
            catch (IOException e)
            {
              
               
                return null;
            }
        }
    }
}
