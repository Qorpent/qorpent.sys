using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using qorpent.v2.security.authentication;

namespace qorpent.v2.security.Tests.authentication
{
    [TestFixture]
    public class TrustedOriginTest
    {
        [Test]
        public void RipToLip() {
            var to = new TrustedOrigin();
            to.RemoteAddressName = "111.111.111.111";
            to.LocalAddressName = "112.112.112.112";
            Assert.True(to.IsMatch(
                new IPEndPoint(IPAddress.Parse("111.111.111.111"),123), 
                 new IPEndPoint(IPAddress.Parse("112.112.112.112"),123),
                 "http://any.com"
                ));
            Assert.False(to.IsMatch(
                new IPEndPoint(IPAddress.Parse("111.111.111.112"), 123),
                 new IPEndPoint(IPAddress.Parse("112.112.112.112"), 123),
                 "http://any.com"
                ));
            Assert.False(to.IsMatch(
               new IPEndPoint(IPAddress.Parse("111.111.111.111"), 123),
                new IPEndPoint(IPAddress.Parse("112.112.112.113"), 123),
                "http://any.com"
               ));
        }
    }
}
