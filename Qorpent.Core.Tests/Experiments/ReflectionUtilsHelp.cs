using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using Qorpent.Experiments;

namespace Qorpent.Bridge.Tests.Utils
{
    [TestFixture]
    public class ReflectionUtilsHelp
    {
        class A {
            public int IntProp { get; set; }
            internal int IntProp2 { get; set; }
        }

        [Test]
        public void CanDetectDictionaryCompatibility() {
            Assert.True(ReflectionUtils.IsGenericCompatible<IDictionary<object,object>>(typeof(Dictionary<string,string>)));
            Assert.True(ReflectionUtils.IsGenericCompatible<IEnumerable<KeyValuePair<object,object>>>(typeof(Dictionary<string,string>)));
            Assert.True(ReflectionUtils.IsGenericCompatible<IEnumerable<KeyValuePair<object,object>>>(typeof(Dictionary<int,object>)));
            Assert.False(ReflectionUtils.IsGenericCompatible<IEnumerable<KeyValuePair<object,object>>>(typeof(List<int>)));
            Assert.True(ReflectionUtils.IsGenericCompatible<IEnumerable<KeyValuePair<object,object>>>(typeof(List<KeyValuePair<int,object>>)));
        }

       

        [Test]
        public void CanSetAndGetPublicPropertyNoGeneric()
        {
            var a = new A();
            var seter = ReflectionUtils.BuildSetter(typeof(A).GetProperty("IntProp"));
            var geter = ReflectionUtils.BuildGetter(typeof(A).GetProperty("IntProp"));
            seter(a, 2);
            Assert.AreEqual(2, a.IntProp);
            var x = geter(a);
            Assert.AreEqual(2, x);
        }



        [Test]
        public void CanSetAndNonPublicPropertyNoGeneric()
        {
            var a = new A();
            var seter = ReflectionUtils.BuildSetter(typeof(A).GetProperty("IntProp2",  BindingFlags.NonPublic|BindingFlags.Instance));
            var geter = ReflectionUtils.BuildGetter(typeof(A).GetProperty("IntProp2", BindingFlags.NonPublic | BindingFlags.Instance));
            seter(a, 2);
            Assert.AreEqual(2, a.IntProp2);
            var x = geter(a);
            Assert.AreEqual(2, x);
        }

        [Explicit]
        [Test]
        public void CompareWithUsualReflectionSet() {
            var a = new A();
            var pi = typeof (A).GetProperty("IntProp");
            var refseter = pi.GetSetMethod();
            var setter = ReflectionUtils.BuildSetter(pi);
            var dict = new Dictionary<string, object>();
            dict["IntProp"] = setter;
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000000; i++) {
                a.IntProp = i;
            }
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000000; i++)
            {
               refseter.Invoke(a,new object[]{i});
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
            sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000000; i++) {
                setter(a, i);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);

            sw = Stopwatch.StartNew();
            for (var i = 0; i < 10000000; i++)
            {
                (dict["IntProp"] as Action<object,object>)(a,i);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }

    }
}
