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
// Original file : IExceptionRegistry.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;

namespace Qorpent.Log {
	/// <summary>
	/// 	Интерфейс регистратора ошибок
	/// </summary>
	public interface IExceptionRegistry {
		/// <summary>
		/// </summary>
		/// <param name="error"> Ошибка </param>
		/// <param name="severity"> Уровень ошибки </param>
		/// <param name="title"> Заголовок ошибки </param>
		/// <param name="advancedParameters"> Дополнительные параметры </param>
		/// <returns> Уникальный ключ сообщения </returns>
		string Send(Exception error, ErrorLevel severity, object advancedParameters, string title = "");
	}
}