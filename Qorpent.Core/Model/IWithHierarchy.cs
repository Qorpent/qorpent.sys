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
// PROJECT ORIGIN: Qorpent.Core/IWithHierarchy.cs
#endregion

using System;
using System.Collections.Generic;

namespace Qorpent.Model {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IWithSimpleHierarchy<TEntity> {
		/// <summary>
		/// 	Ссылка на родительский объект
		/// </summary>
		TEntity Parent { get; set; }

		/// <summary>
		/// 	Коллекция дочерних объектов
		/// </summary>
		ICollection<TEntity> Children { get; set; }
	}
    	/// <summary>
	/// 	Модельный интерфейс - Нечто с иерархией, понимаемой как родитель, путь, дочерние
	/// </summary>
	public interface IWithHierarchy<TEntity> : IWithSimpleHierarchy<TEntity> where TEntity: IWithCode {
		/// <summary>
		/// 	Прямой идентификатор родителя
		/// </summary>
		long? ParentId { get; set; }
		/// <summary>
		///		CODE of parent
		/// </summary>
		string ParentCode { get; set; }

		/// <summary>
		/// 	Cached path of entity
		/// </summary>
		string Path { get; set; }

		/// <summary>
		///		Checks if children existed
		/// </summary>
		/// <returns></returns>
		bool HasChildren();

		/// <summary>
		/// Check that parent is not null
		/// </summary>
		/// <returns></returns>
		bool HasParent();

		/// <summary>
		///  Check that Parent is somehow defined (with Id or Code may be)
		/// </summary>
		/// <returns></returns>
		bool IsParentDefined();

		/// <summary>
		/// Get shallow hierarchy copy ,started with current node as root
		/// </summary>	
		/// <returns></returns>
		/// <remarks>upper nodes are not cloned</remarks>
		IWithHierarchy<TEntity> GetCopyOfHierarchy();

		/// <summary>
		/// Marks that current node doesn't allow further up propagation
		/// </summary>
		bool IsPropagationRoot { get; set; }
	}
}