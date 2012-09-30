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
		public class ManifestTestService : ITestService2 {}
	}

	[TestFixture]
	public class ContainerLoaderTest {
		public interface IMyService {}

		public class MyClass : IMyService {}

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
		public void CanLoadFromBxlManifestWithResolution() {
			var c = new Container();
			c.GetLoader().LoadManifest(new BxlParser().Parse(@"
ref	Qorpent.IoC.Tests
using Qorpent.IoC.Tests
using Qorpent.IoC.Tests.InnerNs
pooled 'name1', ManifestTestService  : ITestService2
", "test"), false);
			var component = c.Components.First();
			Assert.AreEqual(typeof (ITestService2), component.ServiceType);
			Assert.AreEqual(typeof (ManifestTestService), component.ImplementationType);
			Assert.AreEqual(Lifestyle.Pooled, component.Lifestyle);
			Assert.AreEqual("name1", component.Name);
		}
	}
}