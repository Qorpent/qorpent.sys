using System.IO;
using Qorpent.IO.DirtyVersion.Storage;

namespace Qorpent.IO.DirtyVersion {
	/// <summary>
	/// Базовый класс для хэшированных хранилищ
	/// </summary>
	public abstract class HashedDirectoryBase {
		/// <summary>
		/// Корневая директория
		/// </summary>
		protected string _rootDirectory;

		/// <summary>
		/// 
		/// </summary>
		protected readonly Hasher _hasher;
		/// <summary>
		///
		/// </summary>
		protected bool _compress;

		/// <summary>
		/// Создает хэшированную директорию для записи файлов
		/// </summary>
		protected HashedDirectoryBase(string targetDirectoryName, bool compress = true, int hashsize = Const.MaxHashSize) {
			_rootDirectory = targetDirectoryName;
			_compress = compress;
			_hasher = new Hasher(hashsize);
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		protected string ConvertToHash(string filename) {
			return Path.GetFileName( Path.GetDirectoryName(filename)) + Path.GetFileName(filename);
		}

		/// <summary>
		/// Конвертирует хэш в путь
		/// </summary>
		/// <returns></returns>
		public string ConvertToHasedFileName(string filename) {
			return ConvertToHasedFileName(_rootDirectory, MakeHash(filename));
		}
		/// <summary>
		/// Формирует хэш строки
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public string MakeHash(string filename) {
			return _hasher.GetHash(filename);
		}

		/// <summary>
		/// Конвертирует хэш в путь
		/// </summary>
		/// <param name="root"></param>
		/// <param name="hash"></param>
		/// <returns></returns>
		protected string ConvertToHasedFileName(string root, string hash) {
			return Path.GetFullPath( Path.Combine(root, hash.Substring(0, 2) + "/" + hash.Substring(2)));
		}
	}
}