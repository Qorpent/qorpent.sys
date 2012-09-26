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
// Original file : FileResolverTest.cs
// Project: Qorpent.IO.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Tests {
	[TestFixture]
	public class FileResolverTest {
		[SetUp]
		public void Setup() {
			_testpath = Path.Combine(Path.GetTempPath(), "QorpentTest\\FileResolverTest");
			Directory.CreateDirectory(_testpath);
			Directory.Delete(_testpath, true);
			Directory.CreateDirectory(_testpath);
			fr = new FileNameResolver {Root = _testpath};
		}


		private string _testpath;
		protected FileNameResolver fr;

		private string write(string name, string content = "") {
			var path = Path.Combine(_testpath, name);
			Directory.CreateDirectory(Path.GetDirectoryName(path));
			File.WriteAllText(path, content, Encoding.UTF8);
			return path;
		}


		[Test]
		public void CanResolveFileInRoot() {
			var f1 = write("f1");
			Assert.AreEqual(f1.NormalizePath(), fr.Resolve("~/f1"));
			Assert.AreEqual(f1.NormalizePath(), fr.Resolve("f1"));
		}

		[Test]
		public void NullForNonExistedWithoutFlag() {
			Assert.Null(fr.Resolve("f1"));
		}

		[Test]
		public void PassByRootedPath() {
			var f1 = write("f1");
			Assert.AreEqual(f1.NormalizePath(), fr.Resolve(f1));
		}

		[Test]
		public void ResolveAllWithWildCard() {
			var f1 = write("usr/a.txt");
			var f2 = write("b.txt");
			var f3 = write("b.zip");
			var files = fr.ResolveAll(new FileSearchQuery
				{
					ExistedOnly = true,
					PathType = FileSearchResultType.FullPath,
					ProbeFiles = new[] {"*.txt", "*.zip"},
					ProbePaths = new[] {"~/", "~/usr"}
				}).ToList();
			Assert.AreEqual(3, files.Count);
			Assert.True(files.Contains(f1.NormalizePath()));
			Assert.True(files.Contains(f2.NormalizePath()));
			Assert.True(files.Contains(f3.NormalizePath()));

			Assert.AreEqual(f2.NormalizePath(), files[0]);
			Assert.AreEqual(f1.NormalizePath(), files[1]);
			Assert.AreEqual(f3.NormalizePath(), files[2]);
		}

		[Test]
		public void ShrotestAssertedPathForNonExisted() {
			Assert.AreEqual(_testpath.NormalizePath() + "/usr/f1", fr.Resolve("f1", false));
			Assert.AreEqual(_testpath.NormalizePath() + "/f1", fr.Resolve("~/f1", false));
		}
	}
}