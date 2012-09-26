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
// Original file : IResetable.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Events {
	/// <summary>
	/// 	Интерфейс сервиса, который поддерживает реакцию на системное событие <see cref="ResetEvent" />
	/// </summary>
	public interface IResetable {
		/// <summary>
		/// 	Вызывается при вызове Reset
		/// </summary>
		/// <param name="data"> </param>
		/// <returns> любой объект - будет включен в состав результатов <see cref="ResetEventResult" /> </returns>
		/// <remarks>
		/// 	При использовании стандартной настройки из <see cref="ServiceBase" /> не требует фильтрации опций,
		/// 	настраивается на основе атрибута <see cref="RequireResetAttribute" />
		/// </remarks>
		object Reset(ResetEventData data);

		/// <summary>
		/// 	Возващает объект, описывающий состояние до очистки
		/// </summary>
		/// <returns> </returns>
		object GetPreResetInfo();
	}
}