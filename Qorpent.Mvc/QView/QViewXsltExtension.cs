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
// Original file : QViewXsltExtension.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc.QView {
	// ReSharper disable UnusedMember.Global
	// ReSharper disable InconsistentNaming
	// ReSharper disable MemberCanBePrivate.Global
	/// <summary>
	/// </summary>
	public class QViewXsltExtension {
		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <returns> </returns>
		public string open_tag(XPathNavigator nav) {
			var tname = nav.Name;
			if (tname.StartsWith("_")) {
				return "";
			}
			if (tname.EndsWith("_")) {
				tname = tname.Substring(0, tname.Length - 1);
			}
			return "\r\n\t\twrite(\"<" + tname + ">\");";
		}


		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <returns> </returns>
		public string start_tag_head(XPathNavigator nav) {
			var tname = nav.Name;
			if (tname.StartsWith("_")) {
				return "";
			}
			if (tname.EndsWith("_")) {
				tname = tname.Substring(0, tname.Length - 1);
			}
			return "\r\n\t\twrite(\"<" + tname + "\");";
		}

		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <returns> </returns>
		public string end_tag_head(XPathNavigator nav) {
			var tname = nav.Name;
			if (tname.StartsWith("_")) {
				return "";
			}
			if (tname.EndsWith("_")) {
				tname = tname.Substring(0, tname.Length - 1);
			}
			if (tname.ToLower() == "img" || tname.ToLower() == "input") {
				return "\r\n\t\twrite(\"/>\");";
			}
			return "\r\n\t\twrite(\">\");";
		}

		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <param name="iter"> </param>
		/// <returns> </returns>
		public string tag_value(XPathNavigator nav, XPathNodeIterator iter) {
			return iter.Count == 0 ? outv(nav.Value) : outvf(nav.Value, iter);
		}

		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <param name="iter"> </param>
		/// <returns> </returns>
		public string attr_value(XPathNavigator nav, XPathNodeIterator iter) {
			return iter.Count == 0 ? outa(nav.Name, nav.Value) : outaf(nav.Name, nav.Value, iter);
		}

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		public string attr_value(string name, string value , XPathNodeIterator iter)
		{
			return iter.Count == 0 ? outa(name,value) : outaf(name, value, iter);
		}

		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <param name="iter"> </param>
		/// <returns> </returns>
		/// <exception cref="Exception"></exception>
		public string bool_attr(XPathNavigator nav, XPathNodeIterator iter) {
			if (iter.Count == 0) {
				return outba(nav.Name, nav.Value);
			}
			throw new Exception("complex strings with ${} not supported for bool attributes");
		}

		private string outaf(string name, string value, XPathNodeIterator iter) {
			var sb = new StringBuilder();

			sb.AppendFormat("\r\n\t\twritef(\" {0}='", name);

			var src = value;
			var m = Regex.Match(src, @"^(?<name>\w+)@(?<lang>\w+)?\[[\{\d\}]+\]$");
			if (m.Success) {
				src = string.Format("GetResource(\"{0}\",\"{1}\")", m.Groups["name"].Value, m.Groups["lang"].Value);
				src = "\"+" + src + "+\"";
			}
			else {
				src = esc(src).Replace("'", "&apos;");
			}


			sb.Append(src);
			sb.Append("'\"");
			while (iter.MoveNext()) {
				sb.Append(", ");
				if (iter.Current != null) {
					src = iter.Current.Value;
				}
				m = Regex.Match(src, @"^(?<name>\w+)@(?<lang>\w+)?$");
				if (m.Success) {
					src = string.Format("GetResource(\"{0}\",\"{1}\")", m.Groups["name"].Value, m.Groups["lang"].Value);
				}
				sb.Append(src);
			}
			sb.Append(");");
			return sb.ToString();
		}


		/// <summary>
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		/// <exception cref="Exception"></exception>
		public string outba(string name, string value) {
			var sb = new StringBuilder();
			if (!isliteral(value)) {
				sb.Append("\r\nif(asbool(\"" + esc(value) + "\")){");
				sb.AppendFormat("\r\n\t\twrite(\" {0}='", name);
				sb.Append(esc(value).Replace("'", "&apos;"));
				sb.Append("'\");");
				sb.Append("\r\n}");
			}
			else if (value.StartsWith("@@")) {
				sb.Append("\r\nif(asbool(" + value.Substring(2) + ")){");
				sb.Append("\r\n\t\twritef(\" " + name + "='{0}'\"," + value.Substring(2) + ");");
				sb.Append("\r\n}");
			}
			else if (value.StartsWith("@")) {
				sb.Append("\r\nif(asbool(" + value.Substring(1) + ")){");
				sb.AppendFormat("\r\n\t\twrite(\" {0}='", name);
				sb.Append(esc(value.Substring(1)).Replace("'", "&apos;"));
				sb.Append("'\");");
				sb.Append("\r\n}");
			}
			else if (value.EndsWith("@")) {
				throw new Exception("resources not supported on bool attributes");
			}
			else {
				sb.Append("\r\nif(asbool(" + value + ")){");
				sb.Append("\r\n\t\twritef(\" " + name + "='{0}'\",esc(" + value + "));");
				sb.Append("\r\n}");
			}
			return sb.ToString();
		}

		/// <summary>
		/// </summary>
		/// <param name="condition"> </param>
		/// <returns> </returns>
		public string condition(string condition) {
#if USELECONDITION
			if (condition.StartsWith("^")) {
				return lecondition(condition.Substring(1));
			}
#endif
			var prefix = "";
			if (condition.StartsWith("!")) {
				prefix = "!";
				condition = condition.Substring(1);
			}
			return string.Format("({0}asbool({1}))", prefix, condition);
		}

#if USELECONDITION
		private string lecondition(string expr) {
			var sb = new StringBuilder();
			var parsed = new LogicalExpressionParser().Parse(expr);
			sb.Append("(asbool(");
			foreach (var node in parsed.Children) {
				writeCond(sb, node);
			}
			sb.Append("))");
			return sb.ToString();
		}

		private void writeCond(StringBuilder sb, LogicalExpressionNode node) {
			if (node.Negative) {
				sb.Append("!(");
			}
			if (node is LiteralNode) {
				sb.Append("asbool(" + ((LiteralNode) node).Literal + ")");
			}
			else if (node is ConjunctionNode) {
				var fst = true;
				foreach (var n in node.Children) {
					if (!fst) {
						sb.Append(" && ");
					}
					writeCond(sb, n);
					fst = false;
				}
			}
			else if (node is DisjunctionNode) {
				var fst = true;
				foreach (var n in node.Children) {
					if (!fst) {
						sb.Append(" || ");
					}
					writeCond(sb, n);
					fst = false;
				}
			}
			else if (node is EqualNode) {
				var eq = node as EqualNode;
				var fst = eq.FirstLiteral;
				var sec = eq.SecondLiteral;
				if (fst.StartsWith("$") || sec.StartsWith("$")) {
					fst = "asstr(" + fst.Replace("$", "") + ")";
					sec = "asstr(" + sec.Replace("$", "") + ")";
				}
				else {
					fst = "asbool(" + fst + ")";
					sec = "asbool(" + sec + ")";
				}
				sb.AppendFormat("{0} == {1}", fst, sec);
			}
			else if (node is EqualValueNode) {
				var ev = node as EqualValueNode;
				var fst = "asstr(" + ev.Literal + ")";
				var sec = "\"" + ev.Value + "\"";
				sb.AppendFormat("{0} == {1}", fst, sec);
			}

			if (node.Negative) {
				sb.Append(")");
			}
		}

#endif

		/// <summary>
		/// </summary>
		/// <param name="name"> </param>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public string outa(string name, string value) {
			var sb = new StringBuilder();
			if (!isliteral(value)) {
				sb.AppendFormat("\r\n\t\twrite(\" {0}='", name);
				sb.Append(esc(value).Replace("'", "&apos;"));
				sb.Append("'\");");
			}
			else if (value.StartsWith("@@")) {
				sb.Append("\r\n\t\twritef(\" " + name + "='{0}'\"," + value.Substring(2) + ");");
			}
			else if (value.StartsWith("@")) {
				sb.AppendFormat("\r\n\t\twrite(\" {0}='", name);
				sb.Append(esc(value.Substring(1)).Replace("'", "&apos;"));
				sb.Append("'\");");
			}
			else if (value.EndsWith("@")) {
				sb.Append("\r\n\t\twritef(\" " + name + "='{0}'\",GetResource(\"" + value.Substring(0, value.Length - 1) + "\"));");
			}
			else {
				sb.Append("\r\n\t\twritef(\" " + name + "='{0}'\",esc(" + value + "));");
			}
			return sb.ToString();
		}


		/// <summary>
		/// </summary>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public string esc(string value) {
			return value.Replace(@"\", @"\\").Replace("\"", "\\\"").Replace("\r", "\\r").Replace("\n", "\\n");
		}


		/// <summary>
		/// </summary>
		/// <param name="value"> </param>
		/// <returns> </returns>
		public string outv(string value) {
			var sb = new StringBuilder();
			if (!isliteral(value)) {
				sb.Append("\r\n\t\twrite(\"");
				sb.Append(esc(value));
				sb.Append("\");");
			}
			else if (value.StartsWith("@@")) {
				sb.Append("\r\n\t\twrite(");
				sb.Append(value.Substring(2));
				sb.Append(");");
			}
			else if (value.StartsWith("@")) {
				sb.Append("\r\n\t\twrite(\"");
				sb.Append(esc(value.Substring(1)));
				sb.Append("\");");
			}
			else if (value.EndsWith("@")) {
				sb.Append("\r\n\t\twrite(GetResource(\"");
				sb.Append(value.Substring(0, value.Length - 1));
				sb.Append("\"));");
			}
			else {
				sb.Append("\r\n\t\twrite(esc(");
				sb.Append(value);
				sb.Append("));");
			}
			return sb.ToString();
		}

		/// <summary>
		/// </summary>
		/// <param name="value"> </param>
		/// <param name="iter"> </param>
		/// <returns> </returns>
		public string outvf(string value, XPathNodeIterator iter) {
			var sb = new StringBuilder();
			sb.Append("\r\n\t\twritef(");
			//sb.Append("string.Format(");
			var src = value;
			var m = Regex.Match(src, @"^(?<name>\w+)@(?<lang>\w+)?\[[\{\d\}]+\]$");
			if (m.Success) {
				src = string.Format("GetResource(\"{0}\",\"{1}\")", m.Groups["name"].Value, m.Groups["lang"].Value);
			}
			else {
				src = "\"" + esc(src) + "\"";
			}
			sb.Append(src);
			while (iter.MoveNext()) {
				sb.Append(", ");
				if (iter.Current != null) {
					src = iter.Current.Value;
				}
				m = Regex.Match(src, @"^(?<name>\w+)@(?<lang>\w+)?$");
				if (m.Success) {
					src = string.Format("GetResource(\"{0}\",\"{1}\")", m.Groups["name"].Value, m.Groups["lang"].Value);
				}
				sb.Append(src);
			}
			sb.Append(");");
			return sb.ToString();
		}

		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <returns> </returns>
		public string end_tag(XPathNavigator nav) {
			var tname = nav.Name;
			if (tname.StartsWith("_")) {
				tname = tname.Substring(1);
			}
			if (tname.EndsWith("_")) {
				return "";
			}
			if (tname.ToLower() == "img" || tname.ToLower() == "input") {
				return "";
			}
			return "\r\n\t\twrite(\"</" + tname + ">\");";
		}

		/// <summary>
		/// </summary>
		/// <param name="str"> </param>
		/// <param name="liftmode"> </param>
		/// <returns> </returns>
		public string liftup(string str, int liftmode = 0) {
			if (!isliteral(str)) {
				return outsimple(str);
			}
			if (liftmode == 0) {
				if (str.StartsWith("@@")) {
					return str.Substring(2);
				}
				if (str.StartsWith("@")) {
					str = str.Substring(1);
					int i;
					decimal d;
					bool b;
					if (int.TryParse(str, out i) || decimal.TryParse(str, out d) || bool.TryParse(str, out b)) {
						return str;
					}

					return "\"" + str + "\"";
				}
				if (str.EndsWith("@")) {
					return "GetResource(\"" + str.Substring(0, str.Length - 1) + "\")";
				}
				return "esc(" + str + ")";
			}
			if (liftmode == 1) {
				if (str.StartsWith("@@")) {
					return str.Substring(2);
				}
				if (str.StartsWith("@")) {
					return outsimple(str.Substring(1));
				}
				if (str.EndsWith("@")) {
					return "GetResource(\"" + str.Substring(0, str.Length - 1) + "\")";
				}
				return str;
			}
			if (liftmode == 2) {
				if (str.StartsWith("@@")) {
					return str.Substring(2);
				}
				if (str.StartsWith("@")) {
					return str.Substring(1);
				}
				if (str.EndsWith("@")) {
					return "GetResource(\"" + str.Substring(0, str.Length - 1) + "\")";
				}
				return outsimple(str);
			}
			return outsimple(str);
		}

		/// <summary>
		/// </summary>
		/// <param name="str"> </param>
		/// <returns> </returns>
		public string outsimple(string str) {
			int i;
			decimal d;
			bool b;
			if (int.TryParse(str, out i) || decimal.TryParse(str, out d) || bool.TryParse(str, out b)) {
				return str;
			}

			return "\"" + esc(str) + "\"";
		}

		/// <summary>
		/// </summary>
		/// <param name="str"> </param>
		/// <returns> </returns>
		public bool isliteral(string str) {
			if (Regex.IsMatch(str, @"^@?@?[A-Za-z_][A-Za-z\d\._]*@?$")) {
				return true;
			}
			if (Regex.IsMatch(str, @"^\([\s\S]+\)$")) {
				return true;
			}
			return false;
		}

		/// <summary>
		/// </summary>
		/// <param name="str"> </param>
		/// <returns> </returns>
		public bool isescaped(string str) {
			return str.StartsWith("@");
		}

		/// <summary>
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public int issys(string name) {
			if (name.StartsWith("__")) {
				return 1;
			}
			if (name == "_file" || name == "_line") {
				return 1;
			}
			return 0;
		}


		/// <summary>
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="root"> </param>
		/// <returns> </returns>
		public string getlevel(string path, string root) {
			return QViewCompilerHelper.GetLevel(path, root).ToStr();
		}

		/// <summary>
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="root"> </param>
		/// <returns> </returns>
		public string getname(string path, string root) {
			return QViewCompilerHelper.GetViewName(path, root);
		}


		/// <summary>
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="root"> </param>
		/// <returns> </returns>
		public string getactionname(string path, string root) {
			return getname(path, root).Replace("/", ".");
		}

		/// <summary>
		/// </summary>
		/// <param name="path"> </param>
		/// <param name="root"> </param>
		/// <returns> </returns>
		public string getclsname(string path, string root) {
			return QViewCompilerHelper.GetClsName(path, root);
		}


		/// <summary>
		/// </summary>
		/// <param name="nav"> </param>
		/// <param name="iter"> </param>
		/// <param name="liftmode"> </param>
		/// <returns> </returns>
		public string tostr(XPathNavigator nav, XPathNodeIterator iter, int liftmode = 0) {
			if (0 == iter.Count) {
				return liftup(nav.Value, liftmode);
			}
			var sb = new StringBuilder();
			sb.Append("string.Format(");
			var src = nav.Value;
			var m = Regex.Match(nav.Value, @"^(?<name>\w+)@(?<lang>\w+)?\[[\{\d\}]+\]$");
			if (m.Success) {
				src = string.Format("GetResource(\"{0}\",\"{1}\")", m.Groups["name"].Value, m.Groups["lang"].Value);
			}
			else {
				src = "\"" + esc(src) + "\"";
			}
			sb.Append(src);
			while (iter.MoveNext()) {
				sb.Append(", ");
				if (iter.Current != null) {
					src = iter.Current.Value;
				}
				m = Regex.Match(src, @"^(?<name>\w+)@(?<lang>\w+)?$");
				if (m.Success) {
					src = string.Format("GetResource(\"{0}\",\"{1}\")", m.Groups["name"].Value, m.Groups["lang"].Value);
				}
				sb.Append(src);
			}
			sb.Append(")");

			return sb.ToString();
		}
	}

	// ReSharper restore MemberCanBePrivate.Global
	// ReSharper restore UnusedMember.Global
	// ReSharper restore InconsistentNaming
}