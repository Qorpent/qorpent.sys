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
// PROJECT ORIGIN: Qorpent.Core/IContainerBound.cs
#endregion
namespace Qorpent.IoC {
	/// <summary>
	/// 	Interface for component to be called on creation to be bound to container
	/// </summary>
	public interface IContainerBound {
		/// <summary>
		/// 	called on object after creation in IoC with current component context
		/// 	object can perform container bound logic here
		/// </summary>
		/// <param name="container"> </param>
		/// <param name="component"> </param>
		void SetContainerContext(IContainer container, IComponentDefinition component);

		/// <summary>
		/// 	/called then Release is called and container process it
		/// </summary>
		void OnContainerRelease();
		/// <summary>
		/// Событие, вызываемое после выполнения инициализации при помощи контейнера
		/// </summary>
		void OnContainerCreateInstanceFinished();
	}
}