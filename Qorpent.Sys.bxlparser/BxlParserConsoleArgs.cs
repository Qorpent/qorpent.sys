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
// Original file : BxlParserConsoleArgs.cs
// Project: Qorpent.Sys.bxlparser
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Linq;
using Qorpent.IO;
using System;

namespace bxlparser {
	/// <summary>
	/// 	Параметры консоли
	/// </summary>
	public class BxlParserConsoleArgs {
		private IEnumerable<string> _cachedFiles;

		/// <summary>
		/// 	Вызов строки помощи --help
		/// </summary>
		public bool Help { get; set; }
		/// <summary>
		/// Первичный параметр информации о файлах
		/// </summary>
		protected string Arg1 { get; set; }
		/// <summary>
		///Именованный акцессор для параметра файлов
		/// </summary>
		public string Files {get { return Arg1; }} 
		/// <summary>
		/// Возвращает перечень входных файлов
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetSourceFiles() {
			return _cachedFiles ?? (_cachedFiles = InternalSourceFiles().ToArray());
		}

		private IEnumerable<string> InternalSourceFiles() {
			if (string.IsNullOrWhiteSpace(Files)) {
				return new string[] {};
			}
			var resolver = new FileNameResolver {Root = Environment.CurrentDirectory};
			var masks = Files.Split(';').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
			return masks.SelectMany(
				mask =>
				resolver.ResolveAll(new FileSearchQuery
					{All = true, ExistedOnly = true, PathType = FileSearchResultType.FullPath, ProbeFiles = new[] {mask}}))
				.Distinct();
		}

		/// <summary>
		/// True - не включать в итоговый файл лексическую информацию
		/// </summary>
		public bool NoLexInfo { get; set; }

		/// <summary>
		/// Формат результата
		/// </summary>
		public BxlParserOutputFormat OutputFormat { get; set; }
		/// <summary>
		/// Директория для отправки результирующих файлов
		/// </summary>
		public string OutputDirectory { get; set; }
		/// <summary>
		/// Состоит из разделенных ; регека и замены, применяется к имени файла
		/// </summary>
		public string OutputRename { get; set; }

		/// <summary>
		/// Указывает, что следует вывести список исходных файлов, переданных в обработку
		/// </summary>
		public bool EnlistSources { get; set; }

		/// <summary>
		/// Перенаправляет вывод результата в консоль
		/// </summary>
		public bool ToConsole { get; set; }
	}
}