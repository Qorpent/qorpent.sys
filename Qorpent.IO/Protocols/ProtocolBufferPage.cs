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
using System.Text;
using System.Threading;

namespace Qorpent.IO.Protocols{
	/// <summary>
	///     Дескриптор страницы
	/// </summary>
	public class ProtocolBufferPage{
		/// <summary>
		///     Состояние страницы - свободна
		/// </summary>
		public const int Free = 0;

		/// <summary>
		///     Состояние страницы - запись
		/// </summary>
		public const int Write = 1;

		/// <summary>
		///     Состояние страницы - загружена данными
		/// </summary>
		public const int Data = 2;

		/// <summary>
		///     Состояние страницы - чтение
		/// </summary>
		public const int Read = 3;

		/// <summary>
		///     Целевой буфер
		/// </summary>
		public readonly ProtocolBuffer ProtocolBuffer;

		/// <summary>
		///     Смещение в целевом буфере
		/// </summary>
		public readonly int Offset;

		/// <summary>
		///     Размер страницы
		/// </summary>
		public int Size;

		/// <summary>
		///     Состояние страницы
		/// </summary>
		public int State;

		/// <summary>
		///     Создает страницу указанного буфера со смещением
		/// </summary>
		/// <param name="protocolBuffer"></param>
		/// <param name="offset"></param>
		public ProtocolBufferPage(ProtocolBuffer protocolBuffer, int offset){
			ProtocolBuffer = protocolBuffer;
			Offset = offset;
		}

		/// <summary>
		///     Возвращает байт по индексу на странице
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public byte this[int index]{
			get{
				if (index < 0 || index >= Size) throw new IOException("Try read behind page (this[i])");
				return ProtocolBuffer.Buffer[index + Offset];
			}
		}

		/// <summary>
		///     Считывает массив байтов по указанному несмещенному диапазону
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public byte[] this[int start, int length]{
			get{
				if (start < 0 || start < 0 || (start + length) > Size){
					throw new IOException("Try read behind page (this[s,i])");
				}
				var result = new byte[length - start];
				Array.Copy(ProtocolBuffer.Buffer, Offset + start, result, 0, length);
				return result;
			}
		}

		

		/// <summary>
		///     Попытка захвата страницы на запись
		/// </summary>
		/// <returns>true - если страница получила право на запись в текущем потоке</returns>
		public bool GetWriteLock(){
			return Free == Interlocked.CompareExchange(ref State, Write, Free);
		}

		/// <summary>
		///     Операция IndexOf с контроллем смещений
		/// </summary>
		/// <param name="value"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public int IndexOf(byte value, int offset, int count){
			if (offset < 0 || count < 0 || (offset + count) > Size){
				throw new IOException("Try search behind page (indexof(v,o,c)");
			}
			return Array.IndexOf(ProtocolBuffer.Buffer, offset + Offset, count);
		}

		/// <summary>
		///     Считывает ASCII - строку
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public string ReadAscii(int start =0, int length = -1){
			if (length == -1){
				length = Size;
			}
			if (start < 0 || start < 0 || (start + length) > Size){
				throw new IOException("Try read behind page (readascii(s,l))");
			}
			return Encoding.ASCII.GetString(ProtocolBuffer.Buffer, Offset + start, length);
		}
	}
}