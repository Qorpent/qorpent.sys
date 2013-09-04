using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace Qorpent.IO.DirtyVersion.Storage
{
	/// <summary>
	/// Специальная диреткория, выполняющая сохранение объекта в виде хэшированной записи
	/// </summary>
	public class HashedDirectory : HashedDirectoryBase {
		/// <summary>
		/// Создает хэшированную директорию для записи файлов
		/// </summary>
	
		public HashedDirectory(string targetDirectoryName, bool compress = true, int hashsize = Const.HashSize) : base(targetDirectoryName, compress: compress, hashsize: hashsize) {}

		/// <summary>
		/// Выполняет сохранение файла с формированием хэш -записи
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="data"></param>
		public HashedDirectoryRecord Write(string filename,string data) {
			return Write(filename, new MemoryStream(Encoding.UTF8.GetBytes(data)));
		}

		/// <summary>
		/// Выполняет сохранение файла с формированием хэш -записи
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="data"></param>
		public HashedDirectoryRecord Write(string filename, Stream data) {
			var stubFile = Path.Combine(_rootDirectory, Guid.NewGuid().ToString());
			var fileNameHash = _hasher.GetHash(filename);
			var proxyStream = new CopyOnReadStream(data, stubFile,_compress);
			var dataHash = _hasher.GetHash(proxyStream);
			proxyStream.Close();
			var fileDir = ConvertToFileName(_rootDirectory, fileNameHash);
			var fileName = ConvertToFileName(fileDir, dataHash);
			if (File.Exists(fileName)) {
				File.Delete(stubFile);
				File.SetLastWriteTime(fileName,DateTime.Now);
			}
			else {
				Directory.CreateDirectory(Path.GetDirectoryName(fileName));
				File.Move(stubFile,fileName);
			}

			return new HashedDirectoryRecord {DataHash = dataHash, NameHash = fileNameHash,LastWriteTime = File.GetLastWriteTime(fileName)};
		}
		/// <summary>
		/// Проверка на существовании в директории хэшированного файла
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public bool Exists(string filename) {
			return Exists(new HashedDirectoryRecord {NameHash = _hasher.GetHash(filename)});
		}
		/// <summary>
		/// Проверка на существовании по записи
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public bool Exists(HashedDirectoryRecord record) {
			var dir = ConvertToFileName(_rootDirectory, record.NameHash);
			if (string.IsNullOrWhiteSpace(record.DataHash)) {
				return Directory.Exists(dir) && Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories).Any();
			}
			var fname = ConvertToFileName(dir, record.DataHash);
			return File.Exists(fname);
		}
		/// <summary>
		/// Считывает последнюю версию файла
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public Stream Open(string filename) {
			return Open(new HashedDirectoryRecord { NameHash = _hasher.GetHash(filename)});
		}
		/// <summary>
		/// Считывает последнюю или указанную версию файла
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public Stream Open(HashedDirectoryRecord record) {
			var fname = ResolveNativeFileName(record);
			var result = (Stream)File.Open(fname, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
			if (_compress) {
				result = new GZipStream(result,CompressionMode.Decompress);
			}
			return result;
		}
		/// <summary>
		/// Возвращает реальный полный путь к файлу
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public string ResolveNativeFileName(HashedDirectoryRecord record) {
			string fname;
			var dir = ConvertToFileName(_rootDirectory, record.NameHash);
			if (!Exists(record)) throw new Exception("Запись не существует");
			if (string.IsNullOrWhiteSpace(record.DataHash)) {
				var last = FindLast(record);
				if (null == last) throw new Exception("last record not found");
				fname = ConvertToFileName(dir, last.DataHash);
			}
			else {
				fname = ConvertToFileName(dir, record.DataHash);
			}
			return fname;
		}

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public HashedDirectoryRecord FindLast(string filename)
		{
			return FindLast(filename, DateTime.MaxValue);
		}

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="beforeTime"></param>
		/// <returns></returns>
		public HashedDirectoryRecord FindLast(string filename, DateTime beforeTime)
		{
			return FindLast(new HashedDirectoryRecord {NameHash = _hasher.GetHash(filename), LastWriteTime = beforeTime});
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public HashedDirectoryRecord FindLast(HashedDirectoryRecord file)
		{
			return
				EnumerateFiles(file)
				.Where(_ => _.LastWriteTime < file.LastWriteTime)
				.OrderByDescending(_ => _.LastWriteTime)
				.FirstOrDefault();
		}

		private HashedDirectoryRecord ConvertToResultFileRecord(HashedDirectoryRecord file, string filename) {
			if (null == filename) return null;
			var hash = ConvertToHash(filename);
			return new HashedDirectoryRecord {
				NameHash = file.NameHash,
				DataHash = hash,
				LastWriteTime = File.GetLastWriteTime(filename)
			};
		}

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public HashedDirectoryRecord FindFirst(string filename)
		{
			return FindFirst(filename, DateTime.MinValue);
		}

		/// <summary>
		/// Возвращает последний файл, записанный до указанного времени
		/// </summary>
		/// <param name="filename"></param>
		/// <param name="fromTime"></param>
		/// <returns></returns>
		public HashedDirectoryRecord FindFirst(string filename, DateTime fromTime)
		{
			return FindFirst(new HashedDirectoryRecord { NameHash = _hasher.GetHash(filename), LastWriteTime = fromTime });
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public HashedDirectoryRecord FindFirst(HashedDirectoryRecord file) {
			return
				EnumerateFiles(file)
				.Where(_ => _.LastWriteTime > file.LastWriteTime)
				.OrderBy(_ => _.LastWriteTime)
				.FirstOrDefault();
		}
		/// <summary>
		/// Перечисляет все записи для указанного файла (хэши)
		/// </summary>
		/// <param name="file"></param>
		/// <returns></returns>
		public IEnumerable<HashedDirectoryRecord> EnumerateFiles(HashedDirectoryRecord file) {
			if (!Exists(file)) yield break;
			var dir = ConvertToFileName(_rootDirectory, file.NameHash);
			foreach (var filename in Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories)) {
				yield return ConvertToResultFileRecord(file, filename);
			}
		}
	}
}
