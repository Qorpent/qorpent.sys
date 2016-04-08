using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.Experiments;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests.Experiments
{
    [TestFixture]
    public class PrettyJsonTest
    {
        [Test]
        public void SimpleNest() {
            var j = new {
                x = new {a = 1},
                y = new {b = 2}
            }.stringify(pretty: true).Simplify(SimplifyOptions.SingleQuotes);
            Console.WriteLine(j);
            Assert.AreEqual(@"{
    'x':{
        'a':1
    },
    'y':{
        'b':2
    }
}", j);
        }
    }
}
