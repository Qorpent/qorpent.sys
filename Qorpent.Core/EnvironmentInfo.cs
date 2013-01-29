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
// Original file : EnvironmentInfo.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Diagnostics;
using System.IO;
using System.Web;
using Qorpent.Applications;

namespace Qorpent {
	/// <summary>
	/// 	Contains adapted environment information
	/// </summary>
	public static class EnvironmentInfo {
		private static bool? _isWeb;


		private static bool? _isWebUtility;

		private static string _rootDirectory;

		private static string _binDirectory;

		/// <summary>
		/// 	Type-wide static lock object
		/// </summary>
		public static object Sync = new object();

		/// <summary>
		/// 	True if we are under web context
		/// </summary>
		public static bool IsWeb {
			get {
				if (!_isWeb.HasValue) {
					lock (Sync) {
						var fileName = Path.GetFileName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
						_isWeb = fileName != null && fileName.ToLowerInvariant() ==
						         "web.config";
					}
				}
				return _isWeb.Value;
			}
		}

		/// <summary>
		/// 	True if it's console exe, runed from WEBAPP/bin folder with WEBAPP/bin as CurrentDirectory folder
		/// 	so it must have same root as web app itself
		/// </summary>
		public static bool IsWebUtility {
			get {
				if (!_isWebUtility.HasValue) {
					lock (Sync) {
						_isWebUtility = false;
						if (!IsWeb) {
							var dir1 = Environment.CurrentDirectory.Replace("\\", "/").ToLowerInvariant();
							var codebase = AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/").ToLowerInvariant();
#if !SQL2008
							if (!String.IsNullOrWhiteSpace(codebase)) {
#else
							if (!string.IsNullOrEmpty(codebase))
							{
#endif
								var basepath = Path.GetDirectoryName(codebase);
								Debug.Assert(!String.IsNullOrWhiteSpace(basepath), "basepath empty");
								var webconfig = Path.Combine(basepath, "web.config");
								_isWebUtility = (dir1 == codebase) && File.Exists(webconfig);
							}
						}
					}
				}
				return _isWebUtility.Value;
			}
		}

		/// <summary>
		/// 	Filename resolution base directory
		/// </summary>
		public static string RootDirectory {
			get {
				if (null == _rootDirectory) {
					lock (Sync) {
						if (IsWeb) {
							try {
#if !SQL2008
								_rootDirectory = GetHttpWrapper().GetAppDomainAppPath();
#else
								_rootDirectory = Environment.CurrentDirectory;
#endif
							}
							catch (ArgumentNullException) {
								//occured in pseudo web context (test and so on)
								_rootDirectory = Path.GetDirectoryName(Environment.CurrentDirectory);
							}
						}
						else if (IsWebUtility) {
							_rootDirectory = Path.GetDirectoryName(Environment.CurrentDirectory); //move up
						}
						else {
							_rootDirectory = Environment.CurrentDirectory;
						}
					}
				}
				return _rootDirectory;
			}
		}

		/// <summary>
		/// 	Folder where dlls placed
		/// </summary>
		public static string BinDirectory {
			get {
				if (null == _binDirectory) {
					lock (Sync) {
						if (IsWeb || IsWebUtility) {
							_binDirectory = Path.Combine(RootDirectory, "bin");
						}
						else {
							_binDirectory = AppDomain.CurrentDomain.BaseDirectory;
						}
					}
				}
				return _binDirectory;
			}
		}

		/// <summary>
		/// 	Reset environment info due to some changes in environment
		/// </summary>
		public static void Reset() {
			_binDirectory = null;
			_isWeb = null;
			_isWebUtility = null;
			_rootDirectory = null;
		}

		/// <summary>
		/// Helper method to access httpcontext wrapper without native System.Web access
		/// must be used by system core services
		/// </summary>
		/// <returns></returns>
		public static IHttpContextWrapper GetHttpWrapper() {
			IHttpContextWrapper wrapper = null;
			var mvctype = Type.GetType("Qorpent.Mvc.HttpContextWrapper, Qorpent.Mvc", false);
			if (null != mvctype) {
				wrapper = (IHttpContextWrapper) Activator.CreateInstance(mvctype);
			}
			else {
				wrapper = new StubHttpContextWrapper();
			}
			return wrapper;
		}
	}
}