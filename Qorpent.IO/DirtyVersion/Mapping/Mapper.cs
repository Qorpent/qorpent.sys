using System;
using System.IO;

namespace Qorpent.IO.DirtyVersion.Mapping
{
	/// <summary>
/// Оператор журнала
/// </summary>
	public class Mapper:HashedDirectoryBase, IMapper {
		/// <summary>
		/// Основной конструктор
		/// </summary>

		public Mapper(string targetDirectoryName, bool compress = true, int hashsize = Const.MaxHashSize) :
			base(targetDirectoryName, compress, hashsize) {
			
		}
		/// <summary>
		/// Открывет оператора версий для указанного файла
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public IMappingOperator Open(string filename) {
			var session = OpenSession(filename);
			return session.GetOperator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public IMapperSession OpenSession(string filename) {
			if (GetLock(filename)) {
				return new MapperSession(this, filename,MakeHash(filename),ConvertToHasedFileName(filename));
			}
			throw new Exception("file is locked");
		}

		/// <summary>
		/// Получает эксклюзивную блокировку на файл
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public bool GetLock(string filename) {
			var lockFileName = ConvertToHasedFileName(_rootDirectory, _hasher.GetHash(filename)) + Const.LOCKEXTENSION;
			if (File.Exists(lockFileName)) return false;
			Directory.CreateDirectory(Path.GetDirectoryName(lockFileName));
			using (var fs = new FileStream(lockFileName, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
				var data = BitConverter.GetBytes(DateTime.Now.ToOADate());
				fs.Write(data,0,data.Length);
				fs.Flush();
			}
			return true;
		}
		/// <summary>
		/// Снимает блокировку с файла
		/// </summary>
		/// <param name="filename"></param>
		public void ReleaseLock(string filename) {
			var lockFileName = ConvertToHasedFileName(_rootDirectory, _hasher.GetHash(filename)) + Const.LOCKEXTENSION;
			if (!File.Exists(lockFileName)) return;
			File.Delete(lockFileName);
		}


	}
}
