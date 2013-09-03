using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Selector.Implementations;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Selector {
	/// <summary>
	///     Реализация селектора по умолчанию, осуществляет резолюцию имплементаций селекторов для разных диалектов
	/// </summary>
	[ContainerComponent(Lifestyle = Lifestyle.Transient, Name = "selector.default",ServiceType = typeof (ISelector))]
	public class DefaultSelector : ServiceBase, ISelector {
		/// <summary>
		///     Реализация селестора  для SelectorLanguage.XPath
		/// </summary>
		[Inject(Name = "selector.xpath", FactoryType = typeof (SelectorFactory))]
		public ISelectorImpl XPathSelector { get; protected set; }

		/// <summary>
		///     Реализация селестора  для SelectorLanguage.Regex
		/// </summary>
		[Inject(Name = "selector.regex", FactoryType = typeof (SelectorFactory))]
		public ISelectorImpl RegexSelector { get; protected set; }

		/// <summary>
		///     Реализация селестора  для SelectorLanguage.CSS
		/// </summary>
		[Inject(Name = "selector.css", FactoryType = typeof (SelectorFactory))]
		public ISelectorImpl CssSelector { get; protected set; }

		/// <summary>
		///     Пользовательские расширения для сериализаторов
		/// </summary>
		[Inject]
		public ICustomSelectorImpl[] CustomSelectors { get; protected set; }
		/// <summary>
		/// 
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		public IEnumerable<XElement> Select(XElement root, string query, SelectorLanguage language = SelectorLanguage.Auto) {
			if (null == root) return null;
			if (string.IsNullOrWhiteSpace(query)) return null;
			//поддержка запросов сразу на нескольких языках
			IList<string> subqueries = query.SmartSplit(false, true, '%');
			//если у нас один запрос (основной случай) то возвращаем один набор (уникальность на обработчике) с указанным языком
			if (subqueries.Count == 1) {
				return SelectSingleQuery(root, subqueries[0], language);
			}
			//иначе возвращаем полный набор с автоопределением языка
			return subqueries.SelectMany(_ => SelectSingleQuery(root, _, SelectorLanguage.Auto)).Distinct();
		}

		/// <summary>
		///     Метод обработки одного запроса селектора
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		protected virtual IEnumerable<XElement> SelectSingleQuery(XElement root, string query, SelectorLanguage language) {
			SelectorLanguage reallanguage = ResolveLanguage(query, language);
			string realquery = Preprocess(query, reallanguage);
			ISelectorImpl executor = ResolveExecutor(realquery, reallanguage);
			if (null == executor) {
				throw new ProcessingSelectorException("не найдена реализация языка", 10, reallanguage, realquery);
			}
			return executor.Select(root, realquery);
		}

		/// <summary>
		///     Возвращает класс, действительно выполняющий поиск элементов
		/// </summary>
		/// <param name="realquery"></param>
		/// <param name="reallanguage"></param>
		/// <returns></returns>
		protected virtual ISelectorImpl ResolveExecutor(string realquery, SelectorLanguage reallanguage) {
			switch (reallanguage) {
				case SelectorLanguage.XPath:
					return XPathSelector ?? (XPathSelector = new XPathSelectorImpl());
				case SelectorLanguage.Regex:
					return RegexSelector ?? (RegexSelector = new RegexSelectorImpl());
				case SelectorLanguage.Css:
					return CssSelector ?? (CssSelector = new CssSelectorImpl());
				case SelectorLanguage.Custom:
					if (null == CustomSelectors) return null;
					return CustomSelectors.FirstOrDefault(_ => _.IsSupported(realquery));
				default:
					return null;
			}
		}

		/// <summary>
		///     Выполняет пред-обработку запроса перед выполнением
		/// </summary>
		/// <param name="query"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		protected virtual string Preprocess(string query, SelectorLanguage language) {
			//на данный момент логики препроцессинга не задано, прозрачно возвращаем
			return query;
		}

		/// <summary>
		///     Определяет реальный используемый диалект запроса
		/// </summary>
		/// <param name="query"></param>
		/// <param name="language"></param>
		/// <returns></returns>
		protected virtual SelectorLanguage ResolveLanguage(string query, SelectorLanguage language) {
			//если язык указан явно - возвращаем его
			if (language != SelectorLanguage.Auto) {
				return language;
			}
			if (query.Contains("/")) return SelectorLanguage.XPath;
			if (query.Contains("(") || query.Contains("?") || query.Contains("##")) return SelectorLanguage.Regex;
			//теперь на CSS проверятся только дополнительные неподдерживаемые Unified конструкции
			return SelectorLanguage.Css;
		}
	}
}