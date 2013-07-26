using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Qorpent.Dsl.Json {
	/// <summary>
	/// Вообще единица JSON
	/// </summary>
	public abstract class JsonItem {
		/// <summary>
		/// Родительский элемент
		/// </summary>
		public JsonItem Parent;
		/// <summary>
		/// Значение
		/// </summary>
		public string Value;

		/// <summary>
		/// Признак возможности добавления значений (для массивов и объектов)
		/// </summary>
		public bool CanAddItems = true;

		/// <summary>
		/// Тип значения
		/// </summary>
		public TType Type;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return ToString(true);
		}
		/// <summary>
		/// 
		/// </summary>
		public int Level {
			get { 
				var result = 1;
				if (null != Parent) result += Parent.Level;
				return result;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <returns></returns>
		public abstract string ToString(bool format);

		/// <summary>
		/// Формирует свой контент в указанный элемент
		/// </summary>
		/// <param name="current"></param>
		public abstract XElement WriteToXml(XElement current =null);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected static bool IsNumber(string value) {
			return value.All(char.IsDigit);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected static bool IsLiteral(string value) {
			var fst = value[0];
			if (!(char.IsLetter(fst) || fst == '_')) return false;
			return value.All(_ => char.IsLetterOrDigit(_) || _ == '_' || _ == '.');
		}
	}
}