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
// Original file : EnvironmentInfoTest.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using NUnit.Framework;

namespace Qorpent.Core.Tests {
	[TestFixture]
	public class EnvironmentInfoTest {
		[SetUp]
		public void setup() {
			_currentdir = Environment.CurrentDirectory;
		}

		[TearDown]
		public void tearDown() {
			Environment.CurrentDirectory = _currentdir;
			EnvironmentInfo.Reset();
		}


		private string _currentdir;

		[Test]
		public void DefaultTestProcessInfoIsWell() {
			EnvironmentInfo.Reset();
			Assert.False(EnvironmentInfo.IsWeb);
			Assert.False(EnvironmentInfo.IsWebUtility);
			Assert.AreEqual(Environment.CurrentDirectory, EnvironmentInfo.RootDirectory);
		}


	}
}