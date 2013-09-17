using System.IO;
using Qorpent.IO.DirtyVersion.Mapping;

namespace Qorpent.IO.DirtyVersion {
	/// <summary>
	/// Интерфейс грязного хранилища версий
	/// </summary>
	public interface IDirtyVersionStorage {
		/// <summary>
		/// Прямой доступ к мэперу
		/// </summary>
		/// <returns></returns>
		IMapper GetMapper();

		/// <summary>
		/// Прямой доступ к хранилищу
		/// </summary>
		/// <returns></returns>
		IHashedDirectory GetStorage();

		/// <summary>
		/// Сохраняет строчные значения
		/// </summary>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <param name="basecommit"></param>
		/// <returns></returns>
		Commit Save(string name, string data, string basecommit = null);

		/// <summary>
		/// Сохраняет массив байтов
		/// </summary>
		/// <param name="name"></param>
		/// <param name="data"></param>
		/// <param name="basecommit"></param>
		/// <returns></returns>
		Commit Save(string name, byte[] data, string basecommit = null);

		/// <summary>
		/// Сохраняет данные из потока
		/// </summary>
		/// <param name="name"></param>
		/// <param name="stream"></param>
		/// <param name="basecommit"></param>
		/// <returns></returns>
		Commit Save(string name, Stream stream, string basecommit = null);

		/// <summary>
		/// Открывет на чтение файловый поток
		/// </summary>
		/// <param name="name"></param>
		/// <param name="versionHash"></param>
		/// <returns></returns>
		Stream Open(string name, string versionHash = null);
	}
}