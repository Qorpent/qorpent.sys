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
// Original file : TryDslFramework.cs
// Project: Qorpent.Dsl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.TestSupport;

namespace Qorpent.Dsl.Tests.Dsl {
	[TestFixture]
	[QorpentFixture(UseTemporalFileSystem = true,
		PrepareFileSystemMap = @"
trydsl.bxl~src/trydsl.bxl,
trysql.xslt~dsl/trysql/default.xslt,
trydsl.xslt~dsl/trydsl/default.xslt,
trysplit.xslt~dsl/trysplit/default.xslt
")]
	public class TryDslFramework : QorpentFixture {
		public interface ITryDsl {
			IEnumerable<string> GetStrings();
		}

		[Test]
		public void Dll_With_Param() {
			var app = new Application
				{
					Container =
						ContainerFactory.CreateDefault()
							.SetParameter<IFileNameResolver>("root", Tmpdir)
				};
			var proj = new DslProject
				{
					LangName = "trydsl",
					SetupSources = new[] {"src/*.bxl"},
					OutputDirectory = "~/outbin",
					NativeCodeDirectory = "~/outcode",
					ResultName = "testlib",
					CompilerOptions = "YIELDD",
				};

			var dsl = app.Dsl.GetProvider(proj);
			var result = dsl.Run(proj);
			if (result.MaxLevel == ErrorLevel.Error) {
				foreach (var dslMessage in result.Messages) {
					Console.WriteLine(dslMessage.Message);
				}

				Assert.Fail("compilation error");
			}
			var extensionType = result.LoadAssembly().GetType("Tests.Test");
			var inst = (ITryDsl) Activator.CreateInstance(extensionType);
			CollectionAssert.AreEquivalent(new[] {"A", "B", "C", "D"}, inst.GetStrings());
		}

		[Test]
		public void InApplication() {
			var app = new Application
				{
					Container =
						ContainerFactory.CreateDefault()
							.SetParameter<IFileNameResolver>("root", Tmpdir)
				};
			var proj = new DslProject
				{
					LangName = "trydsl",
					SetupSources = new[] {"src/*.bxl"},
					OutputDirectory = "~/outbin",
					NativeCodeDirectory = "~/outcode",
					ResultName = "testlib",
				};
			var dsl = app.Dsl.GetProvider(proj);

			var extensionType = dsl.Run(proj).LoadAssembly().GetType("Tests.Test");
			var inst = (ITryDsl) Activator.CreateInstance(extensionType);
			CollectionAssert.AreEquivalent(new[] {"A", "B", "C"}, inst.GetStrings());
		}

		[Test]
		public void MainDSLProcessing() {
			var proj = new DslProject
				{
					LangName = "trydsl",
					SetupSources = new[] {"src/*.bxl"},
					OutputDirectory = "~/outbin",
					NativeCodeDirectory = "~/outcode",
					ResultName = "testlib",
					RootDirectory = Tmpdir
				};
			var dsl = new XsltBasedDslProvider();
			dsl.SetFileNameResolver(new FileNameResolver {Root = Tmpdir});
			var result = dsl.Run(proj);
			var lib = Assembly.Load(File.ReadAllBytes(result.GetProductionFileName()));
			var type = lib.GetType("Tests.Test");
			var inst = (ITryDsl) Activator.CreateInstance(type);
			CollectionAssert.AreEquivalent(new[] {"A", "B", "C"}, inst.GetStrings());
		}

		[Test]
		public void SplitProjectTypeTest() {
			var app = new Application
				{
					Container =
						ContainerFactory.CreateDefault()
							.SetParameter<IFileNameResolver>("root", Tmpdir)
				};
			var proj = new DslProject
				{
					LangName = "trysplit",
					ProjectType = DslProjectType.SqlScript,
					SetupSources = new[] {"src/*.bxl"},
					OutputDirectory = "~/outsplit",
					NativeCodeDirectory = "~/outsplitcode",
					ResultName = "testscript",
				};
			var dsl = app.Dsl.GetProvider(proj);

			var script = dsl.Run(proj).GetResult();
			Console.WriteLine(script);
			Assert.AreEqual(@"
-- FILE : c1

		select 'A'


GO
-- FILE : c2

		select 'B'


GO
-- FILE : c3

		select 'C'


GO
".Trim(), script.Trim());
		}

		[Test]
		public void SqlProjectTypeTest() {
			var app = new Application
				{
					Container =
						ContainerFactory.CreateDefault()
							.SetParameter<IFileNameResolver>("root", Tmpdir)
				};
			var proj = new DslProject
				{
					LangName = "trysql",
					ProjectType = DslProjectType.SqlScript,
					SetupSources = new[] {"src/*.bxl"},
					OutputDirectory = "~/outsql",
					NativeCodeDirectory = "~/outcode",
					ResultName = "testscript",
				};
			var dsl = app.Dsl.GetProvider(proj);

			var script = dsl.Run(proj).GetResult();
			Assert.AreEqual(@"
-- FILE : __src__trydsl.bxl

		select 'A'
		select 'B'
		select 'C'

GO
".Trim(), script.Trim());
		}


		[Test]
		public void SqlProjectTypeTest_WithParam() {
			var app = new Application
				{
					Container =
						ContainerFactory.CreateDefault()
							.SetParameter<IFileNameResolver>("root", Tmpdir)
				};
			var proj = new DslProject
				{
					LangName = "trysql",
					ProjectType = DslProjectType.SqlScript,
					SetupSources = new[] {"src/*.bxl"},
					OutputDirectory = "~/outsql",
					NativeCodeDirectory = "~/outcode",
					ResultName = "testscript",
					CompilerOptions = "SELECTD",
				};
			var dsl = app.Dsl.GetProvider(proj);

			var script = dsl.Run(proj).GetResult();
			Console.WriteLine(script);
			Assert.AreEqual(@"
-- FILE : __src__trydsl.bxl

		select 'A'
		select 'B'
		select 'C'
		select 'D'       
    

GO
".Trim(), script.Trim());
		}
	}
}