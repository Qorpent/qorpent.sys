#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// PROJECT ORIGIN: Qorpent.Core/EnvironmentInfo.cs
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
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
			set { _isWeb = value; }
		}
		/// <summary>
		/// Начало полного имения файла на замену
		/// </summary>
		public static readonly string FULL_FILE_NAME_START = "file:///";

		static EnvironmentInfo() {
			if (Environment.OSVersion.Platform == PlatformID.Unix) {
				FULL_FILE_NAME_START = "file://";
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
			set { _isWebUtility = value; }
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
			set { _rootDirectory = Path.GetFullPath(value); }
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
			set { _binDirectory = Path.GetFullPath(value); }
		}
		/// <summary>
		/// Директория для конфигов
		/// </summary>
		public static string ConfigDirectory
		{
			get
			{
				if (null == _configDirectory)
				{
					lock (Sync)
					{
						if (IsWeb || IsWebUtility)
						{
							_configDirectory = Path.Combine(RootDirectory, ".config");
						}
						else
						{
							_configDirectory = AppDomain.CurrentDomain.BaseDirectory;
						}
					}
				}
				return _configDirectory;
			}
			set { _configDirectory = Path.GetFullPath( value); }
		}


		/// <summary>
		/// Директория для временных файлов
		/// </summary>
		public static string TmpDirectory
		{
			get
			{
				if (null == _tmpDirectory)
				{
					lock (Sync)
					{
						if (IsWeb || IsWebUtility)
						{
							_tmpDirectory = Path.Combine(RootDirectory, ".tmp");
						}
						else
						{
							_tmpDirectory = AppDomain.CurrentDomain.BaseDirectory;
						}
					}
				}
				return _tmpDirectory;
			}
			set { _tmpDirectory = Path.GetFullPath(value); }
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


		private static readonly IDictionary<string, string> ResolvedExeCache = new Dictionary<string, string>();
		private static string _configDirectory;
		private static string _tmpDirectory;
		private static string _logDirectory;

		/// <summary>
		/// Директория для лог
		/// </summary>
		public static string LogDirectory
		{
			get
			{
				if (null == _logDirectory)
				{
					lock (Sync)
					{
						if (IsWeb || IsWebUtility)
						{
							_logDirectory = Path.Combine(RootDirectory, ".log");
						}
						else
						{
							_logDirectory = AppDomain.CurrentDomain.BaseDirectory;
						}
					}
				}
				return _logDirectory;
			}
			set { _logDirectory = Path.GetFullPath(value); }
		}

		/// <summary>
		/// Список команд системы
		/// </summary>
		public static readonly string[] SystemProcesses = new[]{
			"mkdir", "xcopy", "del", "copy", "rm"
		};

		/// <summary>
		///Разрешает имя исполнимого файла с учетом параметров среды
		/// </summary>
		/// <param name="executableName"></param>
		/// <returns></returns>
		public static string GetExecutablePath(string executableName) {
			if (-1 != Array.IndexOf(SystemProcesses, executableName)){
				return executableName;
			}
			
			if (!executableName.EndsWith(".exe")) {
				executableName += ".exe";
			}
			
			executableName = executableName.ToLower();
			if (!ResolvedExeCache.ContainsKey(executableName)) {
				string result = null;
				if (!File.Exists(result = Path.Combine(BinDirectory, executableName)))
				{
					if (!File.Exists(result = Path.Combine(RootDirectory, executableName))) {
						result = null;
						var paths = (Environment.GetEnvironmentVariable("PATH")??"").Split(';');
						foreach (string p in paths)
						{
							string realp = Environment.ExpandEnvironmentVariables(p.Trim());
							string checkpath = Path.Combine(realp, executableName);
							if (File.Exists(checkpath)) {
								result = checkpath;
								break;
							}
						}
					}
				}
				ResolvedExeCache[executableName] = result;
			}
			return ResolvedExeCache[executableName];
		}

		/// <summary>
		/// Формирует дочерний консольный процесс с редиректом входного и выходного потока
		/// и с переназначением кодировки стандартного потока вывода
		/// </summary>
		/// <param name="executableName"></param>
		/// <param name="argumentstring"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static Process GetConsoleProcess(string executableName, string argumentstring=null, Encoding encoding = null) {
			var exePath = GetExecutablePath(executableName);
			if (null == exePath) {
				throw new Exception("cannot find executable " + executableName);
			}
			var processStart = new ProcessStartInfo(exePath, argumentstring??"")
			{
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardOutput = true,
				RedirectStandardInput = true,

			};
			var p = new Process { StartInfo = processStart };
			return p;
		}
	}
}