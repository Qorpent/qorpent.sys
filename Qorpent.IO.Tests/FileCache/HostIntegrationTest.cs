using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Host;
using Qorpent.Host.Static;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.FileCache {
	[TestFixture]
	public class HostIntegrationTest {
		private const string ProductionCode = @"
class tiles
	appid=99
	static map %TEMP%/HostIntegrationTest cache
        fallback notfound.png
		source 'http://144.76.82.130:14060/map/' master    
		source 'http://a.tile.openstreetmap.org/'    
		source 'http://b.tile.openstreetmap.org/'    
";

        private const string TestCode = @"
class tiles
	appid=99
	static map %TEMP%/HostIntegrationTest cache
        fallback notfound.png
		source @repos@/tiles master    
";

		[Test]
		public void CanReadCacheConfigFromBSharp() {
			var xml = BSharpCompiler.Compile(ProductionCode)["tiles"].Compiled;
			var conf = new HostConfig(xml);
			Assert.AreEqual(1,conf.StaticContentCacheMap.Count);
		    var qh = new HostServer(conf);
            Assert.NotNull(qh.Factory);//force initialize (HACK)
		    var stats = (HostServerStaticResolver) qh.Static;
            Assert.AreEqual(1,stats.Caches.Count);
		    var resolver = stats.Caches["/map/"];
			Assert.NotNull(resolver);
            Assert.AreEqual(EnvironmentInfo.ResolvePath(" %TEMP%/HostIntegrationTest"), resolver.Root);
            Assert.AreEqual(3,resolver.Sources.Count);
		    var src = resolver.Sources[0];
            Assert.True(src.IsMaster);
            Assert.AreEqual("http://144.76.82.130:14060/map/",src.Root);
            Assert.AreEqual("notfound.png",resolver.Fallback);
		}

	    [Test]
        [Category("NOTC")]
	    public void CanUseCacheOnResolution() {
            var xml = BSharpCompiler.Compile(TestCode)["tiles"].Compiled;
            var conf = new HostConfig(xml);
            var qh = new HostServer(conf);
	        qh.Start();
	        try {
	            var response = new HttpClient().Call("http://127.0.0.1:14990/map/8/173/76.png");
                Assert.AreEqual(200,response.State);
                Assert.AreEqual("image/png; charset=utf-8", response.ContentType);
	            var png = (Bitmap)Image.FromStream(new MemoryStream(response.Data));
                Assert.AreEqual(ImageFormat.Png, png.RawFormat);
	            var pixel = png.GetPixel(70, 40);
                Assert.AreEqual(174,pixel.R);
                Assert.AreEqual(209,pixel.G);
                Assert.AreEqual(160,pixel.B);

	        }
	        finally {
	            qh.Stop();
	        }
	    }

        [Test]
        [Explicit]
        public void CanUseCacheOnResolutionIntegration()
        {
            var xml = BSharpCompiler.Compile(ProductionCode)["tiles"].Compiled;
            var conf = new HostConfig(xml);
            var qh = new HostServer(conf);
            qh.Start();
            try
            {
                var response = new HttpClient().Call("http://127.0.0.1:14990/map/8/173/76.png");
                Assert.AreEqual(200, response.State);
                Assert.AreEqual("image/png; charset=utf-8", response.ContentType);
                var png = (Bitmap)Image.FromStream(new MemoryStream(response.Data));
                Assert.AreEqual(ImageFormat.Png, png.RawFormat);
                var pixel = png.GetPixel(70, 40);
                Assert.AreEqual(174, pixel.R);
                Assert.AreEqual(209, pixel.G);
                Assert.AreEqual(160, pixel.B);
                response = new HttpClient().Call("http://127.0.0.1:14990/map/6/11/26.png");
                Assert.AreEqual(200, response.State);
                Assert.AreEqual("image/png; charset=utf-8", response.ContentType);
                png = (Bitmap)Image.FromStream(new MemoryStream(response.Data));
                Assert.AreEqual(ImageFormat.Png, png.RawFormat);
                pixel = png.GetPixel(54, 18);
                Assert.AreEqual(181, pixel.R);
                Assert.AreEqual(171, pixel.G);
                Assert.AreEqual(201, pixel.B);
            }
            finally
            {
                qh.Stop();
            }
        }
       
	}
}