#region LICENSE

// Copyright 2007-2012 Comdiv (F. Sadykov) - http://code.google.com/u/fagim.sadykov/
// Supported by Media Technology LTD 
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// Solution: Qorpent
// Original file : ConsoleRenderHelper.cs
// Project: Qorpent.Utils
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils {
	/// <summary>
	/// 	Упрощает вывод многострочных данных в консоль с элементами вики
	/// </summary>
	public class ConsoleRenderHelper {
		/// <summary>
		/// 	Выводит текст построчно с подстановкой цветов
		/// </summary>
		/// <param name="value"> </param>
		/// <param name="incolor"> </param>
		public void Render(string value, ConsoleColor incolor = ConsoleColor.Black) {
			if (string.IsNullOrWhiteSpace(value)) {
				return;
			}
			var strings = value.LfOnly().Split('\n').ToArray();
			if (0 == strings.Length) {
				return;
			}
			try {
				if(incolor!=ConsoleColor.Black) {
					Console.ForegroundColor = incolor;
				}
				foreach (var s in strings) {
					if (Regex.IsMatch(s, @"^\[\w+\]$")) {
						if (s == "[DEFAULT]") {
							Console.ResetColor();
						}
						else {
							var colorname = s.Replace("[", "").Replace("]", "");
							var color = Enum.Parse(typeof (ConsoleColor), colorname, true);
							Console.ForegroundColor = (ConsoleColor) color;
						}
					}
					else {
						Console.WriteLine(s);
					}
				}
			}
			finally {
				Console.ResetColor();
			}
		}
	}
}