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
// Original file : xmlSerializerImpl.cs
// Project: Qorpent.Serialization
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	internal class XmlSerializerImpl : ISerializerImpl {
		/// <summary>
		/// 	Gets the current.
		/// </summary>
		/// <remarks>
		/// </remarks>
		private XElement Current {
			get { return _stack.Peek(); }
		}


		/// <summary>
		/// 	Gets or sets the output.
		/// </summary>
		/// <value> The output. </value>
		/// <remarks>
		/// </remarks>
		public TextWriter Output { get; set; }

		/// <summary>
		/// 	Begins the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		public void Begin(string name) {
			Root = new XElement("root");
			_stack.Push(Root);
		}

		/// <summary>
		/// 	Ends this instance.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public virtual void End() {
			var e = Root;
			if (e.Elements().Count() == 1) {
				e = e.Elements().First();
			}
			Output.Write(e.ToString(SaveOptions.DisableFormatting));
		}

		/// <summary>
		/// 	Begins the object.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		public void BeginObject(string name) {
			if (name.Contains("<")) {
				name = "anonymous";
			}
			if (Regex.IsMatch(name, @"^\d")) {
				name = "item";
			}
			var e = new XElement(name);
			Current.Add(e);
			_stack.Push(e);
		}

		/// <summary>
		/// 	Ends the object.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public void EndObject() {
			var current = _stack.Pop();
			if (!current.Attributes().Any() && current.Elements().Count() == 1 &&
			    current.Name.LocalName == current.Elements().First().Name.LocalName) {
				current.ReplaceWith(current.Elements().First());
			}
		}

		/// <summary>
		/// 	Begins the object item.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="final"> if set to <c>true</c> [final]. </param>
		/// <remarks>
		/// </remarks>
		public void BeginObjectItem(string name, bool final) {
			_currentAttr = null;
			if (final) {
				_currentAttr = new XAttribute(name, "");
				Current.Add(_currentAttr);
			}
			else {
				BeginObject(name);
			}
		}

		/// <summary>
		/// 	Ends the object item.
		/// </summary>
		/// <param name="last"> if set to <c>true</c> [last]. </param>
		/// <remarks>
		/// </remarks>
		public void EndObjectItem(bool last) {
			if (_currentAttr != null) {
				_currentAttr = null;
			}
			else {
				EndObject();
			}
		}

		/// <summary>
		/// 	Writes the final.
		/// </summary>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		public void WriteFinal(object value) {
			if (_currentAttr != null) {
				_currentAttr.SetValue(value ?? "");
			}
			else if (_stack.Count == 1) {
				Current.Add(new XElement("value", value));
			}
			else {
				if (null != value) {
					Current.SetValue(value);
				}
			}
		}

		/// <summary>
		/// 	Begins the dictionary.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		public void BeginDictionary(string name) {
			//empty
		}

		/// <summary>
		/// 	Ends the dictionary.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public void EndDictionary() {
			//empty
		}

		/// <summary>
		/// 	Begins the dictionary entry.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		public void BeginDictionaryEntry(string name) {
			var e = new XElement("item", new XAttribute("key", name));
			Current.Add(e);
			_stack.Push(e);
		}

		/// <summary>
		/// 	Ends the dictionary entry.
		/// </summary>
		/// <param name="last"> if set to <c>true</c> [last]. </param>
		/// <remarks>
		/// </remarks>
		public void EndDictionaryEntry(bool last) {
			_stack.Pop();
		}

		/// <summary>
		/// 	Begins the array.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <remarks>
		/// </remarks>
		public void BeginArray(string name) {
			name = name.Replace("[", "_").Replace("]", "_");
			BeginObject(name);
		}

		/// <summary>
		/// 	Ends the array.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public void EndArray() {
			EndObject();
		}

		/// <summary>
		/// 	Begins the array entry.
		/// </summary>
		/// <param name="idx"> The idx. </param>
		/// <remarks>
		/// </remarks>
		public void BeginArrayEntry(int idx) {
			var e = new XElement("item", new XAttribute("idx", idx));
			Current.Add(e);
			_stack.Push(e);
		}

		/// <summary>
		/// 	Ends the array entry.
		/// </summary>
		/// <param name="last"> if set to <c>true</c> [last]. </param>
		/// <remarks>
		/// </remarks>
		public void EndArrayEntry(bool last) {
			var e = _stack.Pop();
			if (e.Elements("item").Count() == 1 && e.Elements().Count() == 1) {
				var subst = e.Elements().FirstOrDefault();
				if (subst != null) {
					subst.Add(e.Attribute("idx"));
					e.ReplaceWith(subst);
				}
			}
		}

		/// <summary>
		/// 	Flushes this instance.
		/// </summary>
		/// <remarks>
		/// </remarks>
		public void Flush() {
			Output.Flush();
		}

		/// <summary>
		/// </summary>
		private readonly Stack<XElement> _stack = new Stack<XElement>();

		/// <summary>
		/// </summary>
		protected XElement Root;

		/// <summary>
		/// </summary>
		private XAttribute _currentAttr;
	}
}