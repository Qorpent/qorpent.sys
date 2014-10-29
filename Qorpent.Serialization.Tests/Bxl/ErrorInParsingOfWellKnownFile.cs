using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.Bxl
{
    [TestFixture]
    public class BugEndOnExpressinInPreFileInBxlParseRow
    {

        [Test]
        public void CanParseFromString()
        {
            var p = new BxlParser();
            p.Parse(@"a ()");
            p.Parse("a bc");
        }


    }
}
