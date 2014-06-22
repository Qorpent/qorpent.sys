using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;

namespace Qorpent.Serialization.Tests.BSharp.V1_2{
	[TestFixture]
	public class Q197MetaClassDetection{
		[TestCase(@"
dataset x
",
 "__ds_x:Dataset",
 "")]
		[TestCase(@"
generator x
",
 "generator_1307375710070072438717639731937091790006:Generator",
 "")]
		public void DetectMetaAndRawClasses(string code, string metamask, string rawmask ){
			var ctx = BSharpCompiler.Compile(code);
			var meta = string.Join(", ", ctx.MetaClasses.Values.OrderBy(_ => _.FullName).Select(_ => _.FullName + ":" + _.GetAttributes()));
			var raw = string.Join(", ", ctx.RawClasses.Values.OrderBy(_ => _.FullName).Select(_ => _.FullName + ":" + _.GetAttributes()));
			Assert.AreEqual(metamask,meta);
			Assert.AreEqual(rawmask,raw);
		}

		/// <summary>
		/// 
		/// </summary>
		[Test]
		public void RegressCannotDetectUsualClass(){
			var ctx = BSharpCompiler.Compile("class C");
			Assert.NotNull(ctx.Get("C"));
		}
	}
}