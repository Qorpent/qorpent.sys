#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : FileDescriptor.cs
// Project: Qorpent.Mvc
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Специальный вид результата, который позволяет моделировать файлы
	/// </summary>
	public class FileDescriptor : IFileDescriptor {
		/// <summary>
		/// 	Имя ресурса возможно с путем
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	Содержимое файла
		/// </summary>
		public string Content { get; set; }

		/// <summary>
		/// 	Mime-Type файла
		/// </summary>
		public string MimeType { get; set; }

		/// <summary>
		/// 	Длина файла
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// 	Время последнего изменения
		/// </summary>
		public DateTime LastWriteTime { get; set; }

		/// <summary>
		/// 	Роль для ограничения ролевого доступа
		/// </summary>
		public string Role { get; set; }
	}
}