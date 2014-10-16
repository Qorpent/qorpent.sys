using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Qorpent.Integration.SqlExtensions.Tests {
    [TestFixture]
    public class SpatialFunctions {
        [Test]
        public void InverseMercator() {
            Assert.AreEqual(56.8215295153752d, SqlExtensions.SpatialFunctions.GetLat(7723728).Value,0.00001);
            Assert.AreEqual(60.5359578355639d, SqlExtensions.SpatialFunctions.GetLon(6738832).Value,0.00001);
        }

        [Test]
        public void ForwardMercator() {
            Assert.AreEqual(6738832, SqlExtensions.SpatialFunctions.GetX(60.5359578355639d).Value, 0.00001);
            Assert.AreEqual(7723728, SqlExtensions.SpatialFunctions.GetY(56.8215295153752d).Value, 0.00001);
        }



    }
}
