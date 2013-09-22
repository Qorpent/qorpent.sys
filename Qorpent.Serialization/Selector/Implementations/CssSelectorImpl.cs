using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.IoC;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Selector.Implementations {
	/// <summary>
	///     Селектор для поддержки кросс-JS-css нотации селекторов
	/// Поддерживает базовый CSS синтаксис без наворотов - тег, ид, класс, аттрибут
	/// разделенные запятыми
	/// то есть поддерживается:
	/// div,td
	/// .div1,.td1
	/// #div-main[type],.td1,p[x=2]
	/// p#id1,div.cls1,div#id2.cls3[u~=x]
	/// </summary>
	/// <remarks>
	/// По сути Unified - это ограниченный вариант CSS, то есть 100% обратно совместима с CSS
	/// </remarks>
	[ContainerComponent(Lifestyle = Lifestyle.Transient, Name = "selector.css", ServiceType = typeof(ISelectorImpl))]
	public class CssSelectorImpl : ISelectorImpl {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="root"></param>
        /// <param name="query"></param>
        /// <returns></returns>
		public IEnumerable<XElement> Select(XElement root, string query) {
	        var xpath = BuildXpath(query);
	        return root.XPathSelectElements(xpath);
        }
		/// <summary>
		/// Конвертирует строку селектора в XPATH
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public string BuildXpath(string query) {
			// выделяем отдельные условия к элементу
			var conditions = query.Split(','); 

			// превращаем каждое условие в выражение XPATH
			var parsedconditions = conditions.Select(BuildConditionXpath);

			// cоединяем все условия в единый XPath относительно произвольного
			// элемента вниз от текущего по иерархии
			var result =  string.Join(" | ", parsedconditions) ;

			return result;
		}
		private string BuildConditionXpath(string condition) {
			var subpaths = condition.SmartSplit(false, true, ' ');
			var xsubpaths = subpaths.Select(BuildSubpath);
			var result = "." + string.Join("",xsubpaths);
			return result;
		}

		private static string BuildSubpath(string condition) {
			var match = Regex.Match(condition.Trim(), 
					@"(?ix)^
						(?<tag>[^\.\#].*?)?
						(\#(?<id>.+?))?
						(\.(?<class>.+))?
						(\[\s*(?<attr>\w+)\s*(?<attrop>(~=)|(=))?\s*(?<attrval>.+?)?\])?$");
			var tag = match.Groups["tag"].Value;
			var id = match.Groups["id"].Value;
			var cls = match.Groups["class"].Value;
			var attr = match.Groups["attr"].Value;
			var attrop = match.Groups["attrop"].Value;
			var attrval = match.Groups["attrval"].Value;
			var subconditions = new List<string>();
			
			if (string.IsNullOrWhiteSpace(tag)) {
				tag = "*";
			}
			if (!string.IsNullOrWhiteSpace(id)) {
				subconditions.Add("@id='" + id + "'");
			}
			if (!string.IsNullOrWhiteSpace(cls)) {
			    var clause = "";
                foreach (var className in cls.Split('.')) {
                    clause += "contains(concat(' ',@class,' '), ' " + className + " ') and ";
			    }
                clause = clause.Substring(0, clause.Length - 5);
                subconditions.Add(clause);
			}

			if (!string.IsNullOrWhiteSpace(attr)) {
				if (string.IsNullOrWhiteSpace(attrop)) {
					subconditions.Add("@" + attr);
				}
				else {
					if (attrop == "=") {
						subconditions.Add("@" + attr + "='" + attrval.Trim() + "'");
					}else if (attrop == "~=") {
						subconditions.Add("contains(concat(' ',@" + attr + ",' '), ' " + attrval.Trim() + " ')");
					}
				}
			}

			var result = "//" + tag;
			if (0 != subconditions.Count) {
				result += "[" + string.Join(" and ", subconditions) + "]";
			}
			return result;
		}
	}
}