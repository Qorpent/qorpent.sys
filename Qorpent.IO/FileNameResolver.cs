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
// Original file : FileNameResolver.cs
// Project: Qorpent.IO
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.Applications;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO {
	/// <summary>
	/// 	Main implementation of IFileNameResolver
	/// </summary>
	/// <remarks>
	/// </remarks>
	[ContainerComponent(Lifestyle.Transient)]
	[RequireReset(Role = "DEVELOPER")]
	//кэш файлов может сбрасываться в случае обновления файловой структуры - это означает, что это специализация DEVELOPER
	public class FileNameResolver : ServiceBase, IFileNameResolver {
		/// <summary>
		/// </summary>
		private static int _id;

#if PARANOID
		static FileNameResolver() {
			if(!Qorpent.Security.Watchdog.Paranoid.Provider.OK) throw new  Qorpent.Security.Watchdog.ParanoidException(Qorpent.Security.Watchdog.ParanoidState.GeneralError);
		}
#endif

		/// <summary>
		/// 	Creates new filename resolver
		/// </summary>
		/// <remarks>
		/// </remarks>
		public FileNameResolver() {
			Id = _id++;
		}

		/// <summary>
		/// 	Instance id (resolvers must be on count checking)
		/// </summary>
		/// <remarks>
		/// </remarks>
		public int Id { get; private set; }

		/// <summary>
		/// 	shortcut for statistics about cache size
		/// </summary>
		/// <remarks>
		/// </remarks>
		public int CacheCount {
			get { return _cache.Count; }
		}

		/// <summary>
		/// 	Reset version
		/// </summary>
		/// <value> The version. </value>
		/// <remarks>
		/// </remarks>
		public int Version { get; protected set; }

		/// <summary>
		/// 	Last reset time
		/// </summary>
		/// <value> The timestamp. </value>
		/// <remarks>
		/// </remarks>
		public DateTime Timestamp { get; protected set; }


		/// <summary>
		/// 	Custom root directory
		/// </summary>
		/// <value> The root. </value>
		/// <remarks>
		/// </remarks>
		public string Root {
			get {
				if (null != _root) {
					return _root;
				}
				if (null != _fileservice) {
					return _fileservice.Root;
				}
				if (null != Application) {
					return Application.RootDirectory;
				}
				return EnvironmentInfo.RootDirectory;
			}
			set { _root = value; }
		}


		/// <summary>
		/// 	Resolves first file that match query
		/// </summary>
		/// <param name="query"> search query </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public string Resolve(FileSearchQuery query) {
			return InternalCachedResolve(query).FirstOrDefault();
		}

		/// <summary>
		/// 	Resolves all files match query
		/// </summary>
		/// <param name="query"> search query </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public string[] ResolveAll(FileSearchQuery query) {
			query.All = true;
			return InternalCachedResolve(query);
		}

		/// <summary>
		/// 	Clears file resolution cache
		/// </summary>
		/// <remarks>
		/// </remarks>
		public void ClearCache() {
			_cache.Clear();
		}


		/// <summary>
		/// 	Возващает объект, описывающий состояние до очистки
		/// </summary>
		/// <returns> </returns>
		public override object GetPreResetInfo() {
			return new {cacheSize = _cache.Count};
		}

		/// <summary>
		/// 	Setup File Name Resolving context
		/// </summary>
		/// <param name="fileservice"> The fileservice. </param>
		/// <param name="application"> The application. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public FileNameResolver SetParentService(IFileService fileservice, IApplication application) {
			_fileservice = fileservice;
			Application = application;
			if (null == Application && null != fileservice) {
				Application = fileservice.Application;
			}
			if (null == Application) {
				Application = Applications.Application.Current;
			}
			Application.Events.Add(new StandardResetHandler(this));
			return this;
		}

		/// <summary>
		/// 	internal all-resolver
		/// </summary>
		/// <param name="query"> The query. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected string[] InternalCachedResolve(FileSearchQuery query) {
			lock (Sync) {
				if (query.ProbeFiles.Length == 1 && query.PathType == FileSearchResultType.FullPath) {
					//quick return of existed/non existed full path
					if (Path.IsPathRooted(query.ProbeFiles[0])) {
						var directpath = query.ProbeFiles[0];
						if (query.ExistedOnly) {
							if (!(File.Exists(directpath) || Directory.Exists(directpath))) {
								return new string[] {};
							}
						}
						return new[] {directpath};
					}
				}
				query.UserLog = query.UserLog ?? Log;
				if (null == query.ProbeFiles || 0 == query.ProbeFiles.Length) {
					throw new ArgumentException("empty probe files");
				}
				if (null != query.ProbeFiles.FirstOrDefault(x => x.Contains(".."))) {
					throw new ArgumentException("cannot use .. path modifiers in files");
				}
				query.ProbeFiles = query.ProbeFiles.Select(FileNameResolverExtensions.NormalizePath).ToArray();
				if (null == query.ProbePaths) {
					var ext = Path.GetExtension(query.ProbeFiles.First());
					query.ProbePaths = FileNameResolverExtensions.GetDefaultProbesPaths(ext);
				}
				else {
					if (null != query.ProbePaths.FirstOrDefault(x => x.Contains(".."))) {
						throw new ArgumentException("cannot use .. path modifiers in dirs");
					}
					query.ProbePaths = query.ProbePaths.Select(FileNameResolverExtensions.NormalizePath).ToArray();
				}

				var key = query.ToString();
				query.UserLog.Trace("start resolving files with key " + key, this);
				string[] result;
				if (!_cache.ContainsKey(key)) {
					query.UserLog.Debug("no cached Context found", this);
					result = InternalResolve(query);
					Version++;
					Timestamp = DateTime.Now;
					_cache[key] = result;
				}
				result = _cache[key];
				query.UserLog.Trace("resolution finished with " + result, this);
				return result;
			}
		}

		/// <summary>
		/// 	Internals the resolve.
		/// </summary>
		/// <param name="query"> The query. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private string[] InternalResolve(FileSearchQuery query) {
			if (!query.ExistedOnly) {
				return new[] {ResolveBestPosiblePath(query.PathType, query.ProbePaths[0], query.ProbeFiles[0], query.UserLog)};
			}
			var result = new List<string>();
			if (query.All) {
				GetAllMatchedFiles(query, result);
			}
			else {
				GetSingleFile(query, result);
			}
			return result.Distinct().ToArray();
		}

		/// <summary>
		/// 	Gets all matched files.
		/// </summary>
		/// <param name="query"> The query. </param>
		/// <param name="result"> The result. </param>
		/// <remarks>
		/// </remarks>
		private void GetAllMatchedFiles(FileSearchQuery query, List<string> result) {
			foreach (var probefile in query.ProbeFiles) {
				foreach (var dir in query.ProbePaths) {
					query.UserLog.Debug("enter dir " + dir, this);
					var ndir = dir;
					if (ndir.StartsWith("~/")) {
						ndir = ndir.Substring(2);
					}
					var path = (Root + "/" + ndir).NormalizePath();
					var pf = probefile;
					if (pf.StartsWith("/")) {
						pf = pf.Substring(1);
					}
					var full = Path.Combine(path, pf);
					var directoryName = Path.GetDirectoryName(full);
					var fileName = Path.GetFileName(full);
					if (directoryName != null && Directory.Exists(directoryName)) {
						if (fileName != null) {
							result.AddRange(Directory.GetFiles(directoryName, fileName).Select(file => AdaptFilePath(query.PathType, file)));
						}
					}
				}
			}
		}

		/// <summary>
		/// 	Gets the single file.
		/// </summary>
		/// <param name="query"> The query. </param>
		/// <param name="result"> The result. </param>
		/// <remarks>
		/// </remarks>
		private void GetSingleFile(FileSearchQuery query, List<string> result) {
			foreach (var probefile in query.ProbeFiles) {
				if (0 != result.Count) {
					break;
				}
				if (probefile.StartsWith("~/")) {
					var path = Path.Combine(Root, probefile.Substring(2)).NormalizePath();
					query.UserLog.Debug("try " + path, this);
					if (File.Exists(path) || Directory.Exists(path)) {
						query.UserLog.Debug("existed", this);
						result.Add(AdaptFilePath(query.PathType, path));
						break;
					}
				}
				foreach (var dir in query.ProbePaths) {
					query.UserLog.Debug("enter dir " + dir, this);
					var path = (Root + "/" + dir).NormalizePath();
					var probe = (path + "/" + probefile).NormalizePath();
					query.UserLog.Debug("try " + probe, Root);
					if (File.Exists(probe) || Directory.Exists(probe)) {
						query.UserLog.Debug("existed", this);
						result.Add(AdaptFilePath(query.PathType, probe));
						break;
					}
				}
			}
		}

		/// <summary>
		/// 	Resolves the best posible path.
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="dir"> The dir. </param>
		/// <param name="file"> The file. </param>
		/// <param name="userLog"> The UserLog. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private string ResolveBestPosiblePath(FileSearchResultType type, string dir, string file,
		                                      IUserLog userLog) {
			if (file.StartsWith("~/")) {
				return Path.Combine(Root, file.Substring(2)).NormalizePath();
			}
			var path = "/" + dir + "/" + file;
			var resolved = (Root + path).NormalizePath();
			var result = AdaptFilePath(type, resolved);
			userLog.Debug("resolved to best possible  " + result, Application);
			return result;
		}

		/// <summary>
		/// 	Adapts the file path.
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="resolved"> The resolved. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		private string AdaptFilePath(FileSearchResultType type, string resolved) {
			switch (type) {
				case FileSearchResultType.FullPath:
					return resolved.NormalizePath();
				case FileSearchResultType.LocalPath:
					return resolved.Substring(Root.Length);
				case FileSearchResultType.FullUrl:
					return
						new Uri(
							new Uri((Application ?? Applications.Application.Current).CurrentMvcContext.Uri.GetLeftPart(UriPartial.Authority)),
							new Uri(
								("/" + (Application ?? Applications.Application.Current).ApplicationName + "/" + resolved.Substring(Root.Length))
									.NormalizePath())).ToString
							().
							Replace("file:/", "").NormalizePath();
				case FileSearchResultType.LocalUrl:
					return
						("/" + (Application ?? Applications.Application.Current).ApplicationName + "/" + resolved.Substring(Root.Length)).
							NormalizePath();
				default:
					throw new Exception("cannot define result for type " + type);
			}
		}


		/// <summary>
		/// 	Вызывается при вызове Reset
		/// </summary>
		/// <param name="data"> </param>
		/// <returns> любой объект - будет включен в состав результатов <see cref="ResetEventResult" /> </returns>
		/// <remarks>
		/// 	При использовании стандартной настройки из <see cref="ServiceBase" /> не требует фильтрации опций,
		/// 	настраивается на основе атрибута <see cref="RequireResetAttribute" />
		/// </remarks>
		public override object Reset(ResetEventData data) {
			ClearCache();
			return new {Id, Version, CacheCount};
		}

		/// <summary>
		/// </summary>
		private readonly IDictionary<string, string[]> _cache = new Dictionary<string, string[]>();

		/// <summary>
		/// </summary>
		private IFileService _fileservice;

		/// <summary>
		/// </summary>
		private string _root;
	}
}