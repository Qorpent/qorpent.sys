﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp{
	/// <summary>
	///     Запись об экспортной
	/// </summary>
	public class ExportRecord{
		/// <summary>
		/// </summary>
		public IBSharpClass cls;

		/// <summary>
		/// </summary>
		public IList<string> names;

		/// <summary>
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="e"></param>
		public ExportRecord(IBSharpClass cls, XElement e){
			this.cls = cls;
			names = new List<string>();
			IEnumerable<XElement> uses = e.Elements("use");
			foreach (XElement u in uses){
				names.Add(u.Attr("code"));
			}
			if (names.Count == 0){
				names.Add("item");
			}
		}

		/// <summary>
		///     Разрешает элемент словаря
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		public string Resolve(string element){
			foreach (string n in names){
				XElement el = cls.Compiled.Descendants(n).FirstOrDefault(_ => element == _.Attr("code"));
				if (null != el){
					string val = el.Value;
					if (string.IsNullOrWhiteSpace(val)){
						val = el.Attr("name");
					}
					return val + "|" + cls.FullName + ":" + n + ":" + element;
				}
			}
			return null;
		}
	}
}