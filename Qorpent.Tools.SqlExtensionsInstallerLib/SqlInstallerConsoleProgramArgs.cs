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
// Original file : SqlInstallerConsoleProgramArgs.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Qorpent.Utils.Extensions;

namespace Qorpent.Tools.SqlExtensionsInstallerLib {
	/// <summary>
	/// 	incapsulates Arguments for SqlInstallerConsole
	/// </summary>
	public class SqlInstallerConsoleProgramArgs {
		/// <summary>
		/// 	File name of assembly to install
		/// </summary>
		public string AssemblyName {
			get { return _assemblyName.IsEmpty() ? Arg1 : _assemblyName; }
			set { _assemblyName = value; }
		}

		/// <summary>
		/// 	checkout that arguments are valid and returns message to display
		/// </summary>
		/// <param name="argsvalidmessage"> message to display </param>
		/// <returns> true if arguments are valid </returns>
		public bool IsValid(out string argsvalidmessage) {
			argsvalidmessage = "";

			if (AssemblyName.IsEmpty() && null == Assembly) {
				argsvalidmessage = "no AssemblyName given";
				return false;
			}

			//not defined task
			if (Server.IsEmpty() && ConnectionString.IsEmpty() && !GenerateScript) {
				argsvalidmessage = "no SQL target given or Generate Script used";
				return false;
			}

			//not defined security
			if (!GenerateScript && !Trusted && ConnectionString.IsEmpty() && (User.IsEmpty() || Password.IsEmpty())) {
				argsvalidmessage = "task is to apply to SQL, but security not defined properly";
				return false;
			}

			return true;
		}


		/// <summary>
		/// 	Generates SqlConnection from given parameters
		/// </summary>
		/// <returns> </returns>
		public IDbConnection CreateConnection() {
			if (ConnectionString.IsNotEmpty()) {
				return new SqlConnection(ConnectionString);
			}
			var cs = "";
			if (User.IsNotEmpty()) {
				cs =
					string.Format(
						"Data Source={0};Initial Catalog={1};Persist Security Info=True;User ID={2};Password={3};Application Name=Qorpent_Sql_Installer",
						Server, Database, User, Password);
			}
			else {
				cs =
					string.Format(
						"Data Source={0};Initial Catalog={1};Integrated Security=True;Application Name=Qorpent_Sql_Installer", Server,
						Database);
			}
			return new SqlConnection(cs);
		}


		/// <summary>
		/// 	loads assembly for script generation
		/// </summary>
		/// <returns> </returns>
		public Assembly LoadAssembly() {
			try {
				if (null != Assembly) {
					return Assembly;
				}
				var fullpath = Path.GetFullPath(AssemblyName);
				_dir = Path.GetDirectoryName(fullpath);
				if (!fullpath.EndsWith(".dll")) {
					fullpath += ".dll";
				}
				if (!File.Exists(fullpath)) {
					throw new SqlInstallerException("file not found: " + fullpath);
				}
				AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

				return Assembly.LoadFile(fullpath);
			}
			finally {
				AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
			}
		}

		private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
			var filename = Path.Combine(_dir, args.Name);
			if (!filename.EndsWith(".dll")) {
				filename += ".dll";
			}
			if (!File.Exists(filename)) {
				throw new SqlInstallerException("file not found: " + filename);
			}
			return Assembly.LoadFile(filename);
		}

		/// <summary>
		/// 	Allows to give Assembly name as first unknown parameter
		/// </summary>
		public string Arg1;

		/// <summary>
		/// 	Directly given Assembly
		/// </summary>
		public Assembly Assembly;

		/// <summary>
		/// 	true to generate only drop scripts
		/// </summary>
		public bool CleanOnly;

		/// <summary>
		/// 	Fully qualified connectionstring
		/// </summary>
		public string ConnectionString;

		/// <summary>
		/// 	Database name
		/// </summary>
		public string Database = "[MYDB]"; // stub name, replace with yours

		/// <summary>
		/// 	Generate-script-only mode
		/// </summary>
		public bool GenerateScript;
		/// <summary>
		/// 
		/// </summary>
		public bool Safe;

		/// <summary>
		/// 	prevents drop of custom dependency
		/// </summary>
		public bool NoDropCustomDependency = true;

		/// <summary>
		/// 	True prevents generation of altering and CLR configure database and use command - script will be database-ignorance
		/// </summary>
		public bool NoScriptDatabase = true;

		/// <summary>
		/// 	Password for non trusted connection
		/// </summary>
		public string Password;

		/// <summary>
		/// 	Target sql Schema
		/// </summary>
		public string Schema = "qorpent";

		/// <summary>
		/// 	File for script output
		/// </summary>
		public string ScriptFile;

		/// <summary>
		/// 	Server name
		/// </summary>
		public string Server;

		/// <summary>
		/// 	Trusted connection mode
		/// </summary>
		public bool Trusted = true;

		/// <summary>
		/// 	User name for non trusted connections
		/// </summary>
		public string User;


		/// <summary>
		/// 	File name of assembly to install
		/// </summary>
		private string _assemblyName;

		private string _dir;
	}
}