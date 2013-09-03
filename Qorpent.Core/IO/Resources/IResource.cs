using System;
using System.IO;
using System.Threading.Tasks;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Интерфейс самого ресурса
	/// </summary>
	public interface IResource:IDisposable {
		/// <summary>
		/// Метаданные ресурса
		/// </summary>
		IResourceConfig Config { get; }
		
		/// <summary>
		/// Асинхронный метод получения данных
		/// </summary>
		/// <returns></returns>
		Task<byte[]> GetData();
		/// <summary>
		/// Метод открытия потока к данным
		/// </summary>
		/// <returns></returns>
		Task<Stream> Open();
	}
}