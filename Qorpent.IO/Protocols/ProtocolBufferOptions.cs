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

namespace Qorpent.IO.Protocols{
	/// <summary>
	///     ��������� ��������� ������
	/// </summary>
	public class ProtocolBufferOptions{
		private readonly int _initialPageCount;
		private readonly int _maxPageCount;
		private readonly int _maxTryWriteCount;
		private readonly int _pageSize;
		private readonly int _streamReadTimeout;
		private readonly int _streamWaitResponseTimeout;
		private readonly int _waitPageTimeout;

		/// <summary>��������� ������ ���������</summary>
		/// <param name="pageSize">������ ��������</param>
		/// <param name="initialPageCount">��������� ����� �������</param>
		/// <param name="maxPageCount">������������ ����� �������</param>
		/// <param name="waitPageTimeout">����� �������� ����� ��������� ������ ������� �� ������ �������</param>
		/// <param name="maxTryWriteCount">���������� ������� ������ ��������� ������� ��� ������</param>
		/// <param name="streamWaitResponseTimeout">����� �������� ������ ������</param>
		/// <param name="streamReadTimeout">����� �������� ������</param>
		public ProtocolBufferOptions(int pageSize = 1024, int initialPageCount = 32, int maxPageCount = 64,
		                             int waitPageTimeout = 20, int maxTryWriteCount = 3, int streamWaitResponseTimeout = 20,
		                             int streamReadTimeout = 50){
			initialPageCount = Math.Max(initialPageCount, 1);
			waitPageTimeout = Math.Max(waitPageTimeout, 0);
			maxTryWriteCount = Math.Max(maxTryWriteCount, 1);
			pageSize = Math.Max(pageSize, 1);
			maxPageCount = Math.Max(initialPageCount, maxPageCount);

			_pageSize = pageSize;
			_initialPageCount = initialPageCount;
			_maxPageCount = maxPageCount;
			_waitPageTimeout = waitPageTimeout;
			_maxTryWriteCount = maxTryWriteCount;
			_streamWaitResponseTimeout = streamWaitResponseTimeout;
			_streamReadTimeout = streamReadTimeout;
		}

		/// <summary>
		///     ������ ��������
		/// </summary>
		public int PageSize{
			get { return _pageSize; }
		}

		/// <summary>
		///     ��������� ����� �������
		/// </summary>
		public int InitialPageCount{
			get { return _initialPageCount; }
		}

		/// <summary>
		///     ������������ ����� �������
		/// </summary>
		public int MaxPageCount{
			get { return _maxPageCount; }
		}

		/// <summary>
		///     ����� ��������� ����� ������ ��������� �������
		/// </summary>
		public int WaitPageTimeout{
			get { return _waitPageTimeout; }
		}

		/// <summary>
		///     ����� ������� ����� ������ ��������� �������
		/// </summary>
		public int MaxTryWriteCount{
			get { return _maxTryWriteCount; }
		}

		/// <summary>
		///     ����� �������� ����������� ������
		/// </summary>
		public int StreamWaitResponseTimeout{
			get { return _streamWaitResponseTimeout; }
		}

		/// <summary>
		///     ����� �������� ������ ������
		/// </summary>
		public int StreamReadTimeout{
			get { return _streamReadTimeout; }
		}
	}
}