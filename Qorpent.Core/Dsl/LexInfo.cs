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

#if !EMBEDQPT
using Qorpent.Serialization;
#endif
namespace Qorpent.Dsl {
	/// <summary>
	/// 	Описывает информацию о позиции исходных файлов в BXL файле
	/// </summary>
#if !EMBEDQPT
	[Serialize]
#endif
	public class LexInfo {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(LexInfo other){
			return Column == other.Column && string.Equals(File, other.File) && Line == other.Line;
		}

		/// <summary>
		/// Serves as a hash function for a particular type. 
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"/>.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode(){
			unchecked{
			    // ReSharper disable once NonReadonlyMemberInGetHashCode
				int hashCode = Column;
			    // ReSharper disable once NonReadonlyMemberInGetHashCode
				hashCode = (hashCode*397) ^ (File?.GetHashCode() ?? 0);
			    // ReSharper disable once NonReadonlyMemberInGetHashCode
				hashCode = (hashCode*397) ^ Line;
				return hashCode;
			}
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param><filterpriority>2</filterpriority>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((LexInfo) obj);
		}

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
		#if !EMBEDQPT
		[SerializeNotNullOnly]
#endif
		public int CharIndex;

		/// <summary>
		/// 	Колонка количества символов в строке
		/// </summary>
#if !EMBEDQPT
		[SerializeNotNullOnly]
#endif
		public int Column;

		/// <summary>
		/// 	Имя исходного файла
		/// </summary>
#if !EMBEDQPT
		[SerializeNotNullOnly]
#endif
		public string File;

		/// <summary>
        /// 	Длина описанного элемента кода
		/// </summary>
#if !EMBEDQPT
		[SerializeNotNullOnly]
#endif
		public int Length;

		/// <summary>
		/// 	Номер строки, описываемый элементом код
		/// </summary>
#if !EMBEDQPT
		[SerializeNotNullOnly]
#endif
		public int Line;

		/// <summary>
		/// Дополнительный контекст
		/// </summary>
#if !EMBEDQPT
		[SerializeNotNullOnly]
#endif
		public string Context;
	}
}