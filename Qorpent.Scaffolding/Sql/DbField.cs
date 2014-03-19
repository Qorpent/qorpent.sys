using System.Data;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// Описывает поле в таблицах
	/// </summary>
	public class DbField:DbObject{
		/// <summary>
		/// 
		/// </summary>
		public DbField(){
			ObjectType = DbObjectType.Field;
		}
		private const string DATATYPE = "datattype";
		private const string DEFAULTVALUE = "defaultvalue";
		private const string COMPUTEBYEXPRESSION = "computeby";
		private const string ISPRIMARYKEY = "isprimarykey";
		private const string ISIDENTITY = "isidentity";
		private const string IDX = "idx";
		private const string UNIQUE = "unique";
		private const string REFTABLE = "reftable";
		private const string REFFIELD = "reffield";
		/// <summary>
		/// Ссылка на содержащую таблицу
		/// </summary>
		public DbTable Table{
			get { return ParentElement as DbTable; }
			set { ParentElement = value; }
		}

		/// <summary>
		/// Ссылка на содержащую таблицу
		/// </summary>
		public int Idx
		{
			get { return Get<int>(IDX); }
			set { Set(IDX,value); }
		}
		/// <summary>
		/// Тип поля
		/// </summary>
		public DbDataType DataType{
			get { return Get<DbDataType>(DATATYPE); }
			set { Set(DATATYPE,value);}
		}
		/// <summary>
		/// Значекние по умолчанию
		/// </summary>
		public DbDefaultValue DefaultValue{
			get { return Get<DbDefaultValue>(DEFAULTVALUE); }
			set { Set(DEFAULTVALUE,value);}
		}

		/// <summary>
		/// Выражение для вычисляемых полей
		/// </summary>
		public string ComputeByExpression{
			get { return Get<string>(COMPUTEBYEXPRESSION); }
			set {Set(COMPUTEBYEXPRESSION,value);}
		}

		/// <summary>
		/// Выражение для вычисляемых полей
		/// </summary>
		public bool IsPrimaryKey
		{
			get { return Get<bool>(ISPRIMARYKEY); }
			set { Set(ISPRIMARYKEY, value); }
		}

		/// <summary>
		/// Выражение для вычисляемых полей
		/// </summary>
		public bool IsIdentity
		{
			get { return Get<bool>(ISIDENTITY); }
			set { Set(ISIDENTITY, value); }
		}

		/// <summary>
		/// Выражение для вычисляемых полей
		/// </summary>
		public bool IsUnique
		{
			get { return Get<bool>(UNIQUE); }
			set { Set(UNIQUE, value); }
		}
		/// <summary>
		/// 
		/// </summary>
		public string RefTable{
			get { return Get<string>(REFTABLE); }
			set { Set(REFTABLE, value); }
		}

		/// <summary>
		/// 
		/// </summary>
		public string RefField
		{
			get { return Get<string>(REFFIELD); }
			set { Set(REFFIELD, value); }
		}

		/// <summary>
		/// Перекрыть для настройки DBObject из XML
		/// </summary>
		/// <param name="xml"></param>
		protected override System.Collections.Generic.IEnumerable<DbObject> Setup(XElement xml)
		{
			var basis = base.Setup(xml).ToArray();
			foreach (var dbObject in basis)
			{
				yield return dbObject;
			}
			var desc = xml.Describe();
			Name = desc.Code;
			IsPrimaryKey = xml.Attr("primarykey").ToBool();
			IsIdentity = xml.Attr("identity").ToBool();
			this.IsUnique = xml.Attr("unique").ToBool();
			this.Idx = xml.Attr("idx").ToInt();
			SetRef(xml.Attr("ref"));
			SetRef(xml.Attr("to"));
			if (xml.Attr("nocascade").ToBool())
			{
				this.NoCascadeUpdates = true;
			}
			if (this.DataType.Code == "ref"){
				if (string.IsNullOrWhiteSpace(RefTable)){
					this.RefTable = this.Table.Schema + "." + this.Name;
					this.RefField = "Id";
				}
			}
			var name = xml.Attr("name");
			this.Comment = "";
			if (name == "primarykey")
			{
				this.IsPrimaryKey = true;
			}
			else if (name == "identity")
			{
				this.IsIdentity = true;
			}
			else if (name == "unique")
			{
				this.IsUnique = true;
			}
			
			else
			{
				this.Comment = name;
			}
			if (this.Idx == 0)
			{
				this.Idx = 1000;
			}
			this.DefaultValue = DbDefaultValue.Create(this, xml.Attr("default"), xml);
			if (this.IsRef){
				this.DefaultValue.Value = 0;
				if (this.DataType.DbType == DbType.String){
					this.DefaultValue.Value = "/";
				}
			}
		}
		/// <summary>
		/// 
		/// </summary>
		public bool IsRef{
			get { return !string.IsNullOrWhiteSpace(RefTable); }
		}
		/// <summary>
		/// Признак того, что не требуется формировать каскады обновлений для IsRef
		/// </summary>
		public bool NoCascadeUpdates { get; set; }

		private void SetRef(string attr){
			if(string.IsNullOrWhiteSpace(attr))return;
			var splitter = attr.LastIndexOf('.');
			var tableref = attr.Substring(0, splitter);
			if (!tableref.Contains(".")){
				tableref = Table.Schema + "." + tableref;
			}
			var fieldref = attr.Substring(splitter + 1);
			this.RefTable = tableref;
			this.RefField = fieldref;
		}

		/// <summary>
		/// Создает поле из типа и XML
		/// </summary>
		/// <param name="table"></param>
		/// <param name="dbDataType"></param>
		/// <param name="xElement"></param>
		/// <returns></returns>
		public static DbField CreateField(DbTable table,DbDataType dbDataType, XElement xElement){
			var result = new DbField{Table = table, DataType = dbDataType};
			result.Setup(xElement).ToArray();
			return result;

		}
	}
}