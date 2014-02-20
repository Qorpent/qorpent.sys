using System.Data;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;
namespace Qorpent.Data.BSharpDDL{
	/// <summary>
	/// Значение по умолчанию для поля
	/// </summary>
	public class DbDefaultValue	{
		/// <summary>
		/// Объект со значением
		/// </summary>
		public object Value { get; set; }

		/// <summary>
		/// Тип значения по умолчанию
		/// </summary>
		public DbDefaultValueType DefaultValueType { get; set; }

		/// <summary>
		/// Создает значение по умолчанию
		/// </summary>
		/// <param name="dbField"></param>
		/// <param name="attr"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static DbDefaultValue Create(DbField dbField, string attr, XElement e){
			var result = new DbDefaultValue();
			result.DefaultValueType = DbDefaultValueType.Native;
			if (string.IsNullOrWhiteSpace(attr)){
				if (dbField.DataType.DbType == DbType.String){
					result.Value = "";
				}
				else{
					result.Value = 0;
				}
			}
			else{
	
				if (attr.Contains("'") || attr.Contains("(")){
					result.Value = attr;
					result.DefaultValueType = DbDefaultValueType.Expression;
				}
				else{
					if (dbField.DataType.DbType == DbType.String){
						result.Value = attr;
					}
					else{
						result.Value = attr.ToDecimal();
					}
				}
			}
			return result;
		}
	}
}