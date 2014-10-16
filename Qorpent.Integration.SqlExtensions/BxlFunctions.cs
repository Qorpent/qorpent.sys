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
// Original file : BxlFunctions.cs
// Project: Qorpent.SqlExtensions
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Data.SqlTypes;
using System.Xml.Linq;
using Microsoft.SqlServer.Server;
using Qorpent.Bxl;

namespace Qorpent.Integration.SqlExtensions {
	/// <summary>
	/// 	BXL Utilities for BXL
	/// </summary>
	public static class BxlFunctions {
		/// <summary>
		/// 	Converts given BXL code to XML
		/// </summary>
		[SqlFunction(
			IsDeterministic = false,
			SystemDataAccess = SystemDataAccessKind.None,
			DataAccess = DataAccessKind.None
			)]
		public static SqlXml BxlToXml(SqlString bxlcode) {
			if (bxlcode.IsNull) {
				return SqlXml.Null;
			}
			return new SqlXml(MyBxl.Parse(bxlcode.Value).CreateReader());
		}


		/// <summary>
		/// 	Converts given XML code to BXL
		/// </summary>
		[SqlFunction(
			IsDeterministic = false,
			SystemDataAccess = SystemDataAccessKind.None,
			DataAccess = DataAccessKind.None
			)]
		public static SqlString BxlFromXml(SqlXml xml) {
			if (xml.IsNull) {
				return SqlString.Null;
			}
			return MyBxl.Convert(XElement.Parse(xml.Value));
		}
	}
}