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
// Original file : FunctionWrapper.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Reflection;
using Microsoft.SqlServer.Server;
using Qorpent.Utils.Extensions;

namespace Qorpent.Tools.SqlExtensionsInstallerLib {
	internal class FunctionWrapper : SqlExportMemberWrapper {
		public FunctionWrapper(SqlFunctionAttribute functiondef, MethodInfo functionsygnature, string schema)
			: base(functionsygnature, schema) {
			_functiondef = functiondef;
		}

		public override string GetObjectName() {
			if (_functiondef.Name.IsEmpty()) {
				return QueryGeneratorHelper.GetSafeSqlName("Clr" + _info.Name);
			}
			return QueryGeneratorHelper.GetSafeSqlName( "Clr" + _functiondef.Name);
		}

		public override string GetObjectType() {
			return "FUNCTION";
		}

		public override string GetCreateScript() {
			var assemblyname = "[" + _info.DeclaringType.Assembly.GetName().Name + "]";
			var name = _schema + "." + GetObjectName();
			var args = GetArguments();
			var rettype = GetSqlReturnType();
			var methodname = GetMethodFullName();
			const string pattern = @"
--SQLINSTALL: CREATE FUNCTION {0} ({3}.{4})
CREATE FUNCTION {0} ({1})
RETURNS {2} WITH EXECUTE AS CALLER
AS 
EXTERNAL NAME {3}.{4}";
			return string.Format(pattern, name, args, rettype, assemblyname, methodname);
		}

		private string GetSqlReturnType() {
			if (_functiondef.TableDefinition.IsEmpty()) {
				return QueryGeneratorHelper.GetSqlType(_info.ReturnType, _schema);
			}
			else {
				return " TABLE ( \r\n" + _functiondef.TableDefinition + "\r\n)";
			}
		}

		private readonly SqlFunctionAttribute _functiondef;
	}
}