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
// Original file : AccessRole.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Security {
	/// <summary>
	/// 	Варианты прав доступа к объекту
	/// </summary>
	[Flags]
	public enum AccessRole {
		/// <summary>
		/// 	Базовые права - возможность увидеть, получить экземпляр
		/// </summary>
		Access = 1,

		/// <summary>
		/// 	Чтение
		/// </summary>
		Read = 2,

		/// <summary>
		/// 	Запись, изменение
		/// </summary>
		Write = 4,

		/// <summary>
		/// 	Выполнение
		/// </summary>
		Execute = 8,

		/// <summary>
		/// 	Зарезервировано, расширение
		/// </summary>
		Custom1 = 16,

		/// <summary>
		/// 	Зарезервировано, расширение
		/// </summary>
		Custom2 = 32,

		/// <summary>
		/// 	Зарезервировано, расширение
		/// </summary>
		Custom3 = 64,

		/// <summary>
		/// 	Зарезервировано, расширение
		/// </summary>
		Full = 128
	}
}