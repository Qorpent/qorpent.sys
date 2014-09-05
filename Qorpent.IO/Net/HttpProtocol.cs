using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qorpent.IO.Protocols;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Net
{
	/// <summary>
	/// Имплементация Http протокола для ProtocolBuffer
	/// </summary>
	public class HttpProtocol : Protocol{
		/// <summary>
		/// Результат парсинга HTTP
		/// </summary>
		public HttpResponse2 Response { get; private set; }
		/// <summary>
		/// Признак того, что заголовки прочитаны
		/// </summary>
		public bool HeadersRead;
		/// <summary>
		/// Признак того, что требуется закачка контента
		/// </summary>
		public bool ContentRequired;
		/// <summary>
		/// Признак того, что контент полностью прочитан
		/// </summary>
		public bool ContentTotallyRead;
		/// <summary>
		/// Признак сжатия GZip
		/// </summary>
		public bool GZip;
		/// <summary>
		/// Признак сжатия Deflate
		/// </summary>
		public bool Deflate;
		/// <summary>
		/// Поток записи контента респонза
		/// </summary>
		private MemoryStream RawDataStream;
		private StringBuilder HeaderStream;

		private int _orderedCrlfCount;

		private HttpReaderState State =HttpReaderState.Start;
		private int _contentReadSize;

		
		/// <summary>
		///     Вызывается для очередной страницы загруженных данных, должен быть перекрыт в реальном протоколе
		/// </summary>
		/// <param name="page"></param>
		protected override void ProcessPage(ProtocolBufferPage page){
			if (State == HttpReaderState.Content){
				ReadContent(page,0);
			}
			else{
				ReadHeaderPart(page);
			}
		}

		private void ReadHeaderPart(ProtocolBufferPage page){
			var idx = 0;
			while (true){
				if (idx > page.Size - 1) break;
				var istart = idx;
				for (var i = istart; i <page.Size; i++){
					idx ++;
					var b = page.BufferManager.Buffer[i + page.Offset];
					HeaderStream.Append((char)b);
					if (b == CHAR_CR || b == CHAR_LF){
						_orderedCrlfCount += 1;
					}
					else{
						_orderedCrlfCount = 0;
						break;
					}
					if (4 == _orderedCrlfCount)
					{
						ReadHeaders();
						if (ContentRequired)
						{
							State = HttpReaderState.Content;
							if (i != page.Size - 1)
							{
								ReadContent(page, i + 1);
							}
						}
						return;
					}
				}
				

				var cridx = page.IndexOf(CHAR_CR, idx);
				if (-1 == cridx){
					HeaderStream.Append(page.ReadAscii(idx, page.Size - idx));
					return;
				}
				else{
					
					HeaderStream.Append(page.ReadAscii(idx, cridx - idx));
					idx = cridx;
				}
				for (var i = cridx; i < page.Size; i++){
					idx++;
					var b = page.BufferManager.Buffer[i + page.Offset];
					HeaderStream.Append((char) b);
					if (b == CHAR_CR || b == CHAR_LF){
						_orderedCrlfCount += 1;
					}
					else{
						_orderedCrlfCount = 0;
						break;
					}
					if (4 == _orderedCrlfCount){
						ReadHeaders();
						if (ContentRequired){
							State = HttpReaderState.Content;
							if (i != page.Size - 1){
								ReadContent(page, i + 1);
							}
						}
						return;
					}
				}
			}
		}

		private void ReadHeaders(){
			HeadersRead = true;
			var lines = HeaderStream.ToString().SmartSplit(false, true, '\r', '\n');
			foreach (var line in lines){
				ProcessLine(line);
			}
		}

		private void ReadContent(ProtocolBufferPage page, int initialOffset){
			if (Chunked){
				ReadChunked(page, initialOffset);
			}
			else{
				ReadFixed(page, initialOffset);
			}
		}

		private void ReadFixed(ProtocolBufferPage page,int initOffset){
			if (_contentReadSize >= ContentLength){
				Success = true;
				ContentTotallyRead = true;
			}
			if (_contentReadSize < ContentLength){
				var len = Math.Min(page.Size - initOffset, ContentLength - _contentReadSize);
				_contentReadSize += len;
				RawDataStream.Write(page.BufferManager.Buffer, initOffset+page.Offset, len);
			}
			if (_contentReadSize >= ContentLength){
				Success = true;
				ContentTotallyRead = true;
			}
		}

		private StringBuilder ChunkHeaderBuffer;

		private int CurrentChunkSize;
		private int CurrentChunkRead;

		private void ReadChunked(ProtocolBufferPage page, int initOffset)
		{
			for (var i = initOffset; i < page.Size; i++){
				if (CurrentChunkRead < CurrentChunkSize){
					var len = Math.Min(CurrentChunkSize - CurrentChunkRead, page.Size - i);
					RawDataStream.Write(page.BufferManager.Buffer,i+page.Offset,len);
					CurrentChunkRead += len;
					i += len - 1;
					if (CurrentChunkSize == CurrentChunkRead){
						CurrentChunkSize = 0;
						CurrentChunkRead = 0;
						ChunkHeaderBuffer.Clear();
					}
					continue;
				}

				var b = page.BufferManager.Buffer[i + page.Offset];
				if (b == CHAR_CR) continue;
				if (b == CHAR_LF){
					if(ChunkHeaderBuffer.Length==0)continue;
					CurrentChunkSize = Convert.ToInt32(ChunkHeaderBuffer.ToString(), 16);
					if (0 == CurrentChunkSize){
						Success = true;
						ContentTotallyRead = true;
						return;
					}
					continue;
				}
				ChunkHeaderBuffer.Append((char) b);

			}
		}

		

		

		private void ProcessLine(string line){
			if (State == HttpReaderState.Start){
				ProcessPreamble(line);
				State = HttpReaderState.Headers;
			}
			else{
				ProcessHeader(line);
			}
		}
		private const string HEADER_TRANSFER_ENCODING = "Transfer-Encoding";
		private const string CHUNKED_MARK = "chunked";
		private const string HEADER_CONTENT_LENGTH = "Content-Length";
		private const string HEADER_CONTENT_TYPE = "Content-Type";
		private const string GZIP_MARK = "gzip";
		private const string DEFLATE_MARK = "deflate";

		private void ProcessHeader(string line)
		{
			var endName = line.IndexOf(':');
			var startValue = endName + 2;
			var valueLength = line.Length - startValue;
			var hasValue = valueLength > 0;
			var nameLength = endName;
			var headerName = line.Substring(0, nameLength);
			var headerValue = hasValue ? line.Substring(startValue,valueLength) : String.Empty;

			if (headerName == HEADER_TRANSFER_ENCODING)
			{
				Chunked = headerValue.Contains(CHUNKED_MARK);
				ContentRequired = true;
			}
			else if (headerName == HEADER_CONTENT_LENGTH)
			{
				ContentLength = Convert.ToInt32(headerValue);
				ContentRequired = true;
			}
			else if (headerName == HEADER_CONTENT_TYPE)
			{
				GZip = headerValue.Contains(GZIP_MARK);
				Deflate = headerValue.Contains(DEFLATE_MARK);
			}
			Response.AddHeader(headerName, headerValue);
		}

		private void ProcessPreamble(string line)
		{
			//устанавливаем точку начала статуса
			var startState = line.IndexOf(' ', 0) + 1;
			//точка окончания статуса
			var endState = line.IndexOf(' ', startState);
			var stateLength = endState - startState;
			//точка начала описания дескриптора
			var startStateName = endState + 1;

			var stateString = line.Substring(startState, stateLength);
			var state = Convert.ToInt32(stateString);
			var stateName = line.Substring(startStateName);

			Response.State = state;
			Response.StateName = stateName;

		}

		private const byte CHAR_CR = 13;
		private const byte CHAR_LF = 10;

		/// <summary>
		/// Признак чанкового отлика
		/// </summary>
		public bool Chunked	;
		/// <summary>
		/// Признак отклика фиксированного размера
		/// </summary>
		public int ContentLength;

		

		/// <summary>
		/// Команда начала чтения протокола
		/// </summary>
		public override void Start()
		{
			base.Start();
			State = HttpReaderState.Start;
			Response = new HttpResponse2();
			RawDataStream =new MemoryStream();
			HeaderStream =new StringBuilder();
			ChunkHeaderBuffer = new StringBuilder();
			CurrentChunkRead = 0;
			CurrentChunkSize = 0;

		}
		/// <summary>
		/// 
		/// </summary>
		public override void Finish()
		{
			base.Finish();
			if (!HeadersRead){
				Error = Response.Error = new IOException("http: headers still not read on end");
			}
			else if (ContentRequired && !ContentTotallyRead){
				Error = Response.Error = new IOException("http: content not load on the end");
				SetRawData(true);
			}else if (ContentRequired){
				SetData();
			}
		}

		private void SetData(){
			if (!GZip && !Deflate){
				SetRawData(false);
				return;
			}
			var len = RawDataStream.Position;
			RawDataStream.Position = 0;
			var buffer = new byte[len];
			RawDataStream.Read(buffer, 0, buffer.Length);
			var resultStream = new MemoryStream();
			using (var zippedStream = GZip ? (Stream)new GZipStream(new MemoryStream(buffer), CompressionMode.Decompress) : new DeflateStream(new MemoryStream(buffer), CompressionMode.Decompress))
			{
				zippedStream.CopyTo(resultStream);
			}
			Response.Data = resultStream.GetBuffer();
		}

		private void SetRawData(bool markAsRaw = false){
			var len = RawDataStream.Position;
			RawDataStream.Position = 0;
			if (markAsRaw){
				Response.IsRawData = true;
			}
			var buffer = new byte[len];
			RawDataStream.Read(buffer, 0, buffer.Length);
			Response.Data = buffer;
		}
	}
}
