using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Console.WriteLine(forms.Length
                );
        }
    }
}
