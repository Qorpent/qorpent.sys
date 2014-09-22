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
		[TestCase("http://Конфуз с начальником управления")]
		public void DisallowInvalidLinks(string url)
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
			ctx.Level |= PortableHtmlStrictLevel.TrustAllImages;
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", false));
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", true));
		}

		[Test]
		public void CanTrustAllLinks()
		{
			var ctx = new PortableHtmlContext();
			ctx.Level|=PortableHtmlStrictLevel.TrustAllLinks;
			Assert.AreEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", false));
			Assert.AreNotEqual(PortableHtmlSchemaErorr.None, ctx.GetUriTrustState("http://my.site.com:8080/x", true));
		}

		[TestCase("div")]
		[TestCase("br")]
		[TestCase("table")]
		[TestCase("tr")]
		[TestCase("td")]
		[TestCase("tbody")]
		[TestCase("thead")]
		[TestCase("th")]
		[TestCase("ol")]
		[TestCase("ul")]
		[TestCase("li")]
		[TestCase("b")]
		[TestCase("i")]
		[TestCase("blockuote")]
		[TestCase("article")]
		[TestCase("font")]
		[Test(Description = "Проверка фильтра недозволенных элеменов в strict_mode (default)")]
		public void NotAllowedNonStrictElements(string tag)
		{
			Assert.AreEqual(PortableHtmlSchemaErorr.UnknownElement,new PortableHtmlContext().GetTagState(tag));
		}


		[TestCase("p")]
		[TestCase("a")]
		[TestCase("img")]
		[TestCase("strong")]
		[TestCase("em")]
		[TestCase("del")]
		[TestCase("sub")]
		[TestCase("sup")]
		[TestCase("u")]
		[TestCase("span")]
		[Test(Description = "Проверка фильтра разрешенных элеменов в strict_mode (default)")]
		public void AllowedNonStrictElements(string tag)
		{
			Assert.AreEqual(PortableHtmlSchemaErorr.None, new PortableHtmlContext().GetTagState(tag));
		}

		[TestCase("table")]
		[TestCase("tr")]
		[TestCase("td")]
		[TestCase("tbody")]
		[TestCase("thead")]
		[TestCase("th")]
		[Test(Description = "Режим с разрешенными таблицами")]
		public void AllowedTableElements(string tag)
		{
			Assert.AreEqual(PortableHtmlSchemaErorr.None, new PortableHtmlContext{Level = PortableHtmlStrictLevel.AllowTables}.GetTagState(tag));
		}

		[TestCase("ol")]
		[TestCase("ul")]
		[TestCase("li")]
		[Test(Description = "Режим с разрешенными списками")]
		public void AllowedListElements(string tag)
		{
			Assert.AreEqual(PortableHtmlSchemaErorr.None, new PortableHtmlContext { Level = PortableHtmlStrictLevel.AllowLists }.GetTagState(tag));
		}

		[TestCase("br")]
		[Test(Description = "Режим с разрешенными br")]
		public void AllowedBrElements(string tag)
		{
			Assert.AreEqual(PortableHtmlSchemaErorr.None, new PortableHtmlContext { Level = PortableHtmlStrictLevel.AllowBr }.GetTagState(tag));
		}



		[TestCase("br",PortableHtmlStrictLevel.AllowLists|PortableHtmlStrictLevel.AllowTables)]
		[TestCase("table",PortableHtmlStrictLevel.AllowLists|PortableHtmlStrictLevel.AllowBr)]
		[TestCase("ul",PortableHtmlStrictLevel.AllowTables|PortableHtmlStrictLevel.AllowBr)]
		[Test(Description = "Проверяем, что открытие доп опций не открывает дорогу не тем тегам")]
		public void CoverExclusiveOpen(string tag, PortableHtmlStrictLevel level)
		{
			Assert.AreEqual(PortableHtmlSchemaErorr.UnknownElement, new PortableHtmlContext { Level = level }.GetTagState(tag));
		}

		[Test(Description = "Проверка вывода результата в виде строки")]
		public void CoverToString(){
			var ctx = new PortableHtmlContext{
				Strategy = PortableHtmlVerificationStrategy.Full,
				Level = PortableHtmlStrictLevel.AllowLists,
				BaseUri = new Uri("http://ya.ru/x")
			};
			ctx.TrustedHosts.Add("me.com");
			ctx.TrustedHosts.Add("you.com");
			const string html = "<!--x--><DIV a='1'><p/><a href='javascript:hack()'/></DIV>";
			ctx = PortableHtmlSchema.Validate(html, ctx);
			var result = ctx.ToString();
			Console.WriteLine(result);
			Assert.AreEqual(@"Level : AllowLists
Strategy : Full
IsOk : False
SchemaError : InvalidRootTag, CommentsDetected, RootInline, UnknownAttribute, EmptyElement, UpperCaseDetected, DangerousLink
BaseUri : 'http://ya.ru/x'
Trust : 'me.com'
Trust : 'you.com'
CommentsDetected 0:0/ 
InvalidRootTag 1:10/./ 
UnknownAttribute 1:10/.//@a 
UpperCaseDetected 1:10/./ 
EmptyElement 1:21/./p[1] 
EmptyElement 1:25/./a[1] 
DangerousLink 1:25/./a[1] 
RootInline 1:25/./a[1] 
", result);
		}
	}
}