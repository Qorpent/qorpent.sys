using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class PokoAdaptersGeneratorTests {
		private const string PokoExpectedBase = @"using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
#if !NOQORPENT
using Qorpent.Data;
#endif
namespace .Adapters {
	///<summary>
	/// Data Adapter for a
	///</summary>
#if !NOQORPENT
	public partial class aDataAdapter : ObjectDataAdapterBase<a> {
#else
	public partial class aDataAdapter {
#endif
		///<summary>Implementation of GetTableName</summary>
#if NOQORPENT
		public string GetTableName(object options = null) {
#else
		public override string GetTableName(object options = null) {
#endif
			return ""\""dbo\"".\""a\"""";
		}
		///<summary>Implementation of PrepareSelectQuery</summary>
#if NOQORPENT
		public string PrepareSelectQuery(object conditions = null, object hints = null) {
#else
		public override string PrepareSelectQuery(object conditions = null, object hints = null) {
#endif
			var sb = new StringBuilder();
			sb.Append(""select \""id\"", \""code\"" from \""dbo\"".\""a\"" "");
			var where = conditions as string;
			if ( null != where ) {
				sb.Append("" where "");
				sb.Append(where);
			}
			return sb.ToString();
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecord</summary>
		public a ProcessRecord(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder) as a;
		}
#endif
		///<summary>Implementation of ProcessRecordNative</summary>
#if NOQORPENT
		public object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#else
		public override object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#endif
			var result = new a();
			if ( nativeorder ) {
				result.Id = reader.GetInt32(0);
				result.Code = reader.GetString(1);
			}else{
				for(var i=0;i<reader.FieldCount;i++){
					var name = reader.GetName(i).ToLowerInvariant();
					var value = reader.GetValue(i);
					if(value is DBNull)continue;
					switch(name){
						case ""id"": result.Id = Convert.ToInt32(value);break;
						case ""code"": result.Code = Convert.ToString(value);break;
					}
				}
			}
			return result;
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecordSet</summary>
		public IEnumerable<a> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder).OfType<a>().ToArray();
		}
		///<summary>Implementation of ProcessRecordSetNative</summary>
		public IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false) {
			while(reader.Read()){
				yield return ProcessRecordNative(reader,nativeorder);
			}
		}
#endif
		";
		private const string PokoInsertExpected = @"///<summary>Insert a record</summary>
		///<returns>Affected rows</returns>
		public int InsertRecord(IDbConnection connection, a record) {
			var sqlCommand = connection.CreateCommand();
			const string command = @""INSERT INTO """"dbo"""".""""a"""" (id, code) VALUES (@id, @code)"";
			sqlCommand.CommandText = command;
			sqlCommand.Parameters.Add(record.Id);
			sqlCommand.Parameters.Add(record.Code);
			sqlCommand.Connection.Open();
			var affected = sqlCommand.ExecuteNonQuery();
			sqlCommand.Connection.Close();
			return affected;
		}";
		private const string PokoQorpentAccessWriteOnlyExpected = PokoExpectedBase + PokoInsertExpected + @"
	}
}";
		private const string PokoQorpentAccessReadWriteExpected = PokoExpectedBase + PokoInsertExpected + @"
		///<summary>Update a record by Code</summary>
		///<returns>Affected rows</returns>
		public int UpdateRecordByCode(IDbConnection connection, String code, a record) {
			var sqlCommand = connection.CreateCommand();
			const string command = @""UPDATE """"dbo"""".""""a"""" SET id = @id WHERE code = @wherecode"";
			sqlCommand.CommandText = command;
			sqlCommand.Parameters.Add(record.Id);
			sqlCommand.Parameters.Add(record.Code);
			sqlCommand.Parameters.Add(code);
			sqlCommand.Connection.Open();
			var affected = sqlCommand.ExecuteNonQuery();
			sqlCommand.Connection.Close();
			return affected;
		}" + ClassEnding;
		private const string ClassEnding = @"
	}
}";

		private const string PokoQorpentAccessWriteOnlyClass = @"require data
class a prototype=dbtable qorpent-access=Write
	import IWithCode";
		private const string PokoQorpentAccessReadWriteClass = @"require data
class a prototype=dbtable qorpent-access=ReadWrite
	import IWithCode";


		[TestCase(@"class a prototype=dbtable", "a", DbAccessMode.Read)]
		[TestCase(@"class a prototype=dbtable qorpent-access=Read", "a", DbAccessMode.Read)]
		[TestCase(@"class a prototype=dbtable qorpent-access=ReadWrite", "a", DbAccessMode.Write)]
		[TestCase(@"require data
class a prototype=dbtable qorpent-access=ReadWrite
	import IWithCode", "a", DbAccessMode.ReadWrite)]
		public void IsCorrectDeterminingAccessModel(string cs,  string name, DbAccessMode mode) {
			var model = PersistentModel.Compile(cs);
			Assert.AreEqual(mode, model[name].AccessMode);
		}
		[TestCase(PokoQorpentAccessWriteOnlyClass, PokoQorpentAccessWriteOnlyExpected)]
		[TestCase(PokoQorpentAccessReadWriteClass, PokoQorpentAccessReadWriteExpected)]
		public void IsCorrectUpdateInsertGenerationInPokoAdapter(string table, string expected) {
			Assert.Ignore("Ничего на самом деле не сделано");
			
			var model = PersistentModel.Compile(table);
			var code = new PokoAdapterWriter(model["a"]) {WithHeader = false}.ToString();
			Console.WriteLine(code);
			Assert.AreEqual(expected.Trim(), code.Trim());
		}
		[Test]
		public void SimplestTable(){
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
");
			var code = new PokoAdapterWriter(model["a"]) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
#if !NOQORPENT
using Qorpent.Data;
#endif
namespace .Adapters {
	///<summary>
	/// Data Adapter for a
	///</summary>
#if !NOQORPENT
	public partial class aDataAdapter : ObjectDataAdapterBase<a> {
#else
	public partial class aDataAdapter {
#endif
		///<summary>Implementation of GetTableName</summary>
#if NOQORPENT
		public string GetTableName(object options = null) {
#else
		public override string GetTableName(object options = null) {
#endif
			return ""\""dbo\"".\""a\"""";
		}
		///<summary>Implementation of PrepareSelectQuery</summary>
#if NOQORPENT
		public string PrepareSelectQuery(object conditions = null, object hints = null) {
#else
		public override string PrepareSelectQuery(object conditions = null, object hints = null) {
#endif
			var sb = new StringBuilder();
			sb.Append(""select \""id\"" from \""dbo\"".\""a\"" "");
			var where = conditions as string;
			if ( null != where ) {
				sb.Append("" where "");
				sb.Append(where);
			}
			return sb.ToString();
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecord</summary>
		public a ProcessRecord(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder) as a;
		}
#endif
		///<summary>Implementation of ProcessRecordNative</summary>
#if NOQORPENT
		public object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#else
		public override object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#endif
			var result = new a();
			if ( nativeorder ) {
				result.Id = reader.GetInt32(0);
			}else{
				for(var i=0;i<reader.FieldCount;i++){
					var name = reader.GetName(i).ToLowerInvariant();
					var value = reader.GetValue(i);
					if(value is DBNull)continue;
					switch(name){
						case ""id"": result.Id = Convert.ToInt32(value);break;
					}
				}
			}
			return result;
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecordSet</summary>
		public IEnumerable<a> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder).OfType<a>().ToArray();
		}
		///<summary>Implementation of ProcessRecordSetNative</summary>
		public IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false) {
			while(reader.Read()){
				yield return ProcessRecordNative(reader,nativeorder);
			}
		}
#endif
	}
}".Trim(), code.Trim());
		}

		[Test]
		public void Q201_TableWithGeometry()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	geometry gis
