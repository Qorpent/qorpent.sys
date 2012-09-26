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
// Original file : ContainerInterfaceTest.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Qorpent.IoC.Tests {
	[TestFixture]
	public class ContainerInterfaceTest {
		[SetUp]
		public void Setup() {
			_c = new Container();
		}


		private Container _c;

		[TestCase(Lifestyle.Transient, false, false)]
		[TestCase(Lifestyle.Extension, false, false)]
		[TestCase(Lifestyle.Default, false, false)]
		[TestCase(Lifestyle.Singleton, false, true)]
		[TestCase(Lifestyle.PerThread, true, true)]
		[TestCase(Lifestyle.Pooled, false, true)]
		public void Release_And_Clean_On_Container_Bound_Behavior(Lifestyle lifestyle, bool release, bool clean) {
			var component = new ComponentDefinition<TestService1, TestService1>(lifestyle);
			_c.Register(component);
			var t1 = _c.Get<TestService1>();
			Assert.NotNull(t1.Container);
			Assert.NotNull(t1.Component);

			_c.Release(t1);
			Assert.AreEqual(release, t1.Released);

			t1 = _c.Get<TestService1>();
			_c.CleanUp();
			Assert.AreEqual(clean, t1.Released);
		}


		[Test]
		public void Can_Register_And_Return_Components() {
			_c.Register(new ComponentDefinition<ITestService1, TestService1>());
			_c.Register(new ComponentDefinition<ITestService2, TestService2>());
			Assert.AreEqual(2, _c.GetComponents().Count());
			Assert.AreEqual(0, _c.GetComponents().ToList()[0].ContainerId);
			Assert.AreEqual(1, _c.GetComponents().ToList()[1].ContainerId);
		}

		[Test]
		public void Extensions_Behavior() {
			var o1 = new TestService1();
			var o2 = new TestService1();
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.Extension));
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.Extension, implementation: o1));
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.Extension, implementation: o2));
			var ext1 = _c.All<ITestService1>().ToList();
			var ext2 = _c.All<ITestService1>().ToList();
			Assert.AreSame(ext1[1], o1);
			Assert.AreSame(ext1[2], o2);
			Assert.AreSame(ext2[1], o1);
			Assert.AreSame(ext2[2], o2);
			Assert.AreNotSame(ext1[0], ext2[0]);
		}

		[Test]
		public void Pooled_Behavior() {
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.Pooled));
			var o1 = _c.Get<ITestService1>();
			var o2 = _c.Get<ITestService1>();
			Assert.AreNotSame(o1, o2);
			_c.Release(o1);
			var o3 = _c.Get<ITestService1>();
			Assert.AreSame(o1, o3);
			ITestService1 s1 = null;
			using (var w1 = new ContainerObjectWrapper<ITestService1>(_c)) {
				s1 = w1.Object;
			}
			using (var w1 = new ContainerObjectWrapper<ITestService1>(_c)) {
				Assert.AreSame(s1, w1.Object);
			}
		}

		[Test]
		public void Singleton_Behavior() {
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.Singleton));
			var o1 = _c.Get<ITestService1>();
			var o2 = _c.Get<ITestService1>();
			Assert.NotNull(o1);
			Assert.AreSame(o1, o2);
		}

		[Test]
		public void Singleton_Implementation_Behavior() {
			var o0 = new TestService1();
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.Singleton, implementation: o0));
			var o1 = _c.Get<ITestService1>();
			var o2 = _c.Get<ITestService1>();
			Assert.NotNull(o1);
			Assert.AreSame(o1, o2);
			Assert.AreSame(o1, o0);
			_c.Release(o1);
			o2 = _c.Get<ITestService1>();
			Assert.AreSame(o2, o1);
		}

		[Test]
		public void Thread_Behavior() {
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.PerThread));
			ITestService1 t1 = null;
			ITestService1 t2 = null;
			ITestService1 t3 = null;
			ITestService1 t4 = null;

			var th1 = new Thread(
				() =>
					{
						Thread.Sleep(10);
						t1 = _c.Get<ITestService1>();
						t2 = _c.Get<ITestService1>();
					}
				);
			var th2 = new Thread(
				() =>
					{
						Thread.Sleep(10);
						t3 = _c.Get<ITestService1>();
						t4 = _c.Get<ITestService1>();
					}
				);

			th1.Start();
			th2.Start();
			th1.Join();
			th2.Join();
			Assert.NotNull(t1);
			Assert.NotNull(t3);
			Assert.AreSame(t1, t2);
			Assert.AreSame(t3, t4);
			Assert.AreNotSame(t1, t3);
		}

		[Test]
		public void Transient_Behavior() {
			_c.Register(new ComponentDefinition<ITestService1, TestService1>(Lifestyle.Transient));
			var o1 = _c.Get<ITestService1>();
			var o2 = _c.Get<ITestService1>();
			Assert.NotNull(o1);
			Assert.AreNotSame(o1, o2);
		}
	}
}