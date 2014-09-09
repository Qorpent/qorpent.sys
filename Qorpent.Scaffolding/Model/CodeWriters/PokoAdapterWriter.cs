using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.Model;
using Qorpent.Serialization;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Sql reader to object adapter writer
	/// </summary>
	public class PokoAdapterWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="output"></param>
		public PokoAdapterWriter(PersistentClass cls, TextWriter output = null) : base(cls, output){
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun(){
			WriteStartClass();
			WriteGetTableQuery();
			WriteGetSelectQuery();
			WriteSingleRecordProcessor();
			WriteEnumerableReaderProcessor();
			WriteWriteAccessors();
			WriteEndClass();
		}
		private void WriteWriteAccessors() {
			var fields = Cls.GetOrderedFields().Where(_ => !_.NoSql && !_.NoCode).ToArray();
			if (Cls.AccessMode.HasFlag(DbAccessMode.Write)) {
				WriteSummary("Insert a record", 2, "Affected rows");
				o.WriteLine("\t\tpublic int InsertRecord(IDbConnection connection, " + Cls.Name + " record) {");
				o.WriteLine("\t\t\tvar sqlCommand = connection.CreateCommand();");
				o.Write("\t\t\tconst string command = @\"INSERT INTO " + Cls.FullSqlName.SqlQuoteName().Replace("\"", "\"\"") + " (");
				for (var i = 0; i < fields.Length; i++) {
					o.Write(fields[i].Name.ToLowerInvariant());
					if (i != fields.Length - 1) {
						o.Write(", ");
					}
				}
				o.Write(") VALUES (");
				for (var i = 0; i < fields.Length; i++) {
					o.Write("@" + fields[i].Name.ToLowerInvariant());
					if (i != fields.Length - 1) {
						o.Write(", ");
					}
				}
				o.WriteLine(")\";");
				o.WriteLine("\t\t\tsqlCommand.CommandText = command;");
				for (var i = 0; i < fields.Length; i++) {
					o.WriteLine("\t\t\tsqlCommand.Parameters.Add(record." + fields[i].Name + ");");
				}
				o.WriteLine("\t\t\tsqlCommand.Connection.Open();");
				o.WriteLine("\t\t\tvar affected = sqlCommand.ExecuteNonQuery();");
				o.WriteLine("\t\t\tsqlCommand.Connection.Close();");
				o.WriteLine("\t\t\treturn affected;");
				o.WriteLine("\t\t}");
			}
			if (Cls.AccessMode.HasFlag(DbAccessMode.ReadWrite)) {
				foreach (var uniqueField in GetFields().Where(_ => _.IsUnique)) {
					WriteUpdateMethod(uniqueField);
				}
			}
		}
		/// <summary>
		///		Определяет перечисление полей
		/// </summary>
		/// <param name="inSql">Признак того, что поле содержится в SQL</param>
		/// <param name="inCode">Признак того, чтополе содержится в коде</param>
		/// <returns>Перечисление полей</returns>
		private IEnumerable<Field> GetFields(bool inSql = true, bool inCode = true) {
			return Cls.GetOrderedFields().Where(_ => !_.NoCode == inCode && !_.NoSql == inSql);
		}
		private void WriteUpdateMethod(Field byField) {
			var fields = GetFields().ToArray();
			WriteSummary("Update a record by " + byField.Name, 2, "Affected rows");
			o.WriteLine("\t\tpublic int UpdateRecordBy" + byField.Name + "(IDbConnection connection, " + byField.DataType.CSharpDataType + " " + byField.Name.ToLowerInvariant() + ", " + Cls.Name + " record) {");
			o.WriteLine("\t\t\tvar sqlCommand = connection.CreateCommand();");
			o.Write("\t\t\tconst string command = @\"UPDATE " + Cls.FullSqlName.SqlQuoteName().Replace("\"", "\"\"") + " SET ");
			var updateFields = fields.Where(_ => _.Name.ToLowerInvariant() != byField.Name.ToLowerInvariant()).ToArray();
			for (var i = 0; i < updateFields.Length; i++) {
				var name = updateFields[i].Name.ToLowerInvariant();
				o.Write(name + " = @" + name);
				if (i != updateFields.Length - 1) {
					o.Write(", ");
				}
			}
			o.WriteLine(" WHERE " + byField.Name.ToLowerInvariant() + " = @where" + byField.Name.ToLowerInvariant() + "\";");
			o.WriteLine("\t\t\tsqlCommand.CommandText = command;");
			for (var i = 0; i < fields.Length; i++) {
				o.WriteLine("\t\t\tsqlCommand.Parameters.Add(record." + fields[i].Name + ");");
			}
			WriteLine("sqlCommand.Parameters.Add(" + byField.Name.ToLowerInvariant() + ");", 3);
			o.WriteLine("\t\t\tsqlCommand.Connection.Open();");
			o.WriteLine("\t\t\tvar affected = sqlCommand.ExecuteNonQuery();");
			o.WriteLine("\t\t\tsqlCommand.Connection.Close();");
			o.WriteLine("\t\t\treturn affected;");
			o.WriteLine("\t\t}");
		}
		/// <summary>
		///		Запись описания метода/поля
		/// </summary>
		/// <param name="summary">Описание</param>
		/// <param name="level">Уровень (количество табуляций в начале строки)</param>
		/// <param name="returns">Описание возвращаемого значения</param>
		private void WriteSummary(string summary, int level = 2, string returns = null) {
			WriteLine("///<summary>" + summary + "</summary>", level);
			if (!string.IsNullOrWhiteSpace(returns)) {
				WriteLine("///<returns>" + returns + "</returns>", level);
			}
		}
		/// <summary>
		///		Запись строки с отступами
		/// </summary>
		/// <param name="value"></param>
		/// <param name="level"></param>
		private void WriteLine(string value, int level = 0) {
			for (var i = 0; i < level; i++) {
				o.Write("\t");
			}
			o.WriteLine(value);
		}
		/// <summary>
		///		Определяет признак того, что класс в том или ином виде поддерживает <see cref="IWithCode"/>
		/// </summary>
		/// <returns>Признак того, что класс в том или ином виде поддерживает <see cref="IWithCode"/></returns>
		private bool IsIWithCode() {
			return IsImplements("iwithcode");
		}
		/// <summary>
		///		Определяет признак того, что класс в том или ином виде поддерживает <see cref="IWithId"/>
		/// </summary>
		/// <returns>Признак того, что класс в том или ином виде поддерживает <see cref="IWithId"/></returns>
		private bool IsIWithId() {
			return IsImplements("iwithid");
		}
		/// <summary>
		///		Определяет, имплементирует ли генерируемый класс указанный интерфейс в том или ином виде
		/// </summary>
		/// <param name="interfaceName">Название интерфейса</param>
		/// <returns>Признак имплементации интерфейса</returns>
		private bool IsImplements(string interfaceName) {
			return Cls.TargetClass.AllImports.Any(_ => _.Name.ToLowerInvariant() == interfaceName.ToLowerInvariant());
		}
		private void WriteGetTableQuery(){
			o.WriteLine("\t\t///<summary>Implementation of GetTableName</summary>");
			o.WriteLine("#if NOQORPENT");
			o.WriteLine("\t\tpublic string GetTableName(object options = null) {");
			o.WriteLine("#else");
			o.WriteLine("\t\tpublic override string GetTableName(object options = null) {");
			o.WriteLine("#endif");
			o.WriteLine("\t\t\treturn \"" + Cls.FullSqlName.Replace("\"","\\\"") + "\";");
			o.WriteLine("\t\t}");
		}

		private void WriteSingleRecordProcessor(){
			o.WriteLine("#if NOQORPENT");
			o.WriteLine("\t\t///<summary>Implementation of ProcessRecord</summary>");
			o.WriteLine("\t\tpublic "+Cls.Name+" ProcessRecord(IDataReader reader, bool nativeorder = false) {");
			o.WriteLine("\t\t\treturn ProcessRecordNative(reader,nativeorder) as "+Cls.Name+";");
			o.WriteLine("\t\t}");
			o.WriteLine("#endif");
			o.WriteLine("\t\t///<summary>Implementation of ProcessRecordNative</summary>");
			o.WriteLine("#if NOQORPENT");
			o.WriteLine("\t\tpublic object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {");
			o.WriteLine("#else");
			o.WriteLine("\t\tpublic override object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {");
			o.WriteLine("#endif");
			
			o.WriteLine("\t\t\tvar result = new " + Cls.Name + "();");
			o.WriteLine("\t\t\tif ( nativeorder ) {");
			int i = 0;
			foreach (Field ormField in Cls.GetOrderedFields().Where(_ => !_.NoSql && !_.NoCode)){
				string type = ormField.DataType.ReaderCSharpDataType;
				string name = ormField.Name;
				if (ormField.IsReference){
					//ref
					name += ormField.ReferenceField;
				}

				string cast = "";
				if (type != ormField.DataType.CSharpDataType){
					cast = "(" + ormField.DataType.CSharpDataType + ")";
				}
				var cond = "";
				if (ormField.IsReference && ormField.ReferenceClass.TargetClass == null){
					cond = "if(!reader.IsDBNull("+i+"))";
				}
				o.WriteLine("\t\t\t\t"+cond+"result." + name + " = " + cast + "reader.Get" + type + "(" + i + ");");
				i++;
			}
			o.WriteLine("\t\t\t}else{");
			o.WriteLine("\t\t\t\tfor(var i=0;i<reader.FieldCount;i++){");
			o.WriteLine("\t\t\t\t\tvar name = reader.GetName(i).ToLowerInvariant();");
			o.WriteLine("\t\t\t\t\tvar value = reader.GetValue(i);");
			o.WriteLine("\t\t\t\t\tif(value is DBNull)continue;");
			o.WriteLine("\t\t\t\t\tswitch(name){");
			foreach (Field ormField in Cls.GetOrderedFields().Where(_ => !_.NoSql && !_.NoCode)){
				string type = ormField.DataType.ReaderCSharpDataType;
				string name = ormField.Name;
				if (ormField.IsReference){
					name += ormField.ReferenceField;
				}

				if (type != ormField.DataType.CSharpDataType){
					o.WriteLine("\t\t\t\t\t\tcase \"" + name.ToLower() + "\": result." + name + " = (" +
					            ormField.DataType.CSharpDataType + ")" +
					            "value;break;");
				}
				else{
					o.WriteLine("\t\t\t\t\t\tcase \"" + name.ToLower() + "\": result." + name + " = Convert.To" + type +
					            "(value);break;");
				}
				i++;
			}
			o.WriteLine("\t\t\t\t\t}");
			o.WriteLine("\t\t\t\t}");
			o.WriteLine("\t\t\t}");
			o.WriteLine("\t\t\treturn result;");
			o.WriteLine("\t\t}");
		}

		private void WriteEnumerableReaderProcessor(){
			o.WriteLine("#if NOQORPENT");
			o.WriteLine("\t\t///<summary>Implementation of ProcessRecordSet</summary>");
			o.WriteLine("\t\tpublic IEnumerable<" + Cls.Name + "> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {");
			o.WriteLine("\t\t\treturn ProcessRecordNative(reader,nativeorder).OfType<" + Cls.Name + ">().ToArray();");
			o.WriteLine("\t\t}");
			o.WriteLine("\t\t///<summary>Implementation of ProcessRecordSetNative</summary>");
			o.WriteLine("\t\tpublic IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false) {");
			o.WriteLine("\t\t\twhile(reader.Read()){");
			o.WriteLine("\t\t\t\tyield return ProcessRecordNative(reader,nativeorder);");
			o.WriteLine("\t\t\t}");
			o.WriteLine("\t\t}");
			o.WriteLine("#endif");
		}

		private void WriteGetSelectQuery(){
			o.WriteLine("\t\t///<summary>Implementation of PrepareSelectQuery</summary>");
			o.WriteLine("#if NOQORPENT");
			o.WriteLine("\t\tpublic string PrepareSelectQuery(object conditions = null, object hints = null) {");
			o.WriteLine("#else");
			o.WriteLine("\t\tpublic override string PrepareSelectQuery(object conditions = null, object hints = null) {");
			o.WriteLine("#endif");
			o.WriteLine("\t\t\tvar sb = new StringBuilder();");
			o.Write("\t\t\tsb.Append(\"select ");
			bool fst = true;
			foreach (Field of in Cls.GetOrderedFields().Where(_ => !_.NoSql && !_.NoCode)){
				if (fst){
					fst = false;
				}
				else{
					o.Write(", ");
				}
				string n = of.Name.SqlQuoteName();

				string rn = n;
				if (of.IsReference){
					rn += of.ReferenceField;
				}
				rn = rn.SqlQuoteName().Replace("\"", "\\\"");
				n = n.Replace("\"", "\\\"");
				//геометрический тип трансформируем в nvarchar(max)
				if (of.DataType.ResolveSqlDataType(SqlDialect.SqlServer).Name.StartsWith("geo"))
				{
					n = "CAST(" + n + " as nvarchar(max))";
				}
				o.Write(n);
				if (rn != n){
					o.Write(" as " + rn + "");
				}
			}
			o.Write(" from " + Cls.FullSqlName.Replace("\"", "\\\"") + " \");");
			o.WriteLine();
			o.WriteLine("\t\t\tvar where = conditions as string;");
			o.WriteLine("\t\t\tif ( null != where ) {");
			o.WriteLine("\t\t\t\tsb.Append(\" where \");");
			o.WriteLine("\t\t\t\tsb.Append(where);");
			o.WriteLine("\t\t\t}");
			o.WriteLine("\t\t\treturn sb.ToString();");
			o.WriteLine("\t\t}");
		}

		private void WriteEndClass(){
			o.WriteLine("\t}");
			o.WriteLine("}");
		}

		private void WriteStartClass(){
			WriteHeader();
			o.WriteLine("using System;");
			o.WriteLine("using System.Collections.Generic;");
			o.WriteLine("using System.Text;");
			o.WriteLine("using System.Data;");
			o.WriteLine("#if !NOQORPENT");
			o.WriteLine("using Qorpent.Data;");
			o.WriteLine("#endif");
			o.Write("namespace {0}.Adapters {{\r\n", Cls.Namespace);
			o.WriteLine("\t///<summary>");
			o.WriteLine("\t/// Data Adapter for " + Cls.Name);
			o.WriteLine("\t///</summary>");
			o.WriteLine("#if !NOQORPENT");
			o.Write("\tpublic partial class {0}DataAdapter : ObjectDataAdapterBase<{0}> {{\r\n", Cls.Name);
			o.WriteLine("#else");
			o.Write("\tpublic partial class {0}DataAdapter {{\r\n", Cls.Name);
			o.WriteLine("#endif");
		}
	}
}