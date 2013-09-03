using System.IO;
using System.Threading.Tasks;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Базовая абстрактная реализация ресурса
	/// </summary>
	public class ResourceBase : IResource {
		/// <summary>
		/// Метаданные ресурса
		/// </summary>
		public IResourceConfig Config { get; set; }
		/// <summary>
		/// Простое свойство для явного указания данных
		/// </summary>
		public byte[] FixedContent { get; set; }

		/// <summary>
		/// Метод синхронного получения данных
		/// </summary>
		/// <returns></returns>
		public virtual byte[] SyncGetData() {
			if (null == FixedContent) return new byte[] {};
			return FixedContent;
		}

		/// <summary>
		/// Асинхронный метод получения данных
		/// </summary>
		/// <returns></returns>
		public virtual Task<byte[]> GetData() {
			return Task.Run(() => SyncGetData());
		}

		

		/// <summary>
		/// Метод открытия потока к данным
		/// </summary>
		/// <returns></returns>
		public async virtual Task<Stream> Open() {
			return new MemoryStream(await GetData());
		}
		/// <summary>
		/// 
		/// </summary>
		public void Dispose() {
			FixedContent = null;
			Config = null;
		}
	}
}