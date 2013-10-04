using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using Qorpent.Events;
using Qorpent.IoC;

namespace Qorpent.IO.Zip
{
	/// <summary>
	/// Источник файлов из Zip-архива
	/// </summary>
	[ContainerComponent(Name="zip.file.source",ServiceType = typeof(IFileSource))]
	public class ZipFileSource :ServiceBase, IFileSource
	{
		private Package _package;
	    private string _filename;
	    private bool _shadowed;

	    /// <summary>
	    /// 	CA1063 recomendation match
	    /// </summary>
	    /// <param name="full"> </param>
	    protected override void Dispose(bool full)
        {
            base.Dispose(full);
            if (null != _package) {
                _package.Close();
            }
	        _package = null;
	    }

	    /// <summary>
	    /// Создает обертку над файлом
	    /// </summary>
	    /// <param name="filename"></param>
	    public ZipFileSource(string filename) : this(filename,  false) {
	    }

	    private Stream _streamCopy = null;
        /// <summary>
        /// Создает источник в привязке с потоком
        /// </summary>
        public ZipFileSource  (Stream srcStream) {
            _streamCopy = new MemoryStream();
            srcStream.CopyTo(_streamCopy);
            _package = Package.Open(_streamCopy);
        }



	    /// <summary>
        /// Создает обертку над файлом
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="shadowed"></param>
        public ZipFileSource(string filename, bool shadowed) {
	        _filename = filename;
	        _shadowed = shadowed;
	        ReloadZipFile();
	    }

	    private void ReloadZipFile() {
	        Stream s = null;
	        if (_shadowed) {
	            s = new MemoryStream(File.ReadAllBytes(_filename));
	        }
	        else {
	            s = File.Open(_filename, FileMode.Open, FileAccess.ReadWrite);
	        }
	        _package = Package.Open(s);
	    }

	    /// <summary>
	    /// 	Вызывается при вызове Reset
	    /// </summary>
	    /// <param name="data"> </param>
	    /// <returns> любой объект - будет включен в состав результатов <see cref="ResetEventResult" /> </returns>
	    /// <remarks>
	    /// 	При использовании стандартной настройки из <see cref="ServiceBase" /> не требует фильтрации опций,
	    /// 	настраивается на основе атрибута <see cref="RequireResetAttribute" />
	    /// </remarks>
	    public override object Reset(ResetEventData data)
        {
            if (null != _package) {
                _package.Close();
            }
	        _package = null;
            if (!string.IsNullOrWhiteSpace(_filename)) {
                ReloadZipFile();
            }
	        return null;
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
