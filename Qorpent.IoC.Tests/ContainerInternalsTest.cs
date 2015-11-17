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
// Original file : ContainerInternalsTest.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;

namespace Qorpent.IoC.Tests {
	[TestFixture]
	public class ContainerInternalsTest {
		[SetUp]
		public void Setup() {
			_c = new Container();
			c1n = new ComponentDefinition<ITestService1, TestService1>(name: "test");
			c1n2 = new ComponentDefinition<ITestService1, TestService1>(name: "test2");
			c2nn = new ComponentDefinition<ITestService2, TestService2>(priority: 10);
			c2n = new ComponentDefinition<ITestService2, TestService2>(name: "test");
		}


		private Container _c;
		private ComponentDefinition<ITestService1, TestService1> c1n;
		private ComponentDefinition<ITestService2, TestService2> c2nn;
		private ComponentDefinition<ITestService1, TestService1> c1n2;
		private ComponentDefinition<ITestService2, TestService2> c2n;

		[Test]
		public void RegisterItemsInValidOrderAndStorage() {
			_c.Register(c1n);
			_c.Register(c1n2);
			_c.Register(c2nn);
			_c.Register(c2n);
			Assert.AreEqual(4, _c.CurrentComponentId);
			Assert.AreEqual(4, _c.Components.Count);
			Assert.AreEqual(2, _c.ByTypeCache.Count);
			Assert.AreEqual(2, _c.ByNameCache.Count);
			Assert.AreEqual(2, _c.ByTypeCache[typeof (ITestService1)].Count);
			Assert.AreEqual(2, _c.ByTypeCache[typeof (ITestService2)].Count);
			Assert.AreEqual(2, _c.ByNameCache["test"].Count);
			Assert.AreEqual(1, _c.ByNameCache["test2"].Count);
			Assert.AreEqual(c1n2, _c.ByTypeCache[typeof (ITestService1)][0]); //most late
			Assert.AreEqual(c1n, _c.ByTypeCache[typeof (ITestService1)][1]);
			Assert.AreEqual(c2nn, _c.ByTypeCache[typeof (ITestService2)][0]); //priority seted
			Assert.AreEqual(c2n, _c.ByTypeCache[typeof (ITestService2)][1]);
			Assert.AreEqual(c2n, _c.ByNameCache["test"][0]);
			Assert.AreEqual(c1n, _c.ByNameCache["test"][1]);
			Assert.AreEqual(c1n2, _c.ByNameCache["test2"][0]);
		}
	}

    
}