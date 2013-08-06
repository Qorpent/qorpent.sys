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
// Original file : ContainerLoaderTest.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.Bxl;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.IoC.Tests.InnerNs;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

[assembly: ContainerExport(800, Lifestyle.PerThread)]

namespace Qorpent.IoC.Tests {
	namespace InnerNs {
		public class ManifestTestService : ITestService2 {
			public string[] str_array_param { get; set; }
			public IDictionary<string,string> str_dict_param { get; set; }
			public string Val { get; set; }
		}
	}

	[TestFixture]
	public class ContainerLoaderTest {
		public interface IMyService {}

		public class MyClass : IMyService {}
		
		[Test]
		public void FileNameResolver_Returned_On_IFileNameResolve(){
			var container = new Container();
			container.GetLoader().LoadAssembly(typeof(Qorpent.IO.FileNameResolver).Assembly);
			ContainerFactory.DumpContainer(container);
			var res = container.Get<Qorpent.IO.IFileNameResolver>();
			Assert.True(res is FileNameResolver);
		}

		[Test]
		public void CanLoadAssembly() {
			var c = new Container();
			c.GetLoader().LoadAssembly(typeof (ContainerLoaderTest).Assembly);
			var components = c.GetComponents().ToArray();
			Assert.AreEqual(4, components.Length);
			Assert.NotNull(components.FirstOrDefault(x =>
			                                         x.ServiceType == typeof (ITestService1)
			                                         &&
			                                         x.Lifestyle == Lifestyle.PerThread
			                                         &&
			                                         x.Priority == 800
			                                         &&
			                                         x.Name == ""
			                                         &&
			                                         x.ImplementationType == typeof (ExportedTestService1)
				               ));
			Assert.NotNull(components.FirstOrDefault(x =>
			                                         x.ServiceType == typeof (ITestService2)
			                                         &&
			                                         x.Lifestyle == Lifestyle.Extension
			                                         &&
			                                         x.Name == "name1"
			                                         &&
			                                         x.Priority == 600
			                                         &&
			                                         x.ImplementationType == typeof (ExportedTestService2)
				               ));
			Assert.NotNull(components.FirstOrDefault(x =>
			                                         x.ServiceType == typeof (ITestService2)
			                                         &&
			                                         x.Lifestyle == Lifestyle.Extension
			                                         &&
			                                         x.Name == "name2"
			                                         &&
			                                         x.Priority == 500
			                                         &&
			                                         x.ImplementationType == typeof (ExportedTestService2_2)
				               ));
		}
		
		

		[Test]
		public void CanLoadDefaultManifest() {
			Application.Current.Files.Write("~/main.ioc-manifest.bxl", @"
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
pooled 'name1', ManifestTestService  : ITestService2
");
			Application.Current.Files.Write("~/usr/second.ioc-manifest.bxl", @"
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
transient 'name2', ManifestTestService  : ITestService2
");
			var c = new Container();
			c.Register(c.NewComponent<IFileNameResolver, FileNameResolver>());
			c.Register(c.NewComponent<ILogManager, DefaultLogManager>(Lifestyle.Singleton));
			c.Register(c.NewComponent<IBxlParser, BxlParser>(Lifestyle.Singleton));
			c.GetLoader().LoadDefaultManifest(false);
			Assert.AreEqual(5, c.GetComponents().Count());
		}


		[Test]
		public void Q34CanReadStringArrayParameter()
		{
			var manifest = @"
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
transient 'name1', ManifestTestService  : ITestService2
	str_array_param 
		add x
		add y
		add z
";
			
			var c = new Container();
			var cpt = c.GetLoader().LoadManifest(new BxlParser().Parse(manifest),true).First();
			CollectionAssert.AreEqual(new[]{"x","y","z"},(IEnumerable)cpt.Parameters["str_array_param"]);
			var res = c.Get<ITestService2>()as ManifestTestService;
			CollectionAssert.AreEqual(new[]{"x","y","z"},res.str_array_param);
		}

		[Test]
		public void CanFindWithTags()
		{
			var manifest = @"
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
transient 'name1', ManifestTestService, tag='aaa'  : ITestService2
	Val = 1 
transient 'name2', ManifestTestService, tag='aba'  : ITestService2
	Val = 2
transient 'name3', ManifestTestService, tag='ccc'  : ITestService2
	Val = 3
transient 'name4', ManifestTestService, tag='aac'  : ITestService2
	Val = 4
transient 'name5', ManifestTestService, tag='baa'  : ITestService2
	Val = 5
";

			var c = new Container();
			c.GetLoader().LoadManifest(new BxlParser().Parse(manifest), true);
			var test1 = c.All<object>("tag:^aa");
			CollectionAssert.AreEquivalent(new[]{"1","4"},test1.Cast<ITestService2>().Select(_=>_.Val));
			var test2 = c.All<object>("tag:c");
			CollectionAssert.AreEquivalent(new[] { "3", "4" }, test2.Cast<ITestService2>().Select(_ => _.Val).ToArray());
		}

		[Test]
		public void Q35CanReadStringDictionaryParameter()
		{
			var manifest = @"
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
transient 'name1', ManifestTestService  : ITestService2
	str_dict_param 
		add x a
		add y b
		add z c
";

			var c = new Container();
			var cpt = c.GetLoader().LoadManifest(new BxlParser().Parse(manifest), true).First();
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "x", "a" }, { "y", "b" }, { "z", "c" } }, (IEnumerable)cpt.Parameters["str_dict_param"]);
			var res = c.Get<ITestService2>() as ManifestTestService;
			CollectionAssert.AreEquivalent(new Dictionary<string, string> { { "x", "a" }, { "y", "b" }, { "z", "c" } },  res.str_dict_param);
		}

		[Test]
		public void CanLoadFromBxlManifestWithResolution() {
			var c = new Container();
			var manifest = new BxlParser().Parse(@"
define S Service
define TS ITest{@S}2
define MTS ManifestTest{@S}
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
pooled 'name1', @MTS  : @TS
", "test");
			var loader = c.GetLoader();
			loader.LoadManifest(manifest, false);
			
			var component = c.Components.First();
			Assert.AreEqual(typeof (ITestService2), component.ServiceType);
			Assert.AreEqual(typeof (ManifestTestService), component.ImplementationType);
			Assert.AreEqual(Lifestyle.Pooled, component.Lifestyle);
			Assert.AreEqual("name1", component.Name);
		}


		[Test]
		public void DefineOrder()
		{
			var c = new Container();
			var manifest = new BxlParser().Parse(@"
define X idx=20 : 2
define X idx=30 : 3
define X idx=10 : 1
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
transient 'name1', ManifestTestService  : ITestService2
	Val  = @X
", "test");
			var loader = c.GetLoader();
			loader.LoadManifest(manifest, false);

			var component = c.Components.First();
			Assert.AreEqual(typeof(ITestService2), component.ServiceType);
			Assert.AreEqual(typeof(ManifestTestService), component.ImplementationType);
			Assert.AreEqual("3", component.Parameters["Val"]);

		}
	}



}