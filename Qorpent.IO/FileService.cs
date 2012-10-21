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
// Original file : FileService.cs
// Project: Qorpent.IO
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Qorpent.Applications;
using Qorpent.IoC;

namespace Qorpent.IO {
	/// <summary>
	/// 	Default implementation of Application-wide File service
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, ServiceType = typeof (IFileService))]
	public class FileService : IFileService, IApplicationBound {
		/// <summary>
		/// 	Default Fileresolver instance
		/// </summary>
		protected IFileNameResolver Fileresolver {
			get { return _fileresolver ?? (_fileresolver = GetResolver()); }
		}


		/// <summary>
		/// 	Call service defined Application bound
		/// </summary>
		/// <param name="app"> </param>
		public void SetApplication(IApplication app) {
			_application = app;
		}


		/// <summary>
		/// 	Resolves first file that match query
		/// </summary>
		/// <param name="query"> search query </param>
		/// <returns> </returns>
		public string Resolve(FileSearchQuery query) {
			return Fileresolver.Resolve(query);
		}

		/// <summary>
		/// 	Resolves all files match query
		/// </summary>
		/// <param name="query"> search query </param>
		/// <returns> </returns>
		public string[] ResolveAll(FileSearchQuery query) {
			return Fileresolver.ResolveAll(query);
		}

		/// <summary>
		/// 	Own root of fileresolver
		/// </summary>
		public string Root {
			get {
				if (null != _root) {
					return _root;
				}
				if (null != Application) {
					return Application.RootDirectory;
				}
				return Environment.CurrentDirectory;
			}
			set { _root = value; }
		}

		/// <summary>
		/// 	Clears file resolution cache
		/// </summary>
		public void ClearCache() {
			Fileresolver.ClearCache();
		}

		/// <summary>
		/// 	reference to containing applicatiom
		/// </summary>
		public IApplication Application {
			get { return _application; }
		}

		/// <summary>
		/// 	Returns standalone IFileNameResolver instance
		/// </summary>
		/// <returns> </returns>
		public IFileNameResolver GetResolver() {
			return new FileNameResolver().SetParentService(this, null);
		}

		private IApplication _application;
		private IFileNameResolver _fileresolver;
		private string _root;
	}
}