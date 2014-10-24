using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Qorpent.IO;

namespace Qorpent.Host.Static{
	/// <summary>
	/// </summary>
	public class HostServerStaticResolver : IHostServerStaticResolver{
		private readonly ConcurrentDictionary<string, IWebFileRecord> _cache =
			new ConcurrentDictionary<string, IWebFileRecord>();

		private IHostServer _host;


		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		public void Initialize(IHostServer server){
			_host = server;
			Resolver = new WebFileResolver();
			foreach (var exf in _host.Config.ExtendedContentFolders){

                _host.Config.Log.Info("Ex-Content from " + EnvironmentInfo.ResolvePath(exf) + " added");
				Resolver.Register(new FileSystemWebFileProvider{ExactOnly = true,Root = EnvironmentInfo.ResolvePath( exf)});
			}
			foreach (var f in _host.Config.ContentFolders){

                _host.Config.Log.Info("Content from " + EnvironmentInfo.ResolvePath(f) + " added");
				Resolver.Register(new FileSystemWebFileProvider { ExactOnly = false, Root = EnvironmentInfo.ResolvePath( f) });
			}

            _host.Config.Log.Info("Root-Content from " + EnvironmentInfo.ResolvePath( _host.Config.RootFolder) + " added");
			Resolver.Register(new FileSystemWebFileProvider { ExactOnly = false, Root =EnvironmentInfo.ResolvePath(  _host.Config.RootFolder) });
			foreach (var assembly in  _host.Config.AutoconfigureAssemblies.Select(Assembly.Load).ToArray()){
                _host.Config.Log.Info("Resource-Content from " + assembly.GetName().Name + " added");
				Resolver.Register(new ResourceWebFileProvider{Assembly = assembly});
			}

            _host.Config.Log.Info("Resource-Content from Qorpent.Host added");
			Resolver.Register(new ResourceWebFileProvider{Assembly = typeof(HostServer).Assembly});
		}

		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="context"></param>
		/// <param name="withextended"></param>
		/// <returns></returns>
		public IWebFileRecord Get(string name, object context = null, bool withextended = false){
			IWebFileRecord result;
			if (_cache.TryGetValue(name, out result)){
				return result;
			}
			lock (this){
				IWebFileRecord resolved = Resolve(name, context, withextended);
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
		/// Класс-резольвер для файлов
		/// </summary>
		public IWebFileResolver Resolver { get; private set; }


		/// <summary>
		/// </summary>
		/// <param name="name"></param>
		/// <param name="context"></param>
		/// <param name="withextensions"></param>
		/// <returns></returns>
		private IWebFileRecord Resolve(string name, object context, bool withextensions){
			return Resolver.Find(name);
		}

		

	}
}