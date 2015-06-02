using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.IoC;
using Qorpent.Tasks;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.Installer {
    /// <summary>
    /// </summary>
    public static class DbInstallFactory {
        public static IJob Create(string database = null, bool dropdatabase  = false) {
            var result = new Job();
            Setup(result, database, dropdatabase);
            return result;
        }

        public static void Setup(IJob job, string database = null, bool dropdatabase = false) {
            if (dropdatabase) {
                if (!job.Tasks.ContainsKey("dropdb")) {
                    job.Tasks["dropdb"] = new DropDatabaseTask();
                }
            }
            if (!job.Tasks.ContainsKey("initdb")) {
                job.Tasks["initdb"] = new InitDatabaseTask();
            }
            if (!job.Tasks.ContainsKey("initmeta")) {
                job.Tasks["initmeta"] = new InitMetaTableTask();
            }
            if (!job.Tasks.ContainsKey("initclr")) {
                job.Tasks["initclr"] = new InitClrTask();
            }
            if (!string.IsNullOrWhiteSpace(database)) {
                job["database"] = database;
            }
        }

        public static void InitTestDatabase(string name = null) {
            if (String.IsNullOrWhiteSpace(name))
            {
                var method = new StackFrame(1, true).GetMethod();
                name = method.DeclaringType.Name + "_" + method.Name;
            }
            Create(name,true).Execute();
        }

        /// <summary>
        /// Формирует полную инсталляцию для текущего сервера
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IJob[] CreateServerInstallation(IBSharpContext context, string logicalServerName = null, string basedir = null) {
            
                if (string.IsNullOrWhiteSpace(logicalServerName)) logicalServerName = Environment.MachineName;
                var serverDefinition = FindMatchedServerDefinition(context, logicalServerName);
                var baseConfig = InitializeBaseConfig(serverDefinition, context, logicalServerName);
                var root = string.IsNullOrWhiteSpace(basedir) ? EnvironmentInfo.GetRepositoryRoot() : EnvironmentInfo.ResolvePath(basedir);
                baseConfig["root"] = root;
            baseConfig["resolveoverrides"] = new Dictionary<string, string> {
                {"repos", root},
                {"bin", Path.Combine(root, ".build/bin/all")},
                {"abin", Path.Combine(root, ".build/bin/all")}
            };
            
                var dbdefs = CollectDbDefinitions(serverDefinition, context, logicalServerName).ToArray();
                var jobs = dbdefs.Select(_ => CreateDatabaseInstallation(_, baseConfig)).ToArray();
                return jobs;
            
        }

        private static IEnumerable<XElement> CollectDbDefinitions(XElement serverDefinition, IBSharpContext context, string logicalServerName) {
            foreach (var e in serverDefinition.Elements("db").ToArray()) {
                var theref = e.Attr("ref");
                if (theref.ToBool()) {
                    foreach (var dbdef in CollectReferencedDatabases(theref, context)) {
                        yield return dbdef;
                    }
                }
                else {
                    yield return e;
                }
            }
        }

        private static IEnumerable<XElement> CollectReferencedDatabases(string theref, IBSharpContext context) {
            if (theref == "defaults") {
                foreach (var cls in context.ResolveAll("dbdef")) {
                    if (cls.Compiled.Attr("isdefault").ToBool()) {
                        yield return cls.Compiled;
                    }
                }
            }
            else {
                var db = context.Get(theref);
                if (null == db) {
                    throw new Exception("cannot find db with cls name "+theref);
                }
                yield return db.Compiled;
            }
        }

        private static IScope InitializeBaseConfig(XElement serverDefinition, IBSharpContext context, string logicalServerName) {
            var result = new Scope();
            result["definition"] = serverDefinition;
            result["baseservername"] = serverDefinition.ChooseAttr("server", "code");
            result["servername"] = logicalServerName;
            result["conneciton"] = serverDefinition.Attr("connection", "Server=(local);Trusted_Connection=true");
            return result;
        }

        private static XElement FindMatchedServerDefinition(IBSharpContext context, string logicalServerName) {
            var match =
                context.ResolveAll("dbserver")
                    .Select(_ => IsMatchServer(_, logicalServerName))
                    .Where(_ => _.Item1)
                    .OrderBy(_ => _.Item3)
                    .FirstOrDefault();
            if (null != match) {
                return match.Item2;
            }
            return XElement.Parse("<server code='*'><db ref='defaults'/></server>");
        }

        private static Tuple<bool,XElement,int> IsMatchServer(IBSharpClass bSharpClass, string logicalServerName) {
            var tgname = logicalServerName.Simplify(SimplifyOptions.Full);
            var srname = bSharpClass.Compiled.ChooseAttr("server", "code").Simplify(SimplifyOptions.Full);
            if (srname == "*") {
                return new Tuple<bool, XElement, int>(true,bSharpClass.Compiled,1000);
            }
            if (srname.EndsWith("*") && tgname.StartsWith(srname.Substring(0, srname.Length - 1))) {
                return new Tuple<bool, XElement, int>(true, bSharpClass.Compiled, 100);
            }
            if (srname == tgname) {
                return new Tuple<bool, XElement, int>(true, bSharpClass.Compiled, 10);
            }
            return new Tuple<bool, XElement, int>(false,null,0);
        }


        /// <summary>
        /// Generates job for single B# defined database 
        /// </summary>
        /// <param name="definition"></param>
        /// <returns></returns>
        public static IJob CreateDatabaseInstallation(XElement definition, IScope context = null) {
            var dbname = definition.ChooseAttr("dbname", "code");
            var job = Create(dbname);
            if(null!=context)job.SetParent(context);
            Setup(job,definition,context);
            return job;
        }
        /// <summary>
        /// Настраивает задачу для класса B#
        /// </summary>
        /// <param name="job"></param>
        /// <param name="definition"></param>
        /// <param name="context"></param>
        public static void Setup(IJob job, XElement definition, IScope context)
        {
            Setup(job); //apply initial setup
            job["database"] = definition.ChooseAttr("dbname", "code");
            foreach (var element in definition.Elements()) {
                if (element.Name.LocalName == "script") {
                    SetupScript(job, definition, context);
                }
#if !UNIX
                else if (element.Name.LocalName == "assembly") {
                    SetupAssembly(job, definition, context);
                }
#endif
                else if (element.Name.LocalName == "model") {
                    SetupModel(job, definition, context, true, true);
                }
                else if (element.Name.LocalName == "schema") {
                    SetupModel(job, definition, context, true, false);
                }
                else if (element.Name.LocalName == "data") {
                    SetupModel(job, definition, context, false, true);
                }
            }
        }

        private static void SetupModel(IJob job, XElement definition, IScope context, bool schema, bool data)
        {
            var info = GetTaskInfo(job, definition);
            if (!info.Proceed) return;
            if (schema) {
                var st = new BxlDataProjectSchemaUpdateTask(info.Name) {Job = job,Name = info.Key+".schema"};
                job.Tasks[info.Key + ".schema"] = st;
            }
            if (data)
            {
                var dt = new BxlDataProjectDataUpdateTask(info.Name) { Job = job, Name = info.Key + ".data" };
                job.Tasks[info.Key + ".data"] = dt;
            }

        }
#if !UNIX
        private static void SetupAssembly(IJob job, XElement definition, IScope context)
        {
            var info = GetTaskInfo(job, definition);
            if(!info.Proceed)return;
            job.Tasks[info.Key] = new AssemblyDbUpdateTask(info.Name) {Job = job,Name=info.Key};
        }

#endif
        class TaskInfo {
            public bool Proceed { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public int Idx { get; set; }
            
        }

        private static TaskInfo GetTaskInfo(IJob  job,XElement definition) {
            var code = definition.Attr("code");
            var name = definition.Attr("name");
            var key = code;
            if (string.IsNullOrWhiteSpace(name))
            {
                key = code.GetMd5();
                name = code;
            }

            if (job.Tasks.ContainsKey(key)) return new TaskInfo();
            return new TaskInfo {
                Proceed = true,
                Code = code,
                Name = name,
                Key = key,
                Idx = definition.Attr("idx").ToInt(),
                Value = definition.Value
            };
        }
        private static void SetupScript(IJob job, XElement definition, IScope context)
        {
            var info = GetTaskInfo(job, definition);
            if (!info.Proceed) return;
            ITask task;
            if (!string.IsNullOrWhiteSpace(definition.Value)) {
                 task = new ScriptTextDbUpdateTask(info.Key, definition.Value, definition.Attr("hash")) {Job = job,Idx=info.Idx};
            }
            else {
                task = new ScriptFileDbUpdateTask(info.Name,info.Key+".") { Job = job, Idx = info.Idx };
            }
            job.Tasks[info.Key] = task;
        }
    }
}