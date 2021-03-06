﻿using System.Collections.Generic;
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
	        var result = root.XPathSelectElements(xpath);
            return result;
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
			var sp = subpaths.ToArray();
			subpaths = new List<string>();
			foreach (var s in sp){
				var sps = s.SmartSplit(false, true, '+');
				for (var i = 0; i < sps.Count; i++){
					var _s = sps[i];
					if (i != 0){
						_s = "+" + _s;
					}
					subpaths.Add(_s);
				}
			}
			var xsubpaths = subpaths.Select(BuildSubpath);
			var result = "." + string.Join("",xsubpaths);
			return result;
		}

		private static string BuildSubpath(string condition){
			bool onelevel = condition.StartsWith("+");
			if (onelevel){
				condition = condition.Substring(1);
			}
		    var sc = condition.Split(':');

			var match = Regex.Match(sc[0].Trim(), 
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
		    if (tag!="*") {
		        subconditions.Insert(0,"local-name()='"+tag+"'");
		    }
			var result = (onelevel?"/":"//") + "*";
			if (0 != subconditions.Count) {
				result += "[" + string.Join(" and ", subconditions) + "]";
			}

            if (sc.Count() == 2) {
                if (sc[1].Contains("first-child")) {
                    result += "[1]";
                }

                if (sc[1].Contains("last-child")) {
                    result += "[last()]";
                }

                if (sc[1].Contains("nth-child")) {
                    var openBracketIndex = sc[1].IndexOf("(", System.StringComparison.Ordinal);
                    var closeBracketIndex = sc[1].IndexOf(")", System.StringComparison.Ordinal);
                    var nth = sc[1].Substring(
                        openBracketIndex + 1,
                        closeBracketIndex - openBracketIndex - 1
                    );

                    result += "[" + nth + "]";
                }
            }

			return result;
		}
	}
}