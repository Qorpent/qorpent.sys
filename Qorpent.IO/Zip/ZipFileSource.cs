using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IoC;

namespace Qorpent.IO.Zip
{
	/// <summary>
	/// Источник файлов из Zip-архива
	/// </summary>
	[ContainerComponent(Name="zip.file.source",ServiceType = typeof(IFileSource))]
	public class ZipFileSource : IFileSource
	{
		private Package _package;

		/// <summary>
		/// Загружает Zip - исходник из потока
		/// </summary>
		/// <param name="stream"></param>
		public ZipFileSource(Stream stream) {
			this._package = Package.Open(stream);
		}

		/// <summary>
		/// Загружает Zip-источник из ранее подготовленного потока
		/// </summary>
		/// <param name="package"></param>
		public ZipFileSource(Package package) {
			this._package = package;
		}
		/// <summary>
		/// Возвращает имена файлов из Zip- файла
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetFileNames() {
			return _package.GetParts().Select(_ => _.Uri.ToString()).ToArray();
		}
		/// <summary>
		/// Открывает поток файла на чтение из Zip-файла
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public Stream Open(string name) {
			return _package.GetPart(new Uri(name,UriKind.Relative)).GetStream(FileMode.Open);
		}
	}
}
