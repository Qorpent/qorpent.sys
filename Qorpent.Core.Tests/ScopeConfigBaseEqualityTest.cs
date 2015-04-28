using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Config;
using Qorpent.Mvc;
using Qorpent.Utils.Extensions;

namespace Qorpent.Core.Tests
{
    [TestFixture]
	public class ScopeConfigBaseEqualityTest
	{
		[Test]
		public void OverridesParent() {
			var cfgbase = new Scope();
			var cfgchild = new Scope();
			cfgchild.SetParent(cfgbase);
			cfgbase.Set("a", 1);
			cfgchild.Set("a", 2);
			Assert.AreEqual(2,cfgchild.Get<object>("a"));
		}

		[Test]
		public void CanResolveFromParentImplicitly() {
			var cfgbase = new Scope();
			var cfgchild = new Scope();
			cfgchild.SetParent(cfgbase);
			cfgbase.Set("a", 1);
			Assert.AreEqual(1, cfgchild.Get<object>("a"));
		}

		[Test]
		public void CanResolveFromParentExplcitly()
		{
			var cfgbase = new Scope();
			var cfgchild = new Scope();
			cfgchild.SetParent(cfgbase);
			cfgbase.Set("a", 1);
			cfgchild.Set("a", 2);
			Assert.AreEqual(1, cfgchild.Get<object>(".a"));
		}
		[Test]
		public void CanResolveFromParentExplcitly3dLevel() {
			var cfgbase = new Scope();
			var cfgbase2 = new Scope();
			var cfgchild = new Scope();
			cfgbase2.SetParent(cfgbase);
			cfgchild.SetParent(cfgbase2);
			cfgbase.Set("a", 1);
			cfgbase2.Set("a", 2);
			cfgchild.Set("a", 3);
			Assert.AreEqual(3, cfgchild.Get<object>("a"));
			Assert.AreEqual(2, cfgchild.Get<object>(".a"));
			Assert.AreEqual(1, cfgchild.Get<object>("..a"));
			//пересмотрено поведение по переходу границы - берется самый последний вариант
			//мотивация - иначе очень сложно описать ... для неизвестного уровня вложенности
			//и теперь . обозначает "желательный пропуск значения"
			Assert.AreEqual(1,cfgchild.Get<object>("...a"));
		}
        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void CanPreventInheritance() {
            var cfgbase = new Scope();
            var cfgchild = new Scope();
            cfgchild.SetParent(cfgbase);
            cfgbase.Set("a", 1);
            Assert.AreEqual(1, cfgchild.Get<object>("a"));
            cfgchild.UseInheritance = false;
            Assert.IsNull(cfgchild.Get<object>("a"));
        }

	    [Test]
	    public void CanBeUsedInInterpolation() {
	        var parent = new Scope(new {a = 1});
	        var child = new Scope(new {a = 2}, parent);
            Assert.AreEqual(1,child.Get(".a"));
            Assert.AreEqual(1,child[".a"]);
	        var result = "${a} - ${.a}".Interpolate(child);
            Assert.AreEqual("2 - 1",result);
	    }

        /// <summary>
        /// 
        /// </summary>
		[Test]
		public void DictionaryCompatibilityWorks() {
			var basecfg = new Scope();
			basecfg.Set("a", 1);
			basecfg.Set("c", 4);
			var childcfg = new Scope();
			childcfg.SetParent(basecfg);
			childcfg.Set("a", 2);
			childcfg.Set("b", 3);
			IDictionary<string, object> dict = childcfg;
			CollectionAssert.AreEquivalent(new[]{"a","b","c"},dict.Keys);
			CollectionAssert.AreEquivalent(new[]{2,4,3},dict.Values);
			Assert.True(dict.ContainsKey("c"));


			Assert.True(dict.ContainsKey("a"));
			Assert.True(dict.ContainsKey("b"));
			Assert.False(dict.ContainsKey("d"));

			Assert.AreEqual(3,dict["b"]);
			Assert.AreEqual(2,dict["a"]);
			Assert.AreEqual(4,dict["c"]);

            Assert.True(dict.ContainsKey(".c"));
            Assert.True(dict.ContainsKey(".a"));
           
		}

	    [Test]
	    [Ignore("Due to skip behavior changes")]
	    public void ThisIsNotCompatible() {
            var basecfg = new Scope();
            basecfg.Set("a", 1);
            basecfg.Set("c", 4);
            var childcfg = new Scope();
            childcfg.SetParent(basecfg);
            childcfg.Set("a", 2);
            childcfg.Set("b", 3);
            IDictionary<string, object> dict = childcfg;
            Assert.False(dict.ContainsKey(".b"));
	    }

	}
}
