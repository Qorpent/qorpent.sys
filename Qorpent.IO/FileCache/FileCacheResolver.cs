using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.IO.Tests.FileCache;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.FileCache {
    /// <summary>
    /// Возвращает полный путь к файлу по запросу или 0 при его отсуствии, работает с подключенными другими источниками файлов
    /// </summary>
    [ContainerComponent(Lifestyle = Lifestyle.Transient,ServiceType = typeof(IFileCacheResolver))]
    public class FileCacheResolver : IFileCacheResolver {
        private string _root;
        private readonly IList<IFileCacheSource> _sources = new List<IFileCacheSource>();
        private IList<IFileFilter> _filters= new List<IFileFilter>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="sources"></param>
        public FileCacheResolver(string root, params IFileCacheSource[] sources) {
            Root = root;
            foreach (var source in sources) {
                Sources.Add(source);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="sources"></param>
        public FileCacheResolver(string root, params FileCacheSource[] sources)
        {
            Root = root;
            foreach (var source in sources)
            {
                Sources.Add(source);
            }
        }
        /// <summary>
        /// Создает кэшевой резольвер из конфигурации в формате XML
        /// </summary>
        /// <param name="config"></param>
        public FileCacheResolver(XElement config) {
            Root = EnvironmentInfo.ResolvePath( config.Attr("name"));
            Fallback = config.ResolveValue("fallback");
            foreach (var element in config.Elements("source")) {
                var source = new FileCacheSource(EnvironmentInfo.ResolvePath(element.Attr("code")));
                if (element.GetSmartValue("master").ToBool()) {
                    source.IsMaster = true;
                }
                Sources.Add(source);
            }
            foreach (var filter in config.Elements("filter")) {
                var t = filter.Attr("code");
                var type = Type.GetType(t);
                if(null==type)throw new Exception("cannot find filter "+t);
                var inst = Activator.CreateInstance(type) as IFileFilter;
                if(null==inst)throw new Exception("not IFileFilter "+t);
                this.Filters.Add(inst);
            }
        }

        /// <summary>
        /// Директория локального кэша
        /// </summary>
        public string Root {
            get { return _root; }
            set {
                _root = value;
                if (!string.IsNullOrWhiteSpace(_root) && !Directory.Exists(_root)) {
                    Directory.CreateDirectory(_root);
                }
            }
        }
        /// <summary>
        /// Источники данных
        /// </summary>
        public IList<IFileCacheSource> Sources {
            get { return _sources; }
        }
        /// <summary>
        /// "Главные" источники данных, используются в режиме принудительной синхронизации
        /// </summary>
        private IFileCacheSource[] Masters {
            get {
                if(Sources.Count==0)return new FileCacheSource[]{};
                var configured = Sources.Where(_ => _.IsMaster).ToArray();
                return configured;
            }
        }
        /// <summary>
        /// Имя файла, возвращаемого при возникновении несуществующего результата (замена null)
        /// </summary>
        public string Fallback { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IList<IFileFilter> Filters { get { return _filters; } }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Resolve(string name, bool forceUpdate = false) {
            var result =  InternalResolve(name, forceUpdate);
            if (null == result && !string.IsNullOrWhiteSpace(Fallback)) {
                return InternalResolve(Fallback,true);
            }
            return result;
        }

        private string InternalResolve(string name, bool forceUpdate) {
            var path = Path.Combine(Root, name);
            if (forceUpdate) {
                lock (this) {
                    foreach (var master in Masters) {
                        var streamer = master.GetStreamer(name);
                        if (null != streamer) {
                            using (var s = streamer()) {

                                Directory.CreateDirectory(Path.GetDirectoryName(path));
                                using (var fs = new FileStream(path, FileMode.Create)) {
                                    s.CopyTo(fs);
                                    fs.Flush();
                                }
                                foreach (var filter in Filters) {
                                    filter.Convert(path);
                                }
                                s.Close();
                            }

                            return path;

                        }
                    }
                }
            }
            if (File.Exists(path)) {
                return path;
            }
            lock (this) {
                foreach (var source in Sources)
                {
                    var streamer = source.GetStreamer(name);
                    if (null != streamer)
                    {
                        using (var s = streamer())
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(path));
                            using (var fs = new FileStream(path, FileMode.Create))
                            {
                                s.CopyTo(fs);
                                fs.Flush();
                            }
                            foreach (var filter in Filters)
                            {
                                filter.Convert(path);
                            }
                            s.Close();
                        }
                        return path;
                    }
                }
            }
           
            return null;
        }
    }
}