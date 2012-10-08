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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Collections{
    /// <summary>
    /// Карта строк, работает в режиме
    /// игнорирования или использования
    /// регистра
    /// </summary>
    public class StringMap : Map<string, string>{
        private bool ignoreCase = true;

        private bool returnAsUpperCase = true;

        /// <summary>
        /// Признак игнорирования регистра,
        /// по умолчанию - true (игнорируется)
        /// </summary>
        public bool IgnoreCase{
            get { return ignoreCase; }
            set { ignoreCase = value; }
        }

        /// <summary>
        /// Признак возврата значений с общим (верхним) регистром
        /// </summary>
        public bool ReturnAsUpperCase{
            get { return returnAsUpperCase; }
            set { returnAsUpperCase = value; }
        }

	    /// <summary>
	    /// Основной предикат (проверяет источник)
	    /// </summary>
	    /// <param name="from">ключ источника</param>
	    /// <returns>ламбда проверки источника, по умолчанию m => m.From.Equals(from)</returns>
	    protected override Func<IMapItem<string, string>, bool> GetMainPredicate(string from){
            return m => (!IgnoreCase && (m.From == from)) || (IgnoreCase && (m.From.ToUpper() == from.ToUpper()));
        }

	    /// <summary>
	    /// Реверсивный предикат (проверяет цель)
	    /// </summary>
	    /// <param name="to">ключ цели</param>
	    /// <returns>ламбда проверки цели, по умолчанию m => m.To.Equals(ConvertTo)</returns>
	    protected override Func<IMapItem<string, string>, bool> GetReveresePredicate(string to){
            return m => (!IgnoreCase && (m.To == to)) || (IgnoreCase && (m.To.ToUpper() == to.ToUpper()));
        }

	    /// <summary>
	    /// Основной конвертер - преобразует цели
	    /// </summary>
	    /// <returns>ламбда конвертации элемента карты в ее цель, по умолчанию m=>m.To</returns>
	    protected override Func<IMapItem<string, string>, string> GetMainConverter(){
            return m => ReturnAsUpperCase ? m.To.ToUpper() : m.To;
        }

	    /// <summary>
	    /// Реверсивный конвертер - преобразует источники
	    /// </summary>
	    /// <returns>ламбда конвертации элемента карты в его источник, по умолчанию m=>m.From</returns>
	    protected override Func<IMapItem<string, string>, string> GetReverseConverter(){
            return m => ReturnAsUpperCase ? m.From.ToUpper() : m.From;
        }

        /// <summary>
        /// Возвращает весь список входных узлов
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public string[] ReverseAll(string to){
            var result = new List<string>(this.Where(GetReveresePredicate(to)).Select(GetReverseConverter()).Distinct().ToList());      
            var cnt = 1;
            while (cnt != 0) {
                cnt = 0;
                foreach (var i in result.ToArray()) {
                    var refs = this.Where(GetReveresePredicate(i)).Select(GetReverseConverter()).Distinct().ToArray();
                    foreach (var r in refs) {
                        if(!result.Contains(r)) {
                            result.Add(r);
                            cnt++;
                        }
                    }
                }
            }
            return result.ToArray();

        }

		/// <summary>
		/// Возвращает весь список входных узлов
		/// </summary>
		/// <param name="from"></param>
		/// <returns></returns>
		public string[] ForwardAll(string from)
		{
			var result = new List<string>(this.Where(GetMainPredicate(from)).Select(GetMainConverter()).Distinct().ToList());
			var cnt = 1;
			while (cnt != 0)
			{
				cnt = 0;
				foreach (var i in result.ToArray())
				{
					var refs = this.Where(GetMainPredicate(i)).Select(GetMainConverter()).Distinct().ToArray();
					foreach (var r in refs)
					{
						if (!result.Contains(r))
						{
							result.Add(r);
							cnt++;
						}
					}
				}
			}
			return result.ToArray();

		}

    }
}