// Copyright 2007-2014  Qorpent Team - http://github.com/Qorpent
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Created : 2014-09-01

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net{
	/// <summary>
	///     Класс для сканирования потока  и разбиения его в соответствии с протоколом HTTP
	/// </summary>
	public class HttpResponseReader : IHttpReader{
		private readonly BinaryReader _binaryReader;
		private readonly StringBuilder _builder = new StringBuilder();
		private readonly char[] _charBuffer = new char[512];
		private readonly Stream _stream;
		private readonly byte[] sbuff = new byte[1];

		/// <summary>
		///     Размер буфера по умолчанию
		/// </summary>
		public int DefaultBufferSize = 1024;

		/// <summary>
		///     Максимальный размер контента
		/// </summary>
		public int MaxAllowedContentSize = 4194304;

		/// <summary>
		///     Максимальная разрешенная длина заголовков
		/// </summary>
		public int MaxAllowedHeadersSize = 16384;

		private byte _cachedByte;

		private char _cachedLast = '\0';
		private int _contentLength;
		private int _contentSize;
		private bool _contentstartsent;
		private int _currentLength;
		private bool _finished;
		private int _headerSize;
		private string _lastHeaderName;
		private HttpReaderState _state = HttpReaderState.Start;
		private string _transferEncoding;


		/// <summary>
		///     Создает сканер в виде обертки над потоком
		/// </summary>
		/// <param name="stream"></param>
		public HttpResponseReader(Stream stream){
			_stream = stream;
			_binaryReader = new BinaryReader(_stream, Encoding.ASCII);
		}




		/// <summary>
		///     Считывает следую
		/// </summary>
		/// <returns></returns>
		public HttpEntity Next(){
			try{
				switch (_state){
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
						return null;
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
				throw new HttpException("No valid content mode - no Content-Length or chunked determined while content exists");
			}
			_contentstartsent = true;
			return new HttpEntity{Type = HttpEntityType.ContentStart};
		}

		private HttpEntity NextChunkedPart(){
			var chunklen = ReadValue(6, eofAllowed: true);
			if (_state == HttpReaderState.Finish){
				if (chunklen == "0"){
					_finished = true;
					return new HttpEntity{Type = HttpEntityType.Finish};
				}
				throw new HttpException("unexpected EOF instead of full chunk");
			}
			var len = Convert.ToInt32(chunklen, 16);
			if (0 == len){
				_state = HttpReaderState.Finish;
				_finished = true;
				return new HttpEntity{Type = HttpEntityType.Finish};
			}
			ReadNewLines();

			_contentSize += len;
			if (_contentSize > MaxAllowedContentSize){
				throw new HttpException("chunked content already has "+_contentSize+" size that greater than max allowed size "+MaxAllowedContentSize);
			}
			var buffer = new byte[len];
			buffer[0] = _cachedByte;
			var total = 1;
			while (total < len){
				var actualLength = _binaryReader.Read(buffer, total, len - total);
				total += actualLength;
			}

			ReadNewLines();
			return new HttpEntity{Type = HttpEntityType.Chunk, BinaryData = buffer, Length = len};
		}

		private HttpEntity NextNonChunkedPart(){
			var rest = Math.Min(_contentLength - _currentLength, DefaultBufferSize);
			var buffer = new byte[rest];
			var len = 0;
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
				throw new HttpException("actual data length "+_currentLength+" is not equal to Content-Length "+_contentLength);
			}
			if (_currentLength == _contentLength){
				_state = HttpReaderState.Finish;
			}
			return new HttpEntity{Type = HttpEntityType.Chunk, BinaryData = buffer, Length = len};
		}

		private HttpEntity ReadHeaderValue(){
			var headerValue = ReadValue(MaxAllowedHeadersSize - _headerSize, spaceAllowed: true);
			_headerSize += headerValue.Length;
			if (_headerSize > MaxAllowedHeadersSize){
				throw new HttpException("header section of response "+_headerSize+" more than max allowed header size "+MaxAllowedHeadersSize);
			}
			ProcessMainHeaders(headerValue);
			ReadNewLines();
			if (_state != HttpReaderState.Content){
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

				if (0 == _contentLength){
					throw new HttpException("invalid Content-Length header "+headerValue);
				}

				if (_contentLength > MaxAllowedContentSize){
					throw new HttpException("Content-Lenght "+_contentLength+" exceed max value "+MaxAllowedHeadersSize);
				}
			}
		}


		private HttpEntity ReadHeaderName(){
			var name = ReadValue(MaxAllowedHeadersSize - _headerSize);
			if (!name.EndsWith(":")){
				throw new HttpException("Invalid header name "+name+" must end with symbol \":\"");
			}
			SkipSpaces();
			SetNewState(HttpReaderState.PreHeaderValue);
			name = name.Substring(0, name.Length - 1);
			_lastHeaderName = name;
			_headerSize += name.Length;
			if (_headerSize > MaxAllowedHeadersSize){
				throw new HttpException("header section of response " + _headerSize + " more than max allowed header size " + MaxAllowedHeadersSize);
			}
			return new HttpEntity{Type = HttpEntityType.HeaderName, StringData = name};
		}

		private HttpEntity ReadStateName(){
			var stateString = ReadValue(256, spaceAllowed: true);
			var result = new HttpEntity{Type = HttpEntityType.StateName, StringData = stateString};
			ReadNewLines();
			if (_state != HttpReaderState.Content){
				SetNewState(HttpReaderState.PreHeader, true);
			}
			return result;
		}

		/// <summary>
		/// </summary>
		private void ReadNewLines(){
			_builder.Clear();
			var waitn = false;
			if (_cachedLast == '\r'){
				_builder.Append(_cachedLast);
				_cachedLast = '\0';
				waitn = true;
			}
			while (true){
				var _b = -1;
				var l = _stream.Read(sbuff, 0, 1);
				if (0 != l){
					_b = sbuff[0];
				}
				int _c = (char) _b;
				if (-1 == _b){
					if (_builder.Length == 4){
						_state = HttpReaderState.Finish;
						return;
					}
					throw new HttpException("unexpected EOF without \\r\\n\\r\\n combination");
				}
				var c = (char) _c;
				if (c == '\r'){
					if (waitn) throw new HttpException("invalid crlf -  \\r after \\r");
					_builder.Append(c);
					waitn = true;
				}
				else if (c == '\n'){
					if (!waitn) throw new HttpException("invalid crlf - \\n on not valid place");
					_builder.Append(c);
					waitn = false;
				}
				else{
					if (_builder.Length != 2 && _builder.Length != 4){
						throw new HttpException("invalid crlf - invalid length "+_builder.Length);
					}
					_cachedByte = (byte) _b;
					_cachedLast = c;
					break;
				}
			}
			if (_builder.Length == 4){
				_state = HttpReaderState.Content;
			}
		}

		private HttpEntity ReadState(){
			var stateString = ReadValue(5);
			var stateInt = stateString.ToInt();
			if (0 == stateInt) throw new HttpException("invalid state number - 0");
			SkipSpaces();
			SetNewState(HttpReaderState.StateName);
			return new HttpEntity{Type = HttpEntityType.State, NumericData = stateInt};
		}

		private HttpEntity ReadVersion(){
			var httpslash = _binaryReader.Read(_charBuffer, 0, 5);
			if (5 != httpslash){
				throw new HttpException("cannot read preamble - just "+httpslash+" symbols read");
			}
			var httpslashstr = new string(_charBuffer, 0, 5);
			if (httpslashstr != "HTTP/"){
				throw new HttpException("invalid HTTP preamble "+httpslashstr);
			}
			_state = HttpReaderState.Version;
			var version = ReadValue(4);
			SkipSpaces();
			SetNewState(HttpReaderState.State);
			return new HttpEntity{Type = HttpEntityType.Version, StringData = version};
		}

		private void SetNewState(HttpReaderState _newstate, bool _alloweof = false){
			if (_state == HttpReaderState.Error) return;
			if (_state != HttpReaderState.Finish){
				_state = _newstate;
			}
			else{
				if (!_alloweof){
					throw new HttpException("unexpected EOF on try set state "+_newstate);
				}
			}
		}

		private void SkipSpaces(){
			if (_cachedLast != '\0'){
				if (_cachedLast != ' ') return;
			}
			while (true){
				var _c = _binaryReader.Read();
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
			var len = 0;
			if (_cachedLast != '\0'){
				if (char.IsWhiteSpace(_cachedLast) && !(spaceAllowed && _cachedLast == ' ')){
					return "";
				}
				_builder.Append(_cachedLast);
				_cachedLast = '\0';
				len++;
			}
			while (true){
				var _charcode = _binaryReader.Read();
				if (-1 == _charcode){
					if (eofAllowed){
						_state = HttpReaderState.Finish;
						break;
					}
					throw new HttpException("unexpected EOF during reading value");
				}

				var c = (char) _charcode;
				if (char.IsWhiteSpace(c) && !(spaceAllowed && c == ' ')){
					_cachedLast = c;
					break;
				}
				if (len == maxlen){
					throw new HttpException("insuficient length of value - greater than allowed max "+maxlen);
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
					throw new HttpException(e.Error.ToString());
				}
				yield return e;
			}
		}
	}
}