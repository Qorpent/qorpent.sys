using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Dsl.Json {
	/// <summary>
	///Токенизатор JSON
	/// </summary>
	public class Tokenizer {
		private bool instring;
		private bool inliteral;
		private bool innumber;
		private string currentbuffer;
		private char stringstart;
		private bool inescape;
		/// <summary>
		/// Перевести строку JSON в токены
		/// </summary>
		/// <param name="jsonstring"></param>
		/// <returns></returns>
		public IEnumerable<JsonToken> Tokenize(string jsonstring) {
			return 
				from c in jsonstring 
				let type = GetCType(c) 
				from t in ProcessChar(type, c) 
				select t;
		}

		private IEnumerable<JsonToken> ProcessChar(CType type, char c)
		{

			//проверяем ESC последовательность
			if (CheckEscaping(type)) {
				yield break;
			}

			// проверяем строки
			if (instring) {
				JsonToken outString;
				if (CheckStringInner(type, c, out outString)) {
					yield return outString;
				} yield break;
			}

			// проверяем литералы
			if (inliteral) {
				if (CType.Lit == type || CType.Dig == type) {
					currentbuffer += c;
					yield break;
				}
				if (0 == (type & (CType.Com | CType.Col | CType.WS | CType.NL | CType.Cl|CType.ClA))) {
					throw new Exception("illegal symbol in literal");
				}
				yield return JsonToken.Lit(currentbuffer);
				currentbuffer = "";
				inliteral = false;
			}

			//проверяем числа
			if (innumber) {
				if (CType.Dig == type || CType.Dot == type)
				{
					currentbuffer += c;
					yield break;
				}
				if (0 == (type & (CType.Com | CType.Col | CType.WS | CType.NL | CType.Cl | CType.ClA)))
				{
					throw new Exception("illegal symbol in number");
				}

				yield return JsonToken.Num(Convert.ToDecimal(currentbuffer));
				currentbuffer = "";
				innumber = false;
			}

			//начало литерала
			if (CType.Lit == type) {
				inliteral = true;
				currentbuffer += c;
				yield break;
			}
			//начало числа
			if (CType.Dig == type || CType.Min==type)
			{
				innumber = true;
				currentbuffer += c;
				yield break;
			}
			//начало строки
			if (CType.Apos == type || CType.Quot == type)
			{
				instring = true;
				stringstart = c;
				yield break;
			}

			// проверяем спецсимвлоы
			//во всех остальных случаях -WS - игнор
			if (CType.WS == type || CType.NL == type) {
				yield break;
			}

			if (CType.Com == type) {
				yield return JsonToken.Comma;
				yield break;
			}
			if (CType.Col == type)
			{
				yield return JsonToken.Colon;
				yield break;
			}
			if (CType.Op == type)
			{
				yield return JsonToken.Open;
				yield break;
			}
			if (CType.Cl == type)
			{
				yield return JsonToken.Close;
				yield break;
			}
			if (CType.OpA == type)
			{
				yield return JsonToken.OpenArray;
				yield break;
			}
			if (CType.ClA == type)
			{
				yield return JsonToken.CloseArray;
				yield break;
			}
		}

		private bool CheckStringInner(CType type, char c, out JsonToken processChar) {
			if (CType.NL == type) throw new Exception("cannot nl in string");
			if (stringstart == c) {
				if (inescape) {
					currentbuffer += c;
					inescape = false;
					{
						processChar = null;
						return true;
					}
				}
				else {
					var token = JsonToken.String(currentbuffer);
					currentbuffer = null;
					instring = false;
					stringstart = '\0';
					{
						processChar = token;
						return true;
					}
				}
			}
			if (inescape) {
				if (c == '\\')
				{
					currentbuffer += "\\";
					inescape = false;
				}
				if (c == 'r') {
					currentbuffer += "\r";
					inescape = false;
				}
				else if (c == 't') {
					currentbuffer += "\t";
					inescape = false;
				}
				else if (c == 'n') {
					currentbuffer += "\t";
					inescape = false;
				}
				else if (c == '"')
				{
					currentbuffer += "\"";
					inescape = false;
				}
				else if (c == '\'')
				{
					currentbuffer += "'";
					inescape = false;
				}
				else {
					throw new Exception("unknown esc "+c);
				}
			}
			else {
				currentbuffer += c;
			}
			processChar = null;
			return false;
		}

		private bool CheckEscaping(CType type) {
			if (CType.Esc == type) {
				if (!instring) throw new Exception("esc can be only in string");
				if (inescape) {
					currentbuffer += '\\';
					inescape = false;
				}
				else {
					inescape = true;
				}
				return true;
			}
			return false;
		}


		static CType GetCType(char c) {
			switch (c) {
				case '\\':
					return CType.Esc;
				case '{':
					return CType.Op;
				case '}':
					return CType.Cl;
				case '[':
					return CType.OpA;
				case ']':
					return CType.ClA;
				case ' ':
				case '\t':
					return CType.WS;
				case '\r':
				case '\n':
					return CType.NL;
				case '_':
					return CType.Lit;
				case '.':
					return CType.Dot;
				case '"':
					return CType.Quot;
				case '\'':
					return CType.Apos;
				case '-':
					return CType.Min;
				case ',':
					return CType.Com;
				case ':':
					return CType.Col;
				default :
					if(char.IsDigit(c))return CType.Dig;
					if(char.IsLetter(c))return CType.Lit;
					return CType.NLit;
			}
		}
	}
}