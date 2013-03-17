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
// PROJECT ORIGIN: Qorpent.Core/IFileDescriptor.cs
#endregion
using System;
using System.IO;
using Qorpent.Model;

namespace Qorpent.Mvc {
	/// <summary>
	/// 	Интерфейс дескриптора абстрактного файла
	/// </summary>
	public interface IFileDescriptor : IWithRole {
		/// <summary>
		/// 	Имя ресурса возможно с путем
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// 	Содержимое файла
		/// </summary>
		string Content { get; set; }

		/// <summary>
		/// 	Mime-Type файла
		/// </summary>
		string MimeType { get; set; }

		/// <summary>
		/// 	Длина файла
		/// </summary>
		int Length { get; set; }

		/// <summary>
		/// 	Время последнего изменения
		/// </summary>
		DateTime LastWriteTime { get; set; }
		/// <summary>
		/// Признак того что надо использовать хидер расположения файла
		/// </summary>
		bool NeedDisposition { get; set; }

		/// <summary>
		/// Признак потокового файла
		/// </summary>
		bool IsStream { get; set; }
		/// <summary>
		/// Возвращает поток данных файла
		/// </summary>
		/// <returns></returns>
		Stream GetStream();
	}
}