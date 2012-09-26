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
// Original file : InjectionsTest.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using NUnit.Framework;

namespace Qorpent.IoC.Tests {
	[TestFixture]
	public class InjectionsTest {
		[SetUp]
		public void Setup() {
			var c = new Container();
			c.Register(new ComponentDefinition<IExt1, Ext1>(Lifestyle.Extension, "e1"));
			c.Register(new ComponentDefinition<IExt1, Ext1>(Lifestyle.Extension, "e2"));
			c.Register(new ComponentDefinition<IService1, Service1>(Lifestyle.Transient));
			c.Register(new ComponentDefinition<IAgg, Agg>(Lifestyle.Transient).Set("prop1", "value1"));
			testobj = c.Get<IAgg>();
		}


		private IAgg testobj;

		public interface IExt1 {}

		public class Ext1 : IExt1 {}

		public interface IService1 {}

		public class Service1 : IService1 {}

		public interface IAgg {
			string Prop1 { get; set; }
			[Inject] IService1 Service1 { get; set; }
			[Inject] IExt1[] ExtsArray { get; set; }
			[Inject] IList<IExt1> ExtsNullList { get; set; }
			[Inject] List<IExt1> ExtsNullList2 { get; set; }
			[Inject] IList<IExt1> ExtsNotNullList { get; set; }
		}

		public class Agg : IAgg {
			public Agg() {
				ExtsNotNullList = new List<IExt1>();
				ExtsNotNullList.Add(new Ext1());
			}


			public string Prop1 { get; set; }
			[Inject] public IService1 Service1 { get; set; }
			[Inject] public IExt1[] ExtsArray { get; set; }
			[Inject] public IList<IExt1> ExtsNullList { get; set; }
			[Inject] public List<IExt1> ExtsNullList2 { get; set; }
			[Inject] public IList<IExt1> ExtsNotNullList { get; set; }
		}

		[Test]
		public void ArrayIsSettedWithInjections() {
			Assert.NotNull(testobj.ExtsArray);
			Assert.AreEqual(2, testobj.ExtsArray.Length);
		}

		[Test]
		public void ListIsCreatedByClass() {
			Assert.NotNull(testobj.ExtsNullList2);
			Assert.AreEqual(2, testobj.ExtsNullList2.Count);
		}

		[Test]
		public void ListIsCreatedByInterface() {
			Assert.NotNull(testobj.ExtsNullList);
			Assert.AreEqual(2, testobj.ExtsNullList.Count);
		}

		[Test]
		public void ListIsFilled() {
			Assert.AreEqual(3, testobj.ExtsNotNullList.Count);
		}

		[Test]
		public void PropertyIsSettedWithComponentParameter() {
			Assert.AreEqual("value1", testobj.Prop1);
		}
	}
}