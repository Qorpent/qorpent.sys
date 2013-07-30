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
		public virtual byte[] GetData() {
			if (null == FixedContent) return new byte[] {};
			return FixedContent;
		}

		/// <summary>
		/// Асинхронный метод получения данных
		/// </summary>
		/// <returns></returns>
		public virtual Task<byte[]> BeginGetData() {
			return Task.Run(() => GetData());
		}

		/// <summary>
		/// Метод открытия потока к данным
		/// </summary>
		/// <returns></returns>
		public virtual Stream Open() {
			return new MemoryStream(GetData());
		}
	}
}