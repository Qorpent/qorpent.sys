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
// Original file : QViewCompiler.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Qorpent.Bxl;
using Qorpent.Dsl;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Appliation-wide Full/Diff VBXL compiler
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton)]
	public class QViewCompiler : ServiceBase, IQViewCompiler {
		/// <summary>
		/// </summary>
		public QViewCompiler() {
			DllPath = "~/.qviewbin";
			OutCodePath = "~/tmp/.qviewcode";
		}

#if PARANOID
		static QViewCompiler() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// </summary>
		[Inject] public IFileNameResolver FileNameResolver { get; set; }


		/// <summary>
		/// </summary>
		protected string FullDllPath {
			get { return FileNameResolver.Resolve(DllPath, false); }
		}

		/// <summary>
		/// </summary>
		protected string FullOutCodePath {
			get { return FileNameResolver.Resolve(OutCodePath, false); }
		}

		/// <summary>
		/// </summary>
		protected string DslDir {
			get { return FileNameResolver.Resolve("~/dsl/qview", false); }
		}


		/// <summary>
		/// </summary>
		public string DllPath { get; set; }

		/// <summary>
		/// </summary>
		public string OutCodePath { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="compileType"> </param>
		/// <param name="directSource"> </param>
		/// <returns> Full path to generated assembly </returns>
		public string Compile(QViewCompileType compileType, string directSource = null) {
			lock (this) {
				var proj = CreateProject(compileType, directSource);
				if (proj.FileSources.Count == 0 && proj.DirectSources.Count == 0) {
					return "";
				}
				var provider = ResolveService<IDslProviderService>().GetProvider(proj);
				if (Directory.Exists(proj.NativeCodeDirectory)) {
					Directory.Delete(proj.NativeCodeDirectory, true);
				}
				Directory.CreateDirectory(proj.NativeCodeDirectory);
				var result = provider.Run(proj);
				if (result.MaxLevel >= ErrorLevel.Error) {
					throw new Exception("errors occured during compilation " + result.Messages.ConcatString("\r\n"));
				}
				return result.GetProductionFileName();
			}
		}


		/// <summary>
		/// 	Создает проект для компиляции QView
		/// </summary>
		/// <param name="compileType"> </param>
		/// <param name="directSource"> </param>
		/// <returns> </returns>
		public DslProject CreateProject(QViewCompileType compileType, string directSource) {
			EnsureFileSystem();
			var suffix = "all";
			if (compileType == QViewCompileType.Diff && 0 != Directory.GetFiles(FullDllPath, "*.all.dll").Length) {
				suffix = "diff";
			}
			else {
				compileType = QViewCompileType.Full;
			}
			var proj = new DslProject
				{
					LangName = "qview",
					OutputDirectory = FullDllPath,
					NativeCodeDirectory = FullOutCodePath,
					RootDirectory = FileNameResolver.Root,
					ResultName = DateTime.Now.ToString("yyyyMMddhhmmss") + "." + suffix,
					BxlOptions = BxlParserOptions.SafeAttributeNames
				};
			proj.Extensions["c~http://qorpent/qview"] = new QViewXsltExtension();
			if (directSource.IsEmpty()) {
				var files = Directory.GetFiles(FileNameResolver.Root, "*.vbxl", SearchOption.AllDirectories);
				var lastdate = GetStartDate(compileType);
				foreach (var file in files) {
					var dir = Path.GetDirectoryName(file);
					var search = Path.GetFileNameWithoutExtension(file) + ".*";
					var allmatchedfiled = Directory.GetFiles(dir, search);
					if (compileType == QViewCompileType.Full || allmatchedfiled.Any(x => File.GetLastWriteTime(x) > lastdate)) {
						proj.FileSources.Add(file);
					}
				}
			}
			else {
				proj.DirectSources["directSource"] = directSource;
			}
			return proj;
		}

		private DateTime GetStartDate(QViewCompileType compileType) {
			if (compileType == QViewCompileType.Full) {
				return DateTime.MinValue;
			}
			var lastall =
				Directory.GetFiles(FullDllPath, "*.all.dll").OrderByDescending(x => File.GetLastWriteTime(x)).FirstOrDefault();
			if (null == lastall) {
				return DateTime.MinValue;
			}
			var lastalltime = File.GetLastWriteTime(lastall);
			var lastdif =
				Directory.GetFiles(FullDllPath, "*.diff.dll").OrderByDescending(x => File.GetLastWriteTime(x)).FirstOrDefault();
			if (null == lastdif) {
				return lastalltime;
			}
			var lastdiftime = File.GetLastWriteTime(lastdif);
			if (lastdiftime > lastalltime) {
				return lastdiftime;
			}
			return lastalltime;
		}

		private void EnsureFileSystem() {
			Directory.CreateDirectory(FullDllPath);
			Directory.CreateDirectory(FullOutCodePath);
			Directory.CreateDirectory(DslDir);
			//if (!File.Exists(Path.Combine(DslDir, "default.xslt"))) {
			var resource =
				Assembly.GetExecutingAssembly().GetManifestResourceNames().FirstOrDefault(x => x.Contains("vbxlcompiler"));
			using (var se = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))) {
				File.WriteAllText(Path.Combine(DslDir, "default.xslt"), se.ReadToEnd());
			}
			//}
		}
	}
}