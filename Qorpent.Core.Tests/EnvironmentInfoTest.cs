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

		[Test]
		public void IsWeb() {
			var tempdir = Path.GetTempFileName();
			File.Delete(tempdir);
			Directory.CreateDirectory(tempdir);
			var assembly = Assembly.GetAssembly(typeof (EnvironmentInfo));
			try {
				var binpath = Path.Combine(tempdir, "bin");
				var webconfig = Path.Combine(tempdir, "web.config");
				var dllpath = Path.GetFileName(assembly.CodeBase);
				dllpath = Path.Combine(binpath, dllpath);
				Directory.CreateDirectory(binpath);
				File.Copy(assembly.CodeBase.Replace(EnvironmentInfo.FULL_FILE_NAME_START, ""), dllpath);
				File.WriteAllText(webconfig, "<web/>");
				var setup = new AppDomainSetup();
				setup.ApplicationBase = binpath;
				setup.ShadowCopyFiles = "true";
				setup.ShadowCopyDirectories = "true";
				setup.ConfigurationFile = webconfig;
				var domain = AppDomain.CreateDomain("test", new Evidence(), setup);
				domain.Load("Qorpent.Core");
				var infotester =
					(EnvironmentInfoTestObject) domain.CreateInstanceAndUnwrap("Qorpent.Core", "Qorpent.EnvironmentInfoTestObject");
				infotester.SetCurrentDirectory(binpath);
				var isweb = infotester.IsWeb;
				var iswebutility = infotester.IsWebUtility;
				var root = infotester.RootDirectory;
				//bool isweb = (bool)infotester.GetType().GetProperty("IsWeb").GetValue(infotester,null);
				//bool iswebutility = (bool)infotester.GetType().GetProperty("IsWebUtility").GetValue(infotester, null);
				//string root = (string)infotester.GetType().GetProperty("RootDirectory").GetValue(infotester, null);
				AppDomain.Unload(domain);
				Assert.True(isweb);
				Assert.False(iswebutility);
				Assert.AreEqual(tempdir, root);
			}
			finally {
				try {
					Directory.Delete(tempdir, true);
				}
				catch {}
			}
		}

		[Test]
		public void IsWebUtility() {
			var tempdir = Path.GetTempFileName();
			File.Delete(tempdir);
			Directory.CreateDirectory(tempdir);
			var assembly = Assembly.GetAssembly(typeof (EnvironmentInfo));

			try {
				var binpath = Path.Combine(tempdir, "bin");
				var webconfig = Path.Combine(tempdir, "web.config");
				var dllpath = Path.GetFileName(assembly.CodeBase);
				dllpath = Path.Combine(binpath, dllpath);
				Directory.CreateDirectory(binpath);
				throw new Exception(Environment.OSVersion.Platform + " " + assembly.CodeBase + " " + EnvironmentInfo.FULL_FILE_NAME_START + " " + dllpath + " " + assembly.CodeBase.Replace(EnvironmentInfo.FULL_FILE_NAME_START, ""));
				File.Copy(assembly.CodeBase.Replace(EnvironmentInfo.FULL_FILE_NAME_START, ""), dllpath);
				File.WriteAllText(webconfig, "<web/>");
				var setup = new AppDomainSetup();
				setup.ApplicationBase = binpath;
				setup.ShadowCopyFiles = "true";
				setup.ShadowCopyDirectories = "true";
				var domain = AppDomain.CreateDomain("test", new Evidence(), setup);
				domain.Load("Qorpent.Core");
				var infotester =
					(EnvironmentInfoTestObject) domain.CreateInstanceAndUnwrap("Qorpent.Core", "Qorpent.EnvironmentInfoTestObject");
				infotester.SetCurrentDirectory(binpath);
				var isweb = infotester.IsWeb;
				var iswebutility = infotester.IsWebUtility;
				var root = infotester.RootDirectory;
				//bool isweb = (bool)infotester.GetType().GetProperty("IsWeb").GetValue(infotester,null);
				//bool iswebutility = (bool)infotester.GetType().GetProperty("IsWebUtility").GetValue(infotester, null);
				//string root = (string)infotester.GetType().GetProperty("RootDirectory").GetValue(infotester, null);
				AppDomain.Unload(domain);
				Assert.False(isweb);
				Assert.True(iswebutility);
				Assert.AreEqual(tempdir, root);
			}
			finally {
				try {
					Directory.Delete(tempdir, true);
				}
				catch {}
			}
		}
	}
}