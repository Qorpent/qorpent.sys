#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : XmlIncludeMainTest.cs
// Project: Qorpent.Dsl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Xml.Linq;
using NUnit.Framework;
using Qorpent.Bxl;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.TestSupport;

namespace Qorpent.Dsl.Tests.XmlInclude {
	[TestFixture]
	[QorpentFixture(UseTemporalFileSystem = true,
		PrepareFileSystemMap = @"
main.bxl~main/main.bxl,
near.bxl~main/near.bxl,
near2.bxl~main/near2.bxl,
ext.bxl~ext/ext.bxl,
ext2.bxl~ext/ext2.bxl
")]
	public class XmlIncludeMainTest : QorpentFixture {
		public override void SetUp() {
			base.SetUp();
			includer = new XmlIncludeProcessor(FileResover);
			includer.DirectImports["inclA"] = Bxl.Parse(@"inclA x=1");
			includer.DirectImports["inclD"] = Bxl.Parse(@"inclD x=2");
			includer.DirectImports["impA"] = Bxl.Parse(@"impA z=1");
			includer.DirectImports["impD"] = Bxl.Parse(@"impD w=2");
			includer.DirectImports["recA"] = Bxl.Parse(@"
rec1 w=2
qxi::include direct//inclA
qxi::include direct//inclD delay
");
			Simple_Include_And_Import = Bxl.Parse(Simple_Include_And_Import_);
			Simple_Rec_Include = Bxl.Parse(Simple_Rec_Include_);
			mainfile = Path.Combine(Tmpdir, "main/main.bxl");
		}


		private XmlIncludeProcessor includer;
		private XElement Simple_Include_And_Import;

		private readonly BxlGeneratorOptions genopt = new BxlGeneratorOptions
			{
				NoRootElement = true,
				SkipAttributes = new[] {"_file", "_line"},
				InlineAlwaysAttributes = new[] {"x", "z", "w"},
			};

		private XElement Simple_Rec_Include;
		private string mainfile;

		private const string Simple_Include_And_Import_ = @"
e1 :
	qxi::include direct//inclA
	qxi::include direct//inclD delay
	qxi::import direct//impA : //impA
	qxi::import direct//impD delay : //impD
";

		private const string Simple_Rec_Include_ = @"
e1 :
	qxi::include direct//recA
";

		[Test]
		public void OnDiskTest() {
			var result = includer.Load(mainfile);
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
near
ext2
near2
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}

		[Test]
		public void Replace_Test() {
			var result = includer.Include(Bxl.Parse(@"
qxi::replace 'x+' : 'Z'
e t='xxxA' : 'xB'
	b u='xxC'
", "", BxlParserOptions.NoLexData), "");
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
e : ZB
	t=ZA
	b
		u=ZC
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}

		[Test]
		public void Self_Include_Test() {
			var result = includer.Include(Bxl.Parse(@"
e x
e y
	qxi::include self : ""//*[@id='x']""
", "", BxlParserOptions.NoLexData), "~/test.bxl");
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
e x
e y
	e x
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}


		[Test]
		public void SimpleIncludeWithBxlAndDirectContent() {
			var result = includer.Include(Simple_Include_And_Import, "~/test.bxl");
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
e1 w=2, z=1
	inclA x=1
	inclD x=2
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}


		[Test]
		public void SimpleIncludeWithBxlAndDirectContentNoDelay() {
			var result = includer.Include(Simple_Include_And_Import, "~/test.bxl", false);
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
e1 z=1
	inclA x=1
	qxi::include ""direct//inclD"", delay
	qxi::import ""direct//impD"", delay : ""//impD""
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}

		[Test]
		public void SimpleIncludeWithBxlAndDirectContentNoDelay_Recursive() {
			var result = includer.Include(Simple_Rec_Include, "~/test.bxl", false);
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
e1
	rec1 w=2
	inclA x=1
	qxi::include ""direct//inclD"", delay
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}


		[Test]
		public void SimpleIncludeWithBxlAndDirectContent_Recursive() {
			var result = includer.Include(Simple_Rec_Include, "~/test.bxl");
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
e1
	rec1 w=2
	inclA x=1
	inclD x=2
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}


		[Test]
		public void Template_Test() {
			var result = includer.Include(Bxl.Parse(@"
qxi::template t1
	el1 z=2 : 'test'
		el2
t1 X
	el3
", "", BxlParserOptions.NoLexData), "");
			var bxlresult = Bxl.Generate(result, genopt);
			Console.WriteLine(bxlresult);
			Assert.AreEqual(@"
el1 X, z=2 : test
	el2
	el3
".Trim().LfOnly(), bxlresult.Trim().LfOnly());
		}
	}
}