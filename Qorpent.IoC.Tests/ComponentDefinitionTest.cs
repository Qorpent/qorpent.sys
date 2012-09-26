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
// Original file : ComponentDefinitionTest.cs
// Project: Qorpent.IoC.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;

namespace Qorpent.IoC.Tests {
	[TestFixture]
	public class ComponentDefinitionTest {
		[TestCase(Lifestyle.Default, false)]
		[TestCase(Lifestyle.Extension, true)]
		[TestCase(Lifestyle.Transient, false)]
		[TestCase(Lifestyle.Singleton, true)]
		[TestCase(Lifestyle.Pooled, false)]
		[TestCase(Lifestyle.PerThread, false)]
		public void Implementation_object_Keeping(Lifestyle lifestyle, bool expectedResult) {
			var c = new ComponentDefinition<object, object>(lifestyle, implementation: new object());
			if (expectedResult) {
				Assert.NotNull(c.Implementation);
			}
			else {
				Assert.Null(c.Implementation);
			}
		}

		private interface Itest1 {}

		private interface Itest2 {}

		public class test1 : Itest1 {}

		public class test2 : Itest2 {}


		[TestCase(typeof (Itest1), typeof (test1), true, true, true)]
		[TestCase(typeof (Itest2), typeof (test2), true, true, true)]
		[TestCase(typeof (test1), typeof (test1), true, true, true)]
		[TestCase(typeof (test2), typeof (test2), true, true, true)]
		[TestCase(typeof (Itest1), typeof (test2), false, true, false)]
		[TestCase(typeof (Itest2), typeof (test1), false, true, false)]
		[TestCase(typeof (Itest1), typeof (Itest1), false, true, false)]
		[TestCase(typeof (Itest2), typeof (Itest2), false, true, false)]
		[TestCase(typeof (Itest1), typeof (test1), true, false, false)]
		[TestCase(typeof (Itest2), typeof (test2), true, false, false)]
		[TestCase(null, typeof (test1), false, true, false)]
		[TestCase(typeof (Itest2), null, false, true, false)]
		public void NonGeneric_Constructor_Validation(Type tService, Type tImplementation, bool makeimpl, bool usematchimpl,
		                                              bool willcreated) {
			object impl = null;
			if (makeimpl) {
				if (usematchimpl) {
					if (tImplementation == typeof (test1)) {
						impl = new test1();
					}
					else {
						impl = new test2();
					}
				}
				else {
					if (tImplementation == typeof (test2)) {
						impl = new test1();
					}
					else {
						impl = new test2();
					}
				}
			}
			if (willcreated) {
				new ComponentDefinition(tService, tImplementation, implementation: impl);
			}
			else {
				try {
					Assert.Throws<ArgumentException>(() => new ComponentDefinition(tService, tImplementation, implementation: impl));
				}
				catch {
					Assert.Throws<ArgumentNullException>(() => new ComponentDefinition(tService, tImplementation, implementation: impl));
				}
			}
		}
	}
}