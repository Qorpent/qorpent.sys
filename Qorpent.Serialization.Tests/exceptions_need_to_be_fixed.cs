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
// Original file : exceptions_need_to_be_fixed.cs
// Project: Qorpent.Serialization.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;

namespace Qorpent.Serialization.Tests {
	[TestFixture]
	public class exceptions_need_to_be_fixed {
		[Test]
		public void bug_bxl_serializer_must_correctly_serilize_nulls() {
			Assert.AreEqual("value : null", new BxlSerializer().DoSerialize("value", null));
		}
	}
}