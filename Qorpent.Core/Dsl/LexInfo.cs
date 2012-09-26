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
// Original file : LexInfo.cs
// Project: Qorpent.Core
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Dsl {
	/// <summary>
	/// 	Describe information of source file position in Bxl file
	/// </summary>
	public struct LexInfo {
		/// <summary>
		/// 	Creates new instance of BxlLexInfo
		/// </summary>
		/// <param name="filename"> source file name </param>
		/// <param name="line"> line number </param>
		/// <param name="col"> column number </param>
		/// <param name="charindex"> global char index </param>
		/// <param name="length"> length of item </param>
		public LexInfo(string filename = "", int line = 0, int col = 0, int charindex = 0, int length = 0) {
			File = filename;
			Line = line;
			Column = col;
			Length = length;
			CharIndex = charindex;
		}

		/// <summary>
		/// 	Generates readable lexinfo string
		/// </summary>
		/// <returns> </returns>
		public override string ToString() {
			return " at " + (File ?? "") + " : " + Line + ":" + Column;
		}

		/// <summary>
		/// 	Generates copy of current lexinfo
		/// </summary>
		/// <returns> </returns>
		public LexInfo Clone() {
			return (LexInfo) MemberwiseClone();
		}

		/// <summary>
		/// 	Not-lined char index in whole file
		/// </summary>
		public int CharIndex;

		/// <summary>
		/// 	Column number of char in row
		/// </summary>
		public int Column;

		/// <summary>
		/// 	Source file name
		/// </summary>
		public string File;

		/// <summary>
		/// 	Length of described code element
		/// </summary>
		public int Length;

		/// <summary>
		/// 	Line number of described code element
		/// </summary>
		public int Line;
	}
}