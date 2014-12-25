using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Qorpent.Serialization.Tests.Json {
    [TestFixture]
    public class ZippedToArrayGeneration {
        [Test]
        public void CanGenerateArrayBased() {
            var obj = new {
                x = new[] {"a", "b", "c"},
                y = new[] {new[] {"1", "2", "3"}, new[] {"4", "5", "6"}},
                z = new Dictionary<string, string> { { "f", "g" }, { "h","i"} }
            };
            var str = obj.SerializeAsJsonString().Replace("\"", "'");
            Console.WriteLine(str);
            Assert.AreEqual(@"{'x': ['a', 'b', 'c'], 'y': [['1', '2', '3'], ['4', '5', '6']], 'z': {'f': 'g', 'h': 'i'}}", str);
        }
    }
}