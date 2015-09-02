using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Elasticsearch.Net.Connection;
using Npgsql;
using qorpent.tasks.processor;
using Qorpent;
using Qorpent.Log.NewLog;
using Qorpent.Mvc.Remoting;
using Qorpent.Utils.Extensions;

namespace qorpent.tasks.tasks.database
{
    public abstract class DbTaskBase : TaskBase {
        public string ConnectionName { get; set; }
        public string Database { get; set; }
        public override void Initialize(TaskEnvironment environment, ITask parent, XElement config) {
            base.Initialize(environment, parent, config);
            ConnectionName = config.Attr("connection", "default");
            Database = config.Attr("database");

        }

        public IDbConnection GetConnection(string name,IScope scope) {
            name = Interpolate((name ?? ConnectionName), scope);

            var xcon = ResolveConnectionDefinition(name,Config,scope);
            if (null == xcon) {
                throw new Exception("cannot find connection with code "+name);
            }
            var type = xcon.Attr("type", "mssql");
            var cstring = xcon.Value;
            IDbConnection result = null;
            if (type.StartsWith("pg")) {
                result = new NpgsqlConnection(cstring);
            }else if (type.StartsWith("ms")) {
                result = new SqlConnection(cstring);
            }else if (type == "odbc") {
                result = new OdbcConnection(cstring);
            }else if (type == "oledb") {
                result = new OleDbConnection(cstring);
            }
            if (!string.IsNullOrWhiteSpace(Database)) {
                var db = Interpolate(Database, scope);
                result.ChangeDatabase(db);
                L.Debug("open connection '" + name + "' as " + result.GetType().Name + " : " + cstring + " as " + db);
            }
            else {
                L.Debug("open connection '" + name + "' as " + result.GetType().Name + " : " + cstring);
            }
            return result;
        }

        protected XElement ResolveConnectionDefinition(string realName, XElement config, IScope scope) {
            if (null!=scope && scope.ContainsKey("connection_" + realName + "_def")) {
                return scope["connection_" + realName + "_def"] as XElement;
                
            }
            var local = config.Elements("connection").FirstOrDefault(_ => _.Attr("code") == realName);
            if(null!=local) {
                return local;
            }
            if (null != config.Parent) {
                return ResolveConnectionDefinition(realName,config.Parent,null);
            }
            return null;
        }
    }
}
