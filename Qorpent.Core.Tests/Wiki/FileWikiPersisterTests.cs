using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Wiki;

namespace Qorpent.Core.Tests.Wiki {
    class FileWikiPersisterTests {

        [Test]
        public void CanSavePage() {
            var p = new FileWikiPersister {
                Container = new SimpleContainer()
            };
            var w = GetWikiPage();
            p.Container.Register(new ComponentDefinition<IFileService, FileService>());
            p.Save(w);

            var u = p.Get("/test/article");
            Assert.NotNull(u);
            Assert.NotNull(u.FirstOrDefault());

            var y = u.FirstOrDefault();

            Assert.AreEqual(y.Code, w.Code);
            Assert.AreEqual(y.Title, w.Title);
            Assert.AreEqual(y.Text, w.Text);
        }

        public WikiPage GetWikiPage() {
            return new WikiPage {
                Code = "/test/article",
                Editor = "local\\remalloc",
                Existed = true,
                LastWriteTime = DateTime.Now,
                Locker = null,
                Owner = "local\\some",
                Text = "someText",
                Title = "someTitle"
            };
        }

        [Test]
        public void G() {
            var path = ".Wiki\\test\\article";

            var elements = path.Replace('\\', '/').Split(new[] { '/' }, StringSplitOptions.None).ToList();
            elements.RemoveAt(elements.Count - 1);
            var dir = String.Join("/", elements);

            Assert.AreEqual(".Wiki/test", dir);
        }
    }
}
