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
// PROJECT ORIGIN: Qorpent.Core/IComponentSource.cs
#endregion
using System;
using System.Collections.Generic;

namespace Qorpent.IoC {
	/// <summary>
	/// Интерфейс источника компонентов
	/// </summary>
	public interface IComponentSource {
		/// <summary>
		/// 	Получить все зарегистрированные компоненты
		/// </summary>
		/// <returns> Все компоненты контейнера </returns>
		IEnumerable<IComponentDefinition> GetComponents();

		/// <summary>
		/// 	Find best matched component for type/name or null for
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="name"> The name. </param>
		/// <returns> </returns>
		/// <exception cref="NotImplementedException"></exception>
		/// <remarks>
		/// </remarks>
		IComponentDefinition FindComponent(Type type, string name);
	}
}