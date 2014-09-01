namespace Qorpent.IO.Net{
	/// <summary>
	/// Сущность HTTP
	/// </summary>
	public class HttpEntity{
		/// <summary>
		/// Тип сущности
		/// </summary>
		public HttpEntityType Type;
		/// <summary>
		/// Значение в виде бинарника
		/// </summary>
		public byte[] BinaryData;
		/// <summary>
		/// Значение в виде строки
		/// </summary>
		public string StringData;
		/// <summary>
		/// Значение в виде числа
		/// </summary>
		public int NumericData;
	}
}