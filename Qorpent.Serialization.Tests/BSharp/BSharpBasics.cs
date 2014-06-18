﻿using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Qorpent.BSharp;
using Qorpent.Config;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.BSharp
{
	[TestFixture]
	public class BSharpBasics : CompileTestBase {
		[Test]
		public void SingleSourceMode() {
			var result = CompileAll(false, @"class custom abstract", @"custom a");
			Assert.AreEqual(0, result.Working.Count);
			Assert.AreEqual(1, result.Orphans.Count);
			result = CompileAll(true, @"class custom abstract", @"custom a");
			Assert.AreEqual(0, result.Orphans.Count);
			Assert.AreEqual(1, result.Working.Count);
		}

		[Test]
		public void CanCompileSingleClass() {
			var result = Compile(@"class A");
			Assert.AreEqual(1,result.Working.Count);
			Assert.AreEqual("A",result.Working[0].Name);
		}
		[Test]
		public void CanCompileSingleClassInNamespace()
		{
			var result = Compile(@"
namespace X
	class A");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("X.A", result.Working[0].FullName);
		}


		[Test]
		public void DefaultImport()
		{
			var result = Compile(@"
class custom abstract
custom A");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("custom", result.Working[0].DefaultImportCode);
			Assert.NotNull(result.Working[0].DefaultImport);
			Assert.AreEqual("custom", result.Working[0].DefaultImport.Name);
		}

		[Test]
		public void CycleInterpolation(){
			var result = Compile(@"
class A name='${x}' x=1
A B name='${name} 1'
"
);
			Assert.AreEqual(1,result.GetErrors(ErrorLevel.Error).Count());
			
		}

		

		[Test]
		public void DefaultImportInNamespace()
		{
			var result = Compile(@"
namespace X
	class custom abstract
	custom A");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("custom", result.Working[0].DefaultImportCode);
			Assert.NotNull(result.Working[0].DefaultImport);
			Assert.AreEqual("custom", result.Working[0].DefaultImport.Name);
		}


		[Test]
		public void MergableAsyncCall(){
			var ctx = new BSharpContext();
			var p1 = BSharpCompiler.CompileAsync("class A").ContinueWith(_=>ctx.Merge(_.Result));
			var p2 = BSharpCompiler.CompileAsync("class B").ContinueWith(_ => ctx.Merge(_.Result));
			var p3 = BSharpCompiler.CompileAsync("class C").ContinueWith(_ => ctx.Merge(_.Result));
			Task.WaitAll(p1, p2, p3);
			Assert.AreEqual(3,ctx.Working.Count);
		}

		[Test]
		public void NestedNamespaces()
		{
			var result = Compile(@"
namespace X
	namespace Y
		class Z");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("Z", result.Working[0].Name);
			Assert.AreEqual("X.Y.Z", result.Working[0].FullName);
		}

		[Test]
		public void DefaultImportFromRootNamespace()
		{
			var result = Compile(@"
class custom abstract
namespace X
	custom A");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("custom", result.Working[0].DefaultImportCode);
			Assert.NotNull(result.Working[0].DefaultImport);
			Assert.AreEqual("custom", result.Working[0].DefaultImport.Name);
		}

		

		[Test]
		public void DefaultImportCrossNamespace()
		{
			var result = Compile(@"
namespace Z
	class custom abstract
namespace X
	Z.custom A");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("Z.custom", result.Working[0].DefaultImportCode);
			Assert.NotNull(result.Working[0].DefaultImport);
			Assert.AreEqual("custom", result.Working[0].DefaultImport.Name);
		}


		[Test]
		public void ResolveNamespaceUp()
		{
			var result = Compile(@"
namespace X
	class A
	namespace Y
		A B
		namespace Z
			B C");
			var th = result.Get("X.A");
			Assert.NotNull(th);
			th = result.Get("X.Y.Z.C");
			Assert.NotNull(th);
			
		}
		[Test]
		public void ResolveNamespaceUp2()
		{
			var result = Compile(@"
namespace X
	class A
namespace X.Y
	A B
namespace X.Y.Z
	B C");
			var th = result.Get("X.A");
			Assert.NotNull(th);
			th = result.Get("X.Y.Z.C");
			Assert.NotNull(th);
		}


		[Test]
        [Ignore("allow singleton anywhere resolution")]
		public void UnresolvedNamespaceNoRoot()
		{
			var result = Compile(@"
namespace Z
	class custom abstract
namespace X
	custom A");
			Assert.AreEqual(1, result.Orphans.Count);
			
		}
        [Test]
        
        public void UnresolvedNamespaceNoRootNew() {
            var result = Compile(@"
namespace Z
	class custom abstract
namespace Y
	class custom abstract
namespace X
	custom A");
            Assert.AreEqual(1, result.Orphans.Count);

        }
		[Test]
		public void MergesInner() {
			var result = Compile(@"
class A
	test 1
class B
	import A
	test 2");
			Console.WriteLine(result["B"].Compiled);
			Assert.AreEqual(2, result.Get("B").Compiled.Elements("test").Count());
		}

		

		[Test]
		public void CanInterpolateAttributeNames(){
			var result = Compile(@"
class A test${t}=${t} 
class B t=2
	import A");

			var b = result.Get("B");
			Console.WriteLine(b.Compiled);
			Assert.AreEqual("2", b.Compiled.Attr("test2"));
		}


		[Test]
		public void MergesInnerNonStatic()
		{
			var result = Compile(@"
class A
	_x = 1
	_y = '${_x}'
	test '${_y}${_y}'
class B _y = 2
	import A
	");
			Console.WriteLine(result.Get("B").Compiled);
			Assert.AreEqual("22", result.Get("B").Compiled.Element("test").Attr("code"));
		}


		[Test]
		public void MergesInnerStatic()
		{
			var result = Compile(@"
class A static
	_x = 1
	_y = '${_x}'
	test '${_y}${_y}'
class B _y = 2
	import A
	");
			Console.WriteLine(result.Working[1].Compiled);
			Assert.AreEqual("11", result.Working[1].Compiled.Element("test").Attr("code"));
		}

		[Test]
		public void MergesInnerStaticInHierarchy()
		{
			var result = Compile(@"
class A abstract explicit
	test 1
	test 2
A B static abstract
A C static abstract
class D 
	import B
	import C
	");
			Console.WriteLine(result.Working[0].Compiled);
			Assert.AreEqual(4, result.Working[0].Compiled.Elements("test").Count());
		}

		[Test]
		public void MergesInnerNonStaticInHierarchy()
		{
			var result = Compile(@"
class A abstract
	test 1
	test 2
A B abstract
A C abstract
class D 
	import B
	import C
	");
			Console.WriteLine(result.Working[0].Compiled);
			Assert.AreEqual(2, result.Working[0].Compiled.Elements("test").Count());
		}

		[Test]
		public void MergesInnerStaticWithParametersOverride()
		{
			var result = Compile(@"
class A static
	x = 1
	y = '${x}'
	z = '${y}'
class B y = 2
	import A
	");
			var c = result.Get("B").Compiled;
			Console.WriteLine(c);
			Assert.AreEqual("1",c.Attr("x"));
			Assert.AreEqual("2",c.Attr("y"));
			Assert.AreEqual("1",c.Attr("z"));
		}

		[Test]
		public void MergesInnerNonStaticWithParametersOverride()
		{
			var result = Compile(@"
class A 
	x = 1
	y = '${x}'
	z = '${y}'
class B y = 2 
	import A
	");
			var c = result.Get("B").Compiled;
			Console.WriteLine(c);
			Assert.AreEqual("1", c.Attr("x"));
			Assert.AreEqual("2", c.Attr("y"));
			Assert.AreEqual("2", c.Attr("z"));
		}

		[Test]
		public void UnresolvedNamespaceMissed()
		{
			var result = Compile(@"
namespace Z
	class custom abstract
namespace X
	Y.custom A");
			Assert.AreEqual(1, result.Orphans.Count);

		}


		[Test]
		public void ImportsProvided()
		{
			var result = Compile(@"
class custom abstract
custom A
custom B
	import A");
			Assert.AreEqual(2, result.Working.Count);
			Assert.AreEqual(1, result.Get("B").SelfImports.Count);
			Assert.AreEqual("A", result.Get("B").SelfImports[0].TargetCode);
			Assert.NotNull(result.Get("B").SelfImports[0].Target);
			Assert.AreEqual("A", result.Get("B").SelfImports[0].Target.Name);
			
		}

		[Test]
		public void ImportsProvidedHierarchically()
		{
			var result = Compile(@"
class custom abstract
custom A
	import C
	import D
custom B
	import A
	import D
custom C
custom D");
			Assert.AreEqual(4, result.Working.Count);
			Assert.AreEqual(4, result.Get("B").AllImports.Count());
			CollectionAssert.AreEqual(new[]{"custom","C","D","A"},
				result.Get("B").AllImports.Select(_=>_.Name).ToArray()
				);
		}


		[Test]
		public void BreakCycles()
		{
			var result = Compile(@"
class custom abstract
custom A
	import C
	import D
	import B
custom B
	import A
	import D
custom C
custom D");
			Assert.AreEqual(4, result.Working.Count);
			Assert.AreEqual(4, result.Get("B").AllImports.Count());
			CollectionAssert.AreEqual(new[] { "custom", "C", "D", "A" },
				result.Get("B").AllImports.Select(_ => _.Name).ToArray()
				);
		}

		[Test]
		public void MergeProvided() {
			var result = Compile(@"
class thema
	element report
	element reportset override=report
	element reportex extend=report
	");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("thema", result.Working[0].Name);
			//включая дефолты
			Assert.AreEqual(5, result.Working[0].AllElements.Count);

		}


		[Test]
		public void MergeProvidedHierarchically()
		{
			var result = Compile(@"
class thema abstract
	element report
	element reportset override=report
	element reportex extend=report
thema mythema
	element report
	element myreportset override=report
	");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("mythema", result.Working[0].Name);
			Assert.AreEqual(6, result.Working[0].AllElements.Count);

		}

		[Test]
		public void OrphanClassBasics()
		{
			var result = Compile(@"custom A");
			Assert.AreEqual(1, result.Orphans.Count);
			Assert.AreEqual("A", result.Orphans[0].Name);
			Assert.AreEqual("custom", result.Orphans[0].DefaultImportCode);
			Assert.Null( result.Orphans[0].DefaultImport);
		}


		[Test]
		public void OrphanClassHierarchy()
		{
			var result = Compile(@"
custom A
A B");
			Assert.AreEqual(2, result.Orphans.Count);
			Assert.AreEqual("custom", result.Orphans.First(_=>_.Name=="A").DefaultImportCode);
			Assert.AreEqual("A", result.Orphans.First(_ => _.Name == "B").DefaultImportCode);
			Assert.Null(result.Orphans.First(_ => _.Name == "A").DefaultImport);
			Assert.NotNull(result.Orphans.First(_ => _.Name == "B").DefaultImport);
		}

		[Test]
		public void CanCompileSingleClassInNamespaceAbstract()
		{
			var result = Compile(@"
namespace X
	class A abstract");
			Assert.AreEqual(0, result.Working.Count);
			Assert.AreEqual(1, result.Abstracts.Count);
			Assert.AreEqual("A", result.Abstracts[0].Name);
			Assert.AreEqual("X.A", result.Abstracts[0].FullName);
		}

		[Test]
		public void AbstractsNotIncluded(){
			var result = Compile(@"class x abstract");
			Assert.AreEqual(0,result.Working.Count);
		}

		[Test]
		public void SimplestInterpolate(){
			var result = Compile(@"
class custom abstract
	_priv=ZZZ
namespace X
	custom A x='${_priv}'");

			var cls = result.Get("X.A");
			Console.WriteLine(cls.Compiled.ToString());
			Assert.AreEqual("<custom code=\"A\" x=\"ZZZ\" fullcode=\"X.A\" />",cls.Compiled.ToString());
		}

		[Test]
		public void CanInterpolate()
		{


			var result = Compile(@"

namespace X
	custom A c='${u:3}' x='${c}${.x}${_priv}2'
		import B
		any '${c}${x}!!!'
	custom B
		x=4
class custom abstract
	_priv=ZZZ
	x=1
	y='${x}${x}'
	z='${y}!'
");
			Assert.AreEqual(2, result.Working.Count);
			var xml = result.Get("A").Compiled;
			Console.WriteLine(xml);
			Console.WriteLine("=================================");
			Console.WriteLine(result.Get("A").ParamSourceIndex.ToString(ConfigRenderType.SimpleBxl));
			Console.WriteLine("=====================================");
			Console.WriteLine(result.Get("A").ParamIndex.ToString(ConfigRenderType.SimpleBxl));
			Assert.AreEqual("34ZZZ2", xml.Attr("x"));
			Assert.AreEqual("34ZZZ234ZZZ2", xml.Attr("y"));
			Assert.AreEqual("34ZZZ234ZZZ2!", xml.Attr("z"));
			Console.WriteLine("=================================");

		}

		[Test]
		public void ConditionalImport()
		{
			var result = Compile(@"
class custom abstract
	test='${_a}${_b}${_c}'
	import A if='_x'
	import B if='_y'
	import C if='_x & _y'

class A abstract _a = 1
class B abstract _b = 2
class C abstract _c = 3

custom X 'xed' _x
custom Y 'yed' _y
custom Z 'zed' _x _y
");
			Assert.AreEqual(3, result.Working.Count);
			Assert.AreEqual("1", result.Get("X").Compiled.Attribute("test").Value);
			Assert.AreEqual("2", result.Get("Y").Compiled.Attribute("test").Value);
			Assert.AreEqual("123", result.Get("Z").Compiled.Attribute("test").Value);
			Console.WriteLine(result.Working[2].Compiled);
		}
	}
}