");
			var code = new PokoAdapterWriter(model["a"]) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
#if !NOQORPENT
using Qorpent.Data;
#endif
namespace .Adapters {
	///<summary>
	/// Data Adapter for a
	///</summary>
#if !NOQORPENT
	public partial class aDataAdapter : ObjectDataAdapterBase<a> {
#else
	public partial class aDataAdapter {
#endif
		///<summary>Implementation of GetTableName</summary>
#if NOQORPENT
		public string GetTableName(object options = null) {
#else
		public override string GetTableName(object options = null) {
#endif
			return ""\""dbo\"".\""a\"""";
		}
		///<summary>Implementation of PrepareSelectQuery</summary>
#if NOQORPENT
		public string PrepareSelectQuery(object conditions = null, object hints = null) {
#else
		public override string PrepareSelectQuery(object conditions = null, object hints = null) {
#endif
			var sb = new StringBuilder();
			sb.Append(""select \""id\"", CAST(\""gis\"" as nvarchar(max)) as \""gis\"" from \""dbo\"".\""a\"" "");
			var where = conditions as string;
			if ( null != where ) {
				sb.Append("" where "");
				sb.Append(where);
			}
			return sb.ToString();
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecord</summary>
		public a ProcessRecord(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder) as a;
		}
#endif
		///<summary>Implementation of ProcessRecordNative</summary>
#if NOQORPENT
		public object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#else
		public override object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#endif
			var result = new a();
			if ( nativeorder ) {
				result.Id = reader.GetInt32(0);
				result.gis = reader.GetString(1);
			}else{
				for(var i=0;i<reader.FieldCount;i++){
					var name = reader.GetName(i).ToLowerInvariant();
					var value = reader.GetValue(i);
					if(value is DBNull)continue;
					switch(name){
						case ""id"": result.Id = Convert.ToInt32(value);break;
						case ""gis"": result.gis = Convert.ToString(value);break;
					}
				}
			}
			return result;
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecordSet</summary>
		public IEnumerable<a> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder).OfType<a>().ToArray();
		}
		///<summary>Implementation of ProcessRecordSetNative</summary>
		public IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false) {
			while(reader.Read()){
				yield return ProcessRecordNative(reader,nativeorder);
			}
		}
#endif
	}
}".Trim(), code.Trim());
		}


		[Test]
		public void TableWithReference()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	ref b
class b prototype=dbtable
");
			var code = new PokoAdapterWriter(model["a"]) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
#if !NOQORPENT
using Qorpent.Data;
#endif
namespace .Adapters {
	///<summary>
	/// Data Adapter for a
	///</summary>
#if !NOQORPENT
	public partial class aDataAdapter : ObjectDataAdapterBase<a> {
#else
	public partial class aDataAdapter {
#endif
		///<summary>Implementation of GetTableName</summary>
#if NOQORPENT
		public string GetTableName(object options = null) {
#else
		public override string GetTableName(object options = null) {
#endif
			return ""\""dbo\"".\""a\"""";
		}
		///<summary>Implementation of PrepareSelectQuery</summary>
#if NOQORPENT
		public string PrepareSelectQuery(object conditions = null, object hints = null) {
#else
		public override string PrepareSelectQuery(object conditions = null, object hints = null) {
#endif
			var sb = new StringBuilder();
			sb.Append(""select \""id\"", \""b\"" as \""bid\"" from \""dbo\"".\""a\"" "");
			var where = conditions as string;
			if ( null != where ) {
				sb.Append("" where "");
				sb.Append(where);
			}
			return sb.ToString();
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecord</summary>
		public a ProcessRecord(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder) as a;
		}
#endif
		///<summary>Implementation of ProcessRecordNative</summary>
#if NOQORPENT
		public object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#else
		public override object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#endif
			var result = new a();
			if ( nativeorder ) {
				result.Id = reader.GetInt32(0);
				result.bId = reader.GetInt32(1);
			}else{
				for(var i=0;i<reader.FieldCount;i++){
					var name = reader.GetName(i).ToLowerInvariant();
					var value = reader.GetValue(i);
					if(value is DBNull)continue;
					switch(name){
						case ""id"": result.Id = Convert.ToInt32(value);break;
						case ""bid"": result.bId = Convert.ToInt32(value);break;
					}
				}
			}
			return result;
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecordSet</summary>
		public IEnumerable<a> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder).OfType<a>().ToArray();
		}
		///<summary>Implementation of ProcessRecordSetNative</summary>
		public IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false) {
			while(reader.Read()){
				yield return ProcessRecordNative(reader,nativeorder);
			}
		}
#endif
	}
}".Trim(), code.Trim());
		}


		[Test]
		public void TableWithNoSqlAndNocodes()
		{
			var model = PersistentModel.Compile(@"
class a prototype=dbtable
	string x
	string y nosql
	string z nocode
");
			var code = new PokoAdapterWriter(model["a"]) { WithHeader = false }.ToString();
			Console.WriteLine(code.Replace("\"", "\"\""));
			Assert.AreEqual(@"
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
#if !NOQORPENT
using Qorpent.Data;
#endif
namespace .Adapters {
	///<summary>
	/// Data Adapter for a
	///</summary>
#if !NOQORPENT
	public partial class aDataAdapter : ObjectDataAdapterBase<a> {
#else
	public partial class aDataAdapter {
#endif
		///<summary>Implementation of GetTableName</summary>
#if NOQORPENT
		public string GetTableName(object options = null) {
#else
		public override string GetTableName(object options = null) {
#endif
			return ""\""dbo\"".\""a\"""";
		}
		///<summary>Implementation of PrepareSelectQuery</summary>
#if NOQORPENT
		public string PrepareSelectQuery(object conditions = null, object hints = null) {
#else
		public override string PrepareSelectQuery(object conditions = null, object hints = null) {
#endif
			var sb = new StringBuilder();
			sb.Append(""select \""id\"", \""x\"" from \""dbo\"".\""a\"" "");
			var where = conditions as string;
			if ( null != where ) {
				sb.Append("" where "");
				sb.Append(where);
			}
			return sb.ToString();
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecord</summary>
		public a ProcessRecord(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder) as a;
		}
#endif
		///<summary>Implementation of ProcessRecordNative</summary>
#if NOQORPENT
		public object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#else
		public override object ProcessRecordNative(IDataReader reader, bool nativeorder = false) {
#endif
			var result = new a();
			if ( nativeorder ) {
				result.Id = reader.GetInt32(0);
				result.x = reader.GetString(1);
			}else{
				for(var i=0;i<reader.FieldCount;i++){
					var name = reader.GetName(i).ToLowerInvariant();
					var value = reader.GetValue(i);
					if(value is DBNull)continue;
					switch(name){
						case ""id"": result.Id = Convert.ToInt32(value);break;
						case ""x"": result.x = Convert.ToString(value);break;
					}
				}
			}
			return result;
		}
#if NOQORPENT
		///<summary>Implementation of ProcessRecordSet</summary>
		public IEnumerable<a> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {
			return ProcessRecordNative(reader,nativeorder).OfType<a>().ToArray();
		}
		///<summary>Implementation of ProcessRecordSetNative</summary>
		public IEnumerable<object> ProcessRecordSetNative(IDataReader reader, bool nativeorder = false) {
			while(reader.Read()){
				yield return ProcessRecordNative(reader,nativeorder);
			}
		}
#endif
	}
}
".Trim(), code.Trim());
		}

	}
}