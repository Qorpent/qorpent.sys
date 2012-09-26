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
// Original file : BxlTokenizer.cs
// Project: Qorpent.Bxl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Qorpent.Dsl;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Build BXL token flow from source (real parser for BXL)
	/// </summary>
	public class BxlTokenizer {
		/// <summary>
		/// 	Line-by-line option: marks that strings and expressions can be not closed at end of file
		/// </summary>
		public bool AllowNonClosedStringsAndExpressions { get; set; }

		/// <summary>
		/// 	Line-by-line flag : marks that line before current was ended with not terminated element Value
		/// </summary>
		public bool HasOpenedValueAtStart { get; set; }

		/// <summary>
		/// 	Line-by-line flag : marks that line before current was ended with not terminated attribute
		/// </summary>
		public bool HasOpenedAttributeAtStart { get; set; }

		/// <summary>
		/// 	Line-by-line flag : marks that line before current was ended with not terminated string
		/// </summary>
		public bool HasOpenedStringAtStart { get; set; }

		/// <summary>
		/// 	Line-by-line flag : marks that line before current was ended with not terminated expression
		/// </summary>
		public bool HasOpenedExpressionAtStart { get; set; }

		/// <summary>
		/// 	Line-by-line flag : current nesting level of non-terminated expression
		/// </summary>
		public int ExpressionNestLevelAtStart { get; set; }

		/// <summary>
		/// 	Line-by-line flag : marks that opened token at start of parsing closed
		/// </summary>
		public bool OpenedStartIsClosed { get; set; }

		/// <summary>
		/// 	set on no-elemnts mode (debug propose)
		/// </summary>
		public BxlTokenizer NoElements {
			get {
				_donotextractelements = true;
				return this;
			}
		}

		/// <summary>
		/// 	Line-by-line flag : marks that hase opened element Value at end of string
		/// </summary>
		public bool HasOpenedValueAtEnd { get; set; }

		/// <summary>
		/// 	Line-by-line flag : marks that hase opened attribute Value at end of string
		/// </summary>
		public bool HasOpenedAttributeAtEnd { get; set; }

		/// <summary>
		/// 	Line-by-line flag : nest number of non closed expression
		/// </summary>
		public int ExpressionNestLevelAtEnd { get; set; }

		/// <summary>
		/// 	Line-by-line flag : indicates opened expression at end of line
		/// </summary>
		public bool HasOpenedExpressionAtEnd { get; set; }

		/// <summary>
		/// 	Line-by-line flag : indicates opened string at end of line
		/// </summary>
		public bool HasOpenedStringAtEnd { get; set; }

		/// <summary>
		/// 	Line-by-line : setup initial state from special statecode
		/// </summary>
		/// <param name="statecode"> TODO: QPT-1 write statecode format </param>
		/// <exception cref="BxlException"></exception>
		public void SetInitialState(int statecode) {
			AllowNonClosedStringsAndExpressions = true;
			HasOpenedAttributeAtStart = false;
			HasOpenedValueAtStart = false;
			HasOpenedExpressionAtStart = false;
			HasOpenedStringAtStart = false;


			var attrvalselector = statecode/10000;
			if (attrvalselector > 0) {
				if (attrvalselector == 1) {
					HasOpenedValueAtStart = true;
				}
				else {
					HasOpenedAttributeAtStart = true;
				}
				var strexprselector = (statecode%10000)/1000;
				if (strexprselector == 0) {
					throw new BxlException("invalid string or expression selector", new LexInfo(_filename, _line, _col));
				}
				if (strexprselector == 1) {
					HasOpenedStringAtStart = true;
				}
				else {
					HasOpenedExpressionAtStart = true;
					var nest = statecode%1000;
					if (0 == nest) {
						throw new BxlException("not closed expressions must have nest level more than zero",
						                       new LexInfo(_filename, _line, _col));
					}
					ExpressionNestLevelAtStart = nest;
				}
			}
		}

		/// <summary>
		/// 	Generates end statecode for VS-like colorizer tools (line-by-line mode)
		/// </summary>
		/// <returns> </returns>
		public int GetFinishState() {
			if (!AllowNonClosedStringsAndExpressions) {
				return 0;
			}
			var result = 0;
			if (HasOpenedValueAtEnd) {
				result += 10000;
			}
			if (HasOpenedAttributeAtEnd) {
				result += 20000;
			}
			if (HasOpenedStringAtEnd) {
				result += 1000;
			}
			if (HasOpenedExpressionAtEnd) {
				result += 2000;
				result += ExpressionNestLevelAtEnd;
			}
			if (0 == result) {
				result = 1; //indicate keep AllowNonClosed
			}
			return result;
		}

		/// <summary>
		/// 	Main parser method - parse and conver source code to list of BxlTokens
		/// </summary>
		/// <param name="input"> source code </param>
		/// <param name="filename"> filename (for lex info) </param>
		/// <returns> </returns>
		public BxlToken[] Tokenize(string input, string filename = "") {
			_filename = filename ?? "main";
			_input = input;

			ResetTokenizer();
			foreach (var c in _input) {
				if (InitIteration(c)) {
					continue;
				}
				_currentType = GetSymbolType(c);
				if (ProcessComment()) {
					continue;
				}
				if (ProcessNewLine()) {
					continue;
				}
				if (ProcessWhiteSpace()) {
					continue;
				}
				if (ProcessEscape()) {
					continue;
				}
				if (ProcessExpression()) {
					continue;
				}
				if (ProcessString()) {
					continue;
				}
				if (ProcessDelimiter()) {
					continue;
				}
				ProcessLiteral();
			}
			FinalizeTokenizing();
			ExtractAttributes();
			if (!_donotextractelements) {
				ExtractValues();
				ExtractElements();
			}
			var r = _result.ToArray();
			if (AllowNonClosedStringsAndExpressions) {
				if (2 < r.Length) {
					var opencandidate = r.Reverse().Skip(1).First();
					if (opencandidate.IsValidOpenState()) {
						if (opencandidate.SubType.IsString() ||
						    (null != opencandidate.Right && opencandidate.Right.Type.IsString())
							) {
							HasOpenedStringAtEnd = true;
						}
						else {
							HasOpenedExpressionAtEnd = true;
							ExpressionNestLevelAtEnd = _exprnest;
						}
						if (opencandidate.IsAttributeOpener()) {
							HasOpenedAttributeAtEnd = true;
						}
						else {
							HasOpenedValueAtEnd = true;
						}
					}
					else if (opencandidate.IsInValidOpenState()) {
						//treat such cases as anonym attributes
						opencandidate.SubType = opencandidate.Type;
						opencandidate.Type = BxlTokenType.AttributeStart;
						HasOpenedAttributeAtEnd = true;
						if (opencandidate.SubType.IsString()) {
							HasOpenedStringAtEnd = true;
						}
						else {
							HasOpenedExpressionAtEnd = true;
							ExpressionNestLevelAtEnd = _exprnest;
						}
					}
				}
			}
			return r;
		}

		private void ExtractValues() {
			if (HasOpenedValueAtStart) {
				if (_result.Count > 3) {
					throw new BxlException("illegal element Value construction", new LexInfo(_filename, _line, _col));
				}
				var candidate = _result[1];
				if (OpenedStartIsClosed) {
					candidate.SubType = candidate.Type;
					candidate.Type = BxlTokenType.ValueEnd;
				}
				else {
					candidate.SubType = candidate.Type;
					candidate.Type = BxlTokenType.ValuePart;
					HasOpenedValueAtEnd = true;
					if (candidate.SubType.IsString()) {
						HasOpenedStringAtEnd = true;
					}
					else {
						HasOpenedExpressionAtEnd = true;
						ExpressionNestLevelAtEnd = _exprnest;
					}
				}
			}
			else {
				for (var i = _result.Count - 1; i >= 0; i--) {
					if (_result[i].ColonPrefixed) {
						var c = _result[i];

						if (c.Type == BxlTokenType.NewLine) {
							continue;
						}


						if (c.IsValue() && c.Next.IsLineFinish()) {
							c.SubType = c.Type;
							c.Type = BxlTokenType.Value;
							continue;
						}
						if (AllowNonClosedStringsAndExpressions) {
							if (c.IsStartValue() && c.Next.IsLineFinish()) {
								c.SubType = c.Type;
								c.Impl = c.Standalone();
								c.Type = BxlTokenType.ValueStart;
								continue;
							}
						}
						throw new BxlException("illegal Value declaration " + _result[i].LexInfo, new LexInfo(_filename, _line, _col));
					}
				}
			}
		}


		private void ExtractElements() {
			if (HasOpenedAttributeAtStart || HasOpenedValueAtStart) {
				return;
			}
			for (var i = _result.Count - 1; i >= 0; i--) {
				if (_result[i].IsLineStart()) {
					var ec = _result[i + 1];

					if (ec.Type == BxlTokenType.Literal) {
						ec.SubType = ec.Type;
						ec.Type = BxlTokenType.Element;
						ec.Name = ec.Value;
						if (ec.Prev.Type == BxlTokenType.Level) {
							ec.Level = ec.Prev.Level;
						}
					}
					else if (ec.Type == BxlTokenType.Attribute || ec.Type == BxlTokenType.Level || ec.Type == BxlTokenType.Value ||
					         ec.Type == BxlTokenType.Finish || ec.Type == BxlTokenType.NewLine) {}
					else if (ec.Type == BxlTokenType.String) {
						ec.SubType = ec.Type;
						ec.Type = BxlTokenType.Value;
					}
					else {
						throw new BxlException(
							"all lines must be start with element and it must be literal or lines can be attributes " +
							_result[i].LexInfo, new LexInfo(_filename, _line, _col));
					}
				}
			}
		}

		private void ExtractAttributes() {
			for (var i = _result.Count - 1; i > 0; i--) {
				if (_result[i].IsAssigner) {
					var left = _result[i];
					var right = _result[i + 1];
					if (left.CanAssign(right)) {
						var attr = _result[i];
						attr.Left = new BxlToken {Type = _result[i].Type, Value = _result[i].Value, LexInfo = attr.LexInfo.Clone()};
						attr.Right = right;
						attr.LexInfo.CharIndex = left.LexInfo.CharIndex;
						attr.LexInfo.Length = (right.LexInfo.CharIndex + right.LexInfo.Length) - left.LexInfo.CharIndex;
						attr.Type = BxlTokenType.Attribute;
						attr.Name = left.Value;
						attr.AttrValue = right.Value;
						right.Next.Prev = attr;
						_result.RemoveAt(i + 1);
						if (attr.Prev.Type == BxlTokenType.Level) {
							attr.Level = attr.Prev.Level;
						}
						if (AllowNonClosedStringsAndExpressions) {
							if (right.IsStartValue()) {
								attr.Type = BxlTokenType.AttributeStart;
							}
						}
					}
					else {
						throw new BxlException(
							"cannot generate attribute from " + left.Type + " and " + right.Type + _result[i].LexInfo,
							new LexInfo(_filename, _line, _col));
					}
				}
				if (1 == i && !_result[i].ColonPrefixed) {
					if (HasOpenedAttributeAtStart && HasOpenedStringAtStart && _result[i].Type == BxlTokenType.StringPart) {
						_result[i].SubType = BxlTokenType.StringPart;
						_result[i].Type = BxlTokenType.AttributePart;
					}
					else if (HasOpenedAttributeAtStart && HasOpenedExpressionAtStart &&
					         _result[i].Type == BxlTokenType.ExpressionPart) {
						_result[i].SubType = BxlTokenType.ExpressionPart;
						_result[i].Type = BxlTokenType.AttributePart;
					}
					else if (HasOpenedAttributeAtStart && HasOpenedStringAtStart &&
					         _result[i].Type == BxlTokenType.StringEnd) {
						_result[i].SubType = BxlTokenType.StringEnd;
						_result[i].Type = BxlTokenType.AttributeEnd;
					}
					else if (HasOpenedAttributeAtStart && HasOpenedExpressionAtStart &&
					         _result[i].Type == BxlTokenType.ExpressionEnd) {
						_result[i].SubType = BxlTokenType.ExpressionEnd;
						_result[i].Type = BxlTokenType.AttributeEnd;
					}
				}
			}
		}

		private void FinalizeTokenizing() {
			if (_inliteral) {
				Appendliteral();
			}
			if (AllowNonClosedStringsAndExpressions) {
				if (_inexpression) {
					if (HasOpenedExpressionAtStart && !OpenedStartIsClosed) {
						Appendexpression(notclosed: true, ispart: true);
					}
					else {
						Appendexpression(notclosed: true);
					}
				}
				if (_instring && _tripplestring) {
					if (HasOpenedStringAtStart && !OpenedStartIsClosed) {
						Appendstring(notclosed: true, ispart: true);
					}
					else {
						Appendstring(notclosed: true);
					}
				}
			}
			else {
				if (_inexpression || _exprnest > 0) {
					throw new BxlException("not closed expression, started at " + _filename + ":" + _startline + ":" + _startcol,
					                       new LexInfo(_filename, _line, _col));
				}
			}
			if (_instring) {
				throw new BxlException("not closed string, started at " + _filename + ":" + _startline + ":" + _startcol,
				                       new LexInfo(_filename, _line, _col));
			}
			while (_result.Last().IsWs()) {
				_result.RemoveAt(_result.Count - 1);
			}

			_result.Add(new BxlToken {Type = BxlTokenType.Finish});
			_result[0].Next = _result[1];
			_result[_result.Count - 1].Prev = _result[_result.Count - 2];
		}


		private void ProcessLiteral() {
//////////////////////////////////////////////////////////////////////////////////////
			// LITERAL SUPPORT
			// //////////////////////////////////////////////////////////////////////////////////

			//try append literal
			if (_inliteral) {
				//NOTE: now any symbol not processed above can be in literal
				//if (currentType.CanBeInLiteral()) {
				_endidx = _idx - 1;
				_buffer[_bufferidx++] = _c;
				return;
				//}
				//throw new BxlParserException("illegal symbol in literal " + filename + ":" + line + ":" + col, new BxlLexInfo(filename, line, col));
			}

			//if no any mode and letter symbol - try start literal
			if (!_inliteral && _currentType.CanBeStartOfLiteral()) {
				_wasdelimiter = false;
				_inliteral = true;
				_bufferidx = 0;
				_buffer[_bufferidx++] = _c;
				_startcol = _col;
				_startidx = _idx - 1;
				_endidx = _idx - 1;
				_startline = _line;
			}
		}

		private bool ProcessDelimiter() {
			/////////////////////////////////////////////////////////////////////
			// SPECIAL SYMBOLS SUPPORT
			// /////////////////////////////////////////////////////////////////

			if (_currentType.IsSpecial()) {
				if (_currentType == BxlTokenType.Colon) {
					if (_inliteral && !_wasnsstart && Read(1) == ":") {
						_endidx = _idx - 1;
						_buffer[_bufferidx++] = ':';
						_wasnsstart = true;
						return true;
					}
					if (_wasnsstart) {
						if (Read(1) == ":") {
							throw new BxlException("illegal tripple namespace delimiter", new LexInfo(_filename, _line, _col));
						}
						_endidx = _idx - 1;
						_buffer[_bufferidx++] = ':';
						_wasnsstart = false;
						return true;
					}
				}
				if (_inliteral) {
					Appendliteral();
				}
				if (_currentType == BxlTokenType.Colon) {
					_wascolon = true;
					_wasdelimiter = true;
					return true;
				}
				if (_currentType == BxlTokenType.Comma) {
					_wasdelimiter = true;
					return true;
				}
				if (_currentType == BxlTokenType.Assign) {
					_wasdelimiter = true;
					if (_last.Type != BxlTokenType.Literal && _last.Type != BxlTokenType.String) {
						throw new BxlException("cannot assign to " + _last.Type, new LexInfo(_filename, _line, _col));
					}
					_last.IsAssigner = true;
					return true;
				}


				if (_wascolon) {
					throw new BxlException("was colon", new LexInfo(_filename, _line, _col));
				}
				Append(_currentType, _c.ToString(CultureInfo.InvariantCulture));
				return true;
			}

			return false;
		}

		private bool ProcessString() {
/////////////////////////////////////////////////////////////////////
			// STRING SUPPORT (we must process string delimiters)
			// suppose that escapes (including quots will be already processed
			// /////////////////////////////////////////////////////////////////


			//process symbols in string which are collected in buffer
			if (_instring && !(BxlTokenType.Quot == _currentType || BxlTokenType.Apos == _currentType)) {
				//при нахождении в строке и с простыми символами все просто
				_buffer[_bufferidx++] = _c;
				return true;
			}
			//if quot is another than current string quot all is simple too
			if (_instring && (BxlTokenType.Quot == _currentType || BxlTokenType.Apos == _currentType) &&
			    _c.ToString(CultureInfo.InvariantCulture) != _stringstart) {
				//при нахождении в строке и с простыми символами все просто
				_buffer[_bufferidx++] = _c;
				return true;
			}
			//if string is not tripple and it's closer we collapse string
			if (_instring && (BxlTokenType.Quot == _currentType || BxlTokenType.Apos == _currentType) &&
			    _c.ToString(CultureInfo.InvariantCulture) == _stringstart &&
			    !_tripplestring) {
				Appendstring();
				return true;
			}
			//close tripple string
			if (_instring && (BxlTokenType.Quot == _currentType) && _tripplestring && Read(2) == "\"\"") {
				_skips++;
				_skips++;
				Appendstring();
				return true;
			}
			// not processed are usual chars
			if (_instring && (BxlTokenType.Quot == _currentType || BxlTokenType.Apos == _currentType)) {
				_buffer[_bufferidx++] = _c;
				return true;
			}

			//not in strings quots treats as open string constructs
			if (!_instring && (BxlTokenType.Quot == _currentType || BxlTokenType.Apos == _currentType)) {
				if (_inliteral) {
					throw new BxlException("illegal character in literal " + _filename + ":" + _line + ":" + _col,
					                       new LexInfo(_filename, _line, _col));
				}
				if (!_wasdelimiter && !_wascolon && _last.Type != BxlTokenType.Start && _last.Type != BxlTokenType.Level &&
				    _last.Type != BxlTokenType.NewLine) {
					throw new BxlException("string must be precedet with delimeter " + _filename + ":" + _line + ":" + _col,
					                       new LexInfo(_filename, _line, _col));
				}
				_instring = true;
				_startcol = _col;
				_startline = _line;
				_startidx = _idx - 1;
				_stringstart = _c.ToString(CultureInfo.InvariantCulture);
				_bufferidx = 0;
				//try to check tripple)
				if (BxlTokenType.Quot == _currentType && Read(2) == "\"\"") {
					_tripplestring = true;
					_skips++;
					_skips++;
				}
				_wasdelimiter = false;
				return true;
			}
			return false;
		}

		private bool ProcessExpression() {
			/////////////////////////////////////////////////////////////////////
			// EXPRESSION SUPPORT
			// /////////////////////////////////////////////////////////////////

			if (_inliteral && (BxlTokenType.OpenExpr == _currentType || BxlTokenType.CloseExpr == _currentType)) {
				_buffer[_bufferidx++] = _c;
				return true;
			}

			if (_inexpression && BxlTokenType.OpenExpr == _currentType) {
				_buffer[_bufferidx++] = _c;
				_exprnest++;
				return true;
			}

			if (_inexpression && BxlTokenType.CloseExpr == _currentType) {
				_buffer[_bufferidx++] = _c;
				_exprnest--;
				if (_exprnest == 0) {
					Appendexpression();
				}
				return true;
			}

			if (_inexpression) {
				_buffer[_bufferidx++] = _c;
				return true;
			}

			if ((!_instring && !_inexpression && !_inliteral) && BxlTokenType.OpenExpr == _currentType) {
				_inexpression = true;
				_wasdelimiter = false;
				_bufferidx = 0;
				_buffer[_bufferidx++] = _c;
				_startidx = _idx;
				_startcol = _col;
				_startline = _line;
				_exprnest ++;
				return true;
			}
			return false;
		}

		private bool ProcessEscape() {
			//////////////////////////////////////////////////////////////////////
			// ESCAPING SUPPORT - in string - reescapes symbols, in end of file
			// or in not string it's BxlParserException
			// /////////////////////////////////////////////////////////////////
			if (BxlTokenType.Escape == _currentType) {
				if (_idx == _input.Length) {
					throw new BxlException("escape at end of file", new LexInfo(_filename, _line, _col));
				}
				if (!(_instring || _inexpression)) {
					throw new BxlException("escape not in string " + _filename + ":" + _line + ":" + _col,
					                       new LexInfo(_filename, _line, _col));
				}
				_skips++;
				var next = _input[_idx];
				if (next == 'r') {
					_buffer[_bufferidx++] = '\r';
				}
				else if (next == 'n') {
					_buffer[_bufferidx++] = '\n';
				}
				else if (next == 't') {
					_buffer[_bufferidx++] = '\t';
				}
				else {
					_buffer[_bufferidx++] = next;
				}
				return true;
			}
			return false;
		}

		private bool ProcessWhiteSpace() {
			//////////////////////////////////////////////////////////////////////
			// White space handling
			// in stirng we keep, in start of line we collect a level,
			// in other cases we leave one SP ws,
			// в начале триммируем
			// //////////////////////////////////////////////////////////////////
			if (BxlTokenType.WhiteSpace == _currentType) {
				if (_instring || _inexpression) {
//in strings all is very simple
					_buffer[_bufferidx++] = _c;
					return true;
				}
				if (_inliteral) {
					Appendliteral();
				}
				else if (_last.Type == BxlTokenType.NewLine || _last.Type == BxlTokenType.Start) {
// если только что был перенос строки то пробелы - это уровень строки
					var tc = 0;
					var wsc = 0;
					_skips--;
					for (var i = _idx - 1; i < _input.Length; i++) {
						_skips++;
						var nc = _input[i]; // берем символ
						if (nc == ' ') {
							wsc++;
						}
						else if (nc == '\t') {
							tc++;
						}
						else {
							_skips--;
							break;
						}
					}
					var lev = wsc/4 + tc;
					var token = new BxlToken
						{
							Type = BxlTokenType.Level,
							Level = lev,
							LexInfo =
								new LexInfo {CharIndex = _idx, Column = _col, File = _filename, Length = wsc + tc, Line = _line}
						};
					RegisterToken(token);

					_last.Number = lev;
					_last.LexInfo.Length = tc + wsc;
					return true;
				}

				_wasdelimiter = true;
				return true;
				//append(BxlTokenType.WhiteSpace, sp);
			}
			return false;
		}

		private bool InitIteration(char c) {
			_c = c;
			_col++;
			_idx++;
			//next checks if it must be skipped
			if (_skips > 0) {
				_skips--;
				return true;
			}
			return false;
		}

		private bool ProcessNewLine() {
			if (BxlTokenType.NewLine == _currentType) {
				var newline = _line; // заготовка на изменение строки лексической информации
				var val = _c.ToString(CultureInfo.InvariantCulture); //значение переноса строки (промежуточный буфер)
				newline++; //в любом случае переводим строку
				for (var nlidx = _idx; nlidx < _input.Length; nlidx++) {
					//смотрим символы вперед на предмет переносов
					var nc = _input[nlidx]; // берем символ
					var nt = GetSymbolType(nc); // получаем тип
					if (BxlTokenType.NewLine == nt) {
// и если продолжается перенос
						_skips++; //помечаем его к пропуску
						val += nc; //заносим в буфер
						if (_c == '\r' && nc != '\n') {
							newline++; //если это не \n в двух-символьных переносах, добавляем строчку
						}
						else if (_c == '\n' && nc == '\n') {
							newline++;
						}
						continue;
					}
					break;
				}
				if (_instring || _inexpression) {
					//если мы были в строке, то просто наращиваем буфер строки для трипплеров 
					if (_tripplestring || _inexpression) {
						foreach (var c in val) {
							_buffer[_bufferidx++] = c;
						}
					}
					else {
						//или ошибку для обычных
						throw new BxlException("illegal newline in regular string at " + _filename + ":" + _line + ":" + _col,
						                       new LexInfo(_filename, _line, _col));
					}
				}
				else {
					if (_inliteral) {
						Appendliteral();
					}


					//иначе формируем токен переноса строки

					if (_last.Type != BxlTokenType.Start && _last.Type != BxlTokenType.NewLine) {
						Append(BxlTokenType.NewLine, val);
					}
				}
				_line = newline; //выставляем новое значение для лексера
				_col = 0;
				if (!(_instring && _tripplestring) && !_inexpression) {
					//	wascolon = false;
					_wasdelimiter = false;
				}
				return true;
			}
			return false;
		}

		private bool ProcessComment() {
			if (_incomment) {
				if (BxlTokenType.NewLine == _currentType) {
					_incomment = false;
					//it it's new line we must  finish comment and process it as usual newline further
				}
				else {
					return true;
				}
				//any symbols except newline is skipped
			}
			else // on another hand we check if it's comment start
				if (BxlTokenType.Comment == _currentType && !_instring && !_inexpression) {
					// comments was checked, the only variant that it's not comment is that it's int string
					_incomment = true;
					return true;
				}
			return false;
		}

		private string Read(int i) {
			var r = "";
			for (var c = _idx; c < _idx + i && c < _input.Length; c++) {
				r += _input[c];
			}
			return r;
		}

		private void Appendexpression(bool notclosed = false, bool ispart = false) {
			var token = new BxlToken
				{
					Type = BxlTokenType.Expression,
					Value = new string(_buffer, 0, _bufferidx),
					LexInfo = new LexInfo
						{
							CharIndex = _startidx,
							File = _filename,
							Column = _startcol,
							Length = _idx - _startcol + 1,
							Line = _startline
						}
				};

			if (notclosed) {
				token.Type = BxlTokenType.ExpressionStart;
				if (ispart) {
					token.Type = BxlTokenType.ExpressionPart;
				}
				token.NestLevel = _exprnest;
				//token.LexInfo.Length--;
			}
			if (HasOpenedExpressionAtStart && !OpenedStartIsClosed && !notclosed) {
				token.Type = BxlTokenType.ExpressionEnd;
				token.LexInfo.CharIndex = 0;
				token.LexInfo.Length = _idx;
				OpenedStartIsClosed = true;
			}
			_bufferidx = 0;
			RegisterToken(token);
		}

		private void Appendliteral() {
			var token = new BxlToken
				{
					Type = BxlTokenType.Literal,
					Value = new string(_buffer, 0, _bufferidx),
					LexInfo = new LexInfo
						{
							CharIndex = _startidx,
							File = _filename,
							Column = _startcol,
							Length = _endidx - _startidx + 1,
							Line = _startline
						}
				};
			_bufferidx = 0;
			RegisterToken(token);
		}

		private void Appendstring(bool notclosed = false, bool ispart = false) {
			var token = new BxlToken
				{
					Type = BxlTokenType.String,
					Value = new string(_buffer, 0, _bufferidx),
					LexInfo = new LexInfo
						{
							CharIndex = _startidx,
							File = _filename,
							Column = _startcol,
							Length = _idx - _startcol + 1,
							Line = _startline
						}
				};
			if (_tripplestring) {
				token.LexInfo.Length += 2;
			}
			if (notclosed) {
				token.Type = BxlTokenType.StringStart;
				if (ispart) {
					token.Type = BxlTokenType.StringPart;
				}
				token.LexInfo.Length -= 2;
			}
			if (HasOpenedStringAtStart && !OpenedStartIsClosed && !notclosed) {
				token.Type = BxlTokenType.StringEnd;
				token.LexInfo.CharIndex = 0;
				token.LexInfo.Length = _idx + 2;
				OpenedStartIsClosed = true;
			}
			_bufferidx = 0;
			RegisterToken(token);
		}

		private void Append(BxlTokenType t, string v) {
			var token = new BxlToken
				{
					Type = t,
					Value = v,
					LexInfo = new LexInfo {CharIndex = _idx, Column = _col, File = _filename, Length = 1, Line = _line}
				};
			RegisterToken(token);
		}

		private void RegisterToken(BxlToken token) {
			if (_wascolon) {
				token.ColonPrefixed = true;
				_wascolon = false;
			}
			if (null != _last) {
				_last.Next = token;
			}
			_result.Add(token);
			_last = token;
			_inexpression = false;
			_inliteral = false;
			_incomment = false;
			_instring = false;
			_tripplestring = false;
			_startidx = 0;
			_startcol = 0;
			_startline = 0;
			//	buffer = new StringBuilder();
		}

		private void ResetTokenizer() {
			_instring = false;
			_stringstart = "";
			_startline = 0;
			_startcol = 0;
			_startidx = 0;
			_tripplestring = false;
			_inliteral = false;
			_last = null;
			_incomment = false;
			_line = 1;
			_col = 0;
			var size = Math.Min(BufferSize, _input.Length);
			_buffer = new char[size];
			_bufferidx = 0;
			_result = new List<BxlToken>(_input.Length/10) {new BxlToken(BxlTokenType.Start)};

			_last = _result[0];
			_idx = 0;
			_skips = 0;
			_inexpression = false;
			_exprnest = 0;
			HasOpenedAttributeAtEnd = false;
			HasOpenedValueAtEnd = false;
			HasOpenedStringAtEnd = false;
			HasOpenedExpressionAtEnd = false;
			ExpressionNestLevelAtEnd = 0;
			if (HasOpenedAttributeAtStart || HasOpenedValueAtStart) {
				AllowNonClosedStringsAndExpressions = true;
			}
			if (HasOpenedExpressionAtStart) {
				_inexpression = true;
				_exprnest = ExpressionNestLevelAtStart;
				_startcol = 1;
			}
			else if (HasOpenedStringAtStart) {
				_instring = true;
				_tripplestring = true;
				_stringstart = "\"";
				_startcol = 1;
			}
		}

		private static BxlTokenType GetSymbolType(char c) {
			switch (c) {
				case '\r':
					goto case '\n';
				case '\n':
					return BxlTokenType.NewLine;
				case '\t':
					goto case ' ';
				case ' ':
					return BxlTokenType.WhiteSpace;
				case '#':
					return BxlTokenType.Comment;
				case '"':
					return BxlTokenType.Quot;
				case '\'':
					return BxlTokenType.Apos;
				case '\\':
					return BxlTokenType.Escape;
				case '=':
					return BxlTokenType.Assign;
				case '.':
					return BxlTokenType.NonLetter;
				case ':':
					return BxlTokenType.Colon;
				case ',':
					return BxlTokenType.Comma;
				case '1':
					goto case '0';
				case '2':
					goto case '0';
				case '3':
					goto case '0';
				case '4':
					goto case '0';
				case '5':
					goto case '0';
				case '6':
					goto case '0';
				case '7':
					goto case '0';
				case '8':
					goto case '0';
				case '9':
					goto case '0';
				case '0':
					return BxlTokenType.NonLetter;
				case '!':
					goto case '+';
				case '~':
					goto case '+';
				case '%':
					goto case '+';
				case '?':
					goto case '+';
				case '-':
					//return  BxlTokenType.Minus;
					goto case '+';
				case '*':
					goto case '+';
				case '/':
					goto case '+';
				case '$':
					goto case '+';
				case '@':
					goto default;
				case ';':
					goto case '+';
				case '<':
					goto case '+';
				case '>':
					goto case '+';
				case '&':
					goto case '+';
				case '^':
					goto case '+';
				case '|':
					goto case '+';
				case '(':
					return BxlTokenType.OpenExpr;
				case ')':
					return BxlTokenType.CloseExpr;
				case '{':
					goto case '+';
				case '}':
					goto case '+';
				case '[':
					goto case '+';
				case ']':
					goto case '+';
				case '`':
					goto case '+';
				case '+':
					return BxlTokenType.NonLetter;
				default:
					return BxlTokenType.Letter;
			}
		}

		/// <summary>
		/// 	Size of char buffer, is max size for strings and literals,
		/// </summary>
		public int BufferSize = 2048;

		/// <summary>
		/// 	buffer for string and literal values
		/// </summary>
		private char[] _buffer;

		/// <summary>
		/// 	current char index in buffer
		/// </summary>
		private int _bufferidx;

		/// <summary>
		/// 	current char in source
		/// </summary>
		private char _c;

		/// <summary>
		/// 	current column in source
		/// </summary>
		private int _col;

		/// <summary>
		/// 	current type of char token
		/// </summary>
		private BxlTokenType _currentType;

		/// <summary>
		/// 	flag to prevent elements extraction (debugproppose)
		/// </summary>
		private bool _donotextractelements;

		/// <summary>
		/// 	current end index of literal in buffer
		/// </summary>
		private int _endidx;

		/// <summary>
		/// 	current expression nest level
		/// </summary>
		private int _exprnest;

		/// <summary>
		/// 	source file name
		/// </summary>
		private string _filename;

		/// <summary>
		/// 	index of current char in source
		/// </summary>
		private int _idx;

		/// <summary>
		/// 	flag - tokenizer is now in comment
		/// </summary>
		private bool _incomment;

		/// <summary>
		/// 	flag - tokenizer is now in expression
		/// </summary>
		private bool _inexpression;

		/// <summary>
		/// 	flag - tokenizer is now in literal
		/// </summary>
		private bool _inliteral;

		/// <summary>
		/// 	source code
		/// </summary>
		private string _input;

		/// <summary>
		/// 	flog - tokenizer is now in string
		/// </summary>
		private bool _instring;

		/// <summary>
		/// 	last parced token
		/// </summary>
		private BxlToken _last;

		/// <summary>
		/// 	current line in source
		/// </summary>
		private int _line;

		/// <summary>
		/// 	tokenizer result as list of tokens
		/// </summary>
		private List<BxlToken> _result;

		/// <summary>
		/// 	count of following chars to skip in source
		/// </summary>
		private int _skips;

		/// <summary>
		/// 	column of start of string/literal
		/// </summary>
		private int _startcol;

		/// <summary>
		/// 	start char index of string/literal
		/// </summary>
		private int _startidx;

		/// <summary>
		/// 	start line of string/literal
		/// </summary>
		private int _startline;

		/// <summary>
		/// 	start of inner string (without quotes)
		/// </summary>
		private string _stringstart;

		/// <summary>
		/// 	flag - tripple quoted string is parsed
		/// </summary>
		private bool _tripplestring;

		/// <summary>
		/// 	flag - nonresolved colon symbol parsed before
		/// </summary>
		private bool _wascolon;

		/// <summary>
		/// 	flag - an delimiter symbol parsed before
		/// </summary>
		private bool _wasdelimiter;

		/// <summary>
		/// 	flaf - namespace prefix was parsed
		/// </summary>
		private bool _wasnsstart;
	}
}