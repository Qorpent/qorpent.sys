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
// Original file : SqlRegexMatch.cs
// Project: Qorpent.SqlExtensions
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Data.SqlTypes;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Server;
using Qorpent.Utils.Extensions;

namespace Qorpent.Integration.SqlExtensions {
	/// <summary>
	/// 	Incapsulates regex match for Sql
	/// </summary>
	[SqlUserDefinedType(Format.UserDefined, MaxByteSize = -1)]
	public struct SqlRegexMatch : INullable, IBinarySerialize {
		/// <summary>
		/// 	generates wrapper over match
		/// </summary>
		public SqlRegexMatch(string input, string pattern) {
			_input = input;
			_pattern = pattern;
		}

		/// <summary>
		/// 	Null representation of objec
		/// </summary>
		public static SqlRegexMatch Null {
			get { return new SqlRegexMatch(); }
		}

		/// <summary>
		/// 	Input string
		/// </summary>
		public string Input {
			get { return _input.Value; }
			set { _input = value; }
		}

		/// <summary>
		/// 	Regex pattern
		/// </summary>
		public string Pattern {
			get { return _pattern.Value; }
			set { _pattern = value; }
		}


		void IBinarySerialize.Read(BinaryReader r) {
			_pattern = r.ReadString();
			_input = r.ReadString();
		}

		void IBinarySerialize.Write(BinaryWriter w) {
			w.Write(_pattern.Value);
			w.Write(_input.Value);
		}


		/// <summary>
		/// 	Indicates whether a structure is null. This property is read-only.
		/// </summary>
		/// <returns> <see cref="T:System.Data.SqlTypes.SqlBoolean" /> true if the Value of this object is null. Otherwise, false. </returns>
		public bool IsNull {
			get { return _input.IsNull || _pattern.IsNull; }
		}


		/// <summary>
		/// 	returns full Value of match
		/// </summary>
		/// <returns> </returns>
		[SqlMethod]
		public SqlString GetValue() {
			return Regex.Match(_input.Value, _pattern.Value).Value;
		}

		/// <summary>
		/// 	returns full Value of match
		/// </summary>
		/// <returns> </returns>
		[SqlMethod]
		public SqlString Replace(SqlString replacer) {
			return Regex.Replace(_input.Value, _pattern.Value, replacer.Value);
		}

		/// <summary>
		/// 	UDT requirement
		/// </summary>
		/// <param name="s"> </param>
		/// <returns> </returns>
		[SqlMethod]
		public static SqlRegexMatch Parse(SqlString s) {
			return new SqlRegexMatch(s.Value.Split('~')[0], s.Value.Split('~')[1]);
		}

		/// <summary>
		/// 	Returns the fully qualified type name of this instance.
		/// </summary>
		/// <returns> A <see cref="T:System.String" /> containing a fully qualified type name. </returns>
		/// <filterpriority>2</filterpriority>
		public override string ToString() {
			return GetValue().Value;
		}

		/// <summary>
		/// 	returns Value of named/numbered group
		/// </summary>
		/// <param name="name_or_id"> </param>
		/// <returns> </returns>
		[SqlMethod]
		public SqlString GetGroup(string name_or_id) {
			if (0 != name_or_id.ToInt()) {
				return Regex.Match(_input.Value, _pattern.Value).Groups[name_or_id.ToInt()].Value;
			}
			else {
				return Regex.Match(_input.Value, _pattern.Value).Groups[name_or_id].Value;
			}
		}

		private SqlString _input;
		private SqlString _pattern;
	}
}