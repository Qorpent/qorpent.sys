#region LICENSE
// Copyright 2007-2013 Qorpent Team - http://github.com/Qorpent
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
// PROJECT ORIGIN: Qorpent.Mvc/FileDescriptor.cs
#endregion
using System;
using System.IO;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	����������� ��� ����������, ������� ��������� ������������ �����
	/// </summary>
	public class FileDescriptor : IFileDescriptor {
		/// <summary>
		/// 	��� ������� �������� � �����
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	���������� �����
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// ������ �������� ������
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		/// 	Mime-Type �����
		/// </summary>
		public string MimeType { get; set; }

		/// <summary>
		/// 	����� �����
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// 	����� ���������� ���������
		/// </summary>
		public DateTime LastWriteTime { get; set; }

		/// <summary>
		/// ������� ���� ��� ���� ������������ ����� ������������ �����
		/// </summary>
		public bool NeedDisposition { get; set; }

		/// <summary>
		/// ������� ���������� �����
		/// </summary>
		public bool IsStream {
			get { return false; }
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		/// ���������� ����� ������ �����
		/// </summary>
		/// <returns></returns>
		public Stream GetStream() {
			throw new NotImplementedException();
		}

		/// <summary>
		/// 	���� ��� ����������� �������� �������
		/// </summary>
		public string Role { get; set; }
	}
}