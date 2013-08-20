using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;
using System;

namespace Qorpent.Serialization.Tests {
    public class BSharpJsonParserTests {
        private const string a1 = @"{""class"": {""0"": {""code"": ""riA"", ""h"" : ""f"", ""fullcode"": ""riA""}, ""1"": {""code"": ""riB"", ""z"" : ""tezt"", ""fullcode"": ""riB""}}}";

        [Test]
        public void CanWorkWithSimpleClasses() {
            Debug.Print(a1);
            var bSharpJsonParser = new BSharpJsonCompiler();
            bSharpJsonParser.LoadJsonContext(json:a1);
            bSharpJsonParser.CompileContext();
            var compiled = (BSharpContext)bSharpJsonParser.GetBSharpContext();

            Console.Write("0: " + compiled.Working[0].Compiled);
            Console.Write("1: " + compiled.Working[1].Compiled);

            Assert.AreEqual(2, compiled.Working.Count);
            Assert.AreEqual("f", compiled.Working[0].Compiled.Attribute("h").Value);
            Assert.AreEqual("tezt", compiled.Working[1].Compiled.Attribute("z").Value);
        }
    }
}
