using System;
using System.IO;
using Qorpent.IoC;
using Qorpent.IO.Net;

namespace Qorpent.IO.Tests.FileCache {
    /// <summary>
    /// Кэш резолюции файлов
    /// </summary>
    [ContainerComponent(Lifestyle = Lifestyle.Transient,ServiceType = typeof(IFileCacheSource))]
    public class FileCacheSource : IFileCacheSource {

        /// <summary>
        /// 
        /// </summary>
        public FileCacheSource() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlOrPath"></param>
        public FileCacheSource(string urlOrPath) {
            Root = urlOrPath;
            if (urlOrPath.StartsWith("http")) {
                IsWeb = true;
            }
        }

        /// <summary>
        /// Корневая директория или URL
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// Признак "главного" источника
        /// </summary>
        public bool IsMaster { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsWeb { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="urlOrPath"></param>
        /// <returns></returns>
        public static implicit operator FileCacheSource(string urlOrPath) {
            return new FileCacheSource(urlOrPath);
        }

        /// <summary>
        /// Возвращает NULL или источник потока при наличии запрошенного файла
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Func<Stream> GetStreamer(string name) {
            
            
            if (IsWeb) {
                return GetWebStreamer(name);
            }
            return GetFileStreamer(name);
        }

        private Func<Stream> GetWebStreamer(string name) {
            var uri = new Uri(new Uri(Root), name);
            try {
                var cli = new HttpClient();
                var resp = cli.Call(uri.ToString());
                if (resp.State == 200) {
                    return () => new MemoryStream(resp.Data);
                }
                return null;

            }
            catch {
                return null;
            }
        }

        private Func<Stream> GetFileStreamer(string name) {
            var path = Path.Combine(Root, name);
            if (File.Exists(path)) {
                return () => File.OpenRead(path);
            }
            return null;
        }
    }
}