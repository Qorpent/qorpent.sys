using System.Collections.Generic;
using NUnit.Framework;
using Qorpent.Config;

namespace Qorpent.Core.Tests
{
	[TestFixture]
	public class ConfigBaseTest
	{
		[Test]
		public void OverridesParent() {
			var cfgbase = new ConfigBase();
			var cfgchild = new ConfigBase();
			cfgchild.SetParent(cfgbase);
			cfgbase.Set("a", 1);
			cfgchild.Set("a", 2);
			Assert.AreEqual(2,cfgchild.Get<object>("a"));
		}

		[Test]
		public void CanResolveFromParentImplicitly() {
			var cfgbase = new ConfigBase();
			var cfgchild = new ConfigBase();
			cfgchild.SetParent(cfgbase);
			cfgbase.Set("a", 1);
			Assert.AreEqual(1, cfgchild.Get<object>("a"));
		}

		[Test]
		public void CanResolveFromParentExplcitly()
		{
			var cfgbase = new ConfigBase();
			var cfgchild = new ConfigBase();
			cfgchild.SetParent(cfgbase);
			cfgbase.Set("a", 1);
			cfgchild.Set("a", 2);
			Assert.AreEqual(1, cfgchild.Get<object>(".a"));
		}
		[Test]
		public void CanResolveFromParentExplcitly3dLevel()
		{
			var cfgbase = new ConfigBase();
			var cfgbase2 = new ConfigBase();
			var cfgchild = new ConfigBase();
			cfgbase2.SetParent(cfgbase);
			cfgchild.SetParent(cfgbase2);
			cfgbase.Set("a", 1);
			cfgbase2.Set("a", 2);
			cfgchild.Set("a", 3);
			Assert.AreEqual(3, cfgchild.Get<object>("a"));
			Assert.AreEqual(2, cfgchild.Get<object>(".a"));
			Assert.AreEqual(1, cfgchild.Get<object>("..a"));
			Assert.Null(cfgchild.Get<object>("...a"));
		}

		[Test]
		public void DictionaryCompatibilityWorks() {
			var basecfg = new ConfigBase();
			basecfg.Set("a", 1);
			basecfg.Set("c", 4);
			var childcfg = new ConfigBase();
			childcfg.SetParent(basecfg);
			childcfg.Set("a", 2);
			childcfg.Set("b", 3);
			IDictionary<string, object> dict = childcfg;
			CollectionAssert.AreEquivalent(new[]{"a","b","c"},dict.Keys);
			CollectionAssert.AreEquivalent(new[]{2,4,3},dict.Values);
			Assert.True(dict.ContainsKey("c"));

			Assert.True(dict.ContainsKey(".c"));
			Assert.True(dict.ContainsKey(".a"));
			Assert.False(dict.ContainsKey(".b"));
			Assert.True(dict.ContainsKey("a"));
			Assert.True(dict.ContainsKey("b"));
			Assert.False(dict.ContainsKey("d"));

			Assert.AreEqual(3,dict["b"]);
			Assert.AreEqual(2,dict["a"]);
			Assert.AreEqual(4,dict["c"]);
		}
	}
}
