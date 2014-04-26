using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Qorpent.Utils.Extensions;

namespace Qorpent.Host{
	/// <summary>
	/// 
	/// </summary>
	public class HostServerStaticResolver : IHostServerStaticResolver{
		private ConcurrentDictionary<string, StaticContentDescriptor> _cache =
			new ConcurrentDictionary<string, StaticContentDescriptor>();
		private IHostServer _host;
		private List<string> _folders;
		private List<Assembly> _assemblies;
		private Dictionary<string, Tuple<Assembly, string>> _resources;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		public void Initialize(IHostServer server){
			_host = server;
			_folders = _host.Config.ContentFolders.ToList();
			_folders.Add(_host.Config.RootFolder);
			_assemblies = _host.Config.AutoconfigureAssemblies.Select(Assembly.Load).ToList();
			_assemblies.Insert(0,typeof(IHostServer).Assembly);
			_resources = new Dictionary<string,Tuple <Assembly,string>>();
			foreach (var assembly in _assemblies){
				foreach (var name in assembly.GetManifestResourceNames()){
					var nameparts = name.SmartSplit(false, true, '.');
					var len = nameparts.Count;
                    var localname = name.Substring(name.IndexOf("Resources") + 10);
					var idx = nameparts.IndexOf("Resources");
					if (idx == -1 || idx >= len - 2){
						idx = 0;
					}
					else{
						idx += 9;
					}
					var path = "/";
					for (var i = idx; i < len - 2; i++){
						path += nameparts[i]+"/";
					}
					path += localname;
					_resources[path] =new Tuple<Assembly, string>(assembly,name);

				}
			}
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="name"></param>
	    /// <param name="context"></param>
	    /// <param name="withextended"></param>
	    /// <returns></returns>
	    public StaticContentDescriptor Get(string name, object context = null, bool withextended =false){
			StaticContentDescriptor result;
			if (_cache.TryGetValue(name, out result)){
				return result;
			}
			lock (this){
				var resolved = Resolve(name, context,withextended);
				if (null != resolved){
					_cache[name] = resolved;
				}
				return resolved;
			}
		}

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="name"></param>
	    /// <param name="context"></param>
	    /// <param name="withextensions"></param>
	    /// <returns></returns>
	    private StaticContentDescriptor Resolve(string name, object context, bool withextensions){
            var result = ResolveByFiles(name, context, withextensions);
			if (null == result){
                result = ResolveByResources(name, context, withextensions);
			}
			return result;
		}

		private static int MatchTail(string path1, string path2){
			path1 = path1.NormalizePath().ToLower();
			path2 = path2.NormalizePath().ToLower();
			var commonlen = Math.Min(path1.Length, path2.Length);
			for (var i = 0; i < commonlen; i++){
				if (path1[path1.Length - 1 - i] != path2[path2.Length - 1 - i]) return i;
			}
			return -1;
		}

		private StaticContentDescriptor ResolveByResources(string name, object context, bool withextensions){
			var local = Path.GetFileName(name) ?? "";
			var resolvedResourcePair = (
				                           from k in _resources
				                           where k.Key.EndsWith(local)
				                           let weight = MatchTail(k.Key, local)
				                           orderby weight descending
				                           select new {k.Key,k.Value}
			                           ).FirstOrDefault();
			if (null == resolvedResourcePair) return null;
			return new ResourceContentDescriptor(resolvedResourcePair.Key,resolvedResourcePair.Value.Item2, resolvedResourcePair.Value.Item1);
		}

		private StaticContentDescriptor ResolveByFiles(string name, object context, bool withextensions){
			var local = Path.GetFileName(name)??"";
		    IEnumerable<string> f = _folders;
            if (withextensions) {
                f = _folders.Union(_host.Config.ExtendedContentFolders);
            }
			var resolvedFileName = (
				                       from dir in f
				                       from file in Directory.GetFiles(dir, local, SearchOption.AllDirectories)
				                       let weight = MatchTail(file, name)
				                       orderby weight descending
				                       select new{file,dir}
			                       ).FirstOrDefault();
			if (null==resolvedFileName) return null;
			return new FileContentDescriptor(resolvedFileName.file,resolvedFileName.dir);
		}

		/// <summary>
		/// Сброс кэша
		/// </summary>
		public void DropCache(){
			_cache.Clear();
		}
	}
}