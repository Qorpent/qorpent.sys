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
// Original file : QorpentFixture.cs
// Project: Qorpent.Core.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Qorpent.Applications;
using Qorpent.Bxl;
using Qorpent.IO;
using Qorpent.IoC;

namespace Qorpent.Utils.TestSupport {
	/// <summary>
	/// 	Base fixture with some lyfecycle options, provided by QorpentFixtureAttribute
	/// </summary>
	public abstract class QorpentFixture {
		protected IFileNameResolver FileResover { get; set; }
		protected IBxlService Bxl { get; set; }

		[SetUp]
		public virtual void SetUp() {
			Qorpentfixture = GetType()
				.GetCustomAttributes(typeof (QorpentFixtureAttribute), true)
				.OfType<QorpentFixtureAttribute>()
				.FirstOrDefault();
			Bxl = Application.Current.Bxl;
			if (null != Qorpentfixture) {
				if (Qorpentfixture.UseTemporalFileSystem) {
					setupTemporalFileSystem();
				}
			}
		}

		protected void setupTemporalFileSystem() {
			Tmpdir = Path.GetTempFileName();
			File.Delete(Tmpdir);
			Directory.CreateDirectory(Tmpdir);
			FileResover = ContainerFactory.ResolveWellKnown<IFileNameResolver>();
			if (null != FileResover) {
				FileResover.Root = Tmpdir;
			}
			if (!string.IsNullOrEmpty(Qorpentfixture.PrepareFileSystemMap)) {
				var resnames = GetType().Assembly.GetManifestResourceNames();
				var rules =
					Qorpentfixture.PrepareFileSystemMap.Split(',').Select(
						x => new {name = x.Split('~')[0].Trim(), path = x.Split('~')[1].Trim()});
				foreach (var rule in rules) {
					var path = Path.Combine(Tmpdir, rule.path);
					Directory.CreateDirectory(Path.GetDirectoryName(path));
					var resname = resnames.First(x => x.EndsWith(rule.name));
					using (var fs = new FileStream(path, FileMode.CreateNew)) {
						if (resname.StartsWith("@")) {
//direct content				
							using (var sw = new StreamWriter(fs, Encoding.UTF8)) {
								sw.Write(rule.name.Substring(1));
							}
						}
						else {
							using (var sr = GetType().Assembly.GetManifestResourceStream(resname)) {
								sr.CopyTo(fs);
								fs.Flush();
							}
						}
					}
				}
			}
		}

		[TearDown]
		public virtual void TearDown() {
			if (null != Qorpentfixture) {
				if (Qorpentfixture.UseTemporalFileSystem) {
					dropTemporalFileSystem();
				}
			}
		}

		protected void dropTemporalFileSystem() {
			try {
				if (Directory.Exists(Tmpdir)) {
					Directory.Delete(Tmpdir, true);
				}
			}
			catch {
				Thread.Sleep(200);
				try {
					if (Directory.Exists(Tmpdir)) {
						Directory.Delete(Tmpdir, true);
					}
				}
				catch (Exception e) {
					Console.WriteLine(e);
				}
			}
		}

		protected QorpentFixtureAttribute Qorpentfixture;
		protected string Tmpdir;
	}
}