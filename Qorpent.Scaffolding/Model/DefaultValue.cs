using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model{
	/// <summary>
	///     Значение по умолчанию для поля
	/// </summary>
	public class DefaultValue{
		/// <summary>
		///     Объект со значением
		/// </summary>
		public object Value { get; set; }
		/// <summary>
		///		Признак того, что значение по умолчанию отличается от default (T)
		/// </summary>
		public bool IsDefault { get; set; }
		/// <summary>
		///     Тип значения по умолчанию
		/// </summary>
		public DbDefaultValueType DefaultValueType { get; set; }

		/// <summary>
		///     Создает значение по умолчанию
		/// </summary>
		/// <param name="dbField"></param>
		/// <param name="attr"></param>
		/// <param name="e"></param>
		/// <returns></returns>
		public static DefaultValue Create(Field dbField, string attr, XElement e){
			var result = new DefaultValue();
			result.DefaultValueType = DbDefaultValueType.Native;
			if (string.IsNullOrWhiteSpace(attr)){
				if (dbField.DataType.IsString){
					result.Value = "";
				}
				else{
					result.Value = 0;
				}
				result.IsDefault = true;
			}
			else{
				if (attr.Contains("'") || attr.Contains("(")){
					result.Value = attr;
					result.DefaultValueType = DbDefaultValueType.Expression;
				}
				else{
					if (dbField.DataType.IsString){
						result.Value = attr;
					}
					else if (dbField.DataType.IsDateTime){
						try{
							attr.ToDate();
							result.DefaultValueType = DbDefaultValueType.Native;

							result.Value = attr;
						}
						catch{
							result.DefaultValueType = DbDefaultValueType.Expression;

							result.Value = attr;
						}
					}
					else{
						result.Value = attr.ToDecimal();
					}
				}
				result.IsDefault = false;
			}
			return result;
		}
	}
}