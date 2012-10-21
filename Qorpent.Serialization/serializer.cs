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
			_s = CreateImpl(name, value);
			if (null == output) {
				throw new ArgumentNullException("output");
			}
			_s.Output = output;
			if (value is XElement) {
				var x = (XElement) value;
				_s.Begin(x.Name.LocalName);
				SerializeElement((XElement) value);
				_s.End();
			}
			else {
				_s.Begin(name);
				InternalSerialize(name, value);
				_s.End();
				_s.Flush();
			}
		}


		/// <summary>
		/// 	Serializes the element.
		/// </summary>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void SerializeElement(XElement value) {
			_s.BeginObject(value.Name.LocalName);
			var c = value.Attributes().OfType<XObject>().Union(
				value.Nodes().Where(z => z is XText)
				).Union(
					value.Elements()).Count();
			foreach (var a in value.Attributes()) {
				c--;
				_s.BeginObjectItem(a.Name.LocalName, true);
				_s.WriteFinal(a.Value);
				_s.EndObjectItem(c == 0);
			}
			var set = value.Nodes().Where(z => z is XText);
			foreach (var x in set) {
				c--;
				_s.BeginObjectItem("_text", true);
				var xText = x as XText;
				if (xText != null) {
					_s.WriteFinal(xText.Value);
				}
				_s.EndObjectItem(c == 0);
			}
			IList<string> checkedx = new List<string>();
			IEnumerable<XElement> elements;
			foreach (var e in value.Elements()) {
				c--;
				if (checkedx.Contains(e.Name.LocalName)) {
					continue;
				}
				checkedx.Add(e.Name.LocalName);
				elements = value.Elements(e.Name).ToArray();
				_s.BeginObjectItem(e.Name.LocalName, false);
				if (elements.Count() == 1 && value.Attribute("isarray") == null) {
					SerializeElement(e);
				}
				else {
					_s.BeginArray(e.Name.LocalName);
					var i = 0;
					foreach (var x in elements) {
						_s.BeginArrayEntry(i++);
						SerializeElement(x);
						_s.EndArrayEntry(i == c);
					}
					_s.EndArray();
				}
				_s.EndObjectItem(c == 0);
			}

			_s.EndObject();
		}

		/// <summary>
		/// 	Creates the impl.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected abstract ISerializerImpl CreateImpl(string name, object value);

		/// <summary>
		/// 	_serializes the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void InternalSerialize(string name, object value) {
			name = name ?? (null == value ? "null" : value.GetType().Name);
			if (null == value) {
				_s.WriteFinal(null);
			}
			else if (value.GetType().IsValueType || value is string) {
				_s.WriteFinal(value);
			}
			else if (value is Uri) {
				_s.WriteFinal(value.ToString());
			}
			else if (value is Exception) {
				SerializeClass(name,
				               new {type = value.GetType().Name, message = ((Exception) value).Message, text = value.ToString()});
			}
			else if (typeof (Array).IsAssignableFrom(value.GetType())) {
				SerializeArray(name, (Array) value);
			}
			else if (typeof (IDictionary).IsAssignableFrom(value.GetType())) {
				SerializeDictionary(name, (IDictionary) value);
			}
			else if (typeof (IEnumerable).IsAssignableFrom(value.GetType())) {
				SerializeArray(name, ((IEnumerable) value).OfType<object>().ToArray());
			}
			else {
				if (_refcache.Contains(value)) {
					SerializeClass("SERIALIZEPROBLEM", new {SERIALIZEPROBLEM = "pcircular reference"});
					return;
				}
				_refcache.Add(value);
				SerializeClass(name, value);
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
		private void SerializeClass(string name, object value) {
			var items = SerializableItem.GetSerializableItems(value).ToArray();
			_s.BeginObject(name);
			var c = items.Count();
			foreach (var i in items) {
				_s.BeginObjectItem(i.Name, i.IsFinal);
				InternalSerialize(i.Name, i.Value);
				_s.EndObjectItem(c == 1);
				c--;
			}
			_s.EndObject();
		}

		/// <summary>
		/// 	Serializes the dictionary.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void SerializeDictionary(string name, IDictionary value) {
			_s.BeginDictionary(name);
			var c = value.Keys.Count;
			foreach (var k in value.Keys) {
				_s.BeginDictionaryEntry(k.ToString());
				InternalSerialize("value", value[k]);
				_s.EndDictionaryEntry(c == 1);
				c--;
			}
			_s.EndDictionary();
		}

		/// <summary>
		/// 	Serializes the array.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="value"> The value. </param>
		/// <remarks>
		/// </remarks>
		private void SerializeArray(string name, Array value) {
			_s.BeginArray(name);
			var i = 0;
			foreach (var val in value) {
				_s.BeginArrayEntry(i);
				InternalSerialize(i.ToString(), val);
				_s.EndArrayEntry(i == value.Length - 1);
				i++;
			}
			_s.EndArray();
		}

		/// <summary>
		/// 	Serializes the specified name.
		/// </summary>
		/// <param name="name"> The name. </param>
		/// <param name="obj"> The obj. </param>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		public string DoSerialize(string name, object obj) {
			var sw = new StringWriter();
			Serialize(name, obj, sw);
			return sw.ToString();
		}

		/// <summary>
		/// </summary>
		private readonly IList<object> _refcache = new List<object>();

		/// <summary>
		/// </summary>
		private ISerializerImpl _s;
	}
}