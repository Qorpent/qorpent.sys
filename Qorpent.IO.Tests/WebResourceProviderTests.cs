using NUnit.Framework;
using Qorpent.IO.Resources;
using Qorpent.IO.Web;
using System;

namespace Qorpent.IO.Tests {
    [TestFixture]
    public class WebResourceProviderTests {
        /*
        /// <summary>
        /// 
        /// </summary>
        public IWebFixtureBase WebFixtureBase { get; set; }
        [Test]
        public void GetSize() {
            var content = "test";
            LongSrv.Pages.Add("/test.html", content);
            LongSrv.Synchronize();
            var drp = new DefaultResourceProvider {
                Extensions = new IResourceProviderExtension[] {new WebResourceProviderGetSizeExtension()}
            };
            Assert.AreEqual(content.Length, drp.GetSize(new Uri(LongSrv.GetUrl("/test.html"))));
        }
         * */
    }
}
