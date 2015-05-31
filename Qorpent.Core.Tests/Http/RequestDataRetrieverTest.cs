using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.IO.Http;

namespace Qorpent.Host.Lib.Tests
{
	[TestFixture]
	public class RequestDataRetrieverTest
	{
		[TestCase("http://x/?a=3 2&y=3","a=3 2;y=3")]
		[TestCase("http://x/?a=3+2&y=3","a=3 2;y=3")]
		[TestCase("http://x/?a=3%2C2&y=3","a=3,2;y=3")]
		[TestCase("http://x/?a=2&y=3","a=2;y=3")]
		[TestCase("http://x/?a=2&y=3&y=4","a=2;y=3,4")]
		[TestCase("http://x/?a=2+%2B+3","a=2 + 3")]
		public void UrlGetTest(string url, string test)
		{
			var result = RequestParameters.Create(url);
			var str = string.Join(";", result.Query.OrderBy(_ => _.Key).Select(_ => string.Format("{0}={1}",_.Key,_.Value)));
			Assert.AreEqual(test,str);
		}

	    [Test]
	    public void CanGetJsonFromQuery() {
	        var result = RequestParameters.Create(@"http://localhost/test?{""my"":""test""}");
            Assert.AreEqual("test",result.Get("my"));
            Assert.NotNull(result.QueryJson);
	    }

        [Test]
        public void CanGetJsonFromForm() {
            var str = new MemoryStream();
            var strw = new StreamWriter(str);
            strw.Write(@"{""my"":""test""}");
            strw.Flush();
            str.Position = 0;
            var req = new HttpRequestDescriptor {Uri = new Uri(@"http://localhost/test"), Method = "POST", Stream = str, ContentLength = str.Length};
            var result = RequestParameters.Create(req);
            Assert.AreEqual("test", result.Get("my"));
            Assert.NotNull(result.FormJson);
        }


		[Test]
		public void MultipartFormTest()
		{
			var stream = new MemoryStream();
			const string boundary = "--Asrf456BGe4h\n";
			var buffer = new byte[] {1, 2, 3, 4};
			Action<string> w = s =>
				{
					var b = Encoding.UTF8.GetBytes(s);
					stream.Write(b, 0, b.Length);
				};
			w(boundary);
			w("Content-Disposition: form-data; name=\"test1\"\n");
			w("Тест строка\n");
			w(boundary);
			w("Content-Disposition: form-data; name=\"test2\"; filename=\"test2.h\"\n");
			w("Content-Type: text/plain\n");
			stream.Write(buffer,0,buffer.Length);
			w("\n");
			w(boundary);
			w("Content-Disposition: form-data; name=\"test3\"; filename=\"test3.h\"\n");
			w("Content-Type: text/js\n");
			stream.Write(buffer, 0, buffer.Length);
			w("\n");
			w(boundary);
			w("--");
			stream.Position = 0;
            var result = RequestParameters.Create(new HttpRequestDescriptor { Uri = new Uri("http://localhost/test"), Stream = stream, Method = "POST", ContentType = "multipart/form-data;boundary=Asrf456BGe4h" });
			Assert.AreEqual("Тест строка",result.Get("test1"));
			var file = result.Files["test2"];
			Assert.AreEqual("test2.h",file.FileName);
			Assert.AreEqual("text/plain",file.ContentType);
			CollectionAssert.AreEqual(buffer,file.Content);
			file = result.Files["test3"];
			Assert.AreEqual("test3.h", file.FileName);
			Assert.AreEqual("text/js", file.ContentType);
			CollectionAssert.AreEqual(buffer, file.Content);
		}
	}
}
