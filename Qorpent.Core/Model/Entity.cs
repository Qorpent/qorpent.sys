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
// Original file : Entity.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Model {
	/// <summary>
	/// 	Плоская реализация IEntity, может использоваться как универсальный промежуточный класс
	/// </summary>
	public class Entity : IEntity {
		/// <summary>
		/// 	Целочисленный уникальный идентификатор
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 	Строковый уникальный идентификатор
		/// </summary>
		public string Code { get; set; }

		/// <summary>
		/// 	Название/имя
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 	An index of object
		/// </summary>
		public int Idx { get; set; }

		/// <summary>
		/// 	Строка тегов
		/// </summary>
		public string Tags { get; set; }

		/// <summary>
		/// 	Комментарий
		/// </summary>
		public string Comment { get; set; }

		/// <summary>
		/// 	Название
		/// </summary>
		public DateTime Version { get; set; }
	}
}