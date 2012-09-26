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
// Original file : ResetEventDataTest.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using NUnit.Framework;
using Qorpent.Events;

namespace Qorpent.Core.Tests.Events {
	[TestFixture]
	public class ResetEventDataTest {
		[TestCase(true, "X", "", true)]
		[TestCase(false, "X", "A,B,C", false)]
		[TestCase(false, "X", "A,B,C", false)]
		[TestCase(false, "B", "A,B,C", true)]
		[TestCase(false, "X.Z", "X.Z,B,C", true)]
		[TestCase(false, "X.Z", "X.A,B,C", false)]
		[TestCase(false, "X.Z", "X,B,C", true, Description = "by domain segment of command")]
		public void IsSet_Test(bool all, string optiontoset, string options, bool expectedResult) {
			var d = new ResetEventData {All = all};
			foreach (var option in options.Split(',')) {
				d.Set(option);
			}
			Assert.AreEqual(expectedResult, d.IsSet(optiontoset));
		}
	}
}