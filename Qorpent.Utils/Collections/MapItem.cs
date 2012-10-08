// Copyright 2007-2010 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
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
// MODIFICATIONS HAVE BEEN MADE TO THIS FILE
namespace Qorpent.Utils.Collections{
    /// <summary>
    /// Элемент карты
    /// </summary>
    /// <typeparam name="F">тип источника</typeparam>
    /// <typeparam name="T">тип цели</typeparam>
    public class MapItem<F, T> : IMapItem<F, T>{
        #region IMapItem<F,T> Members

        /// <summary>
        /// Источник
        /// </summary>
        public F From { get; set; }

        /// <summary>
        /// Цель
        /// </summary>
        public T To { get; set; }

        /// <summary>
        /// Дополнительный тэг
        /// </summary>
        public object Tag { get; set; }

        #endregion
    }
}