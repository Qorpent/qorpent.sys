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
// Original file : ContainerExtendedLifecycleTest.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.Events;
using Qorpent.Utils.Extensions;

namespace Qorpent.IoC.Tests {
	[TestFixture]
	public class ContainerExtendedLifecycleTest {
		[SetUp]
		public void Setup() {
			c = new Container();
			c.Register(c.NewComponent<IEventManager, EventManager>(Lifestyle.Singleton));
			a = new Application {Container = c};
			c.RegisterExtension(new ExtensionTestServiceExtension_CreateRelease());
			component = new ComponentDefinition<IExtensionTestService, ExtensionTestService>(Lifestyle.PerThread);
			component.Parameters["cbt"] = 1;
			component.Parameters["ebt"] = 2;
			c.Register(component);
			obj = (ExtensionTestService) c.Get<IExtensionTestService>();
			c.Release(obj);
		}


		private ExtensionTestService obj;
		private Container c;
		private ComponentDefinition<IExtensionTestService, ExtensionTestService> component;
		private IApplication a;

		private interface INonDefaultCtor {
			string Val { get; }
		}

		private class NonDefaultCtor : INonDefaultCtor {
			private NonDefaultCtor() {}

			private NonDefaultCtor(string val) {
				_val = val;
			}


			public string Val {
				get { return _val; }
			}

			private readonly string _val = "test1";
		}

		[Test]
		public void Application_Bound_Processed() {
			Assert.AreEqual(a.ApplicationName, obj.AppName);
		}

		[Test]
		public void Application_Bound_Processed_On_Default() {
			Application.Current.Container.Register(component);
			obj = (ExtensionTestService) c.Get<IExtensionTestService>();
			Assert.AreEqual(Application.Current.ApplicationName, obj.AppName);
		}

		[Test]
		public void Cleanup_Can_Be_Called_With_Application_Reset_Event() {
			var app = new Application();
			var c = new Container();
			c.Register(c.NewComponent<IEventManager, EventManager>(Lifestyle.Singleton));
			c.Register(new ComponentDefinition<IExtensionTestService, ExtensionTestService>(Lifestyle.Singleton));
			app.Container = c;
			var s = app.Container.Get<IExtensionTestService>();
			Assert.False(s.ContainerBoundRelease);
			app.Reset(QorpentConst.ContainerResetCode);
			Assert.True(s.ContainerBoundRelease);
		}

		[Test]
		public void ContainerBound_After_Create_Called() {
			Assert.True(obj.ContainerBoundCalled);
		}


		[Test]
		public void ContainerBound_Property_Applyed() {
			Assert.AreEqual(1, obj.ContainerBoundTest);
		}

		[Test]
		public void Extension_AfterCreate_Called() {
			Assert.AreEqual(2, obj.ExtensionBoundTest);
		}

		[Test]
		public void Parametrized_Ctor_Called() {
			var c = new Container();
			c.Register(new ComponentDefinition<INonDefaultCtor, NonDefaultCtor>());
			var result = c.Get<INonDefaultCtor>(null, "test2");
			Assert.AreEqual("test2", result.Val);
		}

		[Test]
		public void Private_Default_Ctor_Called() {
			var c = new Container();
			c.Register(new ComponentDefinition<INonDefaultCtor, NonDefaultCtor>());
			var result = c.Get<INonDefaultCtor>();
			Assert.AreEqual("test1", result.Val);
		}

		[Test]
		public void Release_Extension_Called() {
			Assert.True(obj.ExtensionRelease);
		}

		[Test]
		public void Release_IContainer_Called() {
			Assert.True(obj.ContainerBoundRelease);
		}
	}
}