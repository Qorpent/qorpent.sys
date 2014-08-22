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
// PROJECT ORIGIN: Qorpent.Utils/FileNameResolverExtensions.cs
#endregion
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.IO;
using Qorpent.Log;
using IOException = Qorpent.IO.IOException;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	/// 	Extensions for file name resolver
	/// </summary>
	public static class FileNameResolverExtensions {
		/// <summary>
		/// 	Probe paths for usual view files
		/// </summary>
		public static readonly string[] DEFAULT_VIEW_RESOLVE_PROBE_PATHS = new[]
			{
				"usr/views", "mod/views", "extensions",
				"sys/views", "views", "tmp/_internal",
			};

		/// <summary>
		/// 	Probe paths for QVIEW
		/// </summary>
		public static readonly string[] DEFAULT_QVIEW_RESOLVE_PROBE_PATHS = new[]
			{
				"usr/qviews", "mod/qviews", "extensions",
				"sys/qviews", "locale/qviews", "qviews",
				"tmp/_internal"
			};

		/// <summary>
		/// Возвращает имя файла символов с контролем того компилировался ли проект средствами MONO (под Unix) или Windows
		/// </summary>
		/// <param name="dllname"></param>
		/// <returns></returns>
		public static string GetSymbolFileName(string dllname) {
			// при компиляции под Mono и .NET возникают файлы с разным расширением и разного формата
#if Unix  
			return dllname+".mdb";
#else
			return dllname.Replace(".dll", ".pdb");
#endif
		}

		

		/// <summary>
		/// 	Probe paths for JS
		/// </summary>
		public static readonly string[] DEFAULT_SCRIPT_RESOLVE_PROBE_PATHS = new[]
			{
				"usr/scripts", "usr", "mod/scripts", "mod",
				"extensions", "sys/scripts", "sys", "scripts",
				"tmp/_internal",
				""
			};

		/// <summary>
		/// 	Probe paths for CSS
		/// </summary>
		public static readonly string[] DEFAULT_CSS_RESOLVE_PROBE_PATHS = new[]
			{
				"usr/styles", "usr", "mod/styles", "mod",
				"extensions", "sys/styles", "sys", "styles", "",
				"tmp/_internal"
			};

		/// <summary>
		/// 	Probe paths for Images
		/// </summary>
		public static readonly string[] DEFAULT_IMAGE_RESOLVE_PROBE_PATHS = new[]
			{
				"usr/images", "usr", "mod/images", "mod",
				"extensions", "sys/images", "sys", "images",
				"tmp/_internal"
			};

		/// <summary>
		/// 	Probe paths for configs
		/// </summary>
		public static readonly string[] DEFAULT_CONFIG_RESOLVE_PROBE_PATHS = new[]
			{
				"usr/config", "usr", "mod/config", "mod",
				"extensions", "sys/config", "sys", "config", ""
				, "tmp/_internal"
			};

		/// <summary>
		/// 	Probe paths for custom user files
		/// </summary>
		public static readonly string[] DEFAULT_USRFILE_RESOLVE_PROBE_PATHS = new[]
			{"usr", "mod", "extensions", "sys", "", "bin", "tmp"};

		/// <summary>
		/// 	Bin probe paths
		/// </summary>
		public static readonly string[] DEFAULT_BIN_RESOLVE_PROBE_PATHS = new[] {"bin", "tmp/dlls", "tmp"};

		private static readonly object writelock = new object();

		/// <summary>
		/// 	Determines path set by extension
		/// </summary>
		/// <param name="extension"> </param>
		/// <returns> </returns>
		public static string[] GetDefaultProbesPaths(string extension) {
			//unification of extensions
			extension = extension.ToUpper();
			if (extension.StartsWith(".")) {
				extension = extension.Substring(1);
			}
			switch (extension) {
				case "JS":
					return DEFAULT_SCRIPT_RESOLVE_PROBE_PATHS;
				case "CSS":
					return DEFAULT_CSS_RESOLVE_PROBE_PATHS;
				case "BRAIL":
					return DEFAULT_VIEW_RESOLVE_PROBE_PATHS;

				case "PNG":
					return DEFAULT_IMAGE_RESOLVE_PROBE_PATHS;
				case "JPG":
					return DEFAULT_IMAGE_RESOLVE_PROBE_PATHS;
				case "JPEG":
					return DEFAULT_IMAGE_RESOLVE_PROBE_PATHS;
				case "GIF":
					return DEFAULT_IMAGE_RESOLVE_PROBE_PATHS;
				case "TIF":
					return DEFAULT_IMAGE_RESOLVE_PROBE_PATHS;
				case "TIFF":
					return DEFAULT_IMAGE_RESOLVE_PROBE_PATHS;

				case "CONFIG":
					return DEFAULT_CONFIG_RESOLVE_PROBE_PATHS;

				case "DLL":
					return DEFAULT_BIN_RESOLVE_PROBE_PATHS;
				case "EXE":
					return DEFAULT_BIN_RESOLVE_PROBE_PATHS;
				default:
					return DEFAULT_USRFILE_RESOLVE_PROBE_PATHS;
			}
		}

		/// <summary>
		/// 	Позволяет сформировать файловую структуру из ресурсов сборки
		/// </summary>
		/// <param name="resolver"> резольвер имен файлов </param>
		/// <param name="assembly"> сборка, содержащая ресурсы </param>
		/// <param name="externalizeMap"> мапинк суффиксов имен ресурсов (первый подходящий будет использован) и итоговых имен файлов </param>
		/// <param name="overwriteExisted"> true - файлы будут переписаны даже если они существуют </param>
		/// <param name="throwErrorOnErrorExisted"> ситуация ненахождения ресурса рассматривается как ошибочная (false - пропустить) </param>
		public static void ExternalizeInternalResources(this IFileNameResolver resolver, Assembly assembly,
		                                                IDictionary<string, string> externalizeMap,
		                                                bool overwriteExisted = false, bool throwErrorOnErrorExisted = true) {
			if (resolver == null) {
				throw new ArgumentNullException("resolver");
			}
			if (assembly == null) {
				throw new ArgumentNullException("assembly");
			}
			if (externalizeMap == null) {
				throw new ArgumentNullException("externalizeMap");
			}
			if (0 == externalizeMap.Count) {
				return;
			}
			var allnames = assembly.GetManifestResourceNames();
			foreach (var map in externalizeMap) {
				var resourcename = allnames.FirstOrDefault(x => x.EndsWith(map.Key));
				if (null == resourcename) {
					if (throwErrorOnErrorExisted) {
						throw new QorpentException("Не найдено ресурса с суффиксом " + map.Key);
					}
					continue;
				}
				var outfilename = resolver.Resolve(map.Value, false);
				Directory.CreateDirectory(Path.GetDirectoryName(outfilename));
				if (File.Exists(outfilename)) {
					if (!overwriteExisted) {
						continue;
					}
				}
				byte[] content = null;
				using (var r = assembly.GetManifestResourceStream(resourcename)) {
					content = new byte[r.Length];
					r.Read(content, 0, (int) r.Length);
				}
				File.WriteAllBytes(outfilename, content);
			}
		}

		/// <summary>
		/// 	overload for single file resolving to EXISTED FILE AND DEFAULT PROBES on current Application by default
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="name"> </param>
		/// <param name="existedOnly"> </param>
		/// <param name="probePaths"> </param>
		/// <param name="userLog"> </param>
		/// <param name="pathtype"> </param>
		/// <returns> </returns>
		public static string Resolve(this IFileNameResolver resolver, string name, bool existedOnly = true,
		                             string[] probePaths = null, IUserLog userLog = null,
		                             FileSearchResultType pathtype = FileSearchResultType.FullPath) {
			if (name.IsEmpty()) {
				throw new IOException("cannot resolve empty names");
			}
			name = name.NormalizePath();
			if (!name.StartsWith("~") && Path.IsPathRooted(name)) {
				if (existedOnly) {
					if (File.Exists(name)) {
						return name;
					}
					else {
						return null;
					}
				}
				else {
					return name;
				}
			}
			probePaths = probePaths ?? new string[] {};
			probePaths = probePaths.IsEmptyCollection()
				             ? GetDefaultProbesPaths(Path.GetExtension(name))
				             : probePaths.Select(x => NormalizePath(x)).ToArray();
			return resolver.Resolve(new FileSearchQuery
				{
					PathType = pathtype,
					ExistedOnly = existedOnly,
					ProbeFiles = new[] {name},
					ProbePaths = probePaths,
					UserLog = userLog
				});
		}

		/// <summary>
		/// 	overload for multiple file resolving to EXISTED FILE AND DEFAULT PROBES on current Application by default
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="names"> </param>
		/// <param name="existedOnly"> </param>
		/// <param name="probePaths"> </param>
		/// <param name="userLog"> </param>
		/// <param name="pathtype"> </param>
		/// <returns> </returns>
		public static string Resolve(this IFileNameResolver resolver, string[] names, bool existedOnly = true,
		                             string[] probePaths = null, IUserLog userLog = null,
		                             FileSearchResultType pathtype = FileSearchResultType.FullPath) {
			if (names == null || names.Length == 0) {
				throw new IOException("cannot resolve empty names");
			}
			names = names.Select(x => x.NormalizePath()).ToArray();
			if (probePaths.IsEmptyCollection()) {
				probePaths = DEFAULT_USRFILE_RESOLVE_PROBE_PATHS;
			}
			else {
				if (probePaths != null) {
					probePaths = probePaths.Select(x => x.NormalizePath()).ToArray();
				}
			}
			return resolver.Resolve(new FileSearchQuery
				{
					PathType = pathtype,
					ExistedOnly = existedOnly,
					ProbeFiles = names,
					ProbePaths = probePaths,
					UserLog = userLog
				});
		}

		/// <summary>
		/// 	overload for single file resolving to EXISTED LOCAL FILE AND DEFAULT PROBES on current Application by default
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="name"> </param>
		/// <param name="existedOnly"> </param>
		/// <param name="probePaths"> </param>
		/// <param name="userLog"> </param>
		/// <returns> </returns>
		public static string ResolveLocal(this IFileNameResolver resolver, string name, bool existedOnly = true,
		                                  string[] probePaths = null, IUserLog userLog = null) {
			if (name.IsEmpty()) {
				throw new IOException("cannot resolve empty names");
			}
			name = name.NormalizePath();
			probePaths = probePaths ?? new string[] {};
			probePaths = probePaths.IsEmptyCollection()
				             ? GetDefaultProbesPaths(Path.GetExtension(name))
				             : probePaths.Select(x => x.NormalizePath()).ToArray();
			return resolver.Resolve(new FileSearchQuery
				{
					PathType = FileSearchResultType.LocalPath,
					ExistedOnly = existedOnly,
					ProbeFiles = new[] {name},
					ProbePaths = probePaths,
					UserLog = userLog
				});
		}


		/// <summary>
		/// 	overload for multiple file resolving to EXISTED LOCAL FILE AND DEFAULT PROBES on current Application by default
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="names"> </param>
		/// <param name="existedOnly"> </param>
		/// <param name="probePaths"> </param>
		/// <param name="userLog"> </param>
		/// <returns> </returns>
		public static string ResolveLocal(this IFileNameResolver resolver, string[] names, bool existedOnly = true,
		                                  string[] probePaths = null, IUserLog userLog = null) {
			if (names == null || names.Length == 0) {
				throw new IOException("cannot resolve empty names");
			}
			names = names.Select(x => x.NormalizePath()).ToArray();
			if (probePaths.IsEmptyCollection()) {
				probePaths = DEFAULT_USRFILE_RESOLVE_PROBE_PATHS;
			}
			else {
				if (probePaths != null) {
					probePaths = probePaths.Select(x => x.NormalizePath()).ToArray();
				}
			}
			return resolver.Resolve(new FileSearchQuery
				{
					PathType = FileSearchResultType.LocalPath,
					ExistedOnly = existedOnly,
					ProbeFiles = names,
					ProbePaths = probePaths,
					UserLog = userLog
				});
		}

		/// <summary>
		/// 	overload for single file resolving to EXISTED FILE URL AND DEFAULT PROBES on current Application by default
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="name"> </param>
		/// <param name="existedOnly"> </param>
		/// <param name="probePaths"> </param>
		/// <param name="userLog"> </param>
		/// <returns> </returns>
		public static string ResolveUrl(this IFileNameResolver resolver, string name, bool existedOnly = true,
		                                string[] probePaths = null, IUserLog userLog = null) {
			if (name.IsEmpty()) {
				throw new IOException("cannot resolve empty names");
			}
			name = name.NormalizePath();
			probePaths = probePaths ?? new string[] {};
			probePaths = probePaths.IsEmptyCollection()
				             ? GetDefaultProbesPaths(Path.GetExtension(name))
				             : probePaths.Select(x => x.NormalizePath()).ToArray();
			return resolver.Resolve(new FileSearchQuery
				{
					PathType = FileSearchResultType.LocalUrl,
					ExistedOnly = existedOnly,
					ProbeFiles = new[] {name},
					ProbePaths = probePaths,
					UserLog = userLog
				});
		}

		/// <summary>
		/// 	overload for multiple file resolving to EXISTED FILE URL AND DEFAULT PROBES on current Application by default
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="names"> </param>
		/// <param name="existedOnly"> </param>
		/// <param name="probePaths"> </param>
		/// <param name="userLog"> </param>
		/// <returns> </returns>
		public static string ResolveUrl(this IFileNameResolver resolver, string[] names, bool existedOnly = true,
		                                string[] probePaths = null, IUserLog userLog = null) {
			if (names == null || names.Length == 0) {
				throw new IOException("cannot resolve empty names");
			}
			names = names.Select(x => x.NormalizePath()).ToArray();
			if (probePaths.IsEmptyCollection()) {
				probePaths = DEFAULT_USRFILE_RESOLVE_PROBE_PATHS;
			}
			else {
				if (probePaths != null) {
					probePaths = probePaths.Select(x => x.NormalizePath()).ToArray();
				}
			}
			return resolver.Resolve(new FileSearchQuery
				{
					PathType = FileSearchResultType.FullPath,
					ExistedOnly = existedOnly,
					ProbeFiles = names,
					ProbePaths = probePaths,
					UserLog = userLog
				});
		}

		/// <summary>
		/// 	Removes illegal folder delimiters, converts to single '/' style
		/// </summary>
		/// <param name="path"> </param>
		/// <returns> </returns>
		public static string NormalizePath(this string path) {
			if (null == path) {
				return null;
			}
			return path.Replace("\\", "/").Replace("//", "/").Replace("//", "/").ToLower();
		}

		/// <summary>
		/// Повторяет поведение ResolveAll из старого ядра
		/// </summary>
		/// <param name="resolver"></param>
		/// <param name="rootdir"></param>
		/// <param name="mask"></param>
		/// <param name="existedOnly"></param>
		/// <returns></returns>
		public static string[] ResolveAll(this IFileNameResolver resolver, string rootdir, string mask, bool existedOnly) {
			return
				resolver.ResolveAll(new FileSearchQuery
					{
						All = true,
						ExistedOnly = existedOnly,
						PathType = FileSearchResultType.FullPath,
						ProbeFiles = new[] {mask},
						ProbePaths = new[] {rootdir}
					});
		}


		/// <summary>
		/// 	Extract resources to given path
		/// </summary>
		/// <param name="resolver"> </param>
		/// <param name="mask"> </param>
		/// <param name="assembly"> </param>
		/// <param name="outdir"> </param>
		/// <param name="cleanupnamepattern"> </param>
		/// <param name="userLog"> </param>
		public static void Externalize(this IFileNameResolver resolver, string mask, Assembly assembly = null,
		                               string outdir = "~/tmp/_internal",
		                               string cleanupnamepattern = @"^(?i)[\s\S]+?\.resources\.", IUserLog userLog = null) {
			lock (writelock) {
				if (mask.IsEmpty()) {
					mask = ".";
				}
				assembly = assembly ?? Assembly.GetCallingAssembly();
				outdir = outdir ?? "~/tmp/_internal";
				cleanupnamepattern = cleanupnamepattern ?? @"^(?i)[\s\S]+?\.resources\.";
				var maskregex = new Regex(mask, RegexOptions.Compiled);
				var cleanupregex = new Regex(cleanupnamepattern, RegexOptions.Compiled);
				var resourcenames = assembly.GetManifestResourceNames().Where(x => maskregex.IsMatch(x)).ToArray();
				foreach (var resourcename in resourcenames) {
					using (var sr = new StreamReader(assembly.GetManifestResourceStream(resourcename))) {
						var name = cleanupregex.Replace(resourcename, "").Replace("___", ".").Replace("__", "/");
						//psuedo folder support for embeded resources and avoid for limitation of CS embeded resources
						var path = outdir + "/" + name;
						var fullpath = (resolver).Resolve(path, false, userLog: userLog);
						var dirpath = Path.GetDirectoryName(fullpath);
						Directory.CreateDirectory(dirpath);
						var content = sr.ReadToEnd();
						File.WriteAllText(fullpath, content);
					}
				}
			}
		}
        /// <summary>
        ///     Получение последней даты записи в директорию 
        /// </summary>
        /// <param name="resolver"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime GetDirectoryLastWriteTime(this IFileNameResolver resolver, string name) {
            return new DirectoryInfo(resolver.Resolve(name)).GetFiles().Select(_ => _.LastWriteTime).Max();
        }
		/// <summary>
		/// 	Writes content to file
		/// </summary>
		/// <param name="r"> </param>
		/// <param name="name"> </param>
		/// <param name="content"> </param>
		/// <param name="userLog"> </param>
		/// <param name="append"> </param>
		/// <returns> </returns>
		public static IFileNameResolver Write(this IFileNameResolver r, string name, object content,
		                                      IUserLog userLog = null, bool append = false) {
			if (r == null) {
				throw new ArgumentNullException("r");
			}
			lock (writelock) {
				userLog = checkUserLog(r, userLog);
				userLog.Debug("start write " + name);
				var path = r.Resolve(name, false, null, userLog);
				userLog.Debug("path resolved as " + path);
				Directory.CreateDirectory(Path.GetDirectoryName(path));
				if (append) {
					appendFile(path, content);
				}
				else {
					rewriteFile(path, content);
				}
				r.ClearCache();
				userLog.Trace("file saved " + path);
				return r;
			}
		}

		private static IUserLog checkUserLog(IFileNameResolver r, IUserLog userLog) {
			if (null == userLog) {
				if (r is ILogBound) {
					userLog = ((ILogBound) r).Log;
				}
				if (null == userLog) {
					userLog = Application.Current.LogManager.GetLog(typeof (FileNameResolverExtensions).FullName,
					                                                typeof (FileNameResolverExtensions));
				}
			}
			return userLog;
		}

		private static void rewriteFile(string path, object content) {
			if (content == null) {
				content = "";
			}
			if (content is string) {
				File.WriteAllText(path, (string) content);
			}
			else if (content is byte[]) {
				File.WriteAllBytes(path, (byte[]) content);
			}
			else {
				File.WriteAllText(path, content.ToString());
			}
		}

		private static void appendFile(string path, object content) {
			if (content == null) {
				content = "";
			}
			if (content is string) {
				File.AppendAllText(path, (string) content);
			}
			else if (content is byte[]) {
				throw new NotImplementedException();
			}
			else {
				File.AppendAllText(path, content.ToString());
			}
		}

		/// <summary>
		/// 	Read content of file
		/// </summary>
		/// <param name="r"> </param>
		/// <param name="name"> </param>
		/// <param name="userLog"> </param>
		/// <typeparam name="T"> </typeparam>
		/// <returns> </returns>
		/// <exception cref="FileNotFoundException"></exception>
		/// <exception cref="ArgumentException"></exception>
		public static T Read<T>(this IFileNameResolver r, string name, IUserLog userLog = null) {
			if (r == null) {
				throw new ArgumentNullException("r");
			}
			if (typeof (T) == typeof (string) || typeof (T) == typeof (XElement) || typeof (T) == typeof (byte[])) {
				userLog = checkUserLog(r, userLog);
				userLog.Debug("start write " + name);
				var path = r.Resolve(name, true, null, userLog);

				if (null == path) {
					userLog.Error("file not found");
					throw new FileNotFoundException(name);
				}
				userLog.Debug("path resolved as " + path);
				object result = default(T);
				if (typeof (T) == typeof (string)) {
					result = File.ReadAllText(path);
				}
				else if (typeof (T) == typeof (byte)) {
					result = File.ReadAllBytes(path);
				}
				else if (typeof (T) == typeof (XElement)) {
					result = XElement.Load(path);
				}
				userLog.Trace("file readed " + path);
				return (T) result;
			}
			throw new ArgumentException("Read method supports only strings, XElement and byte[] as result");
		}
	}
}