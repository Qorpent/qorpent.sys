namespace Qorpent.IO.DirtyVersion.Mapping {
	/// <summary>
	/// Общий интерфейс мэппера
	/// </summary>
	public interface IMapper {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		MapperSession Open(string filename);

		/// <summary>
		/// Получает эксклюзивную блокировку на файл
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		bool GetLock(string filename);

		/// <summary>
		/// Снимает блокировку с файла
		/// </summary>
		/// <param name="filename"></param>
		void ReleaseLock(string filename);
	}
}