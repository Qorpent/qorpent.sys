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
// Original file : ComponentManifestTest.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using NUnit.Framework;

namespace Qorpent.IoC.Tests {
	[TestFixture]
	public class ComponentManifestTest {
		public interface ITestServiceForManifestClassTest {}
		public interface ITestServiceForManifestClassTest2 {}

		[ContainerComponent]
		public class TestServiceBaseForManifestClassTest : ServiceBase, ITestServiceForManifestClassTest {}

        [ContainerComponent(Names=new [] {"a","b"},ServiceTypes = new [] {typeof(ITestServiceForManifestClassTest),typeof(ITestServiceForManifestClassTest2) })]
        public class TestServiceBaseForManifestClassTest2 : ServiceBase, ITestServiceForManifestClassTest, ITestServiceForManifestClassTest2 { }

        [ContainerComponent(Lifestyle = Lifestyle.Singleton, Names = new[] { "a", "b" }, ServiceTypes = new[] { typeof(ITestServiceForManifestClassTest), typeof(ITestServiceForManifestClassTest2) })]
        public class TestServiceBaseForManifestClassTest3 : ServiceBase, ITestServiceForManifestClassTest, ITestServiceForManifestClassTest2 { }

        [ContainerComponent(Lifestyle = Lifestyle.Singleton, ServiceType = typeof(ITestServiceForManifestClassTest))]
        [ContainerComponent(Lifestyle = Lifestyle.Singleton, ServiceType = typeof(ITestServiceForManifestClassTest2))]
        public class TestServiceBaseForManifestClassTest4 : ServiceBase, ITestServiceForManifestClassTest, ITestServiceForManifestClassTest2 { }


        [Test]
		public void InterfaceResolvedWell() {
			var mcd = new ManifestClassDefinition(typeof (TestServiceBaseForManifestClassTest));
			Assert.AreEqual(typeof (ITestServiceForManifestClassTest), mcd.GetComponents().FirstOrDefault().ServiceType);
		}

	    [Test]
	    public void MultipleDefinitionTest() {
	        var mcd = new ManifestClassDefinition(typeof(TestServiceBaseForManifestClassTest2));
	        var components = mcd.GetComponents().ToArray();
            Assert.AreEqual(4,components.Length);
            Assert.True(components.Any(_=>_.Name=="a" && _.ServiceType==typeof(ITestServiceForManifestClassTest)));
            Assert.True(components.Any(_=>_.Name=="a" && _.ServiceType==typeof(ITestServiceForManifestClassTest2)));
            Assert.True(components.Any(_=>_.Name=="b" && _.ServiceType==typeof(ITestServiceForManifestClassTest)));
            Assert.True(components.Any(_=>_.Name=="b" && _.ServiceType==typeof(ITestServiceForManifestClassTest2)));
	    }

	    [Test]
	    public void JoinedSingletonInstantiation() {
	        var mcd = new ManifestClassDefinition(typeof(TestServiceBaseForManifestClassTest3));
	        var components = mcd.GetComponents().ToArray();
	        var c = new Container();
	        foreach (var component  in components) {
	            c.Register(component);
	        }
	        var srv = c.Get<object>("a");
	        var srv2 = c.Get<object>("b");
            Assert.AreSame(srv,srv2);
	    }

        [Test]
        public void MultiServiceMultiAttributes()
        {
            var mcd = new ManifestClassDefinition(typeof(TestServiceBaseForManifestClassTest4));
            var components = mcd.GetComponents().ToArray();
            Assert.AreEqual(2,components.Length);
            var c = new Container();
            foreach (var component in components)
            {
                c.Register(component);
            }
            var srv = c.Get<ITestServiceForManifestClassTest>();
            var srv2 = c.Get<ITestServiceForManifestClassTest2>();
            Assert.IsInstanceOf<TestServiceBaseForManifestClassTest4>(srv);
            Assert.IsInstanceOf<TestServiceBaseForManifestClassTest4>(srv2);
            Assert.AreNotSame(srv, srv2);
        }

    }
}