using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp.V1_2{

	[TestFixture]
	public class Q190MultiphasePatchSupport{
		[Test]
		public void SimpleBeforePatch()
		{
			var code = @"
class A x=1 prototype=x
class B x=2 prototype=y
	import A
patch for=x y=${x} z=3 before
";
			var ctx = BSharpCompiler.Compile(code);
			var a = ctx["A"];
			var b = ctx["B"];
			Console.WriteLine(a.Compiled.ToString().Replace("\"","\"\""));
			Console.WriteLine(b.Compiled.ToString().Replace("\"","\"\""));
			Assert.AreEqual(@"<class code=""A"" x=""1"" prototype=""x"" y=""1"" z=""3"" fullcode=""A"" />".Trim().LfOnly(), a.Compiled.ToString().Trim().LfOnly());
			Assert.AreEqual(@"<class code=""B"" x=""2"" prototype=""y"" fullcode=""B"" y=""2"" z=""3"" />".Trim().LfOnly(), b.Compiled.ToString().Trim().LfOnly());
		}
		[Test]
		public void SimpleAfterBuildPatch()
		{
			var code = @"
class A x=1 prototype=x
class B x=2 prototype=y
	import A
patch for=x y=${x} z=3 build
";
			var ctx = BSharpCompiler.Compile(code);
			var a = ctx["A"];
			var b = ctx["B"];
			Console.WriteLine(a.Compiled.ToString().Replace("\"", "\"\""));
			Console.WriteLine(b.Compiled.ToString().Replace("\"", "\"\""));
			Assert.AreEqual(@"<class code=""A"" x=""1"" prototype=""x"" fullcode=""A"" z=""3"" />".Trim().LfOnly(), a.Compiled.ToString().Trim().LfOnly());
			Assert.AreEqual(@"<class code=""B"" x=""2"" prototype=""y"" fullcode=""B"" />".Trim().LfOnly(), b.Compiled.ToString().Trim().LfOnly());
		}
		
	}


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