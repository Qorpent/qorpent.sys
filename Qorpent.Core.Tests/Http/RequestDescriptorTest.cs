using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.IO.Http;

namespace Qorpent.Core.Tests.Http
{
    [TestFixture]
    public class RequestDescriptorTest
    {
        [Test]
        public void GetParameters() {
            var req = RequestDescriptor.Create(new {
                param = new {
                    x = "1ф",
                    y = "2 ф+1"
                }
            });
            Assert.AreEqual("http://127.0.0.1/?x=1ф&y=2 ф%2B1", req.Uri.ToString());
        }
        [Test]
        public void GetParametersJsonMode()
        {
            var req = RequestDescriptor.Create(new
            {
                enc="json",
                param = new
                {
                    x = "1ф",
                    y = "2 ф+1"
                }
            });
            Assert.AreEqual("http://127.0.0.1/?{\"x\":\"1ф\"%2C\"y\":\"2 ф%2B1\"}", req.Uri.ToString());
        }

        [Test]
        public void FormData() {
            var req = RequestDescriptor.Create(new
            {
                method = "POST",
                param = new
                {
                    x = "1ф",
                    y = "2 ф+1"
                }
            });
            Assert.AreEqual("POST",req.Method);
            Assert.AreEqual("x=1%D1%84&y=2%20%D1%84%2B1", req.PostData);
            Assert.AreEqual("http://127.0.0.1/", req.Uri.ToString());
        }

        [Test]
        public void PostData() {
            var req = RequestDescriptor.Create(new {
                param = new {
                    a = 1,
                    b = 2
                },
                data = new {
                    x = "1ф",
                    y = "2 ф+1"
                }
            });
            Assert.AreEqual("POST", req.Method);
            Assert.AreEqual("{\"x\":\"1ф\",\"y\":\"2 ф+1\"}", req.PostData);
            Assert.AreEqual("http://127.0.0.1/?a=1&b=2", req.Uri.ToString());
        }
    }
}
