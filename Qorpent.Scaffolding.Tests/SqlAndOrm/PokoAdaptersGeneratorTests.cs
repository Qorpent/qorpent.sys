using System;
using NUnit.Framework;
using Qorpent.Scaffolding.Model;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Tests.SqlAndOrm{
	[TestFixture]
	public class PokoAdaptersGeneratorTests{
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
			sb.Append(""select ""id"" from \""dbo\"".\""a\"" "");
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
			sb.Append(""select ""id"", ""b"" as ""bid"" from \""dbo\"".\""a\"" "");
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
			sb.Append(""select ""id"", ""x"" from \""dbo\"".\""a\"" "");
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