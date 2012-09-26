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
// Original file : BxlTokenTypeHelper.cs
// Project: Qorpent.Bxl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

namespace Qorpent.Bxl {
	internal static class BxlTokenTypeHelper {
		public static bool IsSpecial(this BxlTokenType type) {
			return BxlTokenType.Comma == type || BxlTokenType.Assign == type || BxlTokenType.Colon == type;
		}


		public static bool IsValidOpenState(this BxlToken token) {
			return IsValidOpenState(token.Type);
		}

		private static bool IsValidOpenState(this BxlTokenType type) {
			if (BxlTokenType.AttributeStart == type || BxlTokenType.AttributePart == type) {
				return true;
			}
			return BxlTokenType.ValueStart == type || BxlTokenType.ValuePart == type;
		}

		public static bool IsAttributeOpener(this BxlToken token) {
			return IsAttributeOpener(token.Type);
		}

		private static bool IsAttributeOpener(this BxlTokenType type) {
			if (BxlTokenType.AttributeStart == type || BxlTokenType.AttributePart == type) {
				return true;
			}
			return false;
		}


		public static bool IsString(this BxlTokenType type) {
			return BxlTokenType.StringStart == type || BxlTokenType.StringPart == type || BxlTokenType.String == type ||
			       BxlTokenType.StringEnd == type;
		}


		public static bool IsInValidOpenState(this BxlToken token) {
			return IsInValidOpenState(token.Type);
		}

		private static bool IsInValidOpenState(this BxlTokenType type) {
			//do not allow non xml matched opens in production
			if (BxlTokenType.StringStart == type || BxlTokenType.ExpressionStart == type) {
				return true;
			}
			if (BxlTokenType.StringPart == type || BxlTokenType.ExpressionPart == type) {
				return true;
			}
			return false;
		}

		public static bool CanAssign(this BxlToken l, BxlToken r) {
			return CanAssign(l.Type, r.Type);
		}

		private static bool CanAssign(this BxlTokenType l, BxlTokenType r) {
			if (!(BxlTokenType.String == l || BxlTokenType.Literal == l)) {
				return false;
			}
			if (!(BxlTokenType.Expression == r || BxlTokenType.Literal == r || BxlTokenType.String == r
			      || BxlTokenType.StringStart == r || BxlTokenType.ExpressionStart == r
			     )) {
				return false;
			}
			return true;
		}

		public static bool IsValue(this BxlToken token) {
			return IsValue(token.Type);
		}

		private static bool IsValue(this BxlTokenType type) {
			return BxlTokenType.String == type || BxlTokenType.Expression == type || BxlTokenType.Literal == type;
		}


		public static bool IsStartValue(this BxlToken token) {
			return IsStartValue(token.Type);
		}

		private static bool IsStartValue(this BxlTokenType type) {
			return BxlTokenType.StringStart == type || BxlTokenType.ExpressionStart == type;
		}


		public static bool IsLineStart(this BxlToken token) {
			return IsLineStart(token.Type);
		}

		private static bool IsLineStart(this BxlTokenType type) {
			return type == BxlTokenType.NewLine || type == BxlTokenType.Level || type == BxlTokenType.Start;
		}

		public static bool IsLineFinish(this BxlToken token) {
			return IsLineFinish(token.Type);
		}

		private static bool IsLineFinish(this BxlTokenType type) {
			return type == BxlTokenType.NewLine || type == BxlTokenType.Finish;
		}

		public static bool IsWs(this BxlToken token) {
			return IsWs(token.Type);
		}

		private static bool IsWs(this BxlTokenType type) {
			return BxlTokenType.WhiteSpace == type || BxlTokenType.Level == type || BxlTokenType.NewLine == type;
		}

		public static bool CanBeStartOfLiteral(this BxlTokenType type) {
			return BxlTokenType.Letter == type || BxlTokenType.NonLetter == type;
		}
	}
}