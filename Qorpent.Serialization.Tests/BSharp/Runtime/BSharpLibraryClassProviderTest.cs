using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp.Runtime;

namespace Qorpent.Serialization.Tests.BSharp.Runtime
{
    [TestFixture]
    public class BSharpLibraryClassProviderTest
    {
        [Test]
        public void CanLoadIndex() {
            var provider = new BSharpLibraryClassProvider {
                Filename = "res:" + GetType().Assembly.GetName().Name + ":demo.bslib"
            };
            var m111 = provider.GetRuntimeClass("demo.import.forms.m111");
            Assert.NotNull(m111);
            var forms = provider.FindClasses(prototype: "formdef").ToArray();
            Assert.Less(20,forms.Length);
            Console.WriteLine(forms.Length);
        }
    }
}
