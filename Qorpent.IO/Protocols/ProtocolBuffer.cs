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
// Created : 2014-09-04

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.Log;

namespace Qorpent.IO.Protocols{
	/// <summary>
	///     Обобщенный асинхронный буфер для отправки транспорта с входящего потока в протокол
	/// </summary>
	public class ProtocolBuffer{
		/// <summary>
		///     Страницы буфера
		/// </summary>
		public readonly ProtocolBufferPage[] Pages;

		/// <summary>
		/// Журнал
		/// </summary>
		public IUserLog Log;

		private readonly ProtocolBufferOptions _options;
		private byte[] _buffer;

		/// <summary>
		///     Создает буффер с указанным размером страницы и числом страниц
		/// </summary>
		public ProtocolBuffer(ProtocolBufferOptions protocolBufferOptions = null){
			_options = protocolBufferOptions ?? new ProtocolBufferOptions();
			Buffer = new byte[_options.PageSize*_options.InitialPageCount];
			Pages = new ProtocolBufferPage[_options.MaxPageCount];
			for (var i = 0; i < _options.InitialPageCount; i++){
				AddPage();
			}
		}

		/// <summary>
		///     Настройки буффера
		/// </summary>
		public ProtocolBufferOptions Options{
			get { return _options; }
		}

		/// <summary>
		///     Буфер
		/// </summary>
		public byte[] Buffer{
			get { return _buffer; }
			private set { _buffer = value; }
		}


		/// <summary>
		///     Число страниц
		/// </summary>
		public int PageCount { get; protected set; }

		private ProtocolBufferPage AddPage(){
			lock (this){
				var offset = _options.PageSize*PageCount;
				if (offset + _options.PageSize > _buffer.Length){
					Array.Resize(ref _buffer, _buffer.Length + _options.PageSize);
				}
				var page = new ProtocolBufferPage(this, offset);
				Pages[PageCount] = page;
				PageCount++;
				return page;
			}
		}

		/// <summary>
		///     Возвращает любую свободную страницу для записи
		/// </summary>
		/// <returns></returns>
		private ProtocolBufferPage GetFreePage(){
			ProtocolBufferPage result = null;
			for (var trys = 0; trys < _options.MaxTryWriteCount; trys++){
				ProtocolBufferPage candidate;
				for (var i = 0; i < PageCount; i++){
					candidate = Pages[i];
					if (candidate.GetWriteLock()){
						result = candidate;
						break;
					}
				}
				if (null == result && PageCount < _options.MaxPageCount){
					candidate = AddPage();
					if (candidate.GetWriteLock()){
						result = candidate;
						break;
					}
				}
				if (null != result) break;
				if (_options.WaitPageTimeout > 0){
					Thread.Sleep(_options.WaitPageTimeout);
				}
			}
			if (null == result){
				throw new IOException("cannot get required free page");
			}
			return result;
		}
		/// <summary>
		/// Синхронный вариант сигнатуры чтения
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="protocol"></param>
		/// <param name="closeStream"></param>
		/// <returns></returns>
		public ProtocolBufferResult Read(Stream stream, Protocol protocol, bool closeStream = true){
			var task = ReadAsync(stream, protocol, closeStream);
			task.Wait();
			return task.Result;
		}


		/// <summary>
		///     Выполняет асинхронное чтение потока через протокол
		/// </summary>
		/// <param name="stream">Поток, из которого осуществляется чтение</param>
		/// <param name="protocol">Целевой протокол</param>
		/// <param name="closeStream"></param>
		/// <returns></returns>
		public async Task<ProtocolBufferResult> ReadAsync(Stream stream, Protocol protocol, bool closeStream = true){
			var result = new ProtocolBufferResult{Ok = true};
			Exception exception = null;
			var existedReadTimeout = 0;
			try{
				if (stream is NetworkStream){
					existedReadTimeout = stream.ReadTimeout;
					stream.ReadTimeout = _options.StreamReadTimeout;
				}
				protocol.Start();
				while (protocol.IsAlive && IsAlive(stream)){
					if (null != Log) Log.Error("ProtocolBuffer : start step");
					ProtocolBufferPage page = GetFreePage();
					if (null != Log) Log.Error("ProtocolBuffer : page accessed at offset "+page.Offset);
					page.State = ProtocolBufferPage.Write;
					page.Size = await stream.ReadAsync(Buffer, page.Offset, _options.PageSize);
					if (null != Log) Log.Error("ProtocolBuffer : read end for offset " + page.Offset + " with size " + page.Size);
					if (0 == page.Size) continue;
					page.State = ProtocolBufferPage.Data;
					protocol.Process(page);
				}
				protocol.Join();
				if (null != Log) Log.Error("ProtocolBuffer : protocol joined");
				protocol.Finish();
				if (null != Log) Log.Error("ProtocolBuffer : protocol finished");
				result.Success = protocol.Success;
				if (protocol.Error != null){
					result.Success = false;
					result.Error = protocol.Error;
				}
			}
			catch (Exception ex){
				if (null != Log) Log.Error("ProtocolBuffer : error detected - "+ex.Message);
				exception = ex;
			}
			finally{
				if (stream is NetworkStream){
					stream.ReadTimeout = existedReadTimeout;	
				}
				if (closeStream){
					stream.Close();
				}
			}
			if (null != exception){
				result.Ok = false;
				result.Success = false;
				result.Error = exception;
			}
			return result;
		}

		

		/// <summary>
		///     Проверка потока на возможность чтения
		/// </summary>
		/// <param name="stream"></param>
		private bool IsAlive(Stream stream){
			if (stream is MemoryStream || stream is FileStream){
				return stream.Position < stream.Length;
			}
			if (stream is NetworkStream){
				var ns = stream as NetworkStream;
				if (ns.DataAvailable) return true;
				if (_options.StreamWaitResponseTimeout > 0){
					Thread.Sleep(_options.StreamWaitResponseTimeout);
					if (ns.DataAvailable) return true;
				}
				return false;
			}
			return stream.CanRead;
		}
	}
}