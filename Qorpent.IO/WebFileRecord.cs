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
		public string GetContent(){
			var enc = Encoding ?? Encoding.UTF8;
			var data = GetData();
			var offset = 0;

			if (enc.Equals(Encoding.UTF8) && data.Length >= 3 && data[0] == '\x00EF' && data[1] == '\x00BB' && data[2] == '\x00BF'){
				offset = 3;
			}
			return enc.GetString(data, offset, data.Length - offset);
		}
		/// <summary>
		/// Кодировка
		/// </summary>
		public Encoding Encoding { get; set; }
	}
}