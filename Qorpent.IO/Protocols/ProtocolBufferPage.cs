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
	///     ���������� ��������
	/// </summary>
	public class ProtocolBufferPage{
		/// <summary>
		///     ��������� �������� - ��������
		/// </summary>
		public const int Free = 0;

		/// <summary>
		///     ��������� �������� - ������
		/// </summary>
		public const int Write = 1;

		/// <summary>
		///     ��������� �������� - ��������� �������
		/// </summary>
		public const int Data = 2;

		/// <summary>
		///     ��������� �������� - ������
		/// </summary>
		public const int Read = 3;

		/// <summary>
		///     ������� �����
		/// </summary>
		public readonly BufferManager BufferManager;

		/// <summary>
		///     �������� � ������� ������
		/// </summary>
		public readonly int Offset;

		/// <summary>
		///     ������ ��������
		/// </summary>
		public int Size;

		/// <summary>
		///     ��������� ��������
		/// </summary>
		public int State;

		/// <summary>
		///     ������� �������� ���������� ������ �� ���������
		/// </summary>
		/// <param name="_bufferManager"></param>
		/// <param name="offset"></param>
		public ProtocolBufferPage(BufferManager _bufferManager, int offset){
			BufferManager = _bufferManager;
			Offset = offset;
		}

		/// <summary>
		///     ���������� ���� �� ������� �� ��������
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public byte this[int index]{
			get{
				if (index < 0 || index >= Size) throw new IOException("Try read behind page (this[i])");
				return BufferManager.Buffer[index + Offset];
			}
		}

		/// <summary>
		///     ��������� ������ ������ �� ���������� ������������ ���������
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
				Array.Copy(BufferManager.Buffer, Offset + start, result, 0, length);
				return result;
			}
		}

		

		/// <summary>
		///     ������� ������� �������� �� ������
		/// </summary>
		/// <returns>true - ���� �������� �������� ����� �� ������ � ������� ������</returns>
		public bool GetWriteLock(){
			return Free == Interlocked.CompareExchange(ref State, Write, Free);
		}

		/// <summary>
		///     �������� IndexOf � ���������� ��������
		/// </summary>
		/// <param name="value"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public int IndexOf(byte value, int offset, int count =-1){
			if (count == -1){
				count = Size - offset;
			}
			if (offset < 0 || count < 0 || (offset + count) > Size){
				throw new IOException("Try search behind page (indexof(v,o,c)");
			}
			var result = Array.IndexOf(BufferManager.Buffer, value, offset + Offset, count) - Offset;
			if (result < -1) result = -1;
			return result;
		}

		/// <summary>
		///     ��������� ASCII - ������
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
			return Encoding.ASCII.GetString(BufferManager.Buffer, Offset + start, length);
		}
	}
}