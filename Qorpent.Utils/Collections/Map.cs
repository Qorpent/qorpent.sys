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
// PROJECT ORIGIN: Qorpent.Utils/Map.cs
#endregion
using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Collections{
    /// <summary>
    /// Двунаправленная карта вида источник->цель c возможностью прямого
    /// и обратного поиска
    /// характер обхода опосредован вычислением предиката,
    /// также можно приписать конвертер получаемых значений
    /// </summary>
    /// <typeparam name="F">тип исходного значения</typeparam>
    /// <typeparam name="T">тип целевого значения</typeparam>
    public class Map<F, T> : List<IMapItem<F, T>>, IMap<F, T>{
        #region IMap<F,T> Members

        /// <summary>
        /// Возвращает массив целей с общим источником
        /// </summary>
        /// <param name="from">источник</param>
        /// <returns>массив целей</returns>
        public T[] this[F from]{
            get{
                return Enumerable.Select(this
                                .Where(GetMainPredicate(from)), GetMainConverter()
                    ).Distinct().ToArray();
            }
            set{
                foreach (var v in value){
                    this.Add(new MapItem<F, T> { From = from, To = v });    
                }
                
            }
           
        }

        /// <summary>
        /// Создает и добавляет новый элемент карты
        /// </summary>
        /// <param name="from">источник</param>
        /// <param name="to">цель</param>
        /// <returns>элемент карты</returns>
        public IMapItem<F, T> Add(F from, T to){
            var result = new MapItem<F, T>{From = from, To = to};
            Add(result);
            return result;
        }

        /// <summary>
        /// Возвращает массив источников для цели
        /// </summary>
        /// <param name="to">цель</param>
        /// <returns>массив источников</returns>
        public F[] Reverse(T to){
            return Enumerable.Select(this.Where(GetreversePredicate(to)), GetReverseConverter()).Distinct().ToArray();
        }

        #endregion

        /// <summary>
        /// Основной конвертер - преобразует цели
        /// </summary>
        /// <returns>ламбда конвертации элемента карты в ее цель, по умолчанию m=>m.To</returns>
        protected virtual Func<IMapItem<F, T>, T> GetMainConverter(){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.To;
                   };
        }

        /// <summary>
        /// Основной предикат (проверяет источник)
        /// </summary>
        /// <param name="from">ключ источника</param>
        /// <returns>ламбда проверки источника, по умолчанию m => m.From.Equals(from)</returns>
        protected virtual Func<IMapItem<F, T>, bool> GetMainPredicate(F from){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.From.Equals(from);
                   };
        }

        /// <summary>
        /// Реверсивный конвертер - преобразует источники
        /// </summary>
        /// <returns>ламбда конвертации элемента карты в его источник, по умолчанию m=>m.From</returns>
        protected virtual Func<IMapItem<F, T>, F> GetReverseConverter(){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.From;
                   };
        }

        /// <summary>
        /// Реверсивный предикат (проверяет цель)
        /// </summary>
        /// <param name="to">ключ цели</param>
        /// <returns>ламбда проверки цели, по умолчанию m => m.To.Equals(ConvertTo)</returns>
        protected virtual Func<IMapItem<F, T>, bool> GetreversePredicate(T to){
            return m =>
                   {
                       //Contract.Assume(m != null);
                       return m.To.Equals(to);
                   };
        }

        
    }
}