using System.IO;

namespace Qorpent.IO{
	/// <summary>
	/// Интерфейс для поддержки оптимизированной бинарной сериализации
	/// </summary>
	public interface IBinarySerializable{
		/// <summary>
		/// Считать объект из ридера
		/// </summary>
		/// <param name="reader"></param>
		void Read(BinaryReader reader);
		/// <summary>
		/// Записать объект в райтер
		/// </summary>
		/// <param name="writer"></param>
		void Write(BinaryWriter writer);
	}
}