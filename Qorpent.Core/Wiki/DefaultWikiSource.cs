using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.IoC;

namespace Qorpent.Wiki {
	/// <summary>
	/// Источник Wiki по умолчанию, работает с персистером
	/// </summary>
	[ContainerComponent(Lifestyle.Transient,Name = "wiki.source.default",ServiceType = typeof(IWikiSource))]
	public class DefaultWikiSource :ServiceBase, IWikiSource {
		/// <summary>
		/// Версия для 304
		/// </summary>
		public const int Version = 1;

		/// <summary>
		/// Играет роль хэш-функции для определенного типа.
		/// </summary>
		/// <returns>
		/// Хэш-код для текущего объекта <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode() {
			return Version;
		}
		/// <summary>
		/// Компонент, реализующий физическое хранение вики
		/// </summary>
		[Inject]public IWikiPersister Persister { get; set; }

		/// <summary>
		/// Фильтры возврата страниц
		/// </summary>
		[Inject]public IWikiGetFilter[] WikiGetFilters { get; set; }
		/// <summary>
		/// Фильтры возврата страниц
		/// </summary>
		[Inject]
		public IWikiSaveFilter[] WikiSaveFilters { get; set; }
		/// <summary>
		/// Фильтры возврата страниц
		/// </summary>
		[Inject]
		public IWikiEmptyFilter[] WikiEmptyFilters { get; set; }

		/// <summary>
		/// Получает страницы из хранилища, фильтрует перед выдачей и формирует шаблон в случае пустой страницы
		/// </summary>
		/// <param name="usage">Варинат использования</param>
		/// <param name="codes"></param>
		/// <returns></returns>
		public IEnumerable<WikiPage> Get(string usage,params string[] codes) {
			CheckPersister();
			var dict = codes.ToDictionary(_ => _, _ => new WikiPage { Code = _ });
			foreach (var page in Persister.Get(codes)) {
				page.Existed = true;
				dict[page.Code] = page;
			}
			foreach (var wikiPage in dict.Values) {
				if (!wikiPage.Existed) {
					if (null != WikiEmptyFilters && 0 != WikiEmptyFilters.Length) {
						foreach (var emptyFilter in WikiEmptyFilters) {
							emptyFilter.Execute(wikiPage);
						}
					}	
				}
				if (null != WikiGetFilters && 0 != WikiGetFilters.Length) {
					foreach (var getFilter in WikiGetFilters)
					{
						getFilter.Execute(wikiPage,usage);
					}
				}
				yield return wikiPage;
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public object CreateVersion(string code, string comment) {
            return Persister.CreateVersion(code, comment);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public object RestoreVersion(string code, string version) {
            return Persister.RestoreVersion(code, version);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public WikiPage GetWikiPageByVersion(string code, string version) {
            return Persister.GetWikiPageByVersion(code, version);
        }

        /// <summary>
        ///     Возвращает список версий и первичную информацию о документе по коду
        /// </summary>
        /// <param name="code">Wiki page code</param>
        /// <returns></returns>
        public IEnumerable<object> GetVersionsList(string code) {
            return Persister.GetVersionsList(code);
        }

		private void CheckPersister() {
			if (null == Persister) {
				throw new Exception("no persister given");
			}
		}
		/// <summary>
		/// Проверяет наличие страниц с указанными кодами в хранилище
		/// </summary>
		/// <param name="codes"></param>
		/// <returns></returns>
		public IEnumerable<WikiPage> Exists(params string[] codes) {
			CheckPersister();
			return Persister.Exists(codes);
		}
		/// <summary>
		/// Производит сохранение страницы в хранилище с предварительной фильтрацией
		/// </summary>
		/// <param name="pages"></param>
		public bool Save(params WikiPage[] pages) {
			CheckPersister();
			foreach (var wikiPage in pages) {
				if (null != WikiSaveFilters && 0 != WikiSaveFilters.Length)
				{
					foreach (var emptyFilter in WikiEmptyFilters)
					{
						emptyFilter.Execute(wikiPage);
					}
				}	
			}
			return Persister.Save(pages);
		}

		/// <summary>
		/// Сохраняет в Wiki файл с указанным кодом
		/// </summary>
		public void SaveBinary(WikiBinary binary) {
			CheckPersister();
			Persister.SaveBinary(binary);
		}

		/// <summary>
		/// Загружает бинарный контент
		/// </summary>
		/// <param name="code"></param>
		/// <param name="withData">Флаг, что требуется подгрузка бинарных данных</param>
		/// <returns></returns>
		public WikiBinary LoadBinary(string code, bool withData = true) {
			CheckPersister();
			return Persister.LoadBinary(code, withData);
		}

        /// <summary>
        ///     Установить блокировку
        /// </summary>
        /// <param name="code">Код страницы</param>
        /// <returns>Результат операции</returns>
        public bool GetLock(string code) {
            return Persister.GetLock(code);
        }

        /// <summary>
        ///     Снять блокировку
        /// </summary>
        /// <param name="code">код страницы</param>
        public bool Releaselock(string code) {
            return Persister.ReleaseLock(code);
        }


		/// <summary>
		/// Поиск объектов Wiki
		/// </summary>
		/// <param name="search"></param>
		/// <param name="count"></param>
		/// <param name="types"></param>
		/// <param name="start"></param>
		/// <returns></returns>
		public IEnumerable<WikiObjectDescriptor> Find(string search, int start=-1, int count =-1, WikiObjectType types = WikiObjectType.All) {
			CheckPersister();
			int currentIdx = 0;

			IEnumerable<WikiPage> pageselector =types.HasFlag(WikiObjectType.Page)?Persister.FindPages(search):  null;
			IEnumerable<WikiBinary> binselector = types.HasFlag(WikiObjectType.File) ? Persister.FindBinaries(search) : null;
			IEnumerator<WikiPage> pageenum = pageselector != null ? pageselector.GetEnumerator() : null;
			IEnumerator<WikiBinary> fileenum = pageselector != null ? binselector.GetEnumerator() : null;
			bool haspages = pageenum == null ? false : true;
			bool hasbins = fileenum == null ? false : true;
			while (haspages||hasbins) {
				if (start != -1 && count != -1)
				{
					if (currentIdx >= (start + count))
					{
						break;
					}
				}
				if (haspages) {
					haspages = pageenum.MoveNext();
					if (haspages) {

						if (-1 == start || currentIdx >= start) {
							yield return new WikiObjectDescriptor(pageenum.Current);
						}
						currentIdx++;
						continue;
					}
				}
				

				if (hasbins)
				{
					hasbins = fileenum.MoveNext();
					if (hasbins)
					{

						if (-1 == start || currentIdx >= start)
						{
							yield return new WikiObjectDescriptor(fileenum.Current);
						}
						currentIdx++;
						continue;
					}
				}

			}
			
		}

		/// <summary>
		/// Возвращает версию объекта
		/// </summary>
		/// <param name="code"></param>
		/// <param name="objectType"></param>
		/// <returns></returns>
		public DateTime GetVersion(string code, WikiObjectType objectType) {
			CheckPersister();
			if (objectType == WikiObjectType.File) {
				return Persister.GetBinaryVersion(code);
			}
			return Persister.GetPageVersion(code);
		}
	}
}