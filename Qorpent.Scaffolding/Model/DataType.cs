using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	///     Описание типа данных для класса
	/// </summary>
	public class DataType{
		private string _cSharpDataType;
		private string _readerCSharpDataType;

		/// <summary>
		/// </summary>
		public DataType(){
			SqlDataTypes = new Dictionary<SqlDialect, SqlDataType>();
		}

		/// <summary>
		///     Признак строкового параметра
		/// </summary>
		public bool IsString{
			get { return CSharpDataType.ToLowerInvariant() == "string"; }
		}

		/// <summary>
		/// Признак табличного типа 
		/// </summary>
		public bool IsTable { get; set; }

		/// <summary>
		/// Признак целевого класса для IsTable и IsIdRef
		/// </summary>
		public PersistentClass TargetType { get; set; }

		/// <summary>
		/// Признак ссылочного (по ид) типа
		/// </summary>
		public bool IsIdRef { get; set; }

		/// <summary>
		///     Признак строкового параметра
		/// </summary>
		public bool IsDateTime{
			get {
				var cSharpType = CSharpDataType.ToLowerInvariant();
				return cSharpType == "system.datetime" || cSharpType == "datetime" || this.Code.Contains("date"); 
			}
		}

		/// <summary>
		///     Тип данных на ридере
		/// </summary>
		public string ReaderCSharpDataType{
			get { return string.IsNullOrWhiteSpace(_readerCSharpDataType) ? CSharpDataType : _readerCSharpDataType; }
			set { _readerCSharpDataType = value; }
		}

		/// <summary>
		///     Код типа данных в BSharp
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		///     Тип C#
		/// </summary>
		public string CSharpDataType{
			get { return _cSharpDataType; }
			set{
				_cSharpDataType = value;
				if (string.IsNullOrWhiteSpace(_readerCSharpDataType)){
					_readerCSharpDataType = value;
				}
			}
		}

		/// <summary>
		/// </summary>
		public IDictionary<SqlDialect, SqlDataType> SqlDataTypes { get; private set; }
		/// <summary>
		/// Признак булевого типа
		/// </summary>
		public bool IsBool{
			get { return CSharpDataType == "Boolean"; }
		}
		/// <summary>
		/// Признак прямого определения типа
		/// </summary>
		public bool IsNative { get; set; }
		/// <summary>
		/// Прямой текст SQL
		/// </summary>
		public string SqlText { get; set; }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		public DataType Copy(){
			return MemberwiseClone() as DataType;
		}

		/// <summary>
		///     Определяет наиболее соответствующий тип данных для указанного диалекта
		/// </summary>
		/// <remarks>Использует приоритет - прямое укзание диалекта, потом ANSI, потом varchar(255)</remarks>
		public SqlDataType ResolveSqlDataType(SqlDialect dialect = SqlDialect.Ansi){
			
			if (SqlDataTypes.ContainsKey(dialect)){
				return SqlDataTypes[dialect];
			}
			if (SqlDataTypes.ContainsKey(SqlDialect.Ansi)){
				return SqlDataTypes[SqlDialect.Ansi];
			}
			return SqlDataType.Default;
		}

		/// <summary>
		///     Возвращает словарь типов данных по умолчанию
		/// </summary>
		/// <returns></returns>
		public static IDictionary<string, DataType> GetDefaultMapping(){
			var result = new Dictionary<string, DataType>();
			result["int"] = new DataType{
				Code = "int",
				CSharpDataType = "Int32",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "integer"}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "int"}},
					{SqlDialect.PostGres, new SqlDataType{Name = "int"}},
				}
			};
            result["float"] = new DataType {
                Code = "float",
                CSharpDataType = "double",
                SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "float"}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "float"}},
					{SqlDialect.PostGres, new SqlDataType{Name = "float"}},
				}
            };
            result["double"] = new DataType
            {
                Code = "double",
                CSharpDataType = "double",
                SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "double"}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "double"}},
					{SqlDialect.PostGres, new SqlDataType{Name = "double"}},
				}
            };
            result["long"] = new DataType {
				Code = "long",
				CSharpDataType = "Int64",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "bigint"}},
				}
			};
			result["bool"] = new DataType{
				Code = "bool",
				CSharpDataType = "Boolean",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "boolean"}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "bit"}},
				}
			};

			result["datetime"] = new DataType{
				Code = "datetime",
				CSharpDataType = "System.DateTime",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "time"}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "datetime"}},
					{SqlDialect.PostGres, new SqlDataType{Name = "timestamp"}},
				}
			};

			result["decimal"] = new DataType{
				Code = "decimal",
				CSharpDataType = "Decimal",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "numeric", Size = 18, Precession = 6}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "decimal", Size = 18, Precession = 6}},
				}
			};

			result["string"] = new DataType{
				Code = "string",
				CSharpDataType = "String",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "varchar", Size = 255}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "nvarchar", Size = 255}},
				}
			};

			result["geometry"] = new DataType
			{
				Code = "geometry",
				CSharpDataType = "String",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "varchar", Size = 255}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "geometry"}},
				}
			};

			result["geography"] = new DataType
			{
				Code = "geography",
				CSharpDataType = "String",
                
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "varchar", Size = 255}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "geography"}},
                    
				}
			};


			result["shortstring"] = new DataType{
				Code = "shortstring",
				CSharpDataType = "String",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "varchar", Size = 20}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "nvarchar", Size = 20}},
				}
			};


			result["longstring"] = new DataType{
				Code = "longstring",
				CSharpDataType = "String",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "varchar", Size = 400}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "nvarchar", Size = 400}},
				}
			};

			result["text"] = new DataType{
				Code = "text",
				CSharpDataType = "String",
				SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "varchar", Size = 8000}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "nvarchar", Size = -1}},
					{SqlDialect.PostGres, new SqlDataType{Name = "text"}},
				}
			};

            result["xml"] = new DataType
            {
                Code = "xml",
                CSharpDataType = "XElement",
                SqlDataTypes ={
					{SqlDialect.Ansi, new SqlDataType{Name = "varchar", Size = 8000}},
					{SqlDialect.SqlServer, new SqlDataType{Name = "xml", Size = -1}},
					{SqlDialect.PostGres, new SqlDataType{Name = "xml"}},
				}
            };


			return result;
		}

		/// <summary>
		///     Конвертирует XML в описатель типа
		/// </summary>
		/// <param name="c"></param>
		/// <param name="dt"></param>
		/// <returns></returns>
		public DataType Setup(IBSharpClass c, XElement dt){
			Code = dt.Attr("code");
			CSharpDataType = dt.ChooseAttr("scharp", "type", Code);
			string ansitype = dt.Attr("sql", Code);
			int size = dt.Attr("size").ToInt();
			int precession = dt.Attr("precession").ToInt();
			SqlDataTypes[SqlDialect.Ansi] = new SqlDataType{Name = ansitype, Size = size, Precession = precession, Dialect = "ansi"};
			foreach (string d in Enum.GetNames(typeof (SqlDialect))){
				string normaltype = d.ToLowerInvariant();
				if (normaltype == "ansi") continue;
				string dialecttype = dt.Attr(normaltype);
				if (!string.IsNullOrWhiteSpace(dialecttype)){
					var sqltype = new SqlDataType{Name = dialecttype, Size = size, Precession = precession, Dialect=normaltype};
					SqlDataTypes[d.To<SqlDialect>()] = sqltype;
				}
			}
			return this;
		}
	}
}