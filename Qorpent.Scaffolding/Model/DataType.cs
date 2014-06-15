﻿using System;
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
		///     Признак строкового параметра
		/// </summary>
		public bool IsDateTime{
			get { return CSharpDataType.ToLowerInvariant() == "datetime"; }
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
			result["long"] = new DataType{
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
					{SqlDialect.PostGres, new SqlDataType{Name = "datetime"}},
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
			SqlDataTypes[SqlDialect.Ansi] = new SqlDataType{Name = ansitype, Size = size, Precession = precession};
			foreach (string d in Enum.GetNames(typeof (SqlDialect))){
				string normaltype = d.ToLowerInvariant();
				if (normaltype == "ansi") continue;
				string dialecttype = dt.Attr(normaltype);
				if (!string.IsNullOrWhiteSpace(dialecttype)){
					SqlDataTypes[d.To<SqlDialect>()] = new SqlDataType{Name = dialecttype, Size = size, Precession = precession};
				}
			}
			return this;
		}
	}
}