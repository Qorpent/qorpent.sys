using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host.Static{
	/// <summary>
	/// </summary>
	public class HostServerStaticResolver : IHostServerStaticResolver{
		private readonly ConcurrentDictionary<string, StaticContentDescriptor> _cache =
			new ConcurrentDictionary<string, StaticContentDescriptor>();

		private List<Assembly> _assemblies;
		private List<string> _folders;
		private IHostServer _host;
		private Dictionary<string, Tuple<Assembly, string>> _resources;


		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		public void Initialize(IHostServer server){
			_host = server;
			_folders = _host.Config.ContentFolders.ToList();
			_folders.Add(_host.Config.RootFolder);
			_assemblies = _host.Config.AutoconfigureAssemblies.Select(Assembly.Load).ToList();
			_assemblies.Insert(0, typeof (HostServer).Assembly);
			_resources = new Dictionary<string, Tuple<Assembly, string>>();
			foreach (Assembly assembly in _assemblies){
				foreach (string name in assembly.GetManifestResourceNames()){
					IList<string> nameparts = name.SmartSplit(false, true, '.');
					int len = nameparts.Count;
					string localname = name.Substring(name.IndexOf("Resources") + 10);
					int idx = nameparts.IndexOf("Resources");
					if (idx == -1 || idx >= len - 2){
						idx = 0;
					}
					else{
						idx += 9;
					}
					string path = "/";
					for (int i = idx; i < len - 2; i++){
						path += nameparts[i] + "/";
					}
					path += localname;
					_resources[path] = new Tuple<Assembly, string>(assembly, name);
				}
			}
		}

		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="context"></param>
		/// <param name="withextended"></param>
		/// <returns></returns>
		public StaticContentDescriptor Get(string name, object context = null, bool withextended = false){
			StaticContentDescriptor result;
			if (_cache.TryGetValue(name, out result)){
				return result;
			}
			lock (this){
				StaticContentDescriptor resolved = Resolve(name, context, withextended);
				if (null != resolved){
					_cache[name] = resolved;
				}
				return resolved;
			}
		}

		/// <summary>
		///     Сброс кэша
		/// </summary>
		public void DropCache(){
			_cache.Clear();
		}

		IDictionary<string,string> masks = new Dictionary<string, string>();

		/// <summary>
		/// Устанавливает корневую директорю для части юрлов
		/// </summary>
		/// <param name="mask"></param>
		/// <param name="rootdirectory"></param>
		public void SetRoot(string mask, string rootdirectory){
			masks[mask] = rootdirectory;
		}

		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="context"></param>
		/// <param name="withextensions"></param>
		/// <returns></returns>
		private StaticContentDescriptor Resolve(string name, object context, bool withextensions){
			StaticContentDescriptor result = ResolveByFiles(name, context, withextensions);
			if (null == result){
				result = ResolveByResources(name, context, withextensions);
			}
			if (null == result){
				result = ResolveByMasks(name, context);
			}
			return result;
		}

		private StaticContentDescriptor ResolveByMasks(string name, object context){
			foreach (var mask in masks){
				if (Regex.IsMatch(name, mask.Key,RegexOptions.Compiled)){
					var filename = Path.Combine(mask.Value, name.Substring(1));
					//if (File.Exists(filename)){
				    return new FileContentDescriptor(filename) {NoCheckVersion=true, UseMemoryCache = true} ;
				    //}
				}
			}
			return null;
		}

		private static int MatchTail(string path1, string path2){
			string subfolder = "";
			string ext = Path.GetExtension(path2).Substring(1);
			if (ext == "html"){
				subfolder = "views";
			}
			else if (ext == "js"){
				subfolder = "js";
			}
			else if (ext == "css"){
				subfolder = "css";
			}
			if (string.IsNullOrWhiteSpace(subfolder)){
				return InternalMatchTail(path1, path2);
			}
			int usualprobe = InternalMatchTail(path1, path2);
			string normpath = Path.Combine(Path.GetDirectoryName(path2), subfolder + "/" + Path.GetFileName(path2));
			int normprobe = InternalMatchTail(path1, normpath);
			return Math.Max(usualprobe, normprobe);
		}

		private static int InternalMatchTail(string path1, string path2){
			path1 = path1.NormalizePath().ToLower();
			path2 = path2.NormalizePath().ToLower();
			int commonlen = Math.Min(path1.Length, path2.Length);
			int i = 0;
			for (i = i = 0; i < commonlen; i++){
				if (path1[path1.Length - 1 - i] != path2[path2.Length - 1 - i]) return i + 1;
			}
			return i;
		}

		private StaticContentDescriptor ResolveByResources(string name, object context, bool withextensions){
			string local = Path.GetFileName(name) ?? "";
			var resolvedResourcePair = (
				                           from k in _resources
				                           where k.Key.EndsWith(local)
				                           let weight = MatchTail(k.Key, local)
				                           orderby weight descending
				                           select new{k.Key, k.Value}
			                           ).FirstOrDefault();
			if (null == resolvedResourcePair) return null;
			return new ResourceContentDescriptor(resolvedResourcePair.Key, resolvedResourcePair.Value.Item2,
			                                     resolvedResourcePair.Value.Item1);
		}

		private StaticContentDescriptor ResolveByFiles(string name, object context, bool withextensions){
			string local = Path.GetFileName(name) ?? "";
			IEnumerable<string> f = _folders;
			if (withextensions){
				f = _folders.Union(_host.Config.ExtendedContentFolders);
			}
			var resolvedFileName = (
				                       from dir in f
				                       from file in Directory.GetFiles(dir, local, SearchOption.AllDirectories)
				                       let weight = MatchTail(file, name)
				                       orderby weight descending
				                       select new{file, dir}
			                       ).FirstOrDefault();
			if (null == resolvedFileName) return null;
			return new FileContentDescriptor(resolvedFileName.file, resolvedFileName.dir);
		}
	}
}