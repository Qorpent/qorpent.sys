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
// PROJECT ORIGIN: Qorpent.Dsl/ReplaceDescriptor.cs
#endregion
using System.Text.RegularExpressions;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Описатель операции замены в тексте
	/// </summary>
	public class ReplaceDescriptor {
		private Regex _Regex {
			get { return _regex ?? (_regex = new Regex(Pattern, RegexOptions)); }
		}

		/// <summary>
		/// 	Регекс-паттерн
		/// </summary>
		public string Pattern { get; set; }

		/// <summary>
		/// 	Опции регес-паттерна
		/// </summary>
		public RegexOptions RegexOptions { get; set; }

		/// <summary>
		/// 	Обычная строковая замена
		/// </summary>
		public string Replacer { get; set; }

		/// <summary>
		/// 	Динамическая замена
		/// </summary>
		public MatchEvaluator Evaluator { get; set; }

		/// <summary>
		/// 	Выполняет замену в целевой строке
		/// </summary>
		/// <param name="sourceString"> </param>
		/// <returns> </returns>
		public string Execute(string sourceString) {
			if (null != Evaluator) {
				return _Regex.Replace(sourceString, Evaluator);
			}
			return _Regex.Replace(sourceString, Replacer);
		}

		private Regex _regex;
	}
}