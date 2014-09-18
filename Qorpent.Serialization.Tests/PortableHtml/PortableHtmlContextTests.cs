using System;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.PortableHtml;

namespace Qorpent.Serialization.Tests.PortableHtml{
	[TestFixture(Description = "Проверка работы самого контекста распознавания")]
	public class PortableHtmlContextTests{
		[Test]
		public void InForcedStrategiesMarksElementsNotToReproduce(){
			var ctx = new PortableHtmlContext{Strategy = PortableHtmlVerificationStrategy.Full};
			var e = new XElement("test");
			Assert.True(ctx.InChecking(e));
			ctx.SetError(PortableHtmlSchemaErorr.None, e);
			Assert.True(ctx.InChecking(e), "Full");
			ctx.Strategy = PortableHtmlVerificationStrategy.ForcedElementResult;
			e = new XElement("test");
			Assert.True(ctx.InChecking(e));
			ctx.SetError(PortableHtmlSchemaErorr.None, e);
			Assert.False(ctx.InChecking(e), "ForcedElement");
			ctx.Strategy = PortableHtmlVerificationStrategy.ForcedResult;
			e = new XElement("test");
			Assert.True(ctx.InChecking(e));
			ctx.SetError(PortableHtmlSchemaErorr.None, e);
			Assert.False(ctx.InChecking(e), "ForcedResult");
		}

		[Test]
		public void DeactivatesInForcedResultMode(){
			var ctx = new PortableHtmlContext { Strategy = PortableHtmlVerificationStrategy.ForcedResult };
			Assert.True(ctx.IsActive);
			ctx.SetError(PortableHtmlSchemaErorr.InvalidRootTag);
			Assert.False(ctx.IsActive,"Forced");

			ctx = new PortableHtmlContext { Strategy = PortableHtmlVerificationStrategy.ForcedElementResult };
			Assert.True(ctx.IsActive);
			ctx.SetError(PortableHtmlSchemaErorr.InvalidRootTag);
			Assert.True(ctx.IsActive, "ForcedResult");

			ctx = new PortableHtmlContext { Strategy = PortableHtmlVerificationStrategy.Full };
			Assert.True(ctx.IsActive);
			ctx.SetError(PortableHtmlSchemaErorr.InvalidRootTag);
			Assert.True(ctx.IsActive, "Full");
		}

		[TestCase("x")]
		[TestCase("../folder/x.html")]
		public void AllowLocalReferences(string url){
			var ctx = new PortableHtmlContext();
			Assert.AreEqual(PortableHtmlSchemaErorr.None,ctx.GetUriTrustState(url,false));
			Assert.AreEqual(PortableHtmlSchemaErorr.None,ctx.GetUriTrustState(url,true));
		}


		[TestCase("")]
		[TestCase(null)]
		[TestCase("#")]
		[TestCase("#xx")]
		[TestCase(" ")]
		public void DisallowEmptiesAndHashes(string url)
		{
			var ctx = new PortableHtmlContext();
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState(url, false));
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState(url, true));
		}

	

		[TestCase("javascript:alert()")]
		[TestCase("file:///my.file")]
		public void DisallowBadSchemasReferences(string url)
		{
			var ctx = new PortableHtmlContext();
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState(url, false));
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState(url, true));
		}

		[Test]
		public void DisallowDataOnAnchors()
		{
			var ctx = new PortableHtmlContext();
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("data:image", false));
			
		}
		[Test]
		public void		AllowDataOnImages()
		{
			var ctx = new PortableHtmlContext();
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("data:image", true));

		}

		[Test]
		public void DisallowNonTrustedAbsoluteLinks(){
			var ctx = new PortableHtmlContext();
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com/x", false));
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com/x", true));
		}

		[Test]
		public void AllowBaseUriAbsoluteLinks()
		{
			var ctx = new PortableHtmlContext();
			ctx.BaseUri = new Uri("https://my.site.com:443/u");
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com/x", false));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com/x", true));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", false));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", true));
		}

		[Test]
		public void AllowTrustedAbsoluteLinks()
		{
			var ctx = new PortableHtmlContext();
			ctx.TrustedHosts.Add("my.site.com");
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com/x", false));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com/x", true));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", false));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", true));
		}

		[Test]
		public void CanTrustAllImages(){
			var ctx = new PortableHtmlContext();
			ctx.TrustAllImages = true;
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", false));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", true));
		}

		[Test]
		public void CanTrustAllSites()
		{
			var ctx = new PortableHtmlContext();
			ctx.TrustAllSites = true;
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", false));
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", true));
		}
	}
}