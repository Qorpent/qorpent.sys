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
// Original file : UserTypeWrapper.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using Microsoft.SqlServer.Server;
using Qorpent.Utils.Extensions;

namespace Qorpent.Tools.SqlExtensionsInstallerLib {
	internal class UserTypeWrapper : SqlExportMemberWrapper {
		public UserTypeWrapper(SqlUserDefinedTypeAttribute typedef, Type usrtype, string schema)
			: base(usrtype, schema) {
			_typedef = typedef;
		}

		public override string GetObjectName() {
			if (_typedef.Name.IsEmpty()) {
				return QueryGeneratorHelper.GetSafeSqlName(_type.Name);
			}
			return QueryGeneratorHelper.GetSafeSqlName(_typedef.Name);
		}

		public override string GetObjectType() {
			return "TYPE";
		}

		public override string GetCreateScript() {
			var assemblyname = "[" + _type.Assembly.GetName().Name + "]";
			var name = _schema + "." + GetObjectName();
			var typename = _type.FullName;
			return string.Format(@"
--SQLINSTALL: create type {0}
CREATE TYPE {0}
EXTERNAL NAME {1}.[{2}]", name, assemblyname, typename
				);
		}

		private readonly SqlUserDefinedTypeAttribute _typedef;
	}
}