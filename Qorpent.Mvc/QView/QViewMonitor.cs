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
// Original file : QViewMonitor.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using Qorpent.IO;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.QView {
	/// <summary>
	/// 	Default library-based QViewMonitor
	/// </summary>
	[ContainerComponent(Lifestyle.Transient)]
	public class QViewMonitor : ServiceBase, IQViewMonitor {
		/// <summary>
		/// </summary>
		public QViewMonitor() {
			Path = "~/.qviewbin";
			MainExtension = "all.dll";
			DiffExtension = "diff.dll";
		}

		/// <summary>
		/// </summary>
		[Inject] public IFileNameResolver Resolver { get; set; }

		/// <summary>
		/// 	Path to folder where qview dll-s will be collected
		/// </summary>
		public string Path { get; set; }

		/// <summary>
		/// 	Mask of main build (full QView compilation) library (to be loaded at startup)
		/// </summary>
		public string MainExtension { get; set; }

		/// <summary>
		/// 	Mask of differential build libraries (to be loaded at startup/dynamically)
		/// </summary>
		public string DiffExtension { get; set; }

		/// <summary>
		/// 	Indicates that main DLL exists
		/// </summary>
		public bool MainExists { get; set; }

		/// <summary>
		/// 	Last-write time of main DLL
		/// </summary>
		public DateTime MainFileVersion { get; set; }

		/// <summary>
		/// </summary>
		public DateTime LastDiffFileLoadedVersion { get; set; }

		/// <summary>
		/// 	Evaluated path to DLL dir
		/// </summary>
		public string TargetDirectory {
			get {
				if (null == _targetDirectory) {
					_targetDirectory = Resolver.Resolve(Path, false);
				}
				return _targetDirectory;
			}
		}

		/// <summary>
		/// 	If true - it means that some changes occured
		/// </summary>
		public bool Invalidated { get; set; }

		/// <summary>
		/// 	Flag for monitoring activity - if not active - it will be just 
		/// 	set Invalidated = true, no reload activity
		/// </summary>
		public bool Active { get; set; }

		/// <summary>
		/// 	Back reference to factory to be refreshed
		/// </summary>
		public IMvcFactory Factory { get; set; }


		/// <summary>
		/// 	Initial QView loading - synchronous
		/// </summary>
		public void Startup() {
			EnsureFileStructure(); //firstly recreate folder to be monitored
			Syncronize();
			SetupDirectoryListeners(); //start DLL folder monitoring
		}

		/// <summary>
		/// 	Asynchronous update QView monitoring and factory cleanup - 
		/// 	if qview updates occured - load them and cleanup factory as needed
		/// </summary>
		public void StartMonitoring() {
			Active = true;
			if (Invalidated) {
				Syncronize();
			}
		}

		/// <summary>
		/// 	Pause minitoring process
		/// </summary>
		public void EndMonitoring() {
			lock (this) {
				Active = false;
			}
		}

		/// <summary>
		/// 	Set factory to be cleaned
		/// </summary>
		/// <param name="factory"> </param>
		public void SetFactory(IMvcFactory factory) {
			Factory = factory;
		}


		private void SetupDirectoryListeners() {
			_watcher = new FileSystemWatcher(TargetDirectory);
			_watcher.Created += OnNewFile;
			_watcher.Changed += OnNewFile;
			_watcher.EnableRaisingEvents = true;
		}

		private void OnNewFile(object sender, FileSystemEventArgs e) {
			if (e.FullPath.EndsWith(MainExtension) || e.FullPath.EndsWith(DiffExtension)) {
				Invalidated = true;
				if (Active) {
					Syncronize();
				}
			}
		}

		private void EnsureFileStructure() {
			Directory.CreateDirectory(TargetDirectory);
		}

		/// <summary>
		/// 	Main sync logic -
		/// </summary>
		protected void Syncronize() {
			lock (this) {
				var oldactive = Active;
				Active = false;
				LoadMainFile(); //then load full compilation
				LoadDiffFiles(); //then load advanced libraries	
				Invalidated = false;
				Active = oldactive;
			}
		}

		private void LoadDiffFiles() {
			if (MainExists) {
				var findmask = "*." + DiffExtension;
				var diffiles = from f in Directory.GetFiles(TargetDirectory, findmask)
				               let v = File.GetLastWriteTime(f)
				               where v > LastDiffFileLoadedVersion
				               orderby v
				               select new {f, v};
				foreach (var diffile in diffiles) {
					loadLibraryFromFile(diffile.f);
					LastDiffFileLoadedVersion = diffile.v;
				}
			}
		}

		private void LoadMainFile() {
			MainExists = false;
			var findmask = "*." + MainExtension;
			var lastmain =
				Directory.GetFiles(TargetDirectory, findmask, SearchOption.TopDirectoryOnly).OrderByDescending(
					x => File.GetLastWriteTime(x)).FirstOrDefault();
			if (null != lastmain) {
				var version = File.GetLastWriteTime(lastmain);
				if (version > MainFileVersion) {
					loadLibraryFromFile(lastmain);
					MainFileVersion = version;
					LastDiffFileLoadedVersion = version;
				}
				MainExists = true;
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="libfile"> </param>
		private void loadLibraryFromFile(string libfile) {
			Thread.Sleep(100);
			var libdata = File.ReadAllBytes(libfile);
			byte[] symbols = null;
			var pdbfile = FileNameResolverExtensions.GetSymbolFileName(libfile);
			if (File.Exists(pdbfile)) {
				symbols = File.ReadAllBytes(pdbfile);
			}
			var assembly = Assembly.Load(libdata, symbols, SecurityContextSource.CurrentAppDomain);
			loadLibrary(assembly);
		}

		/// <summary>
		/// 	Main method which loads classes into container and call factory refreshing
		/// </summary>
		/// <param name="assembly"> </param>
		private void loadLibrary(Assembly assembly) {
			Factory.Register(assembly);
		}

		private string _targetDirectory;
		private FileSystemWatcher _watcher;
	}
}