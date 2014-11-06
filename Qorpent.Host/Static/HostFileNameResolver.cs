using System.Collections.Generic;
using System.Linq;
using Qorpent.Events;
using Qorpent.IO;

namespace Qorpent.Host.Static {
    /// <summary>
    /// Адаптер поиска файлов для хоста
    /// </summary>
    public class HostFileNameResolver : IFileNameResolver,IResetable {
        private HostConfig _cfg;
        private IList<IFileNameResolver> _resolvers = new List<IFileNameResolver>();
        /// <summary>
        /// 
        /// </summary>
        public HostFileNameResolver() {
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        public HostFileNameResolver(IHostServer server) {
            _cfg = server.Config;
            _resolvers.Add(new FileNameResolver(){Root = _cfg.RootFolder});
            foreach (var contentFolder in _cfg.ContentFolders) {
                _resolvers.Add(new FileNameResolver{Root =contentFolder});
            }
            foreach (var extendedContentFolder in _cfg.ExtendedContentFolders) {
                _resolvers.Add(new FileNameResolver { Root = extendedContentFolder });
            }
        }

        /// <summary>
        /// 	Own root of fileresolver
        /// </summary>
        public string Root { get { return _cfg.RootFolder; } set {} }
        /// <summary>
        /// Определение наиболее подходящего файла
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string Resolve(FileSearchQuery query) {
            return _resolvers.Select(_ => _.Resolve(query)).FirstOrDefault(_ => !string.IsNullOrWhiteSpace(_));
        }
        /// <summary>
        /// Возвращает все подходящие файлы
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public string[] ResolveAll(FileSearchQuery query) {
            return _resolvers.SelectMany(_ => _.ResolveAll(query)).Distinct().ToArray();
        }

        /// <summary>
        /// 	Clears file resolution cache
        /// </summary>
        public void ClearCache() {
            foreach (var resolver in _resolvers) {
                resolver.ClearCache();
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
        public object Reset(ResetEventData data) {
            return _resolvers.OfType<IResetable>().Select(_ => _.Reset(data)).ToArray();
        }

        /// <summary>
        /// 	Возващает объект, описывающий состояние до очистки
        /// </summary>
        /// <returns> </returns>
        public object GetPreResetInfo() {
            return null;
        }
    }
}