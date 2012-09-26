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
// Original file : EnumerableExtensionsTest.cs
// Project: Qorpent.Utils.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Tests {
	[TestFixture]
	public class EnumerableExtensionsTest {
		[Test]
		public void EmptyArrayIsEmpty() {
			Assert.True(new string[] {}.IsEmpty());
			Assert.False(new string[] {}.IsNotEmpty());
		}

		[Test]
		public void NotNullOnlyArrayIsNotEmpty() {
			Assert.False(new[] {null, "", null}.IsEmpty());
			Assert.True(new[] {null, "", null}.IsNotEmpty());
		}

		[Test]
		public void NullArrayIsEmpty() {
			Assert.True(((Array) null).IsEmpty());
			Assert.False(((Array) null).IsNotEmpty());
		}

		[Test]
		public void NullOnlyArrayIsEmpty() {
			Assert.True(new string[] {null, null}.IsEmpty());
			Assert.False(new string[] {null, null}.IsNotEmpty());
		}
	}
}