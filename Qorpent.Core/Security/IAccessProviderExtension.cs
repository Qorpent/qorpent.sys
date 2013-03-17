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
// PROJECT ORIGIN: Qorpent.Core/IAccessProviderExtension.cs
#endregion
namespace Qorpent.Security {
	/// <summary>
	/// 	Интерфейс - оболочка для раширений объхекта определения доступа
	/// </summary>
	public interface IAccessProviderExtension : IAccessProvider {
		/// <summary>
		/// 	True - если данный объект поддерживается расширением для подготовки отклика
		/// </summary>
		/// <param name="obj"> </param>
		/// <returns> </returns>
		bool IsSupported(object obj);
	}
}