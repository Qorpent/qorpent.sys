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
// Original file : FileServiceTest.cs
// Project: Qorpent.IO.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using NUnit.Framework;

namespace Qorpent.IO.Tests {
	[TestFixture]
	public class FileServiceTest {
		[Test]
		public void CanCreateAsStandaloneAndCanCreateFileResolver() {
			var fs = new FileService();
			Assert.NotNull(fs.GetResolver());
		}

		[Test]
		public void CanCreateAsStandaloneAndHasRoot() {
			var fs = new FileService();
			Assert.AreEqual(Environment.CurrentDirectory, fs.Root);
		}

		[Test]
		public void FileResolversAreBasedOnSameRootAsCoreService() {
			var fs = new FileService();
			fs.Root = Path.GetTempFileName();
			Assert.AreEqual(fs.Root, fs.GetResolver().Root);
		}

		[Test]
		public void FileResolversAreUnique() {
			var fs = new FileService();
			var r1 = fs.GetResolver();
			var r2 = fs.GetResolver();
			Assert.AreNotSame(r1, r2);
		}
	}
}