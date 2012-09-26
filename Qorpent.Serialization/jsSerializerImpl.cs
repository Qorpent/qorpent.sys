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
// Original file : jsSerializerImpl.cs
// Project: Qorpent.Serialization
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Qorpent.Serialization {
	internal class JsSerializerImpl : ISerializerImpl {
		private objectSerializerState Current {
			get { return 0 == _statestack.Count ? objectSerializerState.undef : _statestack.Peek(); }
		}


		public TextWriter Output { get; set; }

		public void Begin(string name) {}

		public void End() {}

		public void BeginObject(string name) {
			_statestack.Push(objectSerializerState.obj);

			Output.Write("{");
		}

		public void EndObject() {
			var s = _statestack.Pop();
			if (s != objectSerializerState.obj) {
				throw new Exception("cannot close object here");
			}
			Output.Write("}");
		}

		public void BeginObjectItem(string name, bool isfinal) {
			if (Current == objectSerializerState.obj) {
				Output.Write(Literal(name) + ": ");
			}
			else {
				throw new Exception("cannot write object items here");
			}
		}

		public void EndObjectItem(bool last) {
			if (Current == objectSerializerState.obj) {
				Output.Write(", ");
			}
			else {
				throw new Exception("cannot finish object items here");
			}
		}

		public void WriteFinal(object value) {
			if (null == value) {
				Output.Write("null");
			}
			else if (value is JsRaw) {
				Output.Write(value.ToString());
			}
			else if (value is string) {
				Output.Write("'" + escape((string) value) + "'");
			}
			else if (value is DateTime) {
				var d = (DateTime) value;
				Output.Write(string.Format("new Date({0},{1},{2},{3},{4},{5})", d.Year, d.Month - 1, d.Day, d.Hour, d.Minute,
				                           d.Second));
			}
			else if (value is int || value is long) {
				Output.Write(value.ToString());
			}
			else if (value is double || value is decimal || value is float) {
				Output.Write(Convert.ToDouble(value).ToString("0.########", CultureInfo.InvariantCulture));
			}
			else if (value is bool) {
				Output.Write(value.ToString().ToLower());
			}
			else {
				Output.Write("'" + escape(value.ToString()) + "'");
			}
		}


		public void BeginDictionary(string name) {
			BeginObject(name);
		}

		public void EndDictionary() {
			EndObject();
		}

		public void BeginDictionaryEntry(string name) {
			BeginObjectItem(name, false);
		}

		public void EndDictionaryEntry(bool last) {
			EndObjectItem(last);
		}

		public void BeginArray(string name) {
			_statestack.Push(objectSerializerState.array);
			Output.Write("[");
		}

		public void EndArray() {
			var r = _statestack.Pop();
			if (r != objectSerializerState.array) {
				throw new Exception("cannot close array here");
			}
			Output.Write("]");
		}

		public void BeginArrayEntry(int idx) {}

		public void EndArrayEntry(bool last) {
			if (Current == objectSerializerState.array) {
				Output.Write(", ");
			}
			else {
				throw new Exception("cannot finish array items here");
			}
		}

		public void Flush() {
			Output.Flush();
		}


		private string Literal(string name) {
			var esc = escape(name);
			if (
				esc.Trim() == name &&
				esc != "" &&
				!Regex.IsMatch(esc, @"\s")
				&& Regex.IsMatch(esc, @"^\D")
				&& !esc.Contains(".")
				) {
				return name;
			}
			return "'" + esc + "'";
		}

		private string escape(string str) {
			return str.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\r", "\\r").Replace("\t", "\\t").Replace("\n", "\\n");
		}

		private readonly Stack<objectSerializerState> _statestack = new Stack<objectSerializerState>();
	}
}