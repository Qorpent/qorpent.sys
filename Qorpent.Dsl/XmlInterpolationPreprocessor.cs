#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : XmlInterpolationPreprocessor.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.IoC;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Замещает в исходном XML конструкции вида ${} специальным набором вложенных элементов и заместителей
	/// </summary>
	/// <isdefaultimpl cref="IXmlInterpolationPreprocessor" />
	[ContainerComponent(Lifestyle.Transient)]
	public class XmlInterpolationPreprocessor : IXmlInterpolationPreprocessor {
		/// <summary>
		/// 	Выполняет замены в указанном элементе
		/// </summary>
		/// <param name="xml"> </param>
		public void Execute(XElement xml) {
			process(xml);
		}


		private void process(XElement element) {
			foreach (var e in element.Elements()) {
				process(e);
			}
			foreach (var attr in element.Attributes()) {
				if (attr.Value.Contains("${")) {
					process(attr);
				}
			}
			var text = element.Nodes().OfType<XText>().FirstOrDefault();
			if (null != text && text.Value.Contains("${")) {
				process(text);
			}
		}

		private void process(XText e) {
			e.Value = process(e.Value, "_val_", e.Parent);
		}

		private void process(XAttribute attr) {
			attr.Value = process(attr.Value, attr.Name.LocalName, attr.Parent);
		}

		private string process(string val, string key, XElement e) {
			var replaces = new List<string>();
			val = Regex.Replace(val, @"\$\{[\s\S]+?\}", m =>
				{
					var r = m.Value.Substring(2, m.Value.Length - 3);
					int idx;
					if (replaces.Contains(r)) {
						idx = replaces.IndexOf(r);
					}
					else {
						replaces.Add(r);
						idx = replaces.Count - 1;
					}
					return "{" + idx + "}";
				});
			foreach (var r in replaces) {
				e.Add(new XElement("_p_", new XAttribute("k", key), r));
			}

			return val;
		}
	}
}