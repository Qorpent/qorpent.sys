using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Qorpent.IO.Net{
	internal class HttpResponseReaderExecutor:IDisposable{
		public HttpResponseReaderExecutor(Stream stream, int bufferSize){
			if (bufferSize <= 0){
				bufferSize = 1024;
			}

			Stream = stream;
			Result = new HttpResponse();
			_buffer = new byte[bufferSize];
			_hLineTerminals = new int[100];
		}

		private int _hLineTerminalsIndex = 0;
		public HttpResponse Result;
		Stream Stream;
		MemoryStream HeadersBuffer  = new MemoryStream();
		MemoryStream DataBuffer = new MemoryStream();

		public void Dispose(){
			if (null != Stream){
				Stream.Close();
			}
		}

		public void Read(){
			Result.Success = true;
			ReadHeaders();
			if (HasContent())
			{
				ReadContent();
			}
		
			
		}

		private void ReadContent(){
			if (Result.ContentLength > 0){
				ReadFixed();
			}
			else{
				ReadChunked();
			}
		}

		private int _lastChunkSize = -1;
		private int _lastChunkCount = 0;
		private StringBuilder _lastChunkBuffer = new StringBuilder();
		private void ReadChunked(){
			bool complete = true;
			if (TrailDataCount != 0){
				ReadChunkedBuffer(chunkSize-TrailDataCount,TrailDataCount);
			}
			var trys = 3;
			while (_lastChunkSize!=0){
				chunkSize = ReadBuffer();
				if (0 == chunkSize)
				{
					trys--;
					if (0 == trys){
						complete = false;
						break;
					}
					continue;
				}
				trys = 3;
				ReadChunkedBuffer(0, chunkSize);
			}
			if (complete){
				SetData(false);
			}
			else{
				SetInvalidData();
			}
		}


		private void ReadChunkedBuffer(int offset, int count){
			var newoffset = offset;
			while ( newoffset<offset+count){
				var c = newoffset;
				if (_lastChunkSize == -1){
					for (c = newoffset; c < offset + count; c++){
						newoffset = c + 1;
						var b = _buffer[c];
						if (b == CR){
							continue;
						}
						if (b == LF){
							if (_lastChunkBuffer.Length == 0){
								continue;
							}
							else{
								_lastChunkSize = Convert.ToInt32(_lastChunkBuffer.ToString(), 16);
								_lastChunkBuffer.Clear();
								_lastChunkCount = 0;
								newoffset = c + 1;
								break;
							}
						}
						_lastChunkBuffer.Append((char) b);

					}
				}
				if (_lastChunkSize == 0) break;
				if (_lastChunkSize != -1)
				{
					var len = Math.Min( offset + count - newoffset,_lastChunkSize-_lastChunkCount);
					if (0 != len){
						DataBuffer.Write(_buffer, newoffset, len);
						_lastChunkCount += len;
						newoffset += len;
						if (_lastChunkCount == _lastChunkSize){
							_lastChunkSize = -1;
						}
					}
				}
			}

			
			
		
		}


		private void ReadFixed(){
			if (TrailDataCount != 0){
				DataBuffer.Write(_buffer,chunkSize-TrailDataCount,TrailDataCount);
				_currentDataLength += TrailDataCount;
			}
			var trys = 3;
			while (_currentDataLength < Result.ContentLength){
				chunkSize = ReadBuffer();
				if (0 == chunkSize)
				{
					trys--;
					if (0 == trys)
					{
						break;
					}
					continue;
				}
				trys = 3;
				DataBuffer.Write(_buffer,0,chunkSize);
				_currentDataLength += chunkSize;
			}
			if (_currentDataLength < Result.ContentLength){
				SetInvalidData();
			}
			else{
				SetData(false);
			}
		}

		private void SetInvalidData(){
			Result.Success = false;
			Result.IsRawData = true;
			SetData(true);
		}

		private byte[] decompressBuffer = new byte[1024];
		private void SetData(bool rawOnly){
			if (rawOnly || (!Result.GZip && !Result.Deflate)){
				Result.Data = new byte[DataBuffer.Length];
				var buffer = DataBuffer.GetBuffer();
				Array.Copy(buffer, 0, Result.Data, 0, DataBuffer.Length);
			}
			else{
				DataBuffer.Position = 0;
				using (
					
					var rs = Result.GZip
						         ? (Stream)new GZipStream(DataBuffer, CompressionMode.Decompress)
						         : new DeflateStream(DataBuffer, CompressionMode.Decompress)){
					var len = 0;
					var resultBuffer = new MemoryStream();
					while (0!=(len=rs.Read(decompressBuffer,0,decompressBuffer.Length))){
						resultBuffer.Write(decompressBuffer,0,len);

					}
					Result.Data = new byte[resultBuffer.Length];
					var buffer = resultBuffer.GetBuffer();
					Array.Copy(buffer, 0, Result.Data, 0, resultBuffer.Length);
				}
			}
		}

		private bool HasContent(){
			return Result.HasContent;
		}

		private int LastLFIndex = 0;
		private int CurrenHeadersLength = 0;
		private byte[] _buffer;
		private const byte LF = 10;
		private const byte CR = 13;
		private const byte SP = 32;
		private const byte CL = 58;
		private void ReadHeaders(){
			int nextLfIndex = -1;
			int normalizedLfIndex = -1;
			bool headerTerminalDeteced = false;
			var searchBasis = -1;
			var searchCount = -1;
			chunkSize = 0;
			int trys = 3;
			int addlen = 0;
			while (!headerTerminalDeteced){
				chunkSize = ReadBuffer(); //считать очередную часть буфера
				if (0 == chunkSize){
					trys--;
					if (0 == trys){
						break;
					}
					continue;
				}
				trys = 3;
				var lengthBasis = CurrenHeadersLength;
				while (true){
					searchBasis = nextLfIndex +1;
					searchCount = chunkSize - searchBasis;
					nextLfIndex = Array.IndexOf(_buffer, LF, searchBasis, searchCount);
					if (nextLfIndex < 0){//not found
						HeadersBuffer.Write(_buffer, searchBasis,chunkSize-searchBasis);
						CurrenHeadersLength += chunkSize - searchBasis;
						break;
					}
					normalizedLfIndex = nextLfIndex + lengthBasis;
					if (normalizedLfIndex == LastLFIndex + 2){
						HeaderTerminalIndex = nextLfIndex;
						TrailDataCount = chunkSize - nextLfIndex - 1;
						// it means that it's end of headers
						headerTerminalDeteced = true;
						break;
					}
					addlen = nextLfIndex - searchBasis+1;
					HeadersBuffer.Write(_buffer,searchBasis,addlen);
					CurrenHeadersLength += addlen;
					LastLFIndex = normalizedLfIndex;
					_hLineTerminals[_hLineTerminalsIndex++] = LastLFIndex;
				}
			}
			ParseHeaders();
		}

		private void ParseHeaders(){
			var buffer = HeadersBuffer.GetBuffer();
			var startState = Array.IndexOf(buffer, SP, 0) + 1;
			var endState = Array.IndexOf(buffer, SP, startState);
			var startStateName = endState + 1;
			var endStateName = _hLineTerminals[0] - 1;
			Result.State = Convert.ToInt32(Encoding.ASCII.GetString(buffer, startState, endState - startState));
			Result.StateName = Encoding.ASCII.GetString(buffer, startStateName, endStateName - startStateName);
			for (var i = 0; i < _hLineTerminalsIndex - 1; i++){
				var hstart = _hLineTerminals[i]+1;
				var hend = _hLineTerminals[i + 1]-1;
				var endname = Array.IndexOf(buffer, CL, hstart);
				var startval = endname + 2;
				var hname = Encoding.ASCII.GetString(buffer, hstart, endname - hstart);
				var hvalue = Encoding.ASCII.GetString(buffer, startval, hend - startval);
				Result.AddHeader(hname,hvalue);
			}
		}

		private int HeaderTerminalIndex;
		private int TrailDataCount;
		private int[] _hLineTerminals;
		private int chunkSize;
		private int _currentDataLength;

		private int ReadBuffer(){
			return Stream.Read(_buffer, 0,_buffer.Length);
		}
	}
}