using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.IoC;
using Qorpent.Serialization;

namespace Qorpent.Selector.Implementations {
	/// <summary>
	///     Селектор для поддержки REGEX нотации селекторов
	/// </summary>
	[ContainerComponent(Lifestyle = Lifestyle.Transient, Name = "selector.regex", ServiceType = typeof(ISelectorImpl))]
	public class RegexSelectorImpl : ISelectorImpl {
		/// <summary>
		/// Ищет в XML элементы по регулярному выражению 
		/// (по умолчанию проверяет только текстовые элементы, содержащиеся прямо в данном элементе)
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public IEnumerable<XElement> Select(XElement root, string query) {
			//проверяем расширенные опции (указываются в конце селектора при помощи комментария,выделяемого двойной решеткой)
			return Select(root, new RegexSelectorQuery(query));
		}

		/// <summary>
		/// Поиск по подготовленному регекс-запросу с парамтерами
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public IEnumerable<XElement> Select(XElement root, RegexSelectorQuery query) {
			if (IsMatch(root,query)) {
				yield return root;
			}
			foreach (var child in root.Elements()) {
				foreach (var childresult in Select(child, query)) {
					yield return childresult;
				}
			}
		}
		/// <summary>
		/// Проверяет соответствие элемента регекс - запросу
		/// </summary>
		/// <param name="root"></param>
		/// <param name="query"></param>
		/// <returns></returns>
		public bool IsMatch(XElement root, RegexSelectorQuery query) {
			if (null != query.TagFilter && 0 != query.TagFilter.Length) {
				if (query.UseTagFilterAsExclude) {
					if (-1 != Array.IndexOf(query.TagFilter, root.Name.LocalName)) {
						return false;
					}
				}
				else {
					if (-1 == Array.IndexOf(query.TagFilter, root.Name.LocalName)) {
						return false;

					}
				}
			}
			if (query.FindInFullString) {
				if (query.Regex.IsMatch(root.ToString())) return true;
			}
			if (query.FindInAttributes) {
				if (root.Attributes().Any(a => query.Regex.IsMatch(a.Value))) {
					return true;
				}
			}
			if (query.FindInTextValues) {
				if (root.Nodes().OfType<XText>().Any(t => query.Regex.IsMatch(t.Value))) {
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Вспомогательный класс запроса поиска в XML при помощи регексов
		/// </summary>
		public class RegexSelectorQuery {
			/// <summary>
			/// Разделитель регекса на сам регекс и опции, для регекса это комментарий
			/// </summary>
			public const string OPTS_DELIMITER = "##";
			/// <summary>
			/// Количество опций до списка тегов
			/// </summary>
			public const int OPTS_COUNT = 3;
			/// <summary>
			/// Создает запрос на основе переданного регулярного выражения
			/// </summary>
			/// <param name="regex"></param>
			public RegexSelectorQuery(string regex) {
				Regex = new Regex( regex,RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace);
				FindInTextValues = true;
				//проверяет, нет ли в составе регекса специального комментария с доп. опциями
				var optidx = regex.IndexOf(OPTS_DELIMITER);
				if (-1 == optidx) return;


				var options = regex.Substring(optidx + OPTS_DELIMITER.Length, OPTS_COUNT);
				//опции представляют собой строку из 3-х + и - где
				//первый параметр - поиск в значенияз, второй -атрибуты, третий - полный текст
				//по умолчанию - ##+-- , поиск всюду ##+++, поиск в атрибутах только ##-+-, поиск прямо по XML ##--+
				FindInTextValues = options[0] == '+';
				FindInAttributes = options[1] == '+';
				FindInFullString = options[2] == '+';

				//пытаемся считать фильтр тегов - если написано div,td, значит
				//вклчюая теги dev,td, если написано !div,td значит - НЕ div,td
				var tagfilters = regex.Substring(optidx + OPTS_DELIMITER.Length + OPTS_COUNT);
				
				if (!string.IsNullOrWhiteSpace(tagfilters)) {
					if (tagfilters[0] == '!') {
						this.UseTagFilterAsExclude = true;
						tagfilters = tagfilters.Substring(1);
					}
					this.TagFilter = tagfilters.Split(',');
				}

			}
			/// <summary>
			/// Указывает, что список тегов обозначает "исключение"
			/// </summary>
			public bool UseTagFilterAsExclude { get; set; }

			/// <summary>
			/// Регулярное выражение
			/// </summary>
			public Regex Regex { get; set; }
			/// <summary>
			/// Осуществлять поиск в атрибутах (по умолчанию нет)
			/// </summary>
			public bool FindInAttributes { get; set; }
			/// <summary>
			/// Осуществлять поиск в тексте (непосредственном) значениях (по умолчанию да)
			/// </summary>
			public bool FindInTextValues { get; set; }
			/// <summary>
			/// Искать в полном описании элемента
			/// </summary>
			public bool FindInFullString { get; set; }

			/// <summary>
			/// Набор допустимых тегов
			/// </summary>
			public string[] TagFilter { get; set; }

		}
	
	}
}