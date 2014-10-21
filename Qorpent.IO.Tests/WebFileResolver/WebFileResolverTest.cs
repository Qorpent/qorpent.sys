using System.IO;
using NUnit.Framework;

namespace Qorpent.IO.Tests{
	/// <summary>
	/// 
	/// </summary>
	[TestFixture]
	public class WebFileResolverTest{

		
		[TestCase("b.c","/a/b.c")]
		[TestCase("x/b.c","/x/a/b.c")]
		[TestCase("x/a/a/b.c","/x/a/a/b.c")]
		public void BasicResolverTest(string search, string result){
			var resolver = new WebFileResolver();
			var provider = new StubWebStaticProvider{Prefix = "x"};
			provider.Add("/a/b.c", "/a/b/c.d", "/a/b/b.c", "/a/a/b.c");
			resolver.Providers.Add(provider);
			provider = new StubWebStaticProvider { Prefix = "y" };
			provider.Add("/a/b.c", "/a/b/c.x", "/a/b/b.c", "/a/a/b.c");
			resolver.Providers.Add(provider);
			provider = new StubWebStaticProvider { Prefix = "" };
			provider.Add("/a/b.c", "/a/b/c.x", "/a/b/b.c", "/a/a/b.c");
			resolver.Providers.Add(provider);

			Assert.AreEqual(result,resolver.Find(search).Name);
		}

		[TestCase("/app/b.c", "/a/b.c")]
		[TestCase("/app/x/b.c", "/x/a/b.c")]
		[TestCase("/app/x/a/a/b.c", "/x/a/a/b.c")]
		public void GlobalPrefixedBasicResolverTest(string search, string result)
		{
			var resolver = new WebFileResolver{Prefix = "app"};
			var provider = new StubWebStaticProvider { Prefix = "x" };
			provider.Add("/a/b.c", "/a/b/c.d", "/a/b/b.c", "/a/a/b.c");
			resolver.Providers.Add(provider);
			provider = new StubWebStaticProvider { Prefix = "y" };
			provider.Add("/a/b.c", "/a/b/c.x", "/a/b/b.c", "/a/a/b.c");
			resolver.Providers.Add(provider);
			provider = new StubWebStaticProvider { Prefix = "" };
			provider.Add("/a/b.c", "/a/b/c.x", "/a/b/b.c", "/a/a/b.c");
			resolver.Providers.Add(provider);

			Assert.AreEqual(result, resolver.Find(search).Name);
		}


		[Test]
		public void StubWebFileProviderTest()
		{
			var provider = new StubWebStaticProvider { Prefix = "xxx" };
			provider.Add("/a/b.c", "/a/b/c.d", "/a/b/b.c", "/a/a/b.c");
			Assert.AreEqual("/xxx/a/b.c", provider.Find("/a/b.c").Name);
			Assert.AreEqual(null, provider.Find("b.c"));
			Assert.AreEqual("/xxx/a/b.c", provider.Find("B.c", WebFileSerachMode.IgnorePath).Name);
			Assert.AreEqual("/xxx/a/b.c", provider.Find("/a/a/b.c", WebFileSerachMode.IgnorePath).Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("a/a/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/b/b.c", provider.Find("/a/b/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/b.c", provider.Find("a/d/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("/a/A/b.c").Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("/xxx/a/A/b.c").Name);
		}

		[Test]
		public void DirectoryFileProvider(){
			var dir = Path.Combine(Path.GetTempPath(), "WebFileResolverTest_DirectoryFileProvider");
			Directory.CreateDirectory(Path.Combine(dir,"a"));
			Directory.CreateDirectory(Path.Combine(dir, "a/a"));
			Directory.CreateDirectory(Path.Combine(dir, "a/b"));
			File.WriteAllText(Path.Combine(dir,"a/b.c"),"1");
			File.WriteAllText(Path.Combine(dir,"a/b/c.d"),"2");
			File.WriteAllText(Path.Combine(dir,"a/b/b.c"),"3");
			File.WriteAllText(Path.Combine(dir,"a/a/b.c"),"4");

			var provider = new FileSystemWebFileProvider{Root = dir, Prefix = "xxx"};
			Assert.AreEqual("/xxx/a/b.c", provider.Find("B.c", WebFileSerachMode.IgnorePath).Name);
			Assert.AreEqual("/xxx/a/b.c", provider.Find("/a/a/b.c", WebFileSerachMode.IgnorePath).Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("a/a/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/b/b.c", provider.Find("/a/b/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/b.c", provider.Find("a/d/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("/a/A/b.c").Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("/xxx/a/A/b.c").Name);


			Assert.AreEqual("1", provider.Find("B.c", WebFileSerachMode.IgnorePath).Read());
			Assert.AreEqual("1", provider.Find("/a/a/b.c", WebFileSerachMode.IgnorePath).Read());
			Assert.AreEqual("4", provider.Find("a/a/b.c", WebFileSerachMode.ExactThenIgnore).Read());
			Assert.AreEqual("3", provider.Find("/a/b/b.c", WebFileSerachMode.ExactThenIgnore).Read());
			Assert.AreEqual("1", provider.Find("a/d/b.c", WebFileSerachMode.ExactThenIgnore).Read());
			Assert.AreEqual("4", provider.Find("/a/A/b.c").Read());
			Assert.AreEqual("4", provider.Find("/xxx/a/A/b.c").Read());
		}

		[Test]
		public void ResourceFileProvider()
		{


			var provider = new ResourceWebFileProvider{
				Assembly = typeof(WebFileResolverTest).Assembly, 
				Prefix = "xxx",
				StripRegex = @"^[\s\S]+?WebFileResolver"
			};
			Assert.AreEqual("/xxx/a/b.c", provider.Find("B.c", WebFileSerachMode.IgnorePath).Name);
			Assert.AreEqual("/xxx/a/b.c", provider.Find("/a/a/b.c", WebFileSerachMode.IgnorePath).Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("a/a/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/b/b.c", provider.Find("/a/b/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/b.c", provider.Find("a/d/b.c", WebFileSerachMode.ExactThenIgnore).Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("/a/A/b.c").Name);
			Assert.AreEqual("/xxx/a/a/b.c", provider.Find("/xxx/a/A/b.c").Name);


			Assert.AreEqual("1", provider.Find("B.c", WebFileSerachMode.IgnorePath).Read());
			Assert.AreEqual("1", provider.Find("/a/a/b.c", WebFileSerachMode.IgnorePath).Read());
			Assert.AreEqual("4", provider.Find("a/a/b.c", WebFileSerachMode.ExactThenIgnore).Read());
			Assert.AreEqual("3", provider.Find("/a/b/b.c", WebFileSerachMode.ExactThenIgnore).Read());
			Assert.AreEqual("1", provider.Find("a/d/b.c", WebFileSerachMode.ExactThenIgnore).Read());
			Assert.AreEqual("4", provider.Find("/a/A/b.c").Read());
			Assert.AreEqual("4", provider.Find("/xxx/a/A/b.c").Read());
		}
		
	}
}