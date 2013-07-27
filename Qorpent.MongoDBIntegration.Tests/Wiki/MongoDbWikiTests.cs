using System;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.IoC;
using Qorpent.MongoDBIntegration.Wiki;
using Qorpent.Security;
using Qorpent.Wiki;

namespace Qorpent.MongoDBIntegration.Tests.Wiki {
    public class StubIdentity : IIdentity {
        public string Name { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    public class StubClaimsPrincipal : IPrincipal {
        public bool WithAdmin { get; set; }

        public bool IsInRole(string role) {
            if (WithAdmin) {
                if (role == "ADMIN") return true;
            }

            return false;
        }

        public IIdentity Identity { get; set; }
    }

    public class StubPrincipalSource : IPrincipalSource {
        public IPrincipal CurrentUser { get; private set; }
        public IPrincipal BasePrincipal { get; set; }
        public void SetCurrentUser(IPrincipal usr) {
            CurrentUser = usr;
        }
    }

    class WikiTests : ServiceBase {
        private IApplication _app;
        private MongoWikiPersister _storage;

        [SetUp]
        public void SetUp() {
            _storage = new MongoWikiPersister {
               DatabaseName = "Zefs",
               CollectionName = "main"
            };
            _app = new Application();
            _app.Container.Register(new ComponentDefinition<IPrincipalSource, StubPrincipalSource>());
            _app.Principal.SetCurrentUser(new StubClaimsPrincipal { Identity = new StubIdentity { Name = Guid.NewGuid().ToString() } });

            _storage.SetApplication(_app);
        }

        [Test]
        public void CanSaveWikiPageWithCreateVersion() {
            _storage.Database.Drop();
            var wikiPage = new WikiPage {
                Code = "test",
                Editor = "remalloc",
                Existed = true,
                LastWriteTime = DateTime.Now,
                Owner = "remalloc",
                Text = "some text",
                Title = "fgfgdfgd"
            };

            _storage.SetApplication(_app);
            _storage.Save(wikiPage);                                    // сохраним тестовую страницу

            var t = _storage.Get("test").FirstOrDefault();              // получим её
            Assert.NotNull(t);
            Assert.AreEqual("some text", t.Text);                       // и проверим, что текст нормальный

            t.Text = "Test2";                                           // слегка изменим текст
            _storage.Save(t);                                           // и сохраним

            var r = _storage.Get("test").FirstOrDefault();              // снова получим эту страницу из базы
            Assert.NotNull(r);
            Assert.AreEqual("Test2", r.Text);                           // и убедимся, что всё сохранятеся нормально и идентификатор не перебивается

            /* на этом этапе мы убедились, что весь базовый старый функционал работает нормально, ничего не сломалось */

            /* а теперь убедимся, что работает версионность */

            var e = (WikiVersionCreateResult)_storage.CreateVersion("test", "test commit");      // теперь создадим тестовую версию
            Assert.NotNull(e);

            var h = _storage.Get("test").FirstOrDefault();              // теперь получим страницу из базы 
            Assert.NotNull(h);
            Assert.AreEqual("Test2", h.Text);                           // и проверим, что текст не изменился

            var f = _storage.GetWikiPageByVersion("test", e.Version);   // и в нашей версии-бэкапе всё точно так же как и в текущей копии
            Assert.NotNull(f);
            Assert.AreEqual(h.Text, f.Text);


            h.Text = "Some new text";                                   // измением слегка текст
            _storage.Save(h);                                           // и сохраним
            var y =_storage.Get(h.Code).FirstOrDefault();               // вытащим из базы
            Assert.NotNull(y);  
            Assert.AreEqual("Some new text", y.Text);                   // и проверим, что всё хорошо.\

            /* теперь проверим, что последнее изменение не слилось в бэкапы */
            var c = _storage.GetWikiPageByVersion("test", e.Version);
            var u = _storage.Get("test").FirstOrDefault();

            Assert.AreNotEqual(c.Text, u.Text);

        }

        [Test]
        public void CanRestoreVersions() {
            _storage.Database.Drop();
            var wikiPage = new WikiPage {
                Code = "test",
                Editor = "remalloc",
                Existed = true,
                LastWriteTime = DateTime.Now,
                Owner = "remalloc",
                Text = "some text",
                Title = "fgfgdfgd"
            };

            _storage.SetApplication(_app);
            _storage.Save(wikiPage);                                    // сохраним тестовую страницу

            var t = _storage.Get("test").FirstOrDefault();              // получим её
            Assert.NotNull(t);
            Assert.AreEqual("some text", t.Text);                       // и проверим, что текст нормальный

            t.Text = "Test2";                                           // слегка изменим текст
            _storage.Save(t);                                           // и сохраним

            var r = _storage.Get("test").FirstOrDefault();              // снова получим эту страницу из базы
            Assert.NotNull(r);
            Assert.AreEqual("Test2", r.Text);                           // и убедимся, что всё сохранятеся нормально и идентификатор не перебивается

            /* на этом этапе мы убедились, что весь базовый старый функционал работает нормально, ничего не сломалось */

            /* а теперь убедимся, что работает версионность */

            var e = (WikiVersionCreateResult)_storage.CreateVersion("test", "test commit");      // теперь создадим тестовую версию
            Assert.NotNull(e);

            var h = _storage.Get("test").FirstOrDefault();              // теперь получим страницу из базы 
            Assert.NotNull(h);
            Assert.AreEqual("Test2", h.Text);                           // и проверим, что текст не изменился

            var f = _storage.GetWikiPageByVersion("test", e.Version);   // и в нашей версии-бэкапе всё точно так же как и в текущей копии
            Assert.NotNull(f);
            Assert.AreEqual(h.Text, f.Text);


            h.Text = "Some new text";                                   // измением слегка текст
            _storage.Save(h);                                           // и сохраним
            var y = _storage.Get(h.Code).FirstOrDefault();               // вытащим из базы
            Assert.NotNull(y);
            Assert.AreEqual("Some new text", y.Text);                   // и проверим, что всё хорошо.\

            /* теперь проверим, что последнее изменение не слилось в бэкапы */
            var c = _storage.GetWikiPageByVersion("test", e.Version);
            var u = _storage.Get("test").FirstOrDefault();

            Assert.AreNotEqual(c.Text, u.Text);

            _storage.RestoreVersion("test", e.Version);

            var m = _storage.GetWikiPageByVersion("test", e.Version);
            var n = _storage.Get("test").FirstOrDefault();

            Assert.AreEqual(n.Text, m.Text);

        }

        public void GetLockTask(bool isFirst) {
            var storage = new MongoWikiPersister {
                DatabaseName = "Zefs",
                CollectionName = "main"
            };
            storage.SetApplication(_app);
            var locked = storage.GetLock("test");

            if (isFirst) {
                Assert.IsTrue(locked);
            } else {
                Assert.IsFalse(locked);
            }
        }

        [Test]
        public void CanLockWikiPage() {
            _storage.Database.Drop();
            var wikiPage = new WikiPage {
                Code = "test",
                Editor = "remalloc",
                Existed = true,
                LastWriteTime = DateTime.Now,
                Owner = "remalloc",
                Text = "some text",
                Title = "fgfgdfgd"
            };

            _storage.Save(wikiPage);
            var r = _storage.Get("test");
            Assert.NotNull(r);

            var t01 = new Task(() => GetLockTask(true));
            var t02 = new Task(() => GetLockTask(false));
            var t03 = new Task(() => GetLockTask(false));
            var t04 = new Task(() => GetLockTask(false));
            var t05 = new Task(() => GetLockTask(false));

            t01.Start();
            t02.Start();
            t03.Start();
            t04.Start();
            t05.Start();
        }

        [Test]
        public void CheckLocking() {
            _storage.Database.Drop();
            var wikiPage = new WikiPage {
                Code = "test",
                Editor = "remalloc",
                Existed = true,
                LastWriteTime = DateTime.Now,
                Owner = "remalloc",
                Text = "some text",
                Title = "fgfgdfgd"
            };

            ((StubClaimsPrincipal)_app.Principal.CurrentUser).WithAdmin = false;
            Assert.IsTrue(_storage.Save(wikiPage));
            Assert.IsTrue(_storage.GetLock(wikiPage.Code));
            _app.Principal.SetCurrentUser(new StubClaimsPrincipal { Identity = new StubIdentity { Name = Guid.NewGuid().ToString() } });
            Assert.IsFalse(_storage.Save(wikiPage));
            Assert.IsFalse(_storage.GetLock(wikiPage.Code));
            Assert.IsFalse(_storage.ReleaseLock(wikiPage.Code));
            ((StubClaimsPrincipal) _app.Principal.CurrentUser).WithAdmin = true;
            Assert.IsFalse(_storage.Save(wikiPage));
            Assert.IsFalse(_storage.GetLock(wikiPage.Code));
            Assert.IsTrue(_storage.ReleaseLock(wikiPage.Code));
            Assert.IsTrue(_storage.Save(wikiPage));



        }

        [Test]
        public void CorrectReturnForNonExistsPage() {
            _storage.Database.Drop();
            var wikiPage = new WikiPage {
                Code = "test",
                Editor = "remalloc",
                Existed = true,
                LastWriteTime = DateTime.Now,
                Owner = "remalloc",
                Text = "some text",
                Title = "fgfgdfgd"
            };

            Assert.IsTrue(_storage.Save(wikiPage));

            var t = _storage.Exists("test", "notExists");
            Assert.AreEqual(1, t.Count());
        }

        [Test]
        public void CorrectReturnForNonExistsPageFromGet() {
            _storage.Database.Drop();
            var wikiPage = new WikiPage {
                Code = "test",
                Editor = "remalloc",
                Existed = true,
                LastWriteTime = DateTime.Now,
                Owner = "remalloc",
                Text = "some text",
                Title = "fgfgdfgd"
            };

            Assert.IsTrue(_storage.Save(wikiPage));

            var t = _storage.Get("test", "notExists");
            Assert.AreEqual(1, t.Count());
        }
    }
}
