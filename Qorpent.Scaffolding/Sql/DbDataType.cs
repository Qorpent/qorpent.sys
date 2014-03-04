using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.SqlGeneration{
	/// <summary>
	/// Тип данных
	/// </summary>
	public class DbDataType{
		/// <summary>
		/// Код
		/// </summary>
		public string Code { get; set; }
		/// <summary>
		/// Кастомное имя типа
		/// </summary>
		public string CustomTypeName { get; set; }

		/// <summary>
		/// Основной приведенный тип
		/// </summary>
		public DbType DbType { get; set; }

		/// <summary>
		/// Размер
		/// </summary>
		public int Size { get; set; }

		/// <summary>
		/// Точность
		/// </summary>
		public int Precession { get; set; }
		/// <summary>
		/// Парсит тип из XML
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static DbDataType Create(XElement e){
			var n = e.Attr("code");
			var result = new DbDataType();
			DbDataType deftype = null;
			result.Code = n;
			if (_defaults.ContainsKey(n)){
				deftype = _defaults[n];
			}
			SetupDBType(e, deftype, result);
			SetupSize(e,deftype,result);
			SetupPrecession(e,deftype,result);
			return result;
		}

		private static void SetupDBType(XElement e, DbDataType deftype, DbDataType result){
			var dt_ = e.Attr("type");
			if (string.IsNullOrWhiteSpace(dt_)){
				if (null != deftype){
					result.DbType = deftype.DbType;
				}
				else{
					result.DbType = DbType.String;
				}
			}
			else{
				result.DbType = dt_.To<DbType>();
			}
		}
		private static void SetupSize(XElement e, DbDataType deftype, DbDataType result)
		{
			var dt_ = e.Attr("size");
			if (string.IsNullOrWhiteSpace(dt_))
			{
				if (null != deftype){
					result.Size = deftype.Size;
				}
			}
			else
			{
				result.Size = dt_.To<int>();
			}
		}
		private static void SetupPrecession(XElement e, DbDataType deftype, DbDataType result)
		{
			var dt_ = e.Attr("precession");
			if (string.IsNullOrWhiteSpace(dt_))
			{
				if (null != deftype)
				{
					result.Precession = deftype.Precession;
				}
			}
			else
			{
				result.Precession = dt_.To<int>();
			}
		}

		private static IDictionary<string, DbDataType> _defaults = new Dictionary<string, DbDataType>{
			{"int", new DbDataType{DbType = DbType.Int32}},
			{"long", new DbDataType{DbType = DbType.Int64}},
			{"bool", new DbDataType{DbType = DbType.Boolean}},
			{"decimal",new DbDataType{DbType = DbType.Decimal,Size = 18,Precession = 6}},
			{"string",new DbDataType{DbType = DbType.String, Size = 255}},
			{"shortstring",new DbDataType{DbType = DbType.String, Size = 20}},
			{"longstring",new DbDataType{DbType = DbType.String, Size = 400}},
			{"text",new DbDataType{DbType = DbType.String, Size = -1}},


		};
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static IDictionary<string,DbDataType> GetDefaultTypes(){
			return _defaults;
		}
	}
}