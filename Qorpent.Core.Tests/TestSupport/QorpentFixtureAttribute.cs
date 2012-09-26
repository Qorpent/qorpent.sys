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
// Original file : QorpentFixtureAttribute.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Utils.TestSupport {
	/// <summary>
	/// 	Fixture settings to Prepare AdaptableQorpentFixture
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class QorpentFixtureAttribute : Attribute {
		/// <summary>
		/// 	Forces fixture to create/drop directory and 
		/// 	send test files into it in SetUp/TearDown
		/// 	FileResolver will be setted in fixture
		/// </summary>
		public bool UseTemporalFileSystem { get; set; }

		/// <summary>
		/// 	map string in form resourcename->path,resourcename2->path2 where
		/// 	resourcename - name of resource without namespace, path - relative path in
		/// 	temporal file system (UseTemporalFileSystem must be setted on
		/// </summary>
		public string PrepareFileSystemMap { get; set; }
	}
}