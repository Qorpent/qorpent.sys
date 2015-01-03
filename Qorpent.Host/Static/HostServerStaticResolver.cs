using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Qorpent.IO;
using Qorpent.IO.FileCache;

namespace Qorpent.Host.Static {
    /// <summary>
    /// </summary>
    public class HostServerStaticResolver : IHostServerStaticResolver {
        private IHostServer _host;

        private readonly ConcurrentDictionary<string, IWebFileRecord> _cache =
            new ConcurrentDictionary<string, IWebFileRecord>();

        private readonly IDictionary<string, IFileCacheResolver> caches = new Dictionary<string, IFileCacheResolver>();
        private readonly IDictionary<string, string> masks = new Dictionary<string, string>();

        /// <summary>
        /// </summary>
        public IDictionary<string, string> Masks {
            get { return masks; }
        }

        /// <summary>
        /// </summary>
        public IDictionary<string, IFileCacheResolver> Caches {
            get { return caches; }
        }

        /// <summary>
        ///     Класс-резольвер для файлов
        /// </summary>
        public IWebFileResolver Resolver { get; private set; }

        /// <summary>
        /// </summary>
        /// <param name="server"></param>
        public void Initialize(IHostServer server) {
            _host = server;
            Resolver = new WebFileResolver();
            foreach (var exf in _host.Config.ExtendedContentFolders) {
                _host.Config.Log.Info("Ex-Content from " + EnvironmentInfo.ResolvePath(exf) + " added");
                Resolver.Register(new FileSystemWebFileProvider {
                    ExactOnly = true,
                    Root = EnvironmentInfo.ResolvePath(exf)
                });
            }
            foreach (var f in _host.Config.ContentFolders) {
                _host.Config.Log.Info("Content from " + EnvironmentInfo.ResolvePath(f) + " added");
                Resolver.Register(new FileSystemWebFileProvider {
                    ExactOnly = false,
                    Root = EnvironmentInfo.ResolvePath(f)
                });
            }

            _host.Config.Log.Info("Root-Content from " + EnvironmentInfo.ResolvePath(_host.Config.RootFolder) + " added");
            Resolver.Register(new FileSystemWebFileProvider {
                ExactOnly = false,
                Root = EnvironmentInfo.ResolvePath(_host.Config.RootFolder)
            });
            foreach (var assembly in  _host.Config.AutoconfigureAssemblies.Select(Assembly.Load).ToArray()) {
                _host.Config.Log.Info("Resource-Content from " + assembly.GetName().Name + " added");
                Resolver.Register(new ResourceWebFileProvider {Assembly = assembly});
            }

            _host.Config.Log.Info("Resource-Content from Qorpent.Host added");
            Resolver.Register(new ResourceWebFileProvider {Assembly = typeof (HostServer).Assembly});
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="withextended"></param>
        /// <returns></returns>
        public IWebFileRecord Get(string name, object context = null, bool withextended = false) {
            IWebFileRecord result;

            if (_cache.TryGetValue(name, out result)) {
                return result;
            }
            lock (this) {
                if (masks.Count != 0) {
                    IWebFileRecord record;
                    if (GetByMask(name, out record)) return record;
                }
                if (caches.Count != 0)
                {
                    IWebFileRecord record;
                    if (GetByCache(name, false, out record)) return record;
                }
                var resolved = Resolve(name, context, withextended);
                if (null != resolved) {
                    _cache[name] = resolved;
                }
                return resolved;
            }
        }

        private bool GetByCache(string name, bool forced, out IWebFileRecord record)
        {
            foreach (var mask in caches)
            {
                if (name.StartsWith(mask.Key))
                {
                    var resolvedName = name.Substring(mask.Key.Length);
                    var file = mask.Value.Resolve(resolvedName,forced);
                    if (null == file) {
                        record = _cache[name] = null;
                    }
                    else {
                        record = _cache[name] = new FileSystemWebFileRecord {
                            Name = name,
                            FileSystemName = file,
                            FullName = file
                        };
                    }
                        
                    return true;
                    
                }
            }
            record = null;
            return false;
        }

        private bool GetByMask(string name, out IWebFileRecord record) {
            foreach (var mask in masks) {
                if (name.StartsWith(mask.Key)) {
                    var resolvedName = name.Substring(mask.Key.Length);
                    var file = Path.Combine(mask.Value, resolvedName);
                    
                        record = _cache[name] =
                            File.Exists(file)
                                ? new FileSystemWebFileRecord {
                                    Name = name,
                                    FileSystemName = file,
                                    FullName = file
                                }
                                : null;
                        return true;
                    
                }
            }
            record = null;
            return false;
        }

        /// <summary>
        ///     Сброс кэша
        /// </summary>
        public void DropCache() {
            _cache.Clear();
        }

        /// <summary>
        ///     Устанавливает корневую директорю для части юрлов
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="rootdirectory"></param>
        public void SetRoot(string mask, string rootdirectory) {
            masks[mask] = rootdirectory;
        }

        /// <summary>
        ///     Установить источник в виде кэша, завязанного на другие источники
        /// </summary>
        /// <param name="key"></param>
        /// <param name="config"></param>
        public void SetCachedRoot(string key, XElement config) {
            caches[key] = new FileCacheResolver(config);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="withextensions"></param>
        /// <returns></returns>
        private IWebFileRecord Resolve(string name, object context, bool withextensions) {
            return Resolver.Find(name);
        }
    }
}