using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Класс, описывающий результат обработки HTTP протокола
	/// </summary>
	public class HttpResult{
		private readonly IHttpReader _reader;
		private bool _preambleRead;
		private bool _headersRead;
		private bool _hasContent;
		private bool _contentRead;
		private string _version;
		private int _state;
		private string _stateName;
		private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();
		private CookieCollection _cookies = new CookieCollection();
		private int _contentLength;
		private string _contentType;
		private string _charset;
		private string _connection;
		private byte[] _data;

		/// <summary>
		/// 
		/// </summary>
		public HttpResult(){
			
		}
		/// <summary>
		/// Создает HttpResult в виде обертки над Stream
		/// </summary>
		public HttpResult(Stream stream)
		{
			_reader = new HttpReader(stream);
		}
		/// <summary>
		/// Создает HttpResult в виде обертки над HttpReader
		/// </summary>
		/// <param name="reader"></param>
		public HttpResult(IHttpReader reader){
			_reader = reader;
		}
		/// <summary>
		/// Версия HTTP
		/// </summary>
		public string Version{
			get{
				if(!_preambleRead && null!=_reader)ReadPreamble();
				return _version;
			}
			set { _version = value; }
		}
		/// <summary>
		/// Статус
		/// </summary>
		public int State{
			get{
				if (!_preambleRead && null != _reader) ReadPreamble();
				return _state;
			}
			set { _state = value; }
		}
		/// <summary>
		/// Название статуса
		/// </summary>
		public string StateName{
			get{
				if (!_preambleRead && null != _reader) ReadPreamble();
				return _stateName;
			}
			set { _stateName = value; }
		}
		/// <summary>
		/// Заголовки
		/// </summary>
		public IDictionary<string, string> Headers{
			get{
				if(!_headersRead && null!=_reader)ReadHeaders();
				return _headers;
			}
		}

		/// <summary>
		/// Коллекция куки
		/// </summary>
		public CookieCollection Cookies{
			get{
				if (!_headersRead && null != _reader) ReadHeaders();
				return _cookies;
			}
			set { _cookies = value; }
		}
		/// <summary>
		/// Длина контента в байтах
		/// </summary>
		public int ContentLength{
			get{
				if (!_headersRead && null != _reader) ReadHeaders();
				return _contentLength;
			}
			set { _contentLength = value; }
		}
		/// <summary>
		/// Исходный content-Type
		/// </summary>
		public string ContentType{
			get{
				if (!_headersRead && null != _reader) ReadHeaders();
				return _contentType;
			}
			set { _contentType = value; }
		}
		/// <summary>
		/// Исходное определение кодировки
		/// </summary>
		public string Charset{
			get{
				if (!_headersRead && null != _reader) ReadHeaders();
				return _charset;
			}
			set { _charset = value; }
		}
		/// <summary>
		/// Информация о режиме соединения
		/// </summary>
		public string Connection{
			get{
				if (!_headersRead && null != _reader) ReadHeaders();
				return _connection;
			}
			set { _connection = value; }
		}
		/// <summary>
		/// Считанные данные 
		/// </summary>
		public byte[] Data{
			get{
				if (!_contentRead && null != _reader) ReadContent();
				return _data;
			}
			set { _data = value; }
		}
		/// <summary>
		/// Признак сжатия GZip
		/// </summary>
		public bool IsGZiped{
			get { return ContentType.Contains("gzip"); }
		}
		/// <summary>
		/// Признак сжатия GZip
		/// </summary>
		public bool IsDeflated
		{
			get { return ContentType.Contains("deflate"); }
		}

		private Encoding _encoding;
		/// <summary>
		/// Кодировка
		/// </summary>
		public Encoding Encoding{
			get{
				if (null == _encoding){
					if (string.IsNullOrWhiteSpace(Charset)){
						_encoding = Encoding.UTF8;
					}
					else{
						_encoding = Encoding.GetEncoding(Charset);
					}
				}
				return _encoding;
			}
		}
		/// <summary>
		/// Возвращает строчное значение
		/// </summary>
		public string StringData {
			get{
				if (null == Data) return "";
				return Encoding.GetString(Data);
			}
		}

		private void ReadPreamble(){
			if (null == _reader) return;
			if (_preambleRead) return;
			Version = Read(HttpEntityType.Version).StringData;
			State = Read(HttpEntityType.State).NumericData;
			StateName = Read(HttpEntityType.StateName).StringData;
			_preambleRead = true;
		}

		private void ReadContent(){
			if(null==_reader)return;
			if (_contentRead) return;
			if (!_headersRead) ReadHeaders();
			_contentRead = true;
			if (!_hasContent){
				return;
			}
			var ms = new MemoryStream();
			var workingStream = ms;
			
			while (true){
				var chunk = Read(HttpEntityType.Chunk | HttpEntityType.Finish);
				if (chunk.Type == HttpEntityType.Chunk){
					workingStream.Write(chunk.BinaryData,0,chunk.Length);
				}
				else{
					break;
				}
			}
			byte[] buffer = null;
			if (IsGZiped || IsDeflated){
				var src = ms;
				src.Position = 0;
				ms = new MemoryStream();
				var zipbuf = new byte[1024];
				using (
					Stream zipped = (IsGZiped
						            ? (Stream)new GZipStream(src, CompressionMode.Decompress)
									: new DeflateStream(src, CompressionMode.Decompress)))
				{

					while (true){
						var read = zipped.Read(zipbuf, 0, zipbuf.Length);
						if (read <= 0){
							break;
						}
						ms.Write(zipbuf, 0, read);
						
					}
				}
				
			}
			var length = ms.Position;
			ms.Position = 0;
			buffer = new byte[length];
			ms.Read(buffer, 0, buffer.Length);

			Data = buffer;
			
		}

		private void ReadHeaders(){
			if (null == _reader) return;
			if (_headersRead) return;
			if(!_preambleRead)ReadPreamble();
			_headersRead = true;
			HttpEntity previous = null;
			while (true){
				var current = Read(HttpEntityType.HeaderName | HttpEntityType.HeaderValue | HttpEntityType.ContentStart | HttpEntityType.Finish);
				if (current.Type == HttpEntityType.Finish){
					if (previous != null && previous.Type != HttpEntityType.HeaderValue){
						throw new HttpReaderException("unexpected finish position");
					}
					_contentRead = true;
					break;
				}
				if (current.Type == HttpEntityType.ContentStart){
					if (previous != null && previous.Type != HttpEntityType.HeaderValue)
					{
						throw new HttpReaderException("unexpected start content position");
					}
					_hasContent = true;
					break;
				}

				if (current.Type == HttpEntityType.HeaderName){
					if (previous != null && previous.Type != HttpEntityType.HeaderValue)
					{
						throw new HttpReaderException("unexpected header name position");
					}
				}
				else if (current.Type == HttpEntityType.HeaderValue){
					if (null == previous || previous.Type != HttpEntityType.HeaderName){
						throw new HttpReaderException("unexpected header value position");
					}
					RegisterHeader(previous.StringData, current.StringData);
				}

				previous = current;
			}
			
		}

		private void RegisterHeader(string name, string value){
			Headers[name] = value;
			if (name == "Content-Length")
			{
				ContentLength = value.ToInt();
			}
			else if (name == "Content-Type")
			{
				string[] parts = value.Split(';');
				ContentType = parts[0];
				if (parts.Length > 1)
				{
					Charset = parts[1].Split('=')[1];
				}
			}
			else if (null != Cookies && name == "Set-Cookie")
			{
				foreach (Cookie cookie in HttpReaderUtils.ParseCookies(value))
				{
					Cookies.Add(cookie);
				}
			}
			else if (name == "Connection")
			{
				Connection = value;
			}
		}

		private HttpEntity Read(HttpEntityType typeFilter = HttpEntityType.Undefined, bool allowNulls = false, bool throwError = true){
			var result = _reader.Next();
			if (null == result){
				if (allowNulls) return null;
				throw new HttpReaderException("unexpected NULL HTTP entity");
			}
			if (result.Type == HttpEntityType.Error){
				if (throwError){
					throw new HttpReaderException(result.Error.ToString());
				}
				return result;
			}
			if (typeFilter != HttpEntityType.Undefined){
				if (!typeFilter.HasFlag(result.Type)){
					throw new HttpReaderException("unexpected entity type "+result.Type);
				}
			}
			return result;
		}

	}
}