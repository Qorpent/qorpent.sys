using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Data.BSharpDDL{
	/// <summary>
	/// Описывает таблицы
	/// </summary>
	public class DbTable : DbObject{
		/// <summary>
		/// 
		/// </summary>
		public DbTable(){
			ObjectType = DbObjectType.Table;
		}
		private const string FIELDS = "fields";
		private const string TYPES = "types";
		private const string REQUIREDEFAULTRECORD = "requiredefaultrecord";
		/// <summary>
		/// Входящие зависимости
		/// </summary>
		public IDictionary<string,DbField> Fields
		{
			get
			{
				var result = Get<IDictionary<string,DbField>>(FIELDS);
				if (null == result){
					Set(FIELDS, result = new Dictionary<string, DbField>());
				}
				return result;
			}
			set
			{
				Set(FIELDS, value);
			}
		}
		/// <summary>
		/// Признак необходимости наличия дефолтной записи для внешних ключей
		/// </summary>
		public bool RequireDefaultRecord{
			get { return Get<bool>(REQUIREDEFAULTRECORD); }
			set {Set(REQUIREDEFAULTRECORD,value);}
		}

		/// <summary>
		/// Добавить поле в список
		/// </summary>
		/// <param name="field"></param>
		/// <returns></returns>
		public DbTable AddField(DbField field){
			field.Table = this;
			Fields[field.Name] = field;
			return this;
		}


		/// <summary>
		/// Перекрыть для настройки DBObject из XML
		/// </summary>
		/// <param name="xml"></param>
		protected override IEnumerable<DbObject> Setup(System.Xml.Linq.XElement xml)
		{
			foreach (var adv in base.Setup(xml).ToArray()){
				yield return adv;
			}
			var types_ = xml.Elements("datatype").Select(DbDataType.Create);
			this.Types = new Dictionary<string,DbDataType>();
			foreach (var t in types_){
				if (!(Types.ContainsKey(t.Code)))
				{
					Types[t.Code] = t;
				}
			}
			var deftypes = DbDataType.GetDefaultTypes();
			foreach (var dbDataType in deftypes){
				if (!Types.ContainsKey(dbDataType.Key))
				{
					Types[dbDataType.Key] = dbDataType.Value;
				}
			}
			foreach (var e in xml.Elements()){
				if (e.Name == "function"){
					yield return new DbFunction().Initialize(this,e);
				}
				if (e.Name == "trigger")
				{
					yield return new DbTrigger().Initialize(this, e);
				}
				var n = e.Name.LocalName;
				DbDataType type = null;
				if (Types.ContainsKey(n))
				{
					type = Types[n];
				}
				if (null != type){
					var code = e.Attr("code");
					if (!Fields.ContainsKey(code)){
						AddField(DbField.CreateField(this, type, e));
					}
				}else if (n == "ref"){
					var code = e.Attr("code");
					type = new DbDataType{Code="ref"};
					if (!Fields.ContainsKey(code))
					{
						AddField(DbField.CreateField(this, type, e));
					}
				}
				
			}
			yield break;
		}
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string, DbDataType> Types{
			get{
				var result = Get<IDictionary<string, DbDataType>>(TYPES);
				if (null == result){
					Set(TYPES, result = new Dictionary<string, DbDataType>());
				}
				return result;
			}
			set{
				Set(TYPES, value);
			}
		}
	}
}
