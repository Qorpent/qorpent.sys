using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.Host.Utils;

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
		public void UrlGetTest(string url, string test)
		{
			var result = new RequestDataRetriever("text/plain", Encoding.UTF8, 0, new Uri(url), null, "GET").GetRequestData();
			var str = string.Join(";", result.Query.OrderBy(_ => _.Key).Select(_ => string.Format("{0}={1}",_.Key,_.Value)));
			Assert.AreEqual(test,str);
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
			w("\n");
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
			var result = new RequestDataRetriever("multipart/form-data; boundary=Asrf456BGe4h", Encoding.UTF8, 0, new Uri("http://best/test"),stream, "POST").GetRequestData();
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
