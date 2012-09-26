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
// Original file : BxlTokenType.cs
// Project: Qorpent.Bxl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Describes token type of char or complex token type
	/// </summary>
	public enum BxlTokenType {
		/// <summary>
		/// 	NewLineDelimiter \r or \n
		/// </summary>
		NewLine,

		/// <summary>
		/// 	Almost any symbol except delimiters and whitespaces
		/// </summary>
		Letter,

		/// <summary>
		/// 	Attribute assign symbol '='
		/// </summary>
		Assign,

		/// <summary>
		/// 	Whitespaces except newline delimiters
		/// </summary>
		WhiteSpace,

		/// <summary>
		/// 	Comment start symbol '#'
		/// </summary>
		Comment,

		/// <summary>
		/// 	Double quot
		/// </summary>
		Quot,

		/// <summary>
		/// 	Single quot
		/// </summary>
		Apos,

		/// <summary>
		/// 	Escape symbol '\'
		/// </summary>
		Escape,

		/// <summary>
		/// 	Colon symbol ':'
		/// </summary>
		Colon,

		/// <summary>
		/// 	Comma symbol ','
		/// </summary>
		Comma,

		/// <summary>
		/// 	Default symbol type if no other will applyed
		/// </summary>
		NonLetter,

		/// <summary>
		/// 	Default start Value
		/// </summary>
		None,

		/// <summary>
		/// 	Tripple qouts for verbatim string
		/// </summary>
		MultQuot,

		/// <summary>
		/// 	Pseudo token which means 'start of file'
		/// </summary>
		Start,

		/// <summary>
		/// 	Pseudo token which means 'end of file'
		/// </summary>
		Finish,

		/// <summary>
		/// 	Pseudo token which means 'can be treat as letter symbol'
		/// </summary>
		AssumedAsLetter,

		/// <summary>
		/// 	Complex 1 : quoted  string
		/// </summary>
		String,

		/// <summary>
		/// 	Complex 1 : literal Value
		/// </summary>
		Literal,

		/// <summary>
		/// 	Pseudo token - level indentation (set of tabs and 4x spaces)
		/// </summary>
		Level,

		/// <summary>
		/// 	Complex 2 : attribute token
		/// </summary>
		Attribute,

		/// <summary>
		/// 	Complex 2 : element token
		/// </summary>
		Element,

		/// <summary>
		/// 	Complex 2 : element Value token
		/// </summary>
		Value,

		/// <summary>
		/// 	Left brace symbol '('
		/// </summary>
		OpenExpr,

		/// <summary>
		/// 	Right brace symbol ')'
		/// </summary>
		CloseExpr,

		/// <summary>
		/// 	Complex 1 : expression token
		/// </summary>
		Expression,

		/// <summary>
		/// 	Non closed start string for L-b-L
		/// </summary>
		StringStart,

		/// <summary>
		/// 	Non closed start expression for L-b-L
		/// </summary>
		ExpressionStart,

		/// <summary>
		/// 	Non closed middle string for L-b-L
		/// </summary>
		StringPart,

		/// <summary>
		/// 	Non closed middle expression for L-b-L
		/// </summary>
		ExpressionPart,

		/// <summary>
		/// 	String closer for L-b-L
		/// </summary>
		StringEnd,

		/// <summary>
		/// 	Expression closer for L-b-L
		/// </summary>
		ExpressionEnd,

		/// <summary>
		/// 	Non closer attribute start for L-b-L
		/// </summary>
		AttributeStart,

		/// <summary>
		/// 	Non closer attribute part for L-b-L
		/// </summary>
		AttributePart,

		/// <summary>
		/// 	Attribute closer for L-b-L
		/// </summary>
		AttributeEnd,

		/// <summary>
		/// 	Non closed Value start for L-b-L
		/// </summary>
		ValueStart,

		/// <summary>
		/// 	Non closed Value middle for L-b-L
		/// </summary>
		ValuePart,

		/// <summary>
		/// 	Object closer for L-b-L
		/// </summary>
		ValueEnd,

		/// <summary>
		/// 	Error token - any parsing problem
		/// </summary>
		Error
	}
}