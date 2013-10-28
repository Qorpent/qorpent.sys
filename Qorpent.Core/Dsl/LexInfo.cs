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
// PROJECT ORIGIN: Qorpent.Core/LexInfo.cs
#endregion

using Qorpent.Serialization;

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Описывает информацию о позиции исходных файлов в BXL файле
	/// </summary>
	[Serialize]
	public class LexInfo {
		/// <summary>
		/// 
		/// </summary>
		public LexInfo() {
			
		}
		/// <summary>
        /// 	Создает новый экземпляр BxlLexInfo
		/// </summary>
		/// <param name="filename"> имя исходного файла </param>
		/// <param name="line"> номер строки </param>
		/// <param name="col"> номер колонки </param>
		/// <param name="charindex"> глобальный индекс символа </param>
		/// <param name="length"> длина элемента </param>
		/// <param name="context"></param>
		public LexInfo(string filename = "", int line = 0, int col = 0, int charindex = 0, int length = 0, string context =null) {
			File = filename;
			Line = line;
			Column = col;
			Length = length;
			CharIndex = charindex;
			Context = context;
		}

		/// <summary>
		/// 	Создает читаемую строку lexinfo
		/// </summary>
		/// <returns> </returns>
		public override string ToString() {
			return " at " + (File ?? "") + " : " + Line + ":" + Column + (string.IsNullOrWhiteSpace(Context)?"":" : "+Context);
		}

		/// <summary>
		/// 	Создает копию текущего lexinfo
		/// </summary>
		/// <returns> </returns>
		public LexInfo Clone() {
			return (LexInfo) MemberwiseClone();
		}

		/// <summary>
		/// 	****************************************Not-lined char index in whole file
		/// </summary>
		[SerializeNotNullOnly]
		public int CharIndex;

		/// <summary>
		/// 	Колонка количества символов в строке
		/// </summary>
		[SerializeNotNullOnly]
		public int Column;

		/// <summary>
		/// 	Имя исходного файла
		/// </summary>
		[SerializeNotNullOnly]
		public string File;

		/// <summary>
        /// 	Длина описанного элемента кода
		/// </summary>
		[SerializeNotNullOnly]
		public int Length;

		/// <summary>
		/// 	Номер строки, описываемый элементом код
		/// </summary>
		[SerializeNotNullOnly]
		public int Line;

		/// <summary>
		/// Дополнительный контекст
		/// </summary>
		[SerializeNotNullOnly]
		public string Context;
	}
}