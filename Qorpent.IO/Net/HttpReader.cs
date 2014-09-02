using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net{
	/// <summary>
	///     Класс для сканирования потока  и разбиения его в соответствии с протоколом HTTP
	/// </summary>
	public class HttpReader : IHttpReader{
		/// <summary>
		/// Размер буфера по умолчанию
		/// </summary>
		public  int DefaultBufferSize = 1024;
		private readonly BinaryReader _binaryReader;
		private readonly StringBuilder _builder = new StringBuilder();
		private readonly char[] _charBuffer = new char[512];
		private readonly Stream _stream;

		/// <summary>
		///     Максимальный размер контента
		/// </summary>
		public int MaxAllowedContentSize = 4194304;

		/// <summary>
		///     Максимальная разрешенная длина заголовков
		/// </summary>
		public int MaxAllowedHeadersSize = 16384;

		private char _cachedLast = '\0';
		private string _charset;
		private string _connection;
		private int _contentLength;
		private int _contentSize;
		private string _contentType;
		private bool _contentstartsent;
		private int _currentLength;
		private bool _finished;
		private int _headerSize;
		private string _lastHeaderName;
		private HttpReaderState _state = HttpReaderState.Start;
		private string _transferEncoding;
		private byte _cachedByte;


		/// <summary>
		///     Создает сканер в виде обертки над потоком
		/// </summary>
		/// <param name="stream"></param>
		public HttpReader(Stream stream){
			_stream = stream;
			_binaryReader = new BinaryReader(_stream, Encoding.ASCII);
		}


		/// <summary>
		///     Mime-type
		/// </summary>
		public string ContentType{
			get { return _contentType; }
		}

		/// <summary>
		///     Кодировка
		/// </summary>
		public string Charset{
			get { return _charset; }
		}

		/// <summary>
		///     Длина контента
		/// </summary>
		public int ContentLength{
			get { return _contentLength; }
		}

		/// <summary>
		///     Кодировка при передаче
		/// </summary>
		public string TransferEncoding{
			get { return _transferEncoding; }
		}

		/// <summary>
		///     Статус ридера
		/// </summary>
		public HttpReaderState State{
			get { return _state; }
		}

		/// <summary>
		///     Серверный тег соединения
		/// </summary>
		public string Connection{
			get { return _connection; }
		}

		/// <summary>
		///     Считывает следую
		/// </summary>
		/// <returns></returns>
		public HttpEntity Next(){
			try{
				switch (State){
					case HttpReaderState.Start:
						return ReadVersion();
					case HttpReaderState.State:
						return ReadState();
					case HttpReaderState.StateName:
						return ReadStateName();
					case HttpReaderState.PreHeader:
						return ReadHeaderName();
					case HttpReaderState.PreHeaderValue:
						return ReadHeaderValue();
					case HttpReaderState.Content:
						return ReadContent();
					case HttpReaderState.Error:
						return null;
					case HttpReaderState.Finish:
						return ReturnFinish();
					default:
						throw new HttpReaderException("undefined position");
				}
			}
			catch (Exception ex){
				_state = HttpReaderState.Error;
				return new HttpEntity{Type = HttpEntityType.Error, Error = ex};
			}
		}

		private HttpEntity ReturnFinish(){
			if (_finished){
				return null;
			}
			_finished = true;
			return new HttpEntity{Type = HttpEntityType.Finish};
		}

		private HttpEntity ReadContent(){
			if (_contentstartsent){
				if (_contentLength != 0){
					return NextNonChunkedPart();
				}
				return NextChunkedPart();
			}
			if (_contentLength == 0 && !_transferEncoding.Contains("chunked")){
				throw new HttpReaderException("No valid content mode");
			}
			_contentstartsent = true;
			return new HttpEntity{Type = HttpEntityType.ContentStart};
		}

		private HttpEntity NextChunkedPart(){
			string chunklen = ReadValue(6, eofAllowed: true);
			if (_state == HttpReaderState.Finish){
				if (chunklen == "0"){
					_finished = true;
					return new HttpEntity{Type = HttpEntityType.Finish};
				}
				throw new HttpReaderException("insuficient eof");
			}
			int len = Convert.ToInt32(chunklen, 16);
			if (0 == len){
				_state = HttpReaderState.Finish;
				_finished = true;
				return new HttpEntity{Type = HttpEntityType.Finish};
			}
			ReadNewLines();

			_contentSize += len;
			if (_contentSize > MaxAllowedContentSize){
				throw new HttpReaderException("insuficient content max size");
			}
			var buffer = new byte[len];
			buffer[0] = _cachedByte;
			int total = 1;
			while (total < len){
				int actualLength = _binaryReader.Read(buffer, total, len - total);
				total += actualLength;
			}

			ReadNewLines();
			return new HttpEntity{Type = HttpEntityType.Chunk, BinaryData = buffer, Length = len};
		}

		private HttpEntity NextNonChunkedPart(){
			int rest = Math.Min(_contentLength - _currentLength, DefaultBufferSize);
			var buffer = new byte[rest];
			int len = 0;
			if (_cachedLast != '\0'){
				buffer[0] = (byte) _cachedLast;
				_cachedLast = '\0';
				len = _binaryReader.Read(buffer, 1, buffer.Length - 1) + 1;
			}
			else{
				len = _binaryReader.Read(buffer, 0, buffer.Length);
			}
			_currentLength += len;
			if (len == 0 && _currentLength < _contentLength){
				throw new HttpReaderException("invalid content length");
			}
			if (_currentLength == _contentLength){
				_state = HttpReaderState.Finish;
			}
			return new HttpEntity{Type = HttpEntityType.Chunk, BinaryData = buffer, Length = len};
		}

		private HttpEntity ReadHeaderValue(){
			string headerValue = ReadValue(MaxAllowedHeadersSize - _headerSize, spaceAllowed: true);
			_headerSize += headerValue.Length;
			if (_headerSize > MaxAllowedHeadersSize){
				throw new HttpReaderException("insuficient header size");
			}
			ProcessMainHeaders(headerValue);
			ReadNewLines();
			if (State != HttpReaderState.Content){
				SetNewState(HttpReaderState.PreHeader, true);
			}
			return new HttpEntity{Type = HttpEntityType.HeaderValue, StringData = headerValue};
		}

		private void ProcessMainHeaders(string headerValue){
			if (_lastHeaderName == "Transfer-Encoding"){
				_transferEncoding = headerValue;
			}
			else if (_lastHeaderName == "Content-Length"){
				_contentLength = headerValue.ToInt();

				if (0 == ContentLength){
					throw new HttpReaderException("invalid content length");
				}

				if (_contentLength > MaxAllowedContentSize){
					throw new HttpReaderException("insuficient content length");
				}
			}
			else if (_lastHeaderName == "Content-Type"){
				string[] parts = headerValue.Split(';');
				_contentType = parts[0];
				if (parts.Length > 1){
					_charset = parts[1].Split('=')[1];
				}
			}
			else if (_lastHeaderName == "Connection"){
				_connection = headerValue;
			}
		}

		

		private HttpEntity ReadHeaderName(){
			string name = ReadValue(MaxAllowedHeadersSize - _headerSize);
			if (!name.EndsWith(":")){
				throw new HttpReaderException("insuficient header name");
			}
			SkipSpaces();
			SetNewState(HttpReaderState.PreHeaderValue);
			name = name.Substring(0, name.Length - 1);
			_lastHeaderName = name;
			_headerSize += name.Length;
			if (_headerSize > MaxAllowedHeadersSize){
				throw new HttpReaderException("insuficient header size");
			}
			return new HttpEntity{Type = HttpEntityType.HeaderName, StringData = name};
		}

		private HttpEntity ReadStateName(){
			string stateString = ReadValue(256, spaceAllowed: true);
			var result = new HttpEntity{Type = HttpEntityType.StateName, StringData = stateString};
			ReadNewLines();
			if (State != HttpReaderState.Content){
				SetNewState(HttpReaderState.PreHeader, true);
			}
			return result;
		}
		byte[] sbuff = new byte[1];
		/// <summary>
		/// </summary>
		private void ReadNewLines(){
			_builder.Clear();
			bool waitn = false;
			if (_cachedLast == '\r'){
				_builder.Append(_cachedLast);
				_cachedLast = '\0';
				waitn = true;
			}
			while (true){
				int _b = -1;
				var l =_stream.Read(sbuff,0,1);
				if (0 != l){
					_b = sbuff[0];
				}
				int _c = (char) _b;
				if (-1 == _b){
					if (_builder.Length == 4){
						_state = HttpReaderState.Finish;
						return;
					}
					throw new HttpReaderException("insuficient eof");
				}
				var c = (char) _c;
				if (c == '\r'){
					if (waitn) throw new HttpReaderException("invalid crlf");
					_builder.Append(c);
					waitn = true;
				}
				else if (c == '\n'){
					if (!waitn) throw new HttpReaderException("invalid crlf");
					_builder.Append(c);
					waitn = false;
				}
				else{
					if (_builder.Length != 2 && _builder.Length != 4){
						throw new HttpReaderException("invalid crlf");
					}
					_cachedByte = (byte)_b;
					_cachedLast = c;
					break;
				}
			}
			if (_builder.Length == 4){
				_state = HttpReaderState.Content;
			}
		}

		private HttpEntity ReadState(){
			string stateString = ReadValue(5);
			int stateInt = stateString.ToInt();
			if (0 == stateInt) throw new HttpReaderException("invalid state number");
			SkipSpaces();
			SetNewState(HttpReaderState.StateName);
			return new HttpEntity{Type = HttpEntityType.State, NumericData = stateInt};
		}

		private HttpEntity ReadVersion(){
			int httpslash = _binaryReader.Read(_charBuffer, 0, 5);
			if (5 != httpslash){
				throw new HttpReaderException("insufficient length of preamble");
			}
			var httpslashstr = new string(_charBuffer, 0, 5);
			if (httpslashstr != "HTTP/"){
				throw new HttpReaderException("invalid HTTP preamble");
			}
			_state = HttpReaderState.Version;
			string version = ReadValue(4);
			SkipSpaces();
			SetNewState(HttpReaderState.State);
			return new HttpEntity{Type = HttpEntityType.Version, StringData = version};
		}

		private void SetNewState(HttpReaderState _newstate, bool _alloweof = false){
			if (State == HttpReaderState.Error) return;
			if (State != HttpReaderState.Finish){
				_state = _newstate;
			}
			else{
				if (!_alloweof){
					throw new HttpReaderException("insuficient eof");
				}
			}
		}

		private void SkipSpaces(){
			if (_cachedLast != '\0'){
				if (_cachedLast != ' ') return;
			}
			while (true){
				int _c = _binaryReader.Read();
				if (-1 == _c){
					_state = HttpReaderState.Finish;
					return;
				}
				if (((char) _c) != ' '){
					_cachedLast = (char) _c;
					break;
				}
			}
		}


		private string ReadValue(int maxlen = 1024, bool spaceAllowed = false, bool eofAllowed = false){
			_builder.Clear();
			int len = 0;
			if (_cachedLast != '\0'){
				if (char.IsWhiteSpace(_cachedLast) && !(spaceAllowed && _cachedLast == ' ')){
					return "";
				}
				_builder.Append(_cachedLast);
				_cachedLast = '\0';
				len++;
			}
			while (true){
				int _charcode = _binaryReader.Read();
				if (-1 == _charcode){
					if (eofAllowed){
						_state = HttpReaderState.Finish;
						break;
					}
					throw new HttpReaderException("insuficient eof");
				}

				var c = (char) _charcode;
				if (char.IsWhiteSpace(c) && !(spaceAllowed && c == ' ')){
					_cachedLast = c;
					break;
				}
				if (len == maxlen){
					throw new HttpReaderException("insuficient len");
				}
				_builder.Append(c);
				len++;
			}
			return _builder.ToString();
		}

		/// <summary>
		///     Прочитать все сущности
		/// </summary>
		/// <returns></returns>
		public IEnumerable<HttpEntity> Read(bool throwError = true){
			HttpEntity e;
			while (null != (e = Next())){
				if (throwError && e.Type == HttpEntityType.Error){
					throw new HttpReaderException(e.Error.ToString());
				}
				yield return e;
			}
		}
	}
}