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
			var types = new Dictionary<string,DbDataType>();
			foreach (var t in types_){
				if (!(types.ContainsKey(t.Code))){
					types[t.Code] = t;
				}
			}
			var deftypes = DbDataType.GetDefaultTypes();
			foreach (var e in xml.Elements()){
				var n = e.Name.LocalName;
				DbDataType type = null;
				if (types.ContainsKey(n)){
					type = types[n];
				}else if (deftypes.ContainsKey(n)){
					type = deftypes[n];
				}
				if (null != type){
					var code = e.Attr("code");
					if (!this.Fields.ContainsKey(code)){
						AddField(DbField.CreateField(this, type, e));
					}

				}
			}
			yield break;
		}

	}
}
