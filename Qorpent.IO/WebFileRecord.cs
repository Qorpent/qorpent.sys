using System;
using System.IO;
using System.Text;

namespace Qorpent.IO{
	/// <summary>
	/// 
	/// </summary>
	public abstract class WebFileRecord : IWebFileRecord{

		private DateTime _version;
		private string _eTag;
		private string _mimeType;


		/// <summary>
		/// Локальное имя записи
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// Версия записи
		/// </summary>
		public DateTime Version{
			get{
				if (_version.Year <= 1900) return GetVersion();
				return _version;
			}
			set { _version = value; }
		}
		/// <summary>
		/// Кэш-ключ записи
		/// </summary>
		public string ETag{
			get{
				if (string.IsNullOrWhiteSpace(_eTag)) _eTag = GetETag();
				return _eTag;
			}
			set { _eTag = value; }
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected virtual string GetETag(){
			return Version.ToString("yyyyMMddhhmmss");
		}
		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract DateTime GetVersion();

		/// <summary>
		/// Записывает файл в целевой поток
		/// </summary>
		/// <param name="output"></param>
		public abstract long Write(Stream output);
		/// <summary>
		/// Открытие потока на чтение
		/// </summary>
		/// <returns></returns>
		public abstract Stream Open();
		/// <summary>
		/// Тип MIME для файла
		/// </summary>
		public string MimeType{
			get{
				if (string.IsNullOrWhiteSpace(_mimeType)){
					_mimeType =  MimeHelper.GetMimeByExtension(Path.GetExtension(Name));
				}
				return _mimeType;
			}
			set { _mimeType = value; }
		}

		/// <summary>
		/// Возврат значения в виде массива байт
		/// </summary>
		/// <returns></returns>
		public byte[] GetData(){
			var ms = new MemoryStream();
			var len = Write(ms);
			var result = new byte[len];
			Array.Copy(ms.GetBuffer(),result,result.Length);
			return result;
		}
		/// <summary>
		/// Возврат контента в виде строки
		/// </summary>
		/// <returns></returns>
		public string Read(){
			var enc = Encoding ?? Encoding.UTF8;
			var data = GetData();
			var offset = 0;

			if (enc.Equals(Encoding.UTF8) && data.Length >= 3 && data[0] == '\x00EF' && data[1] == '\x00BB' && data[2] == '\x00BF'){
				offset = 3;
			}
			return enc.GetString(data, offset, data.Length - offset);
		}

		/// <summary>
		/// Полное имя
		/// </summary>
		public string FullName { get; set; }

		/// <summary>
		/// Признак фиксированного контента
		/// </summary>
		public bool IsFixedContent { get; set; }

		/// <summary>
		/// Фиксированный контент
		/// </summary>
		public string FixedContent { get; set; }

		/// <summary>
		/// Кодировка
		/// </summary>
		public Encoding Encoding { get; set; }

		/// <summary>
		/// Фиксированные бинарные данные
		/// </summary>
		public byte[] FixedData { get; set; }

	    public string Role { get; set; }
	}
}