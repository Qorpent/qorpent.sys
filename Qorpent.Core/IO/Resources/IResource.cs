using System.IO;
using System.Threading.Tasks;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Интерфейс самого ресурса
	/// </summary>
	public interface IResource {
		/// <summary>
		/// Метаданные ресурса
		/// </summary>
		IResourceConfig Config { get; }
		/// <summary>
		/// Метод синхронного получения данных
		/// </summary>
		/// <returns></returns>
		byte[] GetData();
		/// <summary>
		/// Асинхронный метод получения данных
		/// </summary>
		/// <returns></returns>
		Task<byte[]> BeginGetData();
		/// <summary>
		/// Метод открытия потока к данным
		/// </summary>
		/// <returns></returns>
		Stream Open();
	}
}