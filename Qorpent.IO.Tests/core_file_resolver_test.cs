using System;
using System.IO;
using NUnit.Framework;

namespace Qorpent.IO.Tests
{
	[TestFixture]
	public class core_file_resolver_test
	{
		private string dir;
		private QWebContext ctx;
		private FileNameResolver res;
		private TextWriterLogListener log;

		[Test]
		public void bug_stack_overflow_on_create_with_default_log() {
			var reg = new QWebServiceRegistry();
			var files = reg.FileNameResolver;
			var log = reg.Log;
		}

		[SetUp]
		public void setup() {
			this.dir = Path.Combine(Path.GetTempPath(), "core_file_resolver_test");
			if(Directory.Exists(dir))Directory.Delete(dir,true);
			Directory.CreateDirectory(dir);
			this.ctx = new QWebContext {RootDirectory = dir, ApplicationName = "/app"};
			this.res = new FileNameResolver();
			this.log = new TextWriterLogListener(Console.Out, LogLevel.All);
		}

		[Test]
		public void externalizationTest() {
			res.Externalize(@"(?i)fileresolver[\s\S]*restest");
			var resolvedpath = res.Resolve("~/tmp/_internal/folder/restest.txt",false);
			Assert.True(File.Exists(resolvedpath));
			Assert.AreEqual("content",File.ReadAllText(resolvedpath));
		}

		void testLocal(string query, bool existed, string expected) {
			var local = res.ResolveLocal(query, existed,context:ctx,log:log);
			Assert.AreEqual(expected,local);
		}

		void testLocal(string[] query, bool existed, string expected)
		{
			var local = res.ResolveLocal(query, existed, context: ctx, log: log);
			Assert.AreEqual(expected, local);
		}
		[Test]
		public void resolve_all() {
			res.Write("~/x/1.txt", "1",ctx);
			res.Write("~/y/2.txt", "2",ctx);
			var files = res.ResolveAll(ctx, FileSearchResultType.FullPath, new[] {"x", "y"}, new[] {"*.txt"}, log);
			Assert.AreEqual(2,files.Length);
			Assert.AreEqual("1",File.ReadAllText(files[0]));
			Assert.AreEqual("2", File.ReadAllText(files[1]));
		}

		void testLocalUrl(string query, bool existed, string expected)
		{
			var local = res.ResolveUrl(query, existed, context: ctx, log: log);
			Assert.AreEqual(expected, local);
		}
		void testLocalUrl(string[] query, bool existed, string expected)
		{
			var local = res.ResolveUrl(query, existed, context: ctx, log: log);
			Assert.AreEqual(expected, local);
		}
		
		void write(string name, object content) {
			res.Write(name, content, ctx, log);
		}
		void read(string name, object expected)
		{
			Assert.AreEqual(expected,res.Read<string>(name,ctx,log));
		}

		[Test]
		public void can_find_file_in_sub_folder() {
			write("~/sys/1.txt","test");
			testLocal("1.txt",true,"/sys/1.txt");
		}

		[Test]
		public void can_resolve_local_url()
		{
			write("~/sys/1.txt", "test");
			testLocalUrl("1.txt", true, "/app/sys/1.txt");
		}

		[Test]
		public void can_choose_file()
		{
			write("~/sys/1.txt", "test");
			write("~/usr/2.txt", "test");
			testLocal(new[] { "1.txt", "2.txt" }, true, "/sys/1.txt");
			testLocal(new[] { "2.txt", "1.txt" }, true, "/usr/2.txt");
			testLocal(new[] { "3.txt", "1.txt" }, true, "/sys/1.txt");
		}

		[Test]
		public void can_use_custom_probes()
		{
			write("~/sys/1.txt", "test");
			write("~/usr/1.txt", "test2");
			var f = res.Resolve(ctx, FileSearchResultType.LocalPath, true, null, new[] {"1.txt"}, log);
			Assert.AreEqual("/usr/1.txt", f);
			f = res.Resolve(ctx, FileSearchResultType.LocalPath, true, new[]{"sys","usr"}, new[] { "1.txt" }, log);
			Assert.AreEqual("/sys/1.txt", f);
		}

		[Test]
		public void can_choose_url()
		{
			write("~/sys/1.txt", "test");
			write("~/usr/2.txt", "test");
			testLocalUrl(new[]{"1.txt","2.txt"}, true, "/app/sys/1.txt");
			testLocalUrl(new[] { "2.txt", "1.txt" }, true, "/app/usr/2.txt");
			testLocalUrl(new[] { "3.txt", "1.txt" }, true, "/app/sys/1.txt");
		}

		[Test]
		public void can_resolve_usr_and_sys()
		{
			write("~/sys/1.txt", "test1");
			write("~/sys/2.txt", "test2");
			write("~/usr/2.txt", "test3");
			read("1.txt","test1");
			read("2.txt", "test3");
		}

		[Test]
		public void drops_caches_after_write()
		{
			write("~/sys/1.txt", "test1");
			write("~/sys/2.txt", "test2");
			read("2.txt", "test2");
			write("~/usr/2.txt", "test3");
			read("1.txt", "test1");
			read("2.txt", "test3");
		}

		[Test]
		public void can_find_javascripts_in_scripts_folder()
		{
			write("~/scripts/1.js", "test1");
			read("1.js", "test1");
		}

		[Test]
		public void default_writes_are_made_in_usr_folder() {
			write("1.txt","test1");
			testLocal("1.txt",true,"/usr/1.txt");
		}
	}
}

