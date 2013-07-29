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
// Original file : TokenizerTestHelper.cs
// Project: Qorpent.Bxl.Tests
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;

namespace Qorpent.Bxl.Tests {
	public static class TokenizerTestHelper {
		public static string GetShortTypedNotation(IEnumerable<BxlToken> t, string rn = "\r\n") {
			var result = "";
			foreach (var token in t) {
				var c = GetTypeString(token);
				if (c != "\n") {
					result += c;
				}
				else {
					result += rn;
				}
			}
			return result;
		}

		private static string GetTypeString(BxlToken t) {
			switch (t.Type) {
				case BxlTokenType.Start:
					return "{";
				case BxlTokenType.Finish:
					return "}";
				case BxlTokenType.Literal:
					return "L";
				case BxlTokenType.String:
					return "S";

				case BxlTokenType.Attribute:
					return "A";
				case BxlTokenType.Element:
					return "E";
				case BxlTokenType.Value:
					return "V";
				case BxlTokenType.Level:
					return t.Number.ToString()[0].ToString();
				case BxlTokenType.Comma:
					return ",";
				case BxlTokenType.Assign:
					return "=";
				case BxlTokenType.Colon:
					return ":";
				case BxlTokenType.NewLine:
					return "\n";
				case BxlTokenType.Expression:
					return "X";
				case BxlTokenType.AttributeStart:
					return "A_";
				case BxlTokenType.AttributePart:
					return "_A_";
				case BxlTokenType.AttributeEnd:
					return "_A";
				case BxlTokenType.ValueStart:
					return "V_";
				case BxlTokenType.ValuePart:
					return "_V_";
				case BxlTokenType.ValueEnd:
					return "_V";
				default:
					return "?";
			}
		}
	}
}