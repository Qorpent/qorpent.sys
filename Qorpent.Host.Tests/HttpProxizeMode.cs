using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.IO.Net;

namespace Qorpent.Host.Tests
{
    [TestFixture]
    public class HttpProxizeMode
    {
        [Test]
        public void CanProxiseBasics() {
            var aconfig1 = new HostConfig();
            aconfig1.AddQorpentBinding(71);
            var aconfig2 = new HostConfig();
            aconfig2.AddQorpentBinding(72);
            aconfig2.Proxize["/call1"] = "http://127.0.0.1:14710";
            var h1 = new HostServer(aconfig1);
            var h2 = new HostServer(aconfig2);
            h1.On("/call1", "hello!");
            var result = "";
            try {
                h1.Start();
                h2.Start();
                Thread.Sleep(100);
                result = new HttpClient().GetString("http://127.0.0.1:14720/call1");
                
            }
            finally {
                h1.Stop();
                h2.Stop();
                
            }
            Assert.AreEqual("hello!",result);
        }

        [Test]
        public void CanProxiseByAppid()
        {
            var aconfig1 = new HostConfig();
            aconfig1.AddQorpentBinding(71);
            var aconfig2 = new HostConfig();
            aconfig2.AddQorpentBinding(72);
            aconfig2.Proxize["/call1"] = "appid=71";
            var h1 = new HostServer(aconfig1);
            var h2 = new HostServer(aconfig2);
            h1.On("/call1", "hello!");
            var result = "";
            try
            {
                h1.Start();
                h2.Start(); 
                Thread.Sleep(100);
                result = new HttpClient().GetString("http://127.0.0.1:14720/call1");
            }
            finally
            {
                h1.Stop();
                h2.Stop();

            }
            Assert.AreEqual("hello!", result);
        }


        [Test]
        public void CanProxisePost()
        {
            var aconfig1 = new HostConfig();
            aconfig1.AddQorpentBinding(73);
            var aconfig2 = new HostConfig();
            aconfig2.AddQorpentBinding(74);
            aconfig2.Proxize["/call1"] = "appid=73";
            var h1 = new HostServer(aconfig1);
            var h2 = new HostServer(aconfig2);
            h1.OnContext("/call1", _ => {
                if (_.Request.HttpMethod == "POST") {
                    _.Response.Finish(new StreamReader(_.Request.InputStream).ReadToEnd());
                }
                else {
                    _.Response.Finish("hello!");    
                }
                
            });
            var result = "";
            var resultDirect = "";

            try
            {
                h1.Start();
                h2.Start();
                Thread.Sleep(100);
                resultDirect = new HttpClient().GetString("http://127.0.0.1:14730/call1", "hello2");
                result = new HttpClient().GetString("http://127.0.0.1:14740/call1", "hello2");
            }
            finally
            {
                h1.Stop();
                h2.Stop();

            }
            Console.WriteLine(result);

            Assert.AreEqual("hello2", resultDirect);
            Assert.AreEqual("hello2", result);
        }

        [TestCase("appid=15","http://127.0.0.1:14150")]
        [TestCase("http://192.168.0.1", "http://192.168.0.1")]
        [TestCase("appid=15;secure=true","https://127.0.0.1:14151")]
        [TestCase("appid=15;server=.;secure=true","https://127.0.0.1:14151")]
        [TestCase("appid=15;server=192.168.1.1;secure=true", "https://192.168.1.1:14151")]
        public void HostHelperCanBuildUrlsFromAppidConnectionStrings(string src, string result) {
            var resultUrl = HostUtils.ParseUrl(src);
            Assert.AreEqual(result,resultUrl);

        }


        [Test]
        public void CanSetupValidProxyModeFromBSharp() {
            const string code = @"
class myapp
       proxize /others appid=15
       proxize /others2 appid=16 secure=true
       proxize /others3 url='http://yandex.ru'
";
            var xml = BSharpCompiler.Compile(code)["myapp"].Compiled;
            var config = new HostConfig(xml);
            Assert.AreEqual("appid=15;",config.Proxize["/others"]);
            Assert.AreEqual("appid=16;secure=true;",config.Proxize["/others2"]);
            Assert.AreEqual("http://yandex.ru", config.Proxize["/others3"]);
        }

        [Test]
        public void CanSetupRequireWithProxying()
        {
            const string code = @"
class app1
     appid 76
     service /myserv1

class app2
     appid 78
     require app1 proxize
";
            var ctx = BSharpCompiler.Compile(code);
            var app = ctx["app2"].Compiled;
            var config = new HostConfig(app,ctx);
            Assert.AreEqual("appid=76;", config.Proxize["/myserv1"]);
        }
     
    }
}
