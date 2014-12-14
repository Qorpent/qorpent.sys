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
// Original file : SqlExportMemberWrapper.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.SqlServer.Server;

namespace Qorpent.Tools.SqlExtensionsInstallerLib {
	/// <summary>
	/// 	Wraps Sql Export objects to
	/// </summary>
	public abstract class SqlExportMemberWrapper {
		/// <summary>
		/// </summary>
		/// <param name="info"> </param>
		/// <param name="schema"> </param>
		protected SqlExportMemberWrapper(MethodInfo info, string schema) {
			_info = info;
			_schema = schema;
		}

		/// <summary>
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="schema"> </param>
		protected SqlExportMemberWrapper(Type type, string schema) {
			_type = type;
			_schema = schema;
		}

		/// <summary>
		/// 	generates wrapper for methods
		/// </summary>
		/// <param name="method"> </param>
		/// <param name="schema"> </param>
		/// <returns> </returns>
		public static SqlExportMemberWrapper[] Create(MethodInfo method, string schema) {
			if (!method.IsStatic) {
				return null;
			}
			var attrs = method.GetCustomAttributes(true);
			foreach (Attribute attr in attrs) {
				if (attr is SqlFunctionAttribute) {
					return new SqlExportMemberWrapper[]
						{
							new FunctionWrapper((SqlFunctionAttribute) attr, method, schema),
							new FunctionSqlWrapper((SqlFunctionAttribute) attr, method, schema),
						};
				}
				if (attr is SqlProcedureAttribute) {
					return new[] {new ProcedureWrapper((SqlProcedureAttribute) attr, method, schema)};
				}
			}
			return null;
		}

		/// <summary>
		/// 	generates wrapper for type
		/// </summary>
		/// <param name="type"> </param>
		/// <param name="schema"> </param>
		/// <returns> </returns>
		public static SqlExportMemberWrapper[] Create(Type type, string schema) {
			var attrs = type.GetCustomAttributes(true);
			foreach (Attribute attr in attrs) {
				if (attr is SqlUserDefinedAggregateAttribute) {
					return new[] {new AggregatorWrapper((SqlUserDefinedAggregateAttribute) attr, type, schema)};
				}

				if (attr is SqlUserDefinedTypeAttribute) {
					return new[] {new UserTypeWrapper((SqlUserDefinedTypeAttribute) attr, type, schema)};
				}
			}
			return null;
		}

		/// <summary>
		/// 	Generates name of object
		/// </summary>
		/// <returns> </returns>
		public abstract string GetObjectName();

		/// <summary>
		/// 	generates sql type of DDL object
		/// </summary>
		/// <returns> </returns>
		public abstract string GetObjectType();

		/// <summary>
		/// 	Script of drop-if-exists on object
		/// </summary>
		/// <returns> </returns>
		public virtual string GetDropScript() {
			return string.Format(@"
--SQLINSTALL: DROPS {0}.{1} IF EXISTS
IF OBJECT_ID('{0}.{1}')  IS NOT NULL or TYPE_ID('{0}.{1}')  IS NOT NULL  DROP {2} {0}.{1}
",
			                     _schema, GetObjectName(), GetObjectType());
		}

		/// <summary>
		/// 	Script of creation of object
		/// </summary>
		/// <returns> </returns>
		public abstract string GetCreateScript();

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		protected string GetMethodFullName() {
			return string.Format("[{0}].[{1}]", _info.DeclaringType.FullName, _info.Name);
		}
		/// <summary>
		/// Возвращает полное имя класса
		/// </summary>
		/// <returns></returns>
		protected string GetClassFullName()
		{
			return string.Format("[{0}]", _type.FullName);
		}

		/// <summary>
		/// </summary>
		/// <returns> </returns>
		protected string GetArguments(MethodInfo method = null) {
			var args = new List<string>();
		
			foreach (var parameterInfo in (method??_info).GetParameters()) {
				var type = QueryGeneratorHelper.GetSqlType(parameterInfo.ParameterType, _schema);
				var def = " = null";
				if (type.Contains("max") || type == "xml") {
					def = "";
				}
				args.Add(string.Format("@{0} {1}{2}", parameterInfo.Name,
				                      type,def));
			}
			return string.Join(",", args.ToArray());
		}

		/// <summary>
		/// 	Gets the argument names.
		/// </summary>
		/// <returns> </returns>
		/// <remarks>
		/// </remarks>
		protected string GetArgumentNames() {
			var args = new List<string>();
			foreach (var parameterInfo in _info.GetParameters()) {
				args.Add(string.Format("@{0}", parameterInfo.Name));
			}
			return string.Join(",", args.ToArray());
		}

		/// <summary>
		/// </summary>
		protected MethodInfo _info;

		/// <summary>
		/// </summary>
		protected string _schema
		                 ;

		/// <summary>
		/// </summary>
		protected Type _type;
	}
}