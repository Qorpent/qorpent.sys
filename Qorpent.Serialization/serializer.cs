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
// Original file : serializer.cs
// Project: Qorpent.Serialization
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Serialization {
	/// <summary>
	/// </summary>
	/// <remarks>
	/// </remarks>
	public abstract class Serializer : ISerializer {
		/// <summary>
		/// 	Serializes the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <param name="output"> The output. </param>
		/// <remarks>
		/// </remarks>
		public virtual void Serialize(string name, object value, TextWriter output) {
			s = createImpl(name, value);
			if (null == output) {
				throw new ArgumentNullException("output");
			}
			s.Output = output;
			if (value is XElement) {
				var x = (XElement) value;
				s.Begin(x.Name.LocalName);
				serializeElement((XElement) value);
				s.End();
			}
			else {
				s.Begin(name);
				_serialize(name, value);
				s.End();
				s.Flush();
			}
		}


		/// <summary>
		/// 	Serializes the element.
		/// </summary>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void serializeElement(XElement value) {
			s.BeginObject(value.Name.LocalName);
			var c = value.Attributes().OfType<XObject>().Union(
				value.Nodes().Where(z => z is XText)
				).Union(
					value.Elements()).Count();
			foreach (var a in value.Attributes()) {
				c--;
				s.BeginObjectItem(a.Name.LocalName, true);
				s.WriteFinal(a.Value);
				s.EndObjectItem(c == 0);
			}
			var set = value.Nodes().Where(z => z is XText);
			foreach (var x in set) {
				c--;
				s.BeginObjectItem("_text", true);
				if (x is XText) {
					s.WriteFinal(((XText) x).Value);
				}
				s.EndObjectItem(c == 0);
			}
			IList<string> checkedx = new List<string>();
			foreach (var e in value.Elements()) {
				c--;
				if (checkedx.Contains(e.Name.LocalName)) {
					continue;
				}
				checkedx.Add(e.Name.LocalName);
				var e_ = value.Elements(e.Name);
				s.BeginObjectItem(e.Name.LocalName, false);
				if (e_.Count() == 1 && value.Attribute("isarray") == null) {
					serializeElement(e);
				}
				else {
					s.BeginArray(e.Name.LocalName);
					var i = 0;
					foreach (var x in e_) {
						s.BeginArrayEntry(i++);
						serializeElement(x);
						s.EndArrayEntry(i == c);
					}
					s.EndArray();
				}
				s.EndObjectItem(c == 0);
			}

			s.EndObject();
		}

		/// <summary>
		/// 	Creates the impl.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected abstract ISerializerImpl createImpl(string name, object value);

		/// <summary>
		/// 	_serializes the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void _serialize(string name, object value) {
			name = name ?? (null == value ? "null" : value.GetType().Name);
			if (null == value) {
				s.WriteFinal(null);
			}
			else if (value.GetType().IsValueType || value.GetType() == typeof (string)) {
				s.WriteFinal(value);
			}
			else if (value is Uri) {
				s.WriteFinal(value.ToString());
			}
			else if (value is Exception) {
				serializeClass(name,
				               new {type = value.GetType().Name, message = ((Exception) value).Message, text = value.ToString()});
			}
			else if (typeof (Array).IsAssignableFrom(value.GetType())) {
				serializeArray(name, (Array) value);
			}
			else if (typeof (IDictionary).IsAssignableFrom(value.GetType())) {
				serializeDictionary(name, (IDictionary) value);
			}
			else if (typeof (IEnumerable).IsAssignableFrom(value.GetType())) {
				serializeArray(name, ((IEnumerable) value).OfType<object>().ToArray());
			}
			else {
				if (_refcache.Contains(value)) {
					serializeClass("SERIALIZEPROBLEM", new {SERIALIZEPROBLEM = "circular reference"});
					return;
				}
				_refcache.Add(value);
				serializeClass(name, value);
				_refcache.Remove(value);
			}
		}

		/// <summary>
		/// 	Serializes the class.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void serializeClass(string name, object value) {
			var items = SerializableItem.GetSerializableItems(value);
			s.BeginObject(name);
			var c = items.Count();
			foreach (var i in items) {
				s.BeginObjectItem(i.Name, i.IsFinal);
				_serialize(i.Name, i.Value);
				s.EndObjectItem(c == 1);
				c--;
			}
			s.EndObject();
		}

		/// <summary>
		/// 	Serializes the dictionary.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void serializeDictionary(string name, IDictionary value) {
			s.BeginDictionary(name);
			var c = value.Keys.Count;
			foreach (var k in value.Keys) {
				s.BeginDictionaryEntry(k.ToString());
				_serialize("value", value[k]);
				s.EndDictionaryEntry(c == 1);
				c--;
			}
			s.EndDictionary();
		}

		/// <summary>
		/// 	Serializes the array.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void serializeArray(string name, Array value) {
			s.BeginArray(name);
			var i = 0;
			foreach (var val in value) {
				s.BeginArrayEntry(i);
				_serialize(i.ToString(), val);
				s.EndArrayEntry(i == value.Length - 1);
				i++;
			}
			s.EndArray();
		}

		/// <summary>
		/// 	Serializes the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="obj"> The obj. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public string serialize(string name, object obj) {
			var sw = new StringWriter();
			Serialize(name, obj, sw);
			return sw.ToString();
		}

		/// <summary>
		/// </summary>
		private readonly IList<object> _refcache = new List<object>();

		/// <summary>
		/// </summary>
		private ISerializerImpl s;
	}
}