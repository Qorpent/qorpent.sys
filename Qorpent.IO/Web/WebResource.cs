using System;
using System.IO;
using System.Threading.Tasks;
using Qorpent.IO.Resources;

namespace Qorpent.IO.Web {
	/// <summary>
	/// Результат выполнения запроса
	/// </summary>
	public class WebResource : IResource {
		/// <summary>
		/// Метаданные ресурса
		/// </summary>
		public IResourceConfig Config { get; private set; }

		/// <summary>
		/// Метод синхронного получения данных
		/// </summary>
		/// <returns></returns>
		public byte[] GetData() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Асинхронный метод получения данных
		/// </summary>
		/// <returns></returns>
		public Task<byte[]> BeginGetData() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// Метод открытия потока к данным
		/// </summary>
		/// <returns></returns>
		public Stream Open() {
			throw new NotImplementedException();
		}
	}
}