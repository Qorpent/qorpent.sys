﻿#region LICENSE

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
// Original file : SqlInstallerConsoleProgram.cs
// Project: Qorpent.Tools.SqlExtensionsInstallerLib
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.Installer.SqlExtensions {
    /// <summary>
    ///     Implementation of SqlInstallerConsole
    /// </summary>
    public class SqlInstallerConsoleProgram {
        private SqlInstallerConsoleProgramArgs _args;

        /// <summary>
        /// </summary>
        public string[] Scripts;

        /// <summary>
        ///     executes sql script or generate it
        /// </summary>
        /// <param name="args"> </param>
        /// <returns> </returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Run(SqlInstallerConsoleProgramArgs args) {
            string argsvalidmessage;
            if (!args.IsValid(out argsvalidmessage)) {
                throw new SqlInstallerException("invalid arguments : " + argsvalidmessage);
            }
            Scripts = GenerateScripts(args).ToArray();
            if (args.NoOutput) {
                return 0;
            }
            if (args.GenerateScript) {
                GenerateScript(args);
            }
            else {
                UpdateDatabase(args);
            }
            return 0;
        }

        private void UpdateDatabase(SqlInstallerConsoleProgramArgs args) {
            using (var connection = args.CreateConnection()) {
                connection.Open();
                //using (var tr = connection.BeginTransaction()) {
                foreach (var script in Scripts) {
                    var command = connection.CreateCommand();
                    //command.Transaction = tr;
                    command.CommandText = script;
                    command.ExecuteNonQuery();

                    var message = "";
                    var commandmessage = Regex.Match(script, "--SQLINSTALL:([^\r\n]+)");
                    if (commandmessage.Success) {
                        message = commandmessage.Groups[1].Value;
                    }
                    else {
                        message = script;
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(message + " : executed");
                    Console.ResetColor();
                }
                //tr.Commit();
                //}
            }
        }

        private void GenerateScript(SqlInstallerConsoleProgramArgs args) {
            if (args.NoOutput) {
                return;
            }

            var script = new StringBuilder();
            foreach (var s in Scripts) {
                script.AppendLine(s);
                script.AppendLine("GO");
            }
            if (args.ScriptFile.IsNotEmpty()) {
                File.WriteAllText(args.ScriptFile, script.ToString());
            }
            else {
                Console.WriteLine(script.ToString());
            }
        }

        /// <summary>
        ///     Generates set of commands to execute on SqlServer
        /// </summary>
        /// <param name="args"> </param>
        /// <returns> </returns>
        public IEnumerable<string> GenerateScripts(SqlInstallerConsoleProgramArgs args) {
            _args = args;
            var assembly = args.LoadAssembly();
            var sysdepends =
                (assembly.GetReferencedAssemblies().Where(
                    x =>
                        (x.Name.StartsWith("System.") || x.Name.StartsWith("Microsoft.")) &&
                        x.Name != "Microsoft.SqlServer.Types" && x.Name != "System.Data" && x.Name != "System.Xml" &&
                        x.Name != "System" && x.Name != "System.Web")).
                    ToList();

            var customdepends =
                assembly.GetReferencedAssemblies().Where(
                    x => !(x.Name.StartsWith("System") || x.Name == "mscorlib" || x.Name.StartsWith("Microsoft.")))
                    .ToList();

            foreach (var assemblyName in customdepends.ToArray()) {
                var a = Assembly.Load(assemblyName);
                foreach (
                    var ccd in
                        a.GetReferencedAssemblies().Where(
                            x =>
                                !(x.Name.StartsWith("System") || x.Name == "mscorlib" || x.Name.StartsWith("Microsoft.")))
                    ) {
                    if (!customdepends.Any(x => x.Name == ccd.Name)) {
                        customdepends.Add(ccd);
                    }
                }
                var subsysdepends =
                    (a.GetReferencedAssemblies().Where(
                        x =>
                            (x.Name.StartsWith("System.") || x.Name.StartsWith("Microsoft.")) &&
                            x.Name != "Microsoft.SqlServer.Types" && x.Name != "System.Data" && x.Name != "System.Xml" &&
                            x.Name != "System" && x.Name != "System.Web")).
                        ToList();
                foreach (var subsysdepend in subsysdepends) {
                    if (!sysdepends.Contains(subsysdepend)) {
                        sysdepends.Add(subsysdepend);
                    }
                }
            }
            foreach (var assemblyName in customdepends) {
                var a = Assembly.Load(assemblyName);
                foreach (
                    var csd in
                        a.GetReferencedAssemblies().Where(
                            x =>
                                x.Name.StartsWith("System") && x.Name != "System.Data" && x.Name != "System.Xml" &&
                                x.Name != "System")) {
                    if (!sysdepends.Any(x => x.Name == csd.Name)) {
                        sysdepends.Add(csd);
                    }
                }
            }
            if (sysdepends.Any(_ => _.Name == "Microsoft.CSharp")) {
                sysdepends.Insert(0, new AssemblyName("system.dynamic"));
            }
            foreach (var customdepend in customdepends.ToArray()) {
                if (customdepend.Name.Contains("WindowsBase")) {
                    sysdepends.Add(customdepend);
                    customdepends.Remove(customdepend);
                }
            }
            sysdepends = sysdepends.GroupBy(_ => _.Name).Select(_ => _.First()).ToList();
            customdepends.Sort((a, b) => {
                var aa = Assembly.Load(a);
                var ba = Assembly.Load(b);
                if (aa.GetReferencedAssemblies().Any(x => x.Name == b.Name)) {
                    return 1;
                }
                if (ba.GetReferencedAssemblies().Any(x => x.Name == a.Name)) {
                    return -1;
                }
                return 0;
            });
            var items = GetExportedItems(assembly, args.Schema);
            var dbname = QueryGeneratorHelper.GetSafeSqlName(args.Database);
            //1 Prepare database
            if (!args.NoScriptDatabase) {
                foreach (var p in GetDatabasePrepareScripts(args, dbname)) {
                    yield return p;
                }
            }
            yield return string.Format(
                @"--SQLINSTALL: CHECK SCHEMA
if SCHEMA_ID('{0}') is null {1}  
", args.Schema,
                QueryGeneratorHelper.GenerateExecuteSql("create schema {0}", args.Schema));
            //2 drop existed items
            foreach (var item in items.Where(x => !(x is UserTypeWrapper))) {
                yield return item.GetDropScript();
            }
            foreach (var item in items.Where(x => (x is UserTypeWrapper))) {
                yield return item.GetDropScript();
            }
            //3 Prepare system libs and drop referenced libs
            foreach (var p in GetCleanupAssembliesScripts(sysdepends, customdepends, assembly)) {
                yield return p;
            }
            //4 generate assemblies
            if (!_args.CleanOnly) {
                foreach (var p in GetCreateCustomLibrariesScripts(args, customdepends, assembly)) {
                    yield return p;
                }
                foreach (var item in items.Where(x => (x is UserTypeWrapper))) {
                    yield return item.GetCreateScript();
                }
                foreach (var item in items.Where(x => !(x is UserTypeWrapper))) {
                    yield return item.GetCreateScript();
                }
            }
        }

        private static IEnumerable<string> GetCreateCustomLibrariesScripts(SqlInstallerConsoleProgramArgs args,
            List<AssemblyName> customdepends,
            Assembly assembly) {
            var dir = Path.GetDirectoryName(Path.GetFullPath(args.AssemblyName + ".dll"));
            foreach (var dep in customdepends) {
                var source = QueryGeneratorHelper.GetAssemblyBits(Path.Combine(dir, dep.Name + ".dll"));
                if (args.UseAssemblyPath) {
                    source = "'" + Path.Combine(dir, dep.Name + ".dll") + "'";
                }
                yield return
                    string.Format(
                        @"
--SQLINSTALL: Create assembly {0}
if not exists(select assembly_id from sys.assemblies where name='{0}') 
CREATE ASSEMBLY [{0}]
AUTHORIZATION dbo
FROM {1}
WITH PERMISSION_SET = UNSAFE
else begin
	begin try 
		ALTER ASSEMBLY [{0}]
		FROM {1}
		WITH PERMISSION_SET = UNSAFE
	end try
	begin catch
		declare @message nvarchar(max) set @message = ERROR_MESSAGE()
		if @message not like '%MVID%' begin
			declare @severity int set @severity = ERROR_SEVERITY()
			declare @state int set @state = ERROR_STATE()
			raiserror(@message,@severity,@state)
		end
	end catch	
end
",
                        dep.Name, source);
            }
            var source2 = QueryGeneratorHelper.GetAssemblyBits(Path.Combine(dir, assembly.GetName().Name + ".dll"));
            if (args.UseAssemblyPath) {
                source2 = "'" + Path.Combine(dir, assembly.GetName().Name + ".dll") + "'";
            }
            yield return
                string.Format(
                    @"
--SQLINSTALL: Create assembly {0}
CREATE ASSEMBLY [{0}]
AUTHORIZATION dbo
FROM {1}
WITH PERMISSION_SET = {2}
",
                    assembly.GetName().Name, source2, args.Safe ? "SAFE" : "UNSAFE")
                ;
        }

        private IEnumerable<string> GetCleanupAssembliesScripts(IEnumerable<AssemblyName> sysdepends,
            IEnumerable<AssemblyName> customdepends,
            Assembly assembly) {
            yield return
                string.Format(
                    @"
--SQLINSTALL: Drop Existed Library
if exists(select assembly_id from sys.assemblies where name='{0}') 
drop assembly [{0}]
",
                    assembly.GetName().Name);
            if (!_args.NoDropCustomDependency) {
                foreach (var dep in customdepends.Reverse()) {
                    yield return
                        string.Format(
                            @"
--SQLINSTALL: Drop Existed Library
if exists(select assembly_id from sys.assemblies where name='{0}') 
drop assembly [{0}]
",
                            dep.Name);
                }
            }
            if (!_args.CleanOnly) {
                foreach (var sysasm in sysdepends) {
                    var codebase = sysasm.CodeBase;
                    if (!string.IsNullOrWhiteSpace(codebase)) {
                        codebase = codebase.Replace("file:///", "");
                    }
                    else {
                        codebase = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\" + sysasm.Name + ".dll";
                        if (!File.Exists(codebase)) {
                            codebase =
                                @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5\" +
                                sysasm.Name + ".dll";
                        }
                        if (!File.Exists(codebase)) {
                            codebase =
                                @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.0\" +
                                sysasm.Name + ".dll";
                        }
                    }
                    yield return
                        string.Format(
                            @"
--SQLINSTALL: Recreate system lib {0}
begin try
if not exists(select assembly_id from sys.assemblies where name='{0}') 
Create assembly [{0}] 
from '{1}' 
with permission_set = unsafe 
end try
begin catch
end catch
",
                            sysasm.Name, codebase);
                }
            }
        }

        private IEnumerable<SqlExportMemberWrapper> GetExportedItems(Assembly assembly, string schema) {
            foreach (var type in assembly.GetTypes()) {
                if (type.IsSealed && type.IsAbstract) {
//static class
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
                        var mw = SqlExportMemberWrapper.Create(method, schema);
                        if (null != mw) {
                            foreach (var sqlExportMemberWrapper in mw) {
                                yield return sqlExportMemberWrapper;
                            }
                        }
                    }
                }
                else {
                    var tw = SqlExportMemberWrapper.Create(type, schema);
                    if (null != tw) {
                        foreach (var sqlExportMemberWrapper in tw) {
                            yield return sqlExportMemberWrapper;
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> GetDatabasePrepareScripts(SqlInstallerConsoleProgramArgs args, string dbname) {
            yield return string.Format(
                @"--SQLINSTALL: USE DATABASE
use {0}", dbname);
            yield return string.Format(
                @"--SQLINSTALL: SET DATABASE TRUSTWORTHY
ALTER DATABASE {0} SET TRUSTWORTHY ON", dbname);
            yield return
                @"--SQLINSTALL: SET CLR ON
sp_configure 'clr enabled', 1
";
            yield return
                @"--SQLINSTALL: RECONFIGURE
reconfigure
";
            yield return
                @"--SQLINSTALL: Normalize owner
EXEC sp_changedbowner 'sa' 
";
            yield return string.Format(
                @"--SQLINSTALL: Normalize authorization
ALTER AUTHORIZATION ON DATABASE::{0} to sa
", dbname);
        }
    }
}