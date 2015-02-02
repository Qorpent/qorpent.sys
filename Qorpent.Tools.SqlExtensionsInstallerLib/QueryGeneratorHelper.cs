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
// Original file : QueryGeneratorHelper.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.SqlServer.Types;
using Qorpent.Utils.Extensions;

namespace Qorpent.Tools.SqlExtensionsInstallerLib {
	/// <summary>
	/// 	Helper for building valid SQL string
	/// </summary>
	public static class QueryGeneratorHelper {
		/// <summary>
		/// 	generates varbinary string from assembly file
		/// </summary>
		/// <param name="assemblyPath"> </param>
		/// <returns> </returns>
		public static string GetAssemblyBits(string assemblyPath) {
			var builder = new StringBuilder();
			builder.Append("0x");

			using (var stream = new FileStream(assemblyPath,
			                                   FileMode.Open, FileAccess.Read, FileShare.Read)) {
				var currentByte = stream.ReadByte();
				while (currentByte > -1) {
					builder.Append(currentByte.ToString("X2", CultureInfo.InvariantCulture));
					currentByte = stream.ReadByte();
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// 	return SQL type name for given .NET type
		/// </summary>
		/// <param name="nettype"> </param>
		/// <param name="schema"> </param>
		/// <returns> </returns>
		/// <exception cref="SqlInstallerException"></exception>
		public static string GetSqlType(Type nettype, string schema) {
            if (typeof(object) == nettype)
            {
                return "sql_variant";
            }
            if (typeof (string) == nettype) {
				return "nvarchar(max)";
			}
			if (typeof (SqlString) == nettype) {
				return "nvarchar(max)";
			}
			if (typeof (decimal) == nettype) {
				return "decimal(18,6)";
			}
			if (typeof (SqlDecimal) == nettype) {
				return "decimal(18,6)";
			}
			if (typeof (int) == nettype) {
				return "int";
			}
			if (typeof (SqlInt32) == nettype) {
				return "int";
			}
			if (typeof (long) == nettype) {
				return "bigint";
			}
			if (typeof (SqlInt64) == nettype) {
				return "bigint";
			}
			if (typeof (bool) == nettype) {
				return "bit";
			}
			if (typeof (SqlBoolean) == nettype) {
				return "bit";
			}
			if (typeof (float) == nettype) {
				return "float";
			}
			if (typeof (SqlDouble) == nettype) {
				return "float";
			}
			if (typeof(DateTime) == nettype)
			{
				return "datetime";
			}
			if (typeof(SqlDateTime) == nettype)
			{
				return "datetime";
			}
			if (typeof (SqlXml) == nettype) {
				return "xml";
			}
            if (typeof(SqlGeometry) == nettype)
            {
                return "geometry";
            }
            if (typeof(SqlGeography) == nettype)
            {
                return "geography";
            }
			return schema + "." + nettype.Name;
		}

		/// <summary>
		/// 	converts given name into safe [] form
		/// </summary>
		/// <param name="name"> </param>
		/// <returns> </returns>
		public static string GetSafeSqlName(string name) {
			name = name.Replace("[", "");
			name = name.Replace("]", "");
			var nameparts = name.Split('.');
			name = "";
			foreach (var namepart in nameparts) {
				if (name.IsNotEmpty()) {
					name += ".";
				}
				name += "[" + namepart + "]";
			}
			return name;
		}

		/// <summary>
		/// 	Generates sp_executesql by pattern
		/// </summary>
		/// <param name="query"> </param>
		/// <param name="parameters"> </param>
		/// <returns> </returns>
		public static string GenerateExecuteSql(string query, params object[] parameters) {
			query = String.Format(query, parameters).Replace("'", "''");
			return " exec sp_executesql N'" + query + "'";
		}
	}
}