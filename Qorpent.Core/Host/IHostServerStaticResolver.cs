using System.Xml.Linq;
using Qorpent.IO;

namespace Qorpent.Host{
	/// <summary>
	/// </summary>
	public interface IHostServerStaticResolver{
		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		void Initialize(IHostServer server);

		/// <summary>
		///     Возвращает дескриптор статического файла
		/// </summary>
		/// <param name="name"></param>
		/// <param name="context"></param>
		/// <param name="withextensions"></param>
		/// <returns></returns>
		IWebFileRecord Get(string name, object context = null, bool withextensions = false);

		/// <summary>
		///     Сброс кэша
		/// </summary>
		void DropCache();
		/// <summary>
		/// Устанавливает корневую директорю для части юрлов
		/// </summary>
		/// <param name="mask"></param>
		/// <param name="rootdirectory"></param>
		void SetRoot(string mask, StaticFolderDescriptor rootdirectory);
        /// <summary>
        /// Установить источник в виде кэша, завязанного на другие источники
        /// </summary>
        /// <param name="key"></param>
        /// <param name="config"></param>
	    void SetCachedRoot(string key, XElement config);
	}
}