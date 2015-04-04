using System.IO;
using System.Threading;
using NUnit.Framework;
using Qorpent.Host;
using Qorpent.IO.FileCache;
using Qorpent.IO.Net;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Tests.FileCache
{
    [TestFixture]
    public class MainFileCacheTests
    {
        private string root;
        private string nearest;
        private string master;
        private string reserved;
        private FileCacheResolver cache;
        private string web;

        [SetUp]
        public void Init() {
            root = SetupDirectory("Self", new[] { 1, 2, 0, 4, 5, 0, 0 }); 
            nearest = SetupDirectory("Nearest", new[] { 11, 12, 13, 14, 15, 0, 0 });
            master = SetupDirectory("Mester", new[] { 21, 22, 23, 24, 25, 26, 0 });
            reserved = SetupDirectory("Reserved", new[] { 31, 32, 33, 34, 35, 36, 37 });
            web = SetupDirectory("Web", new[] { 41,42,43,44,45,46,46,48 });
            cache = new FileCacheResolver(root, nearest, new FileCacheSource(master){IsMaster=true}, reserved);
        }
        private string SetupDirectory(string name, int[] data) {
            Thread.Sleep(100); //allow system free directories
            var d = Path.Combine(Path.GetTempPath(),"MainFileCache/"+name);
            var names = new[] { "a.txt", "b/c.txt", "b/d.txt", "b/e/f.txt", "b/e/g.txt", "b/e/h.txt", "b/e/i.txt", "b/e/j.txt" };
            Directory.CreateDirectory(d);
            Directory.Delete(d, true);
            Directory.CreateDirectory(d);
            for (var i = 0; i < data.Length;i++) {
                if (data[i] != 0) {
                    var path = Path.Combine(d, names[i]);
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                    File.WriteAllText(path, data[i].ToString());
                }
            }
            return d;
        }

        private string CheckFile(string name, int content, bool forceUpdate = false) {
            var path = cache.Resolve(name,forceUpdate);
            Assert.NotNull(path);
            var data = File.ReadAllText(path).ToInt();
            Assert.AreEqual(content, data);
            return path;
        }

        [TestCase("a.txt",1)]
        [TestCase("b/c.txt", 2)]
        [TestCase("b/e/f.txt", 4)]
        [TestCase("b/e/g.txt", 5)]
        public void CanGetLocalFiles(string name, int result) {
            CheckFile(name, result);
        }

        [TestCase("a.txt", 1)]
        [TestCase("b/c.txt", 2)]
        [TestCase("b/e/f.txt", 4)]
        [TestCase("b/e/g.txt", 15)]
        [TestCase("b/d.txt", 13)]
        public void CanUseNearestToGetInsufficientFiles(string name, int result)
        {
            File.Delete(Path.Combine(root, "b/e/g.txt"));
            CheckFile(name, result);
        }

       
        [TestCase("b/d.txt", 13)]
        [TestCase("b/e/h.txt", 26)]
        [TestCase("b/e/i.txt", 37)]
        public void CanResolveInsuficientByResolveStack(string name, int result)
        {
           
            CheckFile(name, result);
        }


        [Test]
        public void CanReturnNullOnNonExisted() {
            var path = cache.Resolve("nonex.txt", true);
            Assert.Null(path);
        }

        [Test]
        public void CanReturnFallbakOnNonExistedIfConfigured()
        {
            File.WriteAllText(Path.Combine(reserved, "fallback.txt"), "100");
            cache.Fallback = "fallback.txt";
            CheckFile("nonex.txt", 100);
        }

        [Test]
        public void HttpCacheTest() {
            var conf = new HostConfig();
            conf.AddQorpentBinding(99);
            conf.StaticContentMap["/files/"] = web;
            var qh = new HostServer(conf);
            qh.Start();
            try {
                Thread.Sleep(200);
                Assert.AreEqual("41", new HttpClient().GetString("http://127.0.0.1:14990/files/a.txt"));
                cache.Sources.Add(new FileCacheSource("http://127.0.0.1:14990/files/"));
                CheckFile("b/e/j.txt", 48);
                Assert.Null(cache.Resolve("b/e/k.txt")); //check that 404 doesn't cause error
            }
            finally {
                qh.Stop();
            }
        }

        
    }
}
