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
		/// <param name="codes"></param>
		/// <returns></returns>
		public IEnumerable<WikiPage> Get(params string[] codes) {
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
						getFilter.Execute(wikiPage);
					}
				}
				yield return wikiPage;
			}
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
		public void Save(params WikiPage[] pages) {
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
			Persister.Save(pages);
		}
	}
}