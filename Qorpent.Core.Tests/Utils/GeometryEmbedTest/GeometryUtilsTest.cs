using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Embed;

namespace Qorpent.Core.Tests.Utils.GeometryEmbedTest
{
    [TestFixture]
    public class GeometryUtilsTest
    {
        [TestCase(1, 2, 1, 10, 0)]
        [TestCase(1, 2, 4, 5, 45)]
        [TestCase(1, 2, 4, 2, 90)]
        [TestCase(1, 2, 3, 0, 135)]
        [TestCase(1, 2, 1, -10, 180)]
        [TestCase(4, 5, 1, 2, 225)]
        public void CanGetDegreeAngle(double x1, double y1, double x2, double y2, double angle) {
            Assert.AreEqual(angle,GeometryUtil.GetOffsetAngleDegree(new Point(x1,y1),new Point(x2,y2 )));
        }


        [TestCase(0,5,10,15)]
        [TestCase(30, 10, 15, 18.660254037844389)]
        [TestCase(90,5,15,10)]
        [TestCase(180,5,10,5)]
        [TestCase(270,5,5,10)]
        public void CanFindDistancedPoint(double angle,double distance,double rx, double ry) {
            var point = GeometryUtil.GetDegreeAngledOffset(new Point(10, 10), angle,distance);
            Assert.AreEqual(rx,point.X);
            Assert.AreEqual(ry,point.Y);
        }
    }
}
