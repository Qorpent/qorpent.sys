using System;
using System.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.ObjectXml;
using Qorpent.Utils.Extensions;

namespace Qorpent.Serialization.Tests.ObjectXml
{
	[TestFixture]
	public class SimpleObjectXmlBuilds
	{
		ObjectXmlCompilerIndex Compile(string code) {
			var xml = new BxlParser().Parse(code, "c.bxl");
			var cfg = new ObjectXmlCompilerConfig();
			cfg.UseInterpolation = true;

			var compiler = new ObjectXmlCompiler();
			compiler.Initialize(cfg);
			return  compiler.Compile(new[] {xml});
		}

		ObjectXmlCompilerIndex CompileAll(bool single,params string[] code) {
			var parser = new BxlParser();
			var idx = 0;
			var xmls = code.Select(_ => parser.Parse(_, (idx++) + ".bxl")).ToArray();
			var cfg = new ObjectXmlCompilerConfig();
			cfg.UseInterpolation = true;
			if (single) {
				cfg.SingleSource = true;
			}
			var compiler = new ObjectXmlCompiler();
			compiler.Initialize(cfg);
			return compiler.Compile(xmls);
		}

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
		public void CanInterpolate()
		{
			var result = Compile(@"
class custom abstract
	x=1
	y='${x}${x}'
	z='${y}!'
namespace X
	custom A c='${u:3}' x='${c}${.x}2'
		import B
	custom B
		x=4");
			Assert.AreEqual(2, result.Working.Count);
			var xml = result.Working[0].Compiled;
			Console.WriteLine(xml);
			Console.WriteLine(result.Working[0].ParamIndex);
			Console.WriteLine(result.Working[0].SrcParamIndex);
			Assert.AreEqual("342",xml.Attr("x"));
			Assert.AreEqual("342342", xml.Attr("y"));
			Assert.AreEqual("342342!", xml.Attr("z"));
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
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("B", result.Working[1].Name);
			Assert.AreEqual(1, result.Working[1].Imports.Count);
			Assert.AreEqual("A", result.Working[1].Imports[0].TargetCode);
			Assert.NotNull( result.Working[1].Imports[0].Target);
			Assert.AreEqual("A",result.Working[1].Imports[0].Target.Name);
			
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
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("B", result.Working[1].Name);
			Assert.AreEqual(4, result.Working[1].CollectImports().Count());
			CollectionAssert.AreEqual(new[]{"custom","C","D","A"},
				result.Working[1].CollectImports().Select(_=>_.Name).ToArray()
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
			Assert.AreEqual("A", result.Working[0].Name);
			Assert.AreEqual("B", result.Working[1].Name);
			Assert.AreEqual(4, result.Working[1].CollectImports().Count());
			CollectionAssert.AreEqual(new[] { "custom", "C", "D", "A" },
				result.Working[1].CollectImports().Select(_ => _.Name).ToArray()
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
			Assert.AreEqual(3, result.Working[0].MergeDefs.Count);

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
	element myreportset orverride=report
	");
			Assert.AreEqual(1, result.Working.Count);
			Assert.AreEqual("mythema", result.Working[0].Name);
			Assert.AreEqual(4, result.Working[0].MergeDefs.Count);

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
			Assert.AreEqual("A", result.Orphans[0].Name);
			Assert.AreEqual("B", result.Orphans[1].Name);
			Assert.AreEqual("custom", result.Orphans[0].DefaultImportCode);
			Assert.AreEqual("A", result.Orphans[1].DefaultImportCode);
			Assert.Null(result.Orphans[0].DefaultImport);
			Assert.NotNull(result.Orphans[1].DefaultImport);
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
	}
}
