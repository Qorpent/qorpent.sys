using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.IO.Net{
	/// <summary>
	/// Ускоренная версия HttpReader, может использоваться многократно
	/// </summary>
	public class HttpResponseReader2{
		private const int DEFAULT_STREAM_BUFFER_SIZE = 1024;
		private const int MIDDLE_SIZE_RATE = 32;
		private const int FIRST_CHUNK_RATE = 8;
		private const int SIZE_OF_BUFFER = DEFAULT_STREAM_BUFFER_SIZE*MIDDLE_SIZE_RATE;
		private const int SIZE_OF_FIRST_CHUNK = DEFAULT_STREAM_BUFFER_SIZE*FIRST_CHUNK_RATE;
		private const int SIZE_OF_SECOND_CHUNK = SIZE_OF_BUFFER - SIZE_OF_FIRST_CHUNK;
		//основной буфер
		byte[] _buffer = new byte[SIZE_OF_BUFFER];
		//размер первого чанка, который считывается синхронно
		private int _firstChunkLength = 0;
		//длина второго чанка, считывается асинхронно
		private int _secondChunkLength = 0;
		//текущая позиция в буфере
		private int _currentPosition = 0;
		private HttpResponse2 _result;
		//фазы обработки 
		//преамбула
		private const int PHASE_PREAMBLE = 0;
		private const int PHASE_HEADERS = 1;
		private const int PHASE_CONTENT = 2;
		private const int CHAR_CR = 13;
		private const int CHAR_WS = 32;
		private const string HEADER_TRANSFER_ENCODING = "Transfer-Encoding";
		private const string CHUNKED_MARK = "chunked";
		private const string HEADER_CONTENT_LENGTH = "Content-Length";
		private const int CHAR_LF = 10;
		private const string HEADER_CONTENT_TYPE = "Content-Type";
		private const string GZIP_MARK = "gzip";
		private const string DEFLATE_MARK = "deflate";
		private byte _phase = PHASE_PREAMBLE;
		//дополнительный буфер для остатков
		private byte[] _restBuffer;
		//следующая граница CR
		private int _nextCRIdx;

		private int _contentLength;
		private bool _chunked;
		private bool _gzip;
		private bool _deflate;
		private int _restCrlf;
		private bool _hasContent;
		//признак окончания чтения
		private bool _finished;
		//private int _chunkSize;
		private MemoryStream _contentStream;
		private Task<int> _secondChunk;


		/// <summary>
		/// Считывает результат из потока
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public async Task<HttpResponse2> Read(Stream stream){
			//Подготавливаем исходное состояние
			_phase = PHASE_PREAMBLE;
			_restBuffer = null;
			_nextCRIdx = -1;
			_contentLength = 0;
			_chunked = false;
			_gzip = false;
			_deflate = false;
			_restCrlf = 0;
			_hasContent = false;
			_finished = false;
			//_chunkSize = 0;
			_secondChunk = null;

			//считывание запроса производится в 2 фазы, одна синхронная и предположительно охватывает чтение ридеров
			//другая же наоборот асинхронная и предположитеьно считывает часть контента или дочитывает поток до конца
			_result = new HttpResponse2();
			_firstChunkLength = await stream.ReadAsync(_buffer, 0, SIZE_OF_FIRST_CHUNK);
			bool _requireSecondChunk = _firstChunkLength == SIZE_OF_FIRST_CHUNK;
			if (_requireSecondChunk){
				_secondChunk = stream.ReadAsync(_buffer, SIZE_OF_FIRST_CHUNK, SIZE_OF_SECOND_CHUNK);
			}
			ProcessFirstChunk();
			if (!_finished && _requireSecondChunk){
				_secondChunkLength = await _secondChunk;
				ProcessRestOfResponse();
			}
			return _result;
		}

		private void ProcessRestOfResponse(){
			throw new System.NotImplementedException();
		}

		private void ProcessFirstChunk(){
			//индекс следующего перевода каретки
			_nextCRIdx = -1;
			while (true){
				var restLength = _firstChunkLength - _currentPosition;
				//находим ближайшую
				_nextCRIdx = Array.IndexOf(_buffer, CHAR_CR, _currentPosition, restLength);
				//отсутствие переноса обозначает разрыв значения на границе, 
				if (-1 == _nextCRIdx){
					//переносим остаток в дополнительный 
					//буфер и выходим из первой фазы
					_restBuffer = new byte[restLength];
					Array.Copy(_buffer,_currentPosition,_restBuffer,0,restLength);
					return;
				}
				//найден участок, выполняем обработку 
				ProcessEntity();
				//смещаемся на начало переноса строк
				_currentPosition = _nextCRIdx;

				//проверяем возможную незавершенность позиции по переносу строки и если обнаруживаем, то запоминаем количество и выходим
				// к таким относятся комбинации CR,EOC; CR,LF,EOC; CR,LF,CR,EOC
				if (IsNonCompleteCRLF() ){
					_restCrlf = _firstChunkLength - _currentPosition;
					return;
				}

				//проверяем, не является ли это двойным CRLF - то есть либо началом контента либо окончанием результата
				if (IsEndOfHeaders()){
					//если нет признаков контента, то значит все, выходим
					if (!_hasContent){
						_finished = true;
					}
					else{
						//иначе надо перейти к чтению контента
						_phase = PHASE_CONTENT;
						//сместить каретку на его начало
						_currentPosition += 4;
						//ну и считать контент и выйти
						ReadContent();
					}
					return;
				}
			}
		}

		private void ReadContent(){
			_contentStream = new MemoryStream();

		}

		private bool IsEndOfHeaders(){
			return _buffer[_currentPosition+1]==CHAR_LF && _buffer[_currentPosition+2]==CHAR_CR && _buffer[_currentPosition+3]==CHAR_LF;
		}

		private bool IsNonCompleteCRLF(){
			return _currentPosition >= _firstChunkLength - 3 && (_buffer[_firstChunkLength-1]==CHAR_CR || _buffer[_firstChunkLength-1]==CHAR_LF);
		}

		private void ProcessEntity(){
			switch (_phase){
				case PHASE_PREAMBLE :
					ProcessPreamble();
					_phase = PHASE_HEADERS;
					break;
				case PHASE_HEADERS :
					ProcessHeader();
					break;
			}
		}

		private void ProcessHeader(){
			var endName = Array.IndexOf(_buffer, 58, _currentPosition);
			var startValue = endName + 2;
			var valueLength = _nextCRIdx - startValue;
			var hasValue = valueLength > 0;
			var nameLength = endName - _currentPosition;
			var headerName = Encoding.ASCII.GetString(_buffer, _currentPosition, nameLength);
			var headerValue = hasValue? Encoding.ASCII.GetString(_buffer, startValue, valueLength):String.Empty;

			if (headerName == HEADER_TRANSFER_ENCODING){
				_chunked = headerValue.Contains(CHUNKED_MARK);
				_hasContent = true;
			}else if (headerName == HEADER_CONTENT_LENGTH){
				_contentLength = Convert.ToInt32(headerValue);
				_hasContent = true;
			} else if (headerName == HEADER_CONTENT_TYPE){
				_gzip = headerValue.Contains(GZIP_MARK);
				_deflate = headerValue.Contains(DEFLATE_MARK);
			}
			_result.AddHeader(headerName, headerValue);
		}

		private void ProcessPreamble(){
			//устанавливаем точку начала статуса
			var startState = Array.IndexOf(_buffer, CHAR_WS, _currentPosition)+1;
			//точка окончания статуса
			var endState = Array.IndexOf(_buffer, CHAR_WS, startState);
			var stateLength = endState - startState;
			//точка начала описания дескриптора
			var startStateName = endState + 1;
			var stateDescLength = _nextCRIdx - startStateName;

			var stateString = Encoding.ASCII.GetString(_buffer, startState, stateLength);
			var state = Convert.ToInt32(stateString);
			var stateName = Encoding.ASCII.GetString(_buffer, startStateName, stateDescLength);

			_result.State = state;
			_result.StateName = stateName;

		}
	}


}