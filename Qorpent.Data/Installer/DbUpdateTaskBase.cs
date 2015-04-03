using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using Qorpent.Tasks;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.IO;

namespace Qorpent.Data.Installer
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class DbUpdateTaskBase : UpdateTaskBase {
        private string _databaseName;
        private IDbCommandExecutor _dbExecutor;

        /// <summary>
        /// Перекрыть для получения самих скриптов
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<string> GetScripts();

        /// <summary>
        /// 
        /// </summary>
        public string Database {
            get {
                if (!string.IsNullOrWhiteSpace(_databaseName)) return _databaseName;
                if (null==Job)throw new Exception("not part of job");
                if (!Job.Data.ContainsKey("database"))throw new Exception("database name not exists in job");
                if (string.IsNullOrWhiteSpace(Job.Data["database"].ToStr()))throw new Exception("database name is empty in job");
                return Job.Data["database"].ToStr();
            }
            set { _databaseName = value; }
        }


        public FileDescriptorEx GetMeta(string name = null) {
            if (string.IsNullOrWhiteSpace(name)) {
                name = Source.Name;
            }
            var cmd = InitCommand(
               "select hash,version from qorpent.meta where Code = @code",
                DbCallNotation.SingleRow);
            cmd.ParametersSoruce = new { code = name };
            var result = (IDictionary<string, object>)DbExecutor.GetResultSync(cmd);
            var meta = null == result ? new FileDescriptorEx { Name = Source.Name, Hash = "INIT", Version = DateTime.MinValue } : new FileDescriptorEx { Name = Source.Name, Hash = result["hash"].ToStr(), Version = result["version"].ToDate() };
            return meta;
        }

        public override void Initialize(IJob package = null) {
            base.Initialize(package);
            SetupTargetDescriptor();
        }

        protected override void DoLateTargetReset() {
            Target = GetMeta();
        }

        private void SetupTargetDescriptor() {
            if (null == Source && !string.IsNullOrWhiteSpace(MetaName)) {
                Source = GetSelfMetaDesc();
            }
            if(null==Source)return;
            try {
                Target = GetMeta();
            }
            catch (SqlException) {
                ResetTargetLater = true;
            }
        }

        protected FileDescriptorEx GetSelfMetaDesc(string name = null) {
            if (string.IsNullOrWhiteSpace(name)) {
                name = MetaName;
            }
            var versionstring = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var hash = "INSTALLED_" + versionstring;
            var version = new DateTime(2000, 1, 1);
            var offset = versionstring.Replace(".", "").ToInt();
            version = version.AddSeconds(offset);
            var src = new FileDescriptorEx {Name = name, Hash = hash, Version = version};
            return src;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommandExecutor DbExecutor {
            get {
                if (null != _dbExecutor) return _dbExecutor;
                if (null != Job) {
                    if (Job.Data.ContainsKey("dbexecutor") && null != Job.Data["dbexecutor"] as IDbCommandExecutor) {
                        return (IDbCommandExecutor) Job.Data["dbexecutor"];
                    }
                }
                return DbCommandExecutor.Default;
            }
            set { _dbExecutor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string QueryDatabase {
            get {
                if (string.IsNullOrWhiteSpace(_queryDatabase)) return Database;
                    
                return _queryDatabase;
            }
            set { _queryDatabase = value; }
        }

        public override void Refresh() {
            base.Refresh();
            SetupTargetDescriptor();
        }

        protected DbCommandWrapper InitCommand(string query = null, DbCallNotation notation = DbCallNotation.None) {
            
            var result = new DbCommandWrapper {
                ConnectionString = ConnectionString, 
                Database = QueryDatabase,
                ParametersSoruce = CmdParameters,
                Notation =  notation
            };
            if (!string.IsNullOrWhiteSpace(query)) {
                var si = new StringInterpolation();
                query = si.Interpolate(query, CmdParameters);
                if (query.Contains(" ")) {
                    result.Query = query;
                }
                else {
                    result.ObjectName = query;
                }
            }
            result.OnMessage = (wrapper, s) => internalLog.WriteLine(s);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public string ConnectionString {
            get {
                if (!string.IsNullOrWhiteSpace(_connectionString)) return _connectionString;
                if (null != Job && Job.Data.ContainsKey("connection") &&
                    !string.IsNullOrWhiteSpace(Job.Data["connection"].ToStr())) {
                    return Job.Data["connection"].ToStr();
                }
                return "Server=(local);Trusted_Connection=true;";
            }
            set { _connectionString = value; }
        }

        StringWriter internalLog = new StringWriter();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetLastLog() {
            return internalLog.ToString();
        }

        private IDictionary<string, object> _cmdParameters;
        private string _queryDatabase;
        private string _connectionString;

        protected IDictionary<string, object> CmdParameters {
            get {
                if (null == _cmdParameters) {
                    _cmdParameters = new ConcurrentDictionary<string, object>();
                    if (null != Job)
                    {
                        foreach (var o in Job.Data)
                        {
                            _cmdParameters[o.Key] = o.Value;
                        }
                    }
                    _cmdParameters["database"] = Database;
                    _cmdParameters["name"] = Name;
                    _cmdParameters["grp"] = Group;
                    foreach (var o in Data)
                    {
                        _cmdParameters[o.Key] = o.Value;
                    }
                    if (null != Source && null != Source.Header)
                    {
                        _cmdParameters["xml"] = Source.Header;
                        foreach (var attribute in Source.Header.Attributes())
                        {
                            _cmdParameters[attribute.Name.LocalName] = attribute.Value;
                        }
                    }
                }
                return _cmdParameters;
            }
        }

        public string MetaName { get; set; }

        protected override void InternalWork() {
            internalLog = new StringWriter();
            int count = 0;
            foreach (var script in GetScripts().ToArray()) {
                try {
                    Log.Trace("start script "+count);
                    internalLog.WriteLine("start script "+count);
                    ExecuteScript(script);
                    internalLog.WriteLine("finish script " + count);
                    internalLog.WriteLine(script);
                    Log.Trace("success script " + count);
                    count++;
                }
                catch (Exception e) {
                    Log.Warn("error script " + count+" "+e.Message,e);
                    internalLog.WriteLine("error script "+count+" "+e.Message);
                    if (!IgnoreErrors) {
                        throw;
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        protected override void FixSuccess() {
            base.FixSuccess();
            if (null != Source) {
                SaveMeta(Source);
            }
            
        }

        protected void SaveMeta(IVersionedDescriptor src) {
            var cmd = InitCommand("qorpent.metaupdate");
            cmd.ParametersSoruce = new {code = src.Name, hash = src.Hash, version = src.Version.AddSeconds(-1)};
            DbExecutor.Execute(cmd).Wait();
            if (!cmd.Ok) {
                throw cmd.Error;
            }
        }

        private void ExecuteScript(string script) {
            var cmd = InitCommand(script);
            DbExecutor.Execute(cmd).Wait();
            if (!cmd.Ok) {
                throw cmd.Error;
            }
        }
    }
}
