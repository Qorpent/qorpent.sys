using System.Collections.Generic;
using System.Net;
using NUnit.Framework;
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
        public void CanWorkWithDots() {
            var scope = new Scope();
            scope["a.x"] = 1;
            Assert.AreEqual(1,scope["a.x"]);
                
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
        public void DefaultOneDotBehaviorOn3DLevel() {
            var s1 = new Scope(new {code = 1});
            var s2 = new Scope(new {code = 2},s1);
            var s3 = new Scope(new {}, s2);
            Assert.AreEqual(2,s3["code"]);
            Assert.AreEqual(1,s3[".code"]);
        }

        [Test]
        public void DefaultOneDotBehaviorOn2DLevel()
        {
            var s2 = new Scope(new { code = 2 });
            var s3 = new Scope(new { }, s2);

            Assert.AreEqual(2, s3[".code"]);
        }

        [Test]
        public void DefaultZeroOnNonCompatibleNestWithOption()
        {
            var s2 = new Scope(new { });
            var s3 = new Scope(new {code=1 }, s2);
            Assert.AreEqual(1, s3["code"]);
            Assert.AreEqual(null, s3[".code"]);
        }


        [Test]
        public void DefaultZeroOnNonCompatibleNestWithNonDefaultOption()
        {
            var s2 = new Scope(new { });
            var s3 = new Scope(new { code = 1 }, s2);
            Assert.AreEqual(1, s3["code"]);
            Assert.AreEqual(1, s3.Get("code",new ScopeOptions{TreatFirstDotAsLevelUp = false}));
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

        [Test]
        public void CanGetContainsKeyOfParent() {
            var parent = new Scope(new { a = 1 });
            var child = new Scope(new { a = 2 }, parent);
            Assert.True(child.ContainsKey(".a"));
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

        [Test]
        public void BugMustCreateWithNulls() {
            var scope = new Scope(null);
        }

        [Test]
        public void SupportDotedNames() {
            var cfg = new Scope();
            cfg.Set("a.b",1);
            cfg.Set("a.c",2);
            var cfg2 = new Scope(cfg);
            cfg2.Set("a.b", 3);
            Assert.AreEqual(3,cfg2.Get("a.b"));
            Assert.AreEqual(1,cfg2.Get(".a.b"));
            Assert.AreEqual(2,cfg2.Get("a.c"));
        }

	}
}
