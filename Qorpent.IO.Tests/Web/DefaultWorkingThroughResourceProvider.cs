using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.IO.Resources;
namespace Qorpent.IO.Tests.Web
{
	[TestFixture]
	public class DefaultWorkingThroughResourceProvider
	{
		private IResourceProvider res;

		[SetUp]
		public void Setup() {
			res = Applications.Application.Current.Resources;
		}

		[Test]
		public void CanAccessResourceProvider() {
			Assert.NotNull(res);
		}
		[Test]
		public void SupportHtmlScheme() {
			Assert.True(res.IsSupported(new Uri("http://test")));
		}
		[Test]
		public void CanGetDataFullApi() {
			var request = res.CreateRequest(new Uri("http://localhost"));
			var response = request.GetResponse();
			response.Wait();
			var resource = response.Result.GetResource();
			resource.Wait();
			var stream = resource.Result.Open();
			stream.Wait();
			Task<string> getresult;
			using (var sr = new StreamReader(stream.Result)) {
				getresult = sr.ReadToEndAsync();
			}
			getresult.Wait();
			var result = getresult.Result;
			StringAssert.Contains("<html",result);
		}

		[Test]
		public void GetGetDataExtensionApi() {
			StringAssert.Contains("<html", res.GetString("http://localhost"));
		}
		[Test]
		public void GetGetDataExtensionApiAsync() {
			var t = res.GetStringAsync("http://localhost");
			t.Wait();
			StringAssert.Contains("<html", t.Result);
		}
	}
}
