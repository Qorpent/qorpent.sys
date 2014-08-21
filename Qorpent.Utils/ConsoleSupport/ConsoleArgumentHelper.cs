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
// PROJECT ORIGIN: Qorpent.Utils/ConsoleArgumentHelper.cs
#endregion

using System;
using System.Collections.Generic;

namespace Qorpent.Utils {
	/// <summary>
	/// 	Helper to work with console parameters in form --param1 value1 --param2 value2 --param3 --param4
	/// </summary>
	public class ConsoleArgumentHelper {
		/// <summary>
		/// 	creates new instance of console arguments helper
		/// </summary>
		public ConsoleArgumentHelper() {
			_reflectionhelper = new ReflectionHelper();
		}

		/// <summary>
		/// 	Parses given console arguments into dictionary, -- cropped, not named parameters named as arg1..argN
		/// 	Beaviour:
		/// 	In argument strings:
		/// 	--x XVAL --y YVAL -a -b -z 5 A S
		/// 	=>
		/// 	{ x:XVAL, y:YVAL, -a:1, -b:1, -z:5, arg6:A, arg7:S }
		/// 	method tryes to parse usual Unix-like args --NAME Value, but supports numbered parameters
		/// </summary>
		/// <param name="args"> </param>
		/// <returns> </returns>
		public IDictionary<string, string> ParseDictionary(string[] args) {
			var result = new Dictionary<string, string>();
			var argnumber = 1; //argument number counter
			var lastname = ""; // last parameter parsed in valid --NAME form
			var namedparameteropened = false; //flag that parameter Value is awaiting
			foreach (var str in args) {
				var argname = ""; // temporal for argname
				if (str.StartsWith("--")) {
					// it's named parameter start
					argname = str.Substring(2); //-- cropped
					result[argname] = "1";
					//store default arg Value in result to indicate that parameter persists
					// we use '1' Value to indicate that without Value it's 'true'
					lastname = argname; //set point to last parameter name (for storing Value thurther)
					namedparameteropened = true; //set flag that next string can be parsed as Value
					argnumber++; //increase argument's counter
				}
				else {
					// we se Value or not-named parameter
					if (namedparameteropened) {
						//if parameter was opened, it's Value
						result[lastname] = str; //set Value
						namedparameteropened = false; //close parameter
					}
					else {
						// we see not named parameter, must store as argN=str
						argname = "arg" + argnumber;
						result[argname] = str;
						argnumber++;
					}
				}
			}
			return result;
		}

		/// <summary>
		/// 	Parses given console arguments into TARGS, all '-' cropped
		/// 	<seealso cref="ParseDictionary" />
		/// </summary>
		/// <param name="args"> </param>
		/// <returns> </returns>
		public TArgs Parse<TArgs>(string[] args) where TArgs : new() {
			var result = new TArgs();
			Apply(args, result);
			return result;
		}
		/// <summary>
		/// применяет параметры к существующему классу
		/// </summary>
		/// <typeparam name="TArgs"></typeparam>
		/// <param name="args"></param>
		/// <param name="result"></param>
		public void Apply<TArgs>(string[] args, TArgs result) where TArgs : new(){
			var parseresult = ParseDictionary(args);
			foreach (var parameter in parseresult){
				var name = parameter.Key.Replace("-", "");
				var value = parameter.Value;
				_reflectionhelper.SetValue(result, name, value, ignoreNotFound: true, publicOnly: false);
			}
		}

		private readonly ReflectionHelper _reflectionhelper;
		/// <summary>
		/// Считывает текст с коммандной строки, заменяя символы звездочками
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public string ReadLineSafety(string message) {
			Console.WriteLine();
			Console.Write(message);
			string buffer = "";
			while (true) {
				var key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.Enter) {
					Console.WriteLine();
					return buffer;
				}
				buffer += key.KeyChar;
			}
		}
	}
}