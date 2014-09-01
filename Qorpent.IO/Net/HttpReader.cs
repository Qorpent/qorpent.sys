using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net
{
	/// <summary>
	/// Класс для сканирования потока  и разбиения его в соответствии с протоколом HTTP
	/// </summary>
	public class HttpReader
	{
		private Stream _stream;
		private HttpScanerState _state = HttpScanerState.Start;
		private TextReader _asciiReader;
		private char[] _charBuffer = new char[512];
		private StringBuilder _builder = new StringBuilder();
		private char _cachedLast = '\0';
		private bool _finished = false;
		private bool _contentstartsent = false;
		private string _transferEncoding;
		private int _contentLength;
		private string _lastHeaderName;
		private string _contentType;
		private string _charset;
		//private int _lastChunkSize = -1;


		/// <summary>
		/// Создает сканер в виде обертки над потоком
		/// </summary>
		/// <param name="stream"></param>
		public HttpReader(Stream stream){
			_stream = stream;
			_asciiReader = new StreamReader(_stream,Encoding.ASCII);
		}
		/// <summary>
		/// Mime-type
		/// </summary>
		public string ContentType{
			get { return _contentType; }
		}
		/// <summary>
		/// Кодировка
		/// </summary>
		public string Charset{
			get { return _charset; }
		}
		/// <summary>
		/// Длина контента
		/// </summary>
		public int ContentLength{
			get { return _contentLength; }
		}
		/// <summary>
		/// Кодировка при передаче
		/// </summary>
		public string TransferEncoding{
			get { return _transferEncoding; }
		}

		/// <summary>
		/// Считывает следую
		/// </summary>
		/// <returns></returns>
		public HttpEntity Next(){
			try{
				switch (_state){
					case HttpScanerState.Start: return ReadVersion();
					case HttpScanerState.State: return ReadState();
					case HttpScanerState.StateName: return ReadStateName();
					case HttpScanerState.PreHeader: return ReadHeaderName();
					case HttpScanerState.PreHeaderValue: return ReadHeaderValue();
					case HttpScanerState.StartContent: 
						if (_contentstartsent){
							return NextContentPart();
						}
						return new HttpEntity{Type = HttpEntityType.ContentStart};
					case HttpScanerState.Error:
						return null;
					case HttpScanerState.Finish:
						if (_finished){
							return null;
						}
						_finished = true;
						return new HttpEntity{Type = HttpEntityType.Finish};

					default:
						throw new Exception("undefined position");
				}
			}
			catch (Exception ex){
				_state = HttpScanerState.Error;
				return new HttpEntity{Type = HttpEntityType.Error, StringData = ex.ToString()};
			}
		}

		private HttpEntity NextContentPart(){
			throw new NotImplementedException();
		}

		private HttpEntity ReadHeaderValue(){
			var headerValue = ReadValue(1024, spaceAllowed: true);
			var result = new HttpEntity { Type = HttpEntityType.HeaderValue, StringData = headerValue };
			if (_lastHeaderName== "Transfer-Encoding"){
				_transferEncoding = headerValue;
			}else if (_lastHeaderName == "Content-Length"){
				_contentLength = headerValue.ToInt();
				if (0 == ContentLength){
					throw new Exception("invalid content length");
				}
			}else if (_lastHeaderName == "Content-Type"){
				var parts = headerValue.Split(';');
				_contentType = parts[0];
				if (parts.Length >1){
					_charset = parts[1].Split('=')[1];
				}
			}
			ReadNewLines();
			if (_state != HttpScanerState.StartContent)
			{
				SetNewState(HttpScanerState.PreHeader, true);
			}
			return result;
		}

		private HttpEntity ReadHeaderName(){
			var name = ReadValue();
			if (!name.EndsWith(":")){
				throw new Exception("insuficient header name");
			}
			SkipSpaces();
			SetNewState(HttpScanerState.PreHeaderValue);
			name = name.Substring(0, name.Length - 1);
			_lastHeaderName = name;
			return new HttpEntity{Type = HttpEntityType.HeaderName, StringData =name};
		}

		private HttpEntity ReadStateName(){
			var stateString = ReadValue(256,spaceAllowed : true);
			var result = new HttpEntity { Type = HttpEntityType.StateName, StringData = stateString };
			ReadNewLines();
			if (_state != HttpScanerState.StartContent){
				SetNewState(HttpScanerState.PreHeader,true);
			}
			return result;
		}
		/// <summary>
		/// 
		/// </summary>
		private void ReadNewLines(){
			_builder.Clear();
			bool waitn = false;
			if (_cachedLast == '\r'){
				_builder.Append(_cachedLast);
				waitn = true;
			}
			while (true){
				var _c = _asciiReader.Read();
				if (-1 == _c){
					if (_builder.Length == 4 ){
						_state = HttpScanerState.Finish;
						return;
					}
					throw new Exception("insuficient eof");
				}
				var c = (char) _c;
				if (c == '\r'){
					if (waitn) throw new Exception("invalid crlf");
					_builder.Append(c);
					waitn = true;
				}
				else if (c == '\n'){
					if (!waitn) throw new Exception("invalid crlf");
					_builder.Append(c);
					waitn = false;
				}
				else{
					if (_builder.Length != 2 && _builder.Length != 4){
						throw new Exception("invalid crlf");
					}
					_cachedLast = c;
					break;
				}
			}
			if (_builder.Length == 4){
					_state = HttpScanerState.StartContent;
			}
		}

		private HttpEntity ReadState(){
			var stateString = ReadValue(5);
			var stateInt = stateString.ToInt();
			if(0==stateInt)throw new Exception("invalid state number");
			SkipSpaces();
			SetNewState(HttpScanerState.StateName);
			return new HttpEntity{Type = HttpEntityType.State, NumericData = stateInt};
		}

		private HttpEntity ReadVersion(){

			var httpslash = _asciiReader.ReadBlock(_charBuffer, 0, 5);
			if (5 != httpslash){
				throw new Exception("insufficient length of preamble");
			}
			var httpslashstr = new string(_charBuffer, 0, 5);
			if (httpslashstr != "HTTP/"){
				throw new Exception("invalid HTTP preamble");
			}
			_state = HttpScanerState.Version;
			var version = ReadValue(4);
			SkipSpaces();
			SetNewState(HttpScanerState.State);
			return new HttpEntity{Type = HttpEntityType.Version, StringData = version};
		}

		private void SetNewState(HttpScanerState _newstate, bool _alloweof=false){
			if (_state == HttpScanerState.Error) return;
			if (_state != HttpScanerState.Finish ){
				_state = _newstate;
			}
			else{
				if (!_alloweof){
					throw new Exception("insuficient eof");
				}
			}
		}

		private void SkipSpaces(){
			if (_cachedLast != '\0'){
				if (_cachedLast!=' ') return;
			}
			while (true){
				var _c = _asciiReader.Read();
				if (-1 == _c){
					_state= HttpScanerState.Finish;
					return;
				}
				if (((char)_c)!=' '){
					_cachedLast = (char)_c;
					break;
				}
			}
		}

		private string ReadValue(int maxlen = 1024,bool spaceAllowed =false, bool eofAllowed=false){
			_builder.Clear();
			var len = 0;
			if (_cachedLast != '\0'){
				if (char.IsWhiteSpace(_cachedLast) && !(spaceAllowed && _cachedLast == ' ')){
					return "";
				}
				_builder.Append(_cachedLast);
				_cachedLast = '\0';
				len++;
			}
			while (true)
			{
				var _charcode = _asciiReader.Read();
				if (-1 == _charcode){
					if (eofAllowed) break;
					throw new Exception("insuficient eof");
				}
				var c = (char)_charcode;
				if (char.IsWhiteSpace(c) && !(spaceAllowed && c==' ')){
					_cachedLast = c;
					break;
				}
				if (len == maxlen){
					throw new Exception("insuficient len");
				}
				_builder.Append(c);
				len++;
			}
			return _builder.ToString();
		}

		/// <summary>
		/// Прочитать все сущности
		/// </summary>
		/// <returns></returns>
		public IEnumerable<HttpEntity> Read(){
			HttpEntity e;
			while (null!=(e = Next())){
				yield return e;
			}
		} 
	}
}
