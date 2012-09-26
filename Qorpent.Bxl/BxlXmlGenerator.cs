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
// Original file : BxlXmlGenerator.cs
// Project: Qorpent.Bxl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Generates XElement representation of BxlToken list
	/// </summary>
	public class BxlXmlGenerator {
		/// <summary>
		/// 	Generates XElement from given tokens
		/// </summary>
		/// <param name="tokens"> BxlToken list </param>
		/// <param name="filename"> filename for xml lex info </param>
		/// <param name="lexdata"> true if must render lex info of elements (true by default) </param>
		/// <param name="safeanonymnames"> true to generate safe __ prefixed names for sys attributes (id,code,name), (false by default) </param>
		/// <param name="codeonlyattribute"> true to generate only code attribute instead of id,code pair (false by default) </param>
		/// <param name="extractsingle"> true - return only one Xelement of result set children </param>
		/// <returns> Xml representation of BxlToken flow </returns>
		/// <exception cref="BxlException"></exception>
		public XElement Generate(IEnumerable<BxlToken> tokens, string filename = "", bool lexdata = true,
		                         bool safeanonymnames = false, bool codeonlyattribute = false, bool extractsingle = false) {
			var ns = new Dictionary<string, string>();
			var rootns = "namespace::" + filename;
			ns["IMPLICIT"] = rootns + "_";

			var code = "code";
			var id = "id";
			var name = "name";
			if (safeanonymnames) {
				code = "__code";
				id = "__id";
				name = "__name";
			}
			var root = new XElement("root");
			var current = root;
			var level = 0;
			var anonymcount = 0;
			foreach (var t in tokens) {
				var tp = t.Type;
				if (BxlTokenType.Start == tp) {
					continue;
				}
				if (BxlTokenType.Element == tp) {
					var e = new XElement(t.GetAdaptedName(ns));
					if (e.Name.Namespace != null && ns.ContainsValue(e.Name.Namespace.ToString())) {
						if (null ==
						    root.Attributes().FirstOrDefault(x => x.Name.Namespace == XNamespace.Xmlns && x.Value == e.Name.Namespace)) {
							root.Add(new XAttribute(
								         XNamespace.Xmlns + ns.First(x => x.Key != "IMPLICIT" && x.Value == e.Name.Namespace).Key,
								         e.Name.Namespace));
						}
					}
					if (lexdata) {
						if (!filename.IsEmpty()) {
							e.SetAttributeValue("_file", filename);
						}
						e.SetAttributeValue("_line", t.LexInfo.Line);
					}
					anonymcount = 0;
					var l = 0;
					if (t.Prev.Type == BxlTokenType.Level) {
						l = (int) t.Prev.Number;
					}
					if (l == level + 1 || current == root) {
						current.Add(e);
					}
					else if (l == level) {
						current.Parent.Add(e);
					}
					else if (l < level) {
						for (var i = 0; i < level - l; i++) {
							current = current.Parent;
						}
						current.Parent.Add(e);
					}
					else {
						throw new BxlException("invalid element levelshift " + t.LexInfo, t.LexInfo);
					}
					level = l;
					current = e;
					continue;
				}
				if (BxlTokenType.Value == tp) {
					current.Value = t.Value;
					continue;
				}
				if (BxlTokenType.Attribute == tp) {
					var l = 0;
					if (t.Prev.Type == BxlTokenType.Level) {
						l = (int) t.Prev.Number;
					}
					else {
						if (t.Prev.Type != BxlTokenType.NewLine && t.Prev.Type != BxlTokenType.Start) {
							l = -1;
						}
					}

					if (l > level || l == -1) {
						current.SetAttributeValue(t.GetAdaptedName(ns), t.AttrValue);
					}
					else if (l == 0) {
						//level 0 means as namespace declaration if no elements above
						if (current.Parent == null) //root
						{
							ns[t.Name] = t.AttrValue;
							current.Add(new XAttribute(XNamespace.Xmlns + t.Name, t.AttrValue));
						}
						else {
							throw new BxlException("insuficient attribute level 0" + t.LexInfo, t.LexInfo);
						}
					}
					else if (l == level) {
						current.Parent.SetAttributeValue(t.GetAdaptedName(ns), t.AttrValue);
					}
					else if (l < level && l != 0) {
						for (var i = l; i <= level; i++) {
							if (current.Parent == null) {
								throw new BxlException("insuficient attribute level" + t.LexInfo, t.LexInfo);
							}
							current = current.Parent;
						}
						level = l - 1;
						current.SetAttributeValue(t.GetAdaptedName(ns), t.AttrValue);
					}

					continue;
				}
				if (BxlTokenType.String == tp || BxlTokenType.Expression == tp) {
					if (anonymcount > 2) {
						anonymcount++;
						current.SetAttributeValue("_aa" + anonymcount, t.Value);
					}
					if (anonymcount == 0) {
						anonymcount++;
						anonymcount++;
						current.SetAttributeValue(code, t.Value);
						if (!codeonlyattribute) {
							current.SetAttributeValue(id, t.Value);
						}
					}
					else if (anonymcount == 2) {
						anonymcount++;
						current.SetAttributeValue(name, t.Value);
					}
					continue;
				}
				if (BxlTokenType.Literal == tp) {
					if (anonymcount == 0) {
						anonymcount++;
						anonymcount++;
						current.SetAttributeValue(code, t.Value);
						if (!codeonlyattribute) {
							current.SetAttributeValue(id, t.Value);
						}
					}
					else if (anonymcount == 2) {
						anonymcount++;
						current.SetAttributeValue(name, t.Value);
					}
					else {
						anonymcount++;
						current.SetAttributeValue(t.Value.ConvertToXNameCompatibleString(), "1");
					}
				}
			}
			if (extractsingle) {
				if (root.Elements().Count() == 1) {
					root = root.Elements().First();
				}
			}
			return root;
		}
	}
}