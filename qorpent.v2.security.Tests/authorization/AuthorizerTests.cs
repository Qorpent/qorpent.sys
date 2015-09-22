using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.authorization;
using qorpent.v2.security.user;
using Qorpent.IO.Http;

namespace qorpent.v2.security.Tests.authorization
{
    [TestFixture]
    public class AuthorizerTests:BaseFixture
    {
        private IHttpAuthorizer a;

        public override void Setup() {
            base.Setup();
            this.a = _container.Get<IHttpAuthorizer>();
        }

        [Test]
        public void AllowAnyCss() {
            var id = new Identity {IsAuthenticated = false};
            var req = new HttpRequestDescriptor {User =new GenericPrincipal(id,null),  Uri = new Uri("http://host/test.css")};
            var auth = a.Authorize(req);
            Assert.True(auth.Process);
        }

        [Test]
        public void RedirectsHtmlNotAuth()
        {
            var id = new Identity { IsAuthenticated = false };
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/test.html") };
            var auth = a.Authorize(req);
            Assert.True(!string.IsNullOrWhiteSpace(auth.Redirect));
        }

        [Test]
        public void DenyUnknownNotAuth()
        {
            var id = new Identity { IsAuthenticated = false };
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/operation") };
            var auth = a.Authorize(req);
            Assert.NotNull(auth.Error);
        }


        [Test]
        public void CustomPublicResource() {
            var id = new Identity { IsAuthenticated = false };
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/public.html") };
            var auth = a.Authorize(req);
            Assert.True(auth.Process);
        }

        [Test]
        public void CustomRedirection() {
            var id = new Identity { IsAuthenticated = false };
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/spredir.html") };
            var auth = a.Authorize(req);
            Assert.AreEqual(@"/public.html?referer=/spredir.html", auth.Redirect);
        }

        [Test]
        public void AllowAllAuthToAnyHtml() {
            var id = new Identity { IsAuthenticated = true };
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/some.html") };
            var auth = a.Authorize(req);
            Assert.True(auth.Process);
        }

        [Test]
        public void CustomRoleDeny() {
            var id = new Identity { IsAuthenticated = true, User = new User()};
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/roled.html") };
            var auth = a.Authorize(req);
            Assert.False(auth.Process);
            Assert.NotNull(auth.Error);
        }

        [Test]
        public void DenyPathedRole()
        {
            var id = new Identity { IsAuthenticated = true, IsAdmin = false};
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/admin-data.html") };
            var auth = a.Authorize(req);
            Assert.False(auth.Process);
        }
        [Test]
        public void AllowPathedRole()
        {
            var id = new Identity { IsAuthenticated = true, IsAdmin = true};
            var req = new HttpRequestDescriptor { User = new GenericPrincipal(id, null), Uri = new Uri("http://host/admin-data.html") };
            var auth = a.Authorize(req);
            Assert.True(auth.Process);
        }
    }
}
