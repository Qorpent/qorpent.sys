using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.XDiff;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// Процессор по умолчанию
	/// </summary>
	public class DefaultMetaFileProcessor : IMetaFileProcessor{
		private string connection;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> alldeltas;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> working;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> error;
		/// <summary>
		/// 
		/// </summary>
		protected LinkedList<DatabaseUpdateRecord> ignored;
		
		/// <summary>
		/// 
		/// </summary>
		protected DatabaseUpdateRecord[] commands;

		private LinkedList<DatabaseUpdateRecord> grouped;
		private IDatabaseUpdateRecordMerger _merger;

		/// <summary>
		/// 
		/// </summary>
		public IDatabaseUpdateRecordMerger Merger{
			get { return _merger ?? (_merger=new DefaultDatabaseUpdateRecordMerger()); }
			set { _merger = value; }
		}


		bool Online{
			get { return !string.IsNullOrWhiteSpace(connection); }
		}

		/// <summary>
		/// Встраивает выявленное изменение в общий лог
		/// </summary>
		public void Prepare(string sqlconnection, IEnumerable<DatabaseUpdateRecord> givendelta, LinkedList<DatabaseUpdateRecord> workinglist, LinkedList<DatabaseUpdateRecord> errorlist){
			lock (this){
				connection = sqlconnection;
				alldeltas = new LinkedList<DatabaseUpdateRecord>(givendelta);
				grouped = new LinkedList<DatabaseUpdateRecord>(DatabaseUpdateRecord.Group(alldeltas).Values.Select(_=>Merger.Merge(_)));
				working = workinglist;
				error = errorlist;
				CheckTables();
				foreach (var record in grouped){
					working.AddLast(record);
				}
			}
			
		}
		

		IDbConnection getc(){
			return Applications.Application.Current.DatabaseConnections.GetConnection(connection);
		}
		/// <summary>
		/// 
		/// </summary>
		protected virtual  void CheckTables(){
			if (!Online) return;
			var tables = grouped.GroupBy(_ => _.FullTableName, _ => _);
			var query = string.Join("\r\nunion\r\n", tables.Select(_ =>string.Format("select '{0}' as code,object_id('{0}') as id ",_)));
			var dict = getc().ExecuteDictionaryReader(query);
			var excludes = dict.Where(_ => _.Value == null).Select(_ => _.Key).ToArray();
			foreach (var exclude in excludes){
				foreach (var i in tables.First(_=>_.Key==exclude)){
					i.ErrorCode = "EXT";
					i.Error = "таблица отсутствует в БД (" + i.FullTableName + ")";
					alldeltas.Remove(i);
					error.AddFirst(i);
				}
			}
		}

		/// <summary>
		/// Выполняет обновления
		/// </summary>
		/// <param name="sqlconnection"></param>
		/// <param name="records"></param>
		public void Execute(string sqlconnection, IEnumerable<DatabaseUpdateRecord> records){
			lock (this){
				var script = GetSql(records);
				using (var c = getc()){
					c.Open();
					//var t = c.BeginTransaction();
					try{
						var com = c.CreateCommand(script);
						com.ExecuteNonQuery();
					//	t.Commit();
						
					}
					catch {
						//t.Rollback();
						throw;
					}
				}
			}
		}

		/// <summary>
		/// Сформировать SQL по указанным командам
		/// </summary>
		/// <param name="records"></param>
		/// <param name="comments"></param>
		/// <returns></returns>
		public string GetSql(IEnumerable<DatabaseUpdateRecord> records, bool comments =false){
			commands = records.ToArray();
			CheckSql(comments);
			var script = CollectSql(comments);
			return script;
		}
		/// <summary>
		/// 
		/// </summary>
		public string LastSql { get; private set; }

		private string CollectSql(bool comments){
			var sb = new StringBuilder();
			var tables = commands.Select(_ => _.FullTableName.ToLower()).Distinct();
			sb.Append(@" begin tran
 begin try
");
			foreach (var table in tables){
				sb.AppendLine(string.Format("alter table {0} nocheck constraint all", table));
			}
			foreach (var databaseUpdateRecord in commands){
				if (comments){
					sb.AppendLine();
					sb.AppendLine("/* " + databaseUpdateRecord + " */");
				}
				sb.AppendLine(databaseUpdateRecord.BaseSqlCommand);
			}

			foreach (var databaseUpdateRecord in commands)
			{
				if (!string.IsNullOrWhiteSpace(databaseUpdateRecord.ExtendedSqlCommand)){
					sb.AppendLine(databaseUpdateRecord.ExtendedSqlCommand);
				}
			}
			sb.AppendLine("declare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))");
			foreach (var table in tables)
			{
				sb.AppendLine(string.Format("alter table {0} check constraint all", table));
			}
			foreach (var table in tables)
			{
				sb.AppendLine(string.Format("insert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''{0}'')'", table));
			}
			sb.Append(@"if ((select count(*) from @c)!=0) begin
			select * from @c;
			throw 51012 , 'patch violates constraints', 1;
		end else commit
 end try
 begin catch
	if (@@TRANCOUNT!=0) rollback;
	throw;
 end catch
");
			LastSql = sb.ToString();
			return LastSql;
		}

		private void CheckSql(bool comments){
			foreach (var command in commands){
				if (string.IsNullOrWhiteSpace(command.BaseSqlCommand)){
					command.BaseSqlCommand = BuildBaseSql(command);
					command.ExtendedSqlCommand = BuildExtendedSql(command);
				}
			}
		}

		private string BuildExtendedSql(DatabaseUpdateRecord command){
			var sb = new StringBuilder();
			if (command.DiffItem.Action == XDiffAction.CreateElement )
			{
				var dict = CreateExtendedFieldDictionary(command);
			
				if (dict.Count != 0){
					var where = "";
					if (command.TargetId != 0){
						where = "Id = " + command.TargetId;
					}
					else{
						where = "Code = '" + command.TargetCode.ToSqlString() + "'";
					}
					foreach (var p in dict){
						sb.AppendLine(string.Format("update {0} set {1} = {2} where {3}", command.FullTableName, p.Key, p.Value,where));
					}
				}
			}
			return sb.ToString();

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected virtual  string BuildBaseSql(DatabaseUpdateRecord command){
			if (command.DiffItem.Action == XDiffAction.CreateElement){
				return BuildCreateNewSql(command);
			}
			return "-- it will be nice";
		}

	

		/// <summary>
		/// 
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		protected virtual string BuildCreateNewSql(DatabaseUpdateRecord command){
			var dict_ = CreateBaseFieldDictionary(command);
			if (!dict_.ContainsKey("Active")){
				dict_["Active"] = "1";
			}
			if (!dict_.ContainsKey("Version")){
				dict_["Version"] = "(getdate())";
			}
			var dict = dict_.ToArray();
			var fieldlist = string.Join(", ", dict.Select(_ => _.Key));
			var valuelist = string.Join(", ", dict.Select(_ => GetSqlVar(_.Value)));
			
			var where = "";
			var update = "";
			if (dict_.ContainsKey("Id") && !command.DiffItem.Options.ChangeIds){
				where = "Id = " + dict_["Id"].ToInt();
				update = CollectUpdates(dict.Where(_ => _.Key != "Id"));
			}
			else{
				where = "Code = '" + dict_["Code"].ToSqlString() + "'";
				update = CollectUpdates(dict.Where(_ => _.Key != "Code"));
			}
			return string.Format(@"
if exists (select top 1 id from {0} where {3}) 
update {0} set {4} where {3}
else insert {0} ({1}) values ({2})", command.FullTableName.ToLower(), fieldlist, valuelist,where,update);
		}

		private string GetSqlVar(string value){
			if (value.ToInt() != 0) return value;
			if (value.StartsWith("(") && value.EndsWith(")")) return value;
			return "'" + value.ToSqlString() + "'";
		}

		private string CollectUpdates(IEnumerable<KeyValuePair<string, string>> set){
			return string.Join(", ", set.Select(_ => _.Key + " = " + GetSqlVar(_.Value)));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual IDictionary<string, string> CreateExtendedFieldDictionary(DatabaseUpdateRecord cmd)
		{

			XElement e = cmd.DiffItem.NewestElement;
			var result = new Dictionary<string, string>();
			foreach (var a in e.Attributes())
			{
				if (null != cmd.DiffItem.Options.RefMaps){
					var mapkey = "ref-" + cmd.Schema + "-" + cmd.TargetTable + "-" + a.Name.LocalName.ToLower();
					if (cmd.DiffItem.Options.RefMaps.ContainsKey(mapkey)){
						//no reference
						if (a.Value != "0" && a.Value != "" && a.Value.ToInt() == 0){
							result[a.Name.LocalName] = "(select Id from " + cmd.DiffItem.Options.RefMaps[mapkey] + " where Code = '" +
							                           a.Value.ToSqlString() + "')";
							continue;
						}
						
					}
				}
				if (a.Name.LocalName.ToLower() == "parent")
				{
					//no reference
					if (a.Value != "0" && a.Value != "" && a.Value.ToInt() == 0)
					{
						result[a.Name.LocalName] = "(select Id from " + cmd.FullTableName + " where Code = '" +
												   a.Value.ToSqlString() + "')";
					}
					
				}

			}
			
			return result;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual IDictionary<string, string> CreateBaseFieldDictionary(DatabaseUpdateRecord cmd){

			XElement e = cmd.DiffItem.NewestElement;
			var result = new Dictionary<string, string>();
			var tag = "";
			foreach (var a in e.Attributes()){
				if (a.Name.LocalName.ToLower() == "id"){
					if (a.Value.ToInt() != 0){
						result["Id"] = a.Value;
					}
					continue;
					
				}
				if (a.Name.LocalName.ToLower() == "code")
				{
					if (!string.IsNullOrWhiteSpace(a.Value))
					{
						result["Code"] = a.Value;
					}
					continue;
				}
				if (a.Name.LocalName.ToLower() == "name")
				{
					if (!string.IsNullOrWhiteSpace(a.Value))
					{
						result["Name"] = a.Value;
					}
					continue;
				}

				if (a.Name.LocalName.StartsWith("tag.") || a.Name.LocalName.StartsWith("tag-"))
				{
					var tagname = a.Name.LocalName.Substring(4);
					tag = TagHelper.SetValue(tag, tagname, a.Value);
					continue;
				}

				if (null != cmd.DiffItem.Options.RefMaps){
					var mapkey = "ref-" + cmd.Schema + "-" + cmd.TargetTable + "-" + a.Name.LocalName.ToLower();
					if (cmd.DiffItem.Options.RefMaps.ContainsKey(mapkey)){
						//no reference
						if (a.Value != "0" && a.Value != "" && a.Value.ToInt() == 0){
							continue; //this field must be set further
						}
						else{
							result[a.Name.LocalName] = a.Value.ToInt().ToString();
							continue;

						}
					}
				}
				if (a.Name.LocalName.ToLower() == "parent"){
					//no reference
					if (a.Value != "0" && a.Value != "" && a.Value.ToInt() == 0)
					{
						continue; //this field must be set further
					}
					else
					{
						result[a.Name.LocalName] = a.Value.ToInt().ToString();
						continue;

					}
				}

				result[a.Name.LocalName] = a.Value;
			}
			if (!string.IsNullOrWhiteSpace(e.Value)){
				if (e.Value.StartsWith("<")){
					result["Data"] = e.Value;
				}
				else{
					result["Comment"] = e.Value;
				}
			}
			if (!string.IsNullOrWhiteSpace(tag)){
				result["Tag"] = tag;
			}
			return result;
		}
	}
}