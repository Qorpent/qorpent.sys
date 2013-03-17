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
// PROJECT ORIGIN: Qorpent.Utils/IMap.cs
#endregion
namespace Qorpent.Utils.Collections{
    /// <summary>
    /// ќписывает двунаправленный граф с возможностью поиска и нахождением множества входных узлов
    /// </summary>
    /// <typeparam name="F"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IMap<F, T>{
        /// <summary>
        /// ¬озвращает массив целей с общим источником
        /// </summary>
        /// <param name="from">источник</param>
        /// <returns>массив целей</returns>
        T[] this[F from] { get; set; }

        /// <summary>
        /// —оздает и добавл€ет новый элемент карты
        /// </summary>
        /// <param name="from">источник</param>
        /// <param name="to">цель</param>
        /// <returns>элемент карты</returns>
        IMapItem<F, T> Add(F from, T to);

        /// <summary>
        /// ¬озвращает массив источников дл€ цели
        /// </summary>
        /// <param name="to">цель</param>
        /// <returns>массив источников</returns>
        F[] Reverse(T to);
    }
}