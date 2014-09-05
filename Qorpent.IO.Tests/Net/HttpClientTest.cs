using NUnit.Framework;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.Net{
	[TestFixture]
	[Explicit]
	public class HttpClientTest
	{
		[Test]
		public void CanDownloadString(){
			var result = new HttpClient().GetString("http://www.yandex.ru/");
			Assert.True(result.Contains("KhtmlBorderRadius"));
			Assert.True(result.Contains("ноября"));
		}
	}
}