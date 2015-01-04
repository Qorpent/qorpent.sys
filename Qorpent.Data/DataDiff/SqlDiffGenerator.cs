using System;
using System.IO;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.DataDiff{
	/// <summary>
	/// Формаир
	/// </summary>
	public class SqlDiffGenerator{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public SqlDiffGenerator(TableDiffGeneratorContext context){
			this._context = context;
			if (null == _context.Tables){
				throw new Exception("SqlDiffGenerator: tables are not defined");
			}
			if (null == _context.SqlOutput){
				_context.SqlOutput = sw;
			}
			this._tables = _context.Tables.OrderBy(_=>_.TableName).ToArray();
			var cnt = _tables.SelectMany(_ => _.Definitions).Count();
			if (cnt >= 10000){
				this._filegroups = _tables.GroupBy(_ => _.ScriptFile).ToArray();
			}
			else{
				this._filegroups = _tables.GroupBy(_ => "MAIN").ToArray();
			}
			this._output = _context.SqlOutput;
			
		}
		private StringWriter sw = new StringWriter();
		private DataDiffTable[] _tables;
		private TextWriter _output;
		private TableDiffGeneratorContext _context;
		private IGrouping<string, DataDiffTable>[] _filegroups;
		private string _script;

		/// <summary>
		/// Записать скрипт в переданный поток
		/// </summary>
		public void Generate(){
			
			
			foreach (var tablegroup in _filegroups.OrderBy(_=>_.Key)){
				_tables = tablegroup.ToArray();
				_script = tablegroup.Key;
				_context.Log.Trace("start : "+_script );
				WriteStart();
				WriteBody();
				WriteFinish();
				if (_context.SqlOutput == sw)
				{
					_context.SqlScripts.Add( sw.ToString());
					_output = _context.SqlOutput = sw = new StringWriter();
				}
				_context.Log.Trace("finish : " + _script);
			}
			
			
		}

		private void WriteFinish(){
			_output.WriteLine("\t-- return of foreign keys contraints");
			foreach (var table in _tables)
			{
				_output.WriteLine("\talter table {0} check constraint all", table.TableName);
				foreach (var disableIndex in table.DisableIndexes)
				{
					_output.WriteLine("\tALTER INDEX {0} ON {1} REBUILD", disableIndex, table.TableName);
				}
			}
			foreach (var table in _tables)
			{
				_output.WriteLine("\tinsert @c (t,c,w) exec sp_executesql N'DBCC CHECKCONSTRAINTS (''{0}'')'", table.TableName);
			}
			_output.WriteLine(@"	-- this code allows late control over foreighn keys
		if ((select count(*) from @c)!=0) begin
		select * from @c;
		throw 51012 , 'patch violates constraints', 1;
	end else commit");
			_output.WriteLine("END TRY");
			_output.WriteLine("BEGIN CATCH");
			_output.WriteLine("\tif (@@TRANCOUNT!=0) rollback;");
			_output.WriteLine("\tthrow");
			_output.WriteLine("END CATCH");
			_output.WriteLine("-- END OF SCRIPT: "+_script);
		}

		private void WriteBody(){
			int i = 1;
			foreach (var table in _tables){
				var tn = "t" + i++;
				var allfields = table.Definitions.SelectMany(_ => _.Fields.Keys).Distinct().OrderBy(_=>_).ToArray();
				_output.WriteLine("\t--"+table.TableName+ " base insert and update");	
				GenerateTempTableDeclaration(tn, allfields, table);
				GenerateInsertTemp(tn, allfields, table);
				GenerateExistsBinding(tn, allfields, table);
				
			}
			i = 1;
			foreach (var table in _tables)
			{
				var tn = "t" + i++;
				var allfields = table.Definitions.SelectMany(_ => _.Fields.Keys).Distinct().OrderBy(_ => _).ToArray();
				_output.WriteLine("\t--" + table.TableName + " fk_binding");	
				GenerateFKBinding(tn, allfields, table);
			}
			i = 1;
			foreach (var table in _tables)
			{
				var tn = "t" + i++;
				var allfields = table.Definitions.SelectMany(_ => _.Fields.Keys).Distinct().OrderBy(_ => _).ToArray();
				_output.WriteLine("\t--" + table.TableName + " main update");
				GenerateMainUpdate(tn, allfields, table);
			}
		}

		private void GenerateMainUpdate(string tn, string[] allfields, DataDiffTable table){
			_output.Write("\t\tupdate {0} set ",table.TableName);
			for (var i = 0;i<allfields.Length;i++){
				var name = allfields[i];
				if (table.NoCode && name == "code") continue;
				if (table.NoCode && name == "set_code") continue;
				if (!table.UseAliasCodes && name == "aliascodes") continue;
				
				if (name == "set_id"){
					name = "id";
				}
				if (name == "set_code"){
					name = "code";
				}
				if (name == "set_parent"){
					name = "parent";
				}
				_output.Write("{0} = isnull(x.{2}{0},{1}.{0})", name, table.TableName, (name == "id" || name == "code") ? "set_" : "");
				if (i != allfields.Length - 1){
					_output.Write(", ");
				}
                _output.Write(", ");
                _output.Write("version =getdate()");
			}
			_output.WriteLine("from @{0} x join {1} on x.id = {1}.id ",tn,table.TableName);
		}

		private void GenerateFKBinding(string tn, string[] allfields, DataDiffTable table){
			foreach (var map in table.Mappings){
				if (-1 != Array.IndexOf(allfields, map.Key.ToLowerInvariant())){
					var t = map.Value;
					var usealiascodes =
						_context.Mappings.Any(
							_ => (_.FromTable.ToLowerInvariant() == t.ToLowerInvariant()||_.FromTable=="*") && _.FromField.ToLowerInvariant() == "aliascodes");
					if (usealiascodes){
						_output.WriteLine(
							"\t\tupdate @{0} set {1} = (select id from {2} where {2}.code = {1}_code ) where {1} is null and {1}_code is not null ",
							tn, map.Key, map.Value);
						_output.WriteLine(
							"\t\tupdate @{0} set {1} = isnull((select id from {2} where {2}.aliascodes like '%/'+{1}_code+'/%' ),-1) where {1} is null and {1}_code is not null ",
							tn, map.Key, map.Value);
					}
					else{
						_output.WriteLine(
							"\t\tupdate @{0} set {1} = isnull((select id from {2} where {2}.code = {1}_code ),-1) where {1} is null and {1}_code is not null ",
							tn, map.Key, map.Value);
					}
				}
			}
			if (-1 != Array.IndexOf(allfields, "set_parent")){
				_output.WriteLine("\t\tupdate @{0} set {1} = isnull((select id from {2} where {2}.code = parent_code ),-1) where {1} is null and {1}_code is not null ", tn, "parent",table.TableName);
			}
		}

		private void GenerateExistsBinding(string tn, string[] allfields, DataDiffTable table){
			if (!table.NoCode)
			{
				_output.WriteLine(
					"\t\t\tupdate @{0} set id = this.id, code=this.code, _exists =1 from {1} this join @{0} temp on (temp.code = this.code or temp.id=this.id)",
					tn, table.TableName);
				_output.WriteLine(
					"\t\t\tinsert {1} (id,code) select id,isnull(code,id) from @{0} where _exists = 0 and id is not null", tn,
					table.TableName);
				_output.WriteLine(
					"\t\t\tinsert {1} (code) select code from @{0} where _exists = 0 and code is not null and id is null", tn,
					table.TableName);
				_output.WriteLine(
					"\t\t\tupdate @{0} set id = this.id, code=this.code, _exists =1 from {1} this join @{0} temp on (temp.code = this.code or temp.id=this.id)",
					tn, table.TableName);
			}else 
			if (table.UseAliasCodes){
				_output.WriteLine(
					"\t\t\tupdate @{0} set id = this.id, code=this.code, _exists =1 from {1} this join @{0} temp on (temp.code = this.code or temp.id=this.id or this.aliascodes like '%/'+temp.code+'/%')",
					tn, table.TableName);
				_output.WriteLine(
					"\t\t\tinsert {1} (id,code) select id,isnull(code,id) from @{0} where _exists = 0 and id is not null", tn,
					table.TableName);
				_output.WriteLine(
					"\t\t\tinsert {1} (code) select code from @{0} where _exists = 0 and code is not null and id is null", tn,
					table.TableName);
				_output.WriteLine(
					"\t\t\tupdate @{0} set id = this.id, code=this.code, _exists =1 from {1} this join @{0} temp on (temp.code = this.code or temp.id=this.id  or this.aliascodes like '%/'+temp.code+'/%')",
					tn, table.TableName);
			}
			 
			else{
				_output.WriteLine(
					"\t\t\tupdate @{0} set _exists =1 from {1} this join @{0} temp on ( temp.id=this.id)",
					tn, table.TableName);
				_output.WriteLine(
					"\t\t\tinsert {1} (id) select id  from @{0} where _exists = 0 and id is not null", tn,
					table.TableName);
			}
		}

		private void GenerateInsertTemp(string tn, string[] allfields, DataDiffTable table){
			var chunknumber= 0;
			while(GenerateChunk(tn, allfields, table, chunknumber++)){}
		}

		private bool GenerateChunk(string tn, string[] allfields, DataDiffTable table, int st){
			_output.Write("\t\tinsert @{0} (id"+(table.NoCode?"":", code"), tn);
			foreach (var allfield in allfields){
				if (table.Mappings.ContainsKey(allfield)){
					_output.Write(", {0}, {0}_code", allfield);
				}
				else if (allfield == "set_parent" || allfield == "parent"){
					_output.Write(",parent ,parent_code");
				}
				else{
					_output.Write(",{0}", allfield);
				}
			}
			_output.WriteLine(") values");
			var defs = table.Definitions.OrderBy(_ => _.Id).ThenBy(_ => _.Code).ToArray();
			bool last = false;
			for (var i = st*1000; i < defs.Length && i < st*1000 + 1000; i++){
				if (i == defs.Length - 1) last = true;
				var def = defs[i];
				_output.Write("\t\t\t(");

				if (0 == def.Id){
					_output.Write("null");
				}
				else{
					_output.Write("'{0}'", def.Id);
				}
				if (!table.NoCode){
					if (string.IsNullOrWhiteSpace(def.Code)){
						_output.Write(", null");
					}
					else{
						_output.Write(",'{0}'", def.Code);
					}
				}

				foreach (var allfield in allfields){
					if (table.Mappings.ContainsKey(allfield)){
						OutMappedField(def, allfield);
					}
					else if (allfield == "set_parent" || allfield == "parent"){
						OutParentField(def);
					}
					else{
						OutUsualField(def, allfield);
					}
				}

				if (i != table.Definitions.Count - 1 && i != st * 1000 + 1000-1)
				{
					_output.WriteLine("),");
				}
				else{
					_output.WriteLine(")");
				}
			}
			return !last;
		}

		private void OutMappedField(DataDiffItem def, string allfield){
			if (def.Fields.ContainsKey(allfield)){
				var val = def.Fields[allfield];
				var ival = val.ToInt();
				if (val == ""){ //сброс ссылки
					_output.Write(", 0, null");	
				}else if (ival != 0){
					_output.Write(", {0}, null", ival);
				}
				else{
					_output.Write(", null, '{0}'", val.Replace("'","''"));
				}
			}
			else
			{
				_output.Write(", null, null");
			}
		}

		private void OutParentField(DataDiffItem def){
			if (def.Fields.ContainsKey("set_parent") ){
				var val = def.Fields["set_parent"].Split('-').Last();
				var ival = val.ToInt();
				if (val == "")
				{ //сброс ссылки
					_output.Write(", 0, null");
				}
				else if (ival != 0)
				{
					_output.Write(", {0}, null", ival);
				}
				else
				{
					_output.Write(", null, '{0}'", val.Replace("'", "''"));
				}
			}
			else if (def.Fields.ContainsKey("parent")){
				var val = def.Fields["parent"];
				var ival = val.ToInt();
				if (val == "")
				{ //сброс ссылки
					_output.Write(", 0, null");
				}
				else if (ival != 0)
				{
					_output.Write(", {0}, null", ival);
				}
				else
				{
					_output.Write(", null, '{0}'", val.Replace("'", "''"));
				}
			}
			else{
				_output.Write(", null, null");
			}
		}

		

		private void OutUsualField(DataDiffItem def, string allfield){
			if (def.Fields.ContainsKey(allfield)){
				var val = def.Fields[allfield].Replace("'", "''").Trim();
				if (val.IndexOf(',')!=0 && val.ToDecimal(safe:true) != 0){
					val = val.Replace(",", ".");
				}
				_output.Write(", '" + val + "'");
			}
			else{
				_output.Write(", null");
			}
		}

		private void GenerateTempTableDeclaration(string tn, string[] allfields, DataDiffTable table){
			_output.Write("\tdeclare @" + tn + " table ( id int, "+(table.NoCode?"":"code nvarchar(255),")+" _exists bit default 0 ");
			foreach (var fld in allfields){
				if (table.Mappings.ContainsKey(fld)){
					_output.Write(", " + fld + " int");
					_output.Write(", " + fld + "_code nvarchar(255)");
				}
				else if (fld == "set_parent"){
					_output.Write(", parent int");
					_output.Write(", parent_code nvarchar(255)");
				}
				else if (fld == "parent")
				{
					_output.Write(", parent int");
					_output.Write(", parent_code nvarchar(255)");
				}
				else{
					_output.Write(", " + fld + " nvarchar(max)");
				}
			}
			_output.WriteLine(")");
		}

		private void WriteStart(){
			_output.WriteLine("-- START OF SCRIPT: "+_script);
			_output.WriteLine("BEGIN TRAN");
			_output.WriteLine("\t --table for storing check constraints problems");
			_output.WriteLine("\tdeclare @c table (t nvarchar(255), c nvarchar(255), w nvarchar(255))");
			_output.WriteLine("BEGIN TRY");
			_output.WriteLine("--switch off foreign keys");
			foreach (var table in _tables)
			{
				_output.WriteLine("\talter table {0} nocheck constraint all", table.TableName);
				foreach (var disableIndex in table.DisableIndexes){
					_output.WriteLine("\tALTER INDEX {0} ON {1} DISABLE",disableIndex,table.TableName);
				}
			}
			
		}
	}
}