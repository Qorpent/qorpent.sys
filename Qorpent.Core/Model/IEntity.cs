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
// PROJECT ORIGIN: Qorpent.Core/IEntity.cs
#endregion
namespace Qorpent.Model {
    public interface IEntity : IEntity<long> {
    };
    
	/// <summary>
	/// 	Базовый интерфейс для классов данных
	/// </summary>
	public interface IEntity<TId> : IWithId<TId>, IWithCode, IWithName, IWithIndex, IWithTag, IWithComment, IWithVersion {
        /// <summary>
        /// Общий метод установления некоего контекстного объекта, использование зависит от реализации
        /// </summary>
        /// <param name="context"></param>
	    void SetContext(object context);
        /// <summary>
        /// Возвращает контекстный объект
        /// </summary>
        /// <returns></returns>
	    object GetContext();


		/// <summary>
		/// Ключ сортировки
		/// </summary>
		/// <returns></returns>
		string GetSortKey();
	}

}