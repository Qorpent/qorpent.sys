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
	}
}