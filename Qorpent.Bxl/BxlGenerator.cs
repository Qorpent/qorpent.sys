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
// Original file : BxlGenerator.cs
// Project: Qorpent.Bxl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Converts Xml into Bxl code
	/// </summary>
	public class BxlGenerator {
		private static readonly Regex Literalregex = new Regex(@"^[\w@_?+\*\-/\d\.][\d\w_\.@]*$", RegexOptions.Compiled);


		/// <summary>
		/// 	Perform rendering of givin xml data into output stream
		/// </summary>
		/// <param name="sourceXmlData"> Xml data to render </param>
		/// <param name="output"> stream to send output </param>
		/// <param name="options"> BXL generation options </param>
		public void Write(XElement sourceXmlData, TextWriter output, BxlGeneratorOptions options = null) {
			options = options ?? new BxlGeneratorOptions();
			_level = 0;


			if (options.NoRootElement) {
				foreach (var element in sourceXmlData.Elements()) {
					WriteElement(element, true, options);
				}
			}
			else {
				WriteElement(sourceXmlData, _ns.Count > 0, options);
			}
		}

		private void WriteElement(XElement e, bool newline, BxlGeneratorOptions options) {
			if (newline) {
				Newline();
			}
			Indent();
			var ename = e.Name.LocalName;
			var ns = e.Name.Namespace;
			if (ns.ToString().IsNotEmpty()) {
				var prefix = _ns[ns.ToString()];
				ename = prefix + "::" + ename;
			}
			_output.Write(ename);
			_level++;
			_firstAttribute = true;
			var id = e.Attribute("id");
			var code = e.Attribute("code");
			var name = e.Attribute("name");
			var idval = e.Id();
			var writecode = id != null && code != null && code.Value != id.Value;
			if (idval.IsNotEmpty()) {
				_output.Write(" " + Escape(idval));
				_firstAttribute = false;
			}
			if (writecode && _firstAttribute) {
				_output.Write(" " + Escape(code.Value));
				_firstAttribute = false;
			}
			if (name != null) {
				if (_firstAttribute) {
					_output.Write(" name=" + Escape(name.Value));
					_firstAttribute = false;
				}
				else {
					_output.Write(", " + Escape(name.Value));
				}
			}
			if (writecode && !_firstAttribute) {
				_output.Write(", code=" + Escape(code.Value));
			}
			foreach (
				var a in
					e.Attributes().OrderBy(x => EvaluateOrderKey(x, options)).Where(x => EvaluateOrderKey(x, options).StartsWith("00"))
						.ToArray()
				) {
				if (a.Name == "id" || a.Name == "code" || a.Name == "name") {
					continue;
				}
				WriteAttribute(a, false, options);
				_firstAttribute = false;
			}
			var selfstr = e.Nodes().OfType<XText>().Select(x => x.Value ?? "").ConcatString(Environment.NewLine);
			if (selfstr.IsNotEmpty()) {
				selfstr = Escape(selfstr, options.UseTrippleQuotOnValues);
				_output.Write(" : ");
				_output.Write(selfstr);
			}
			foreach (
				var a in
					e.Attributes().OrderBy(x => EvaluateOrderKey(x, options)).Where(x => EvaluateOrderKey(x, options).StartsWith("01"))
						.ToArray()
				) {
				if (a.Name == "id" || a.Name == "code" || a.Name == "name") {
					continue;
				}
				WriteAttribute(a, true, options);
			}
			foreach (var e2 in e.Elements()) {
				WriteElement(e2, true, options);
			}
			_level--;
		}

		private void WriteAttribute(XAttribute a, bool tripple, BxlGeneratorOptions options) {
			if (-1 != Array.IndexOf(options.SkipAttributes, a.Name.LocalName)) {
				return;
			}
			var name = a.Name.LocalName;
			var ns = a.Name.Namespace;
			if (ns.ToString().IsNotEmpty()) {
				var prefix = _ns[ns.ToString()];
				name = prefix + "::" + name;
			}
			var inline = EvaluateOrderKey(a, options).StartsWith("00");
			if (inline) {
				if (!_firstAttribute) {
					_output.Write(",");
				}
				_output.Write(" ");
				_output.Write(name);
				_output.Write("=");
				_output.Write(Escape(a.Value));
			}
			else {
				Newline();
				Indent();
				_output.Write(name);
				_output.Write("=");
				_output.Write(Escape(a.Value, tripple));
			}
		}

		private static string Escape(string value, bool cantripple = false) {
			if (string.IsNullOrEmpty(value)) {
				return "''";
			}
			if (Literalregex.IsMatch(value)) {
				return value;
			}
			if ((value.Contains("\n") || value.Contains("\r")) && cantripple) {
				return "\"\"\"" + value + "\"\"\"";
			}
			if (value.Contains("\"")) {
				return "'" + value.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t").Replace("'", "\\'") +
				       "'";
			}
			return "\"" + value.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t") +
			       "\"";
		}

		private void Indent() {
			for (var i = 0; i < _level; i++) {
				_output.Write("\t");
			}
		}

		private void Newline() {
			_output.Write(Environment.NewLine);
		}

		private string EvaluateOrderKey(XAttribute attribute, BxlGeneratorOptions options) {
			var oa = attribute.Annotation<OrderAnnotation>();
			if (null != oa) {
				return oa.Value;
			}
			var name = attribute.Name.LocalName;
			var inlineorder = 1;
			if (options.InlineAttributesByDefault) {
				inlineorder = 0;
			}
			if (-1 != Array.IndexOf(options.InlineAlwaysAttributes, name)) {
				inlineorder = 0;
			}
			if (-1 != Array.IndexOf(options.NewlineAlwaysAttributes, name)) {
				inlineorder = 0;
			}
			var grouporder = Array.IndexOf(options.FirstPlaceAttributes, name);
			if (-1 == grouporder) {
				grouporder = 99;
			}

			var result = string.Format("{0:00}{1:00}{2}", inlineorder, grouporder, name.ToUpper());
			oa = new OrderAnnotation {Value = result};
			attribute.AddAnnotation(oa);
			return result;
		}

		/// <summary>
		/// 	Shortcat to quickly get BXL code as string without setting output stream
		/// </summary>
		/// <param name="e"> data to generate BXL </param>
		/// <param name="options"> </param>
		/// <returns> bxl representation of xml data </returns>
		public string Convert(XElement e, BxlGeneratorOptions options = null) {
			var sw = new StringWriter();
			_output = sw;
			ExtractAndWriteNamespaces(e, options);
			Write(e, sw, options);
			return sw.ToString();
		}

		private void ExtractAndWriteNamespaces(XElement xElement, BxlGeneratorOptions options) {
			_ns = new Dictionary<string, string>();
			foreach (var xAttribute in xElement.Attributes().Where(x => x.Name.Namespace == XNamespace.Xmlns).ToArray()) {
				var prefix = xAttribute.Name.LocalName;
				var ns = xAttribute.Value;
				_ns[ns] = prefix;
				xAttribute.Remove();
				if (QorpentConst.Xml.WellKnownNamespaces.ContainsKey(ns)) {
					if (QorpentConst.Xml.WellKnownNamespaces[ns] == prefix) {
						//no need generate well known namespaces directly
						continue;
					}
				}
				WriteAttribute(new XAttribute(xAttribute.Name.LocalName, xAttribute.Value), false, new BxlGeneratorOptions());
			}
		}

		#region Nested type: OrderAnnotation

		private class OrderAnnotation {
			public string Value;
		}

		#endregion

		private bool _firstAttribute;
		private int _level;

		private IDictionary<string, string> _ns;
		private TextWriter _output;
	}
}