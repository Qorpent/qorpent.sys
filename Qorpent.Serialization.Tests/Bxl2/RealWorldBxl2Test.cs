using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Bxl2;
using Qorpent.Utils.Extensions;
namespace Qorpent.Serialization.Tests.Bxl2
{
    [TestFixture]
    public class RealWorldBxl2Test
    {
        [TestCase("demo.import.forms.m600.bxls")]
        [TestCase("presentation_ocm_structure.hql")]
        public void HardTest(String filename) {

            String bxl = GetType().Assembly.ReadManifestResource(filename);
            var xml1 = new BxlParser().Parse(bxl);
            var xml2 = new BxlParser2().Parse(bxl);
            Assert.AreEqual(xml1.ToString(), xml2.ToString());
        }
    }
}
