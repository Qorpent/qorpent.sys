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
// Original file : LogicalExpressionParser.cs
// Project: Qorpent.Dsl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Dsl.LogicalExpressions {
	/// <summary>
	/// 	simple expression to node tree parser for logical expressions
	/// </summary>
	public class LogicalExpressionParser {
		/// <summary>
		/// 	tokens, available in logical expression
		/// </summary>
		public enum TokenType {
			/// <summary>
			/// 	undefined
			/// </summary>
			None,

			/// <summary>
			/// 	open brace (block of nodes)
			/// </summary>
			OpenBlock,

			/// <summary>
			/// 	close breace (end of block)
			/// </summary>
			CloseBlock,

			/// <summary>
			/// 	term reference
			/// </summary>
			Literal,

			/// <summary>
			/// 	Equal operator ==
			/// </summary>
			Eq,

			/// <summary>
			/// 	Not equal operator !=
			/// </summary>
			Neq,

			/// <summary>
			/// 	Negate operator ![LITERAL|BLOCK]
			/// </summary>
			Not,

			/// <summary>
			/// 	And operator - &amp;
			/// </summary>
			And,

			/// <summary>
			/// 	Or operator - |
			/// </summary>
			Or,

			/// <summary>
			/// 	String const "..."
			/// </summary>
			String,

			/// <summary>
			/// 	Block of nodes
			/// </summary>
			Block,
		}


		/// <summary>
		/// 	parses source str into node tree
		/// </summary>
		/// <param name="expression"> </param>
		/// <returns> </returns>
		public LogicalExpressionNode Parse(string expression) {
			var tokens = Tokenize(expression);
			tokens = Resolveblocks(tokens);
			var node = Buildexpression(tokens);
			return node;
		}

		private LogicalExpressionNode Buildexpression(IEnumerable<Token> tokens) {
			var result = new ConjunctionNode();
			foreach (var token in tokens) {
				processTokenToExpression(result, token);
			}
			return result;
		}

		private void processTokenToExpression(LogicalExpressionNode current, Token token) {
			if (TokenType.Literal == token.Type) {
				current.Children.Add(new LiteralNode {Literal = token.Value, Negative = token.Negation});
				return;
			}
			if (TokenType.And == token.Type || TokenType.Block == token.Type) {
				var conj = new ConjunctionNode {Negative = token.Negation};
				current.Children.Add(conj);
				foreach (var child in token.Children) {
					processTokenToExpression(conj, child);
				}
				return;
			}
			if (TokenType.Or == token.Type) {
				var disj = new DisjunctionNode {Negative = token.Negation};
				current.Children.Add(disj);
				foreach (var child in token.Children) {
					processTokenToExpression(disj, child);
				}
				return;
			}
			if (TokenType.Eq == token.Type || TokenType.Neq == token.Type) {
				var fst = token.Children[0];
				var sec = token.Children[1];
				LogicalExpressionNode n;
				if (sec.Type == TokenType.Literal) {
					var eq = new EqualNode();
					n = eq;
					eq.FirstLiteral = fst.Value;
					eq.SecondLiteral = sec.Value;
				}
				else {
					var eq = new EqualValueNode();
					n = eq;
					eq.Literal = fst.Value;
					eq.Value = sec.Value;
				}
				n.Negative = token.Negation;
				if (TokenType.Neq == token.Type) {
					n.Negative = !n.Negative;
				}
				current.Children.Add(n);
			}
		}

		private List<Token> Resolveblocks(List<Token> tokens) {
			//remove not sign
			foreach (var t in tokens.ToList()) {
				if (TokenType.Not == t.Type) {
					if (t.Next == null) {
						throw new Exception("NOT cannot be last operator in expression");
					}
					t.Next.Negation = true;
					t.Remove();
					tokens.Remove(t);
				}
			}
			//resolves eq and neq
			foreach (var t in tokens.ToList()) {
				if (TokenType.Neq == t.Type || TokenType.Eq == t.Type) {
					if (t.Next == null || t.Prev == null) {
						throw new Exception("NEQ/EQ cannot be non-binary");
					}
					var fst = t.Prev;
					var sec = t.Next;
					fst.Remove();
					sec.Remove();
					tokens.Remove(fst);
					tokens.Remove(sec);
					fst.Parent = t;
					sec.Parent = t;
					t.Children.Add(fst);
					t.Children.Add(sec);
				}
			}

			//resolves blocks
			var proceed = true;
			while (proceed) {
				proceed = false;
				IList<Token> content = new List<Token>();
				Token currentopen = null;
				Token closer = null;
				foreach (var t in tokens) {
					if (t.Type == TokenType.OpenBlock) {
						currentopen = t;
						content.Clear();
					}
					else if (t.Type == TokenType.CloseBlock) {
						if (null == currentopen) {
							throw new Exception("invalid brace structure");
						}
						closer = t;
						proceed = true;
						break;
					}
					else {
						content.Add(t);
					}
				}
				if (proceed) {
					if (0 == content.Count) {
						throw new Exception("empty group");
					}

					closer.Remove();
					tokens.Remove(closer);
					foreach (var token in content) {
						token.Parent = currentopen;
						currentopen.Children.Add(token);
						tokens.Remove(token);
					}
					var last = content.Last();
					var next = last.Next;
					last.Next = null;
					currentopen.Next = next;
					currentopen.Type = TokenType.Block;
					ResolveAndAndOr(currentopen.Children);
				}
			}

			ResolveAndAndOr(tokens);

			return tokens;
		}

		private void ResolveAndAndOr(IList<Token> children) {
			var currentoperation = TokenType.None;
			var arguments = new List<Token>();
			foreach (var a in children.ToArray()) {
				children.Remove(a);
				if (a.Type == TokenType.And || a.Type == TokenType.Or) {
					if (a.Type == currentoperation) {
						continue;
					}
					if (TokenType.None == currentoperation) {
						currentoperation = a.Type;
						continue;
					}
					ProcessBinary(children, currentoperation, arguments);
					currentoperation = a.Type;
					continue;
				}

				arguments.Add(a);
			}
			if (currentoperation == TokenType.None) {
				if (arguments.Count == 1) {
					children.Insert(0, arguments[0]);
				}
				else {
					throw new Exception("invalid structure");
				}
				return;
			}
			ProcessBinary(children, currentoperation, arguments);
		}

		private static void ProcessBinary(IList<Token> children, TokenType currentoperation, List<Token> arguments) {
			if (2 > arguments.Count) {
				throw new Exception("insuficient parameters count for binary operation");
			}
			var newtoken = new Token {Type = currentoperation};
			var p = arguments.First().Parent;
			newtoken.Parent = p;
			children.Insert(0, newtoken);
			foreach (var arg in arguments.ToArray()) {
				arg.Remove();
				children.Remove(arg);
				arguments.Remove(arg);
				arg.Parent = newtoken;
				newtoken.Children.Add(arg);
			}
			arguments.Insert(0, newtoken);
		}

		private List<Token> Tokenize(string expression) {
			_tokenlist = new List<Token>();
			_state = State.None;
			_currentvalue = "";
			_opencount = 0;
			_closecount = 0;
			var idx = 0;
			foreach (var c in expression) {
				switch (c) {
					case '(':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
						if (State.AfterLiteral == _state || State.AfterString == _state || State.None == _state ||
						    State.AfterNot == _state ||
						    State.AfterAndOrOr == _state) {
							Openblock();
						}
						else {
							throw new Exception("( at " + idx + " is wrong");
						}
						break;
					case ')':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
						if (State.AfterLiteral == _state || State.AfterString == _state || State.None == _state) {
							Closeblock();
						}
						else {
							throw new Exception(") at " + idx + " is wrong");
						}
						break;
					case ' ':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
						if (State.InString == _state) {
							_currentvalue += c;
						}
						if (State.InNeq == _state) {
							throw new Exception("WS is at wrong place " + idx);
						}
						break;
					case '\'':
						goto case '"';
					case '"':
						if (State.InString == _state) {
							Closestring();
							_state = State.AfterString;
							break;
						}
						if (State.AfterEqOrNeq != _state) {
							throw new Exception("QUOT is at wrong place " + idx);
						}
						_state = State.InString;
						break;
					case '&':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
						if (State.AfterLiteral != _state && State.AfterString != _state && State.None != _state) {
							throw new Exception("& is at wrong place " + idx);
						}
						_tokenlist.Add(new Token {Type = TokenType.And});
						_state = State.AfterAndOrOr;
						break;
					case '|':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
						if (State.AfterLiteral != _state && State.AfterString != _state && State.None != _state) {
							throw new Exception("| is at wrong place " + idx);
						}
						_tokenlist.Add(new Token {Type = TokenType.Or});
						_state = State.AfterAndOrOr;
						break;
					case '=':
						if (State.AfterLiteral == _state) {
							_tokenlist.Add(new Token {Type = TokenType.Eq});
							_state = State.AfterEqOrNeq;
							break;
						}
						if (State.InNeq == _state) {
							_tokenlist.Add(new Token {Type = TokenType.Neq});
							_state = State.AfterEqOrNeq;
							break;
						}
						throw new Exception("= is at wrong place " + idx);
					case '!':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
						if (State.AfterLiteral == _state) {
							_state = State.InNeq;
							break;
						}
						if (State.None == _state || State.AfterAndOrOr == _state) {
							_tokenlist.Add(new Token {Type = TokenType.Not});
							break;
						}
						throw new Exception("! is at wrong place " + idx);
					default:
						if (State.None == _state || State.AfterNot == _state || State.AfterAndOrOr == _state ||
						    State.AfterEqOrNeq == _state) {
							_currentvalue = c.ToString(CultureInfo.InvariantCulture);
							_state = State.InLiteral;
							break;
						}
						if (State.InLiteral == _state || State.InString == _state) {
							_currentvalue += c;
							break;
						}
						throw new Exception(c + " is at wrong place " + idx);
				}
				idx++;
			}

			if (_opencount != _closecount) {
				throw new Exception("opened braces :" + _opencount + ", closed braces :" + _closecount);
			}

			if (State.InLiteral == _state) {
				Closeliteral();
			}
			for (var i = 0; i < _tokenlist.Count; i++) {
				if (0 != i) {
					_tokenlist[i].Prev = _tokenlist[i - 1];
				}
				if (_tokenlist.Count - 1 != i) {
					_tokenlist[i].Next = _tokenlist[i + 1];
				}
			}
			return _tokenlist;
		}

		private void Closeblock() {
			_tokenlist.Add(new Token {Type = TokenType.CloseBlock});
			_closecount++;
			_state = State.None;
		}

		private void Openblock() {
			_tokenlist.Add(new Token {Type = TokenType.OpenBlock});
			_opencount++;
			_state = State.None;
		}

		private void Closeliteral() {
			_tokenlist.Add(new Token {Type = TokenType.Literal, Value = _currentvalue});
			_currentvalue = "";
			_state = State.AfterLiteral;
		}

		private void Closestring() {
			_tokenlist.Add(new Token {Type = TokenType.String, Value = _currentvalue});
			_currentvalue = "";
			_state = State.AfterLiteral; // no differences in post processing
		}

		#region Nested type: State

		private enum State {
			None,
			InLiteral,
			InString,
			AfterAndOrOr,
			AfterLiteral,
			InNeq,
			AfterEqOrNeq,
			AfterNot,
			AfterString,
		}

		#endregion

		#region Nested type: Token

		/// <summary>
		/// 	describes token of source str
		/// </summary>
		public class Token {
			/// <summary>
			/// 	creates new
			/// </summary>
			public Token() {
				Children = new List<Token>();
			}

			/// <summary>
			/// 	is NOT setted !
			/// </summary>
			public bool Negation { get; set; }

			/// <summary>
			/// 	tokent type
			/// </summary>
			public TokenType Type { get; set; }

			/// <summary>
			/// 	containing node
			/// </summary>
			public Token Parent { get; set; }

			/// <summary>
			/// 	contained nodes
			/// </summary>
			public IList<Token> Children { get; private set; }

			/// <summary>
			/// 	next sibling node
			/// </summary>
			public Token Next { get; set; }

			/// <summary>
			/// 	previous sibling node
			/// </summary>
			public Token Prev { get; set; }

			/// <summary>
			/// 	value of node
			/// </summary>
			public string Value { get; set; }

			/// <summary>
			/// 	build-bound Remove from tree method
			/// </summary>
			public void Remove() {
				if (null != Next) {
					Next.Prev = Prev;
				}
				if (null != Prev) {
					Prev.Next = Next;
				}
				if (null != Parent) {
					Parent.Children.Remove(this);
				}
			}

			/// <summary>
			/// 	Returns a <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" />.
			/// </summary>
			/// <returns> A <see cref="T:System.String" /> that represents the current <see cref="T:System.Object" /> . </returns>
			/// <filterpriority>2</filterpriority>
			public override string ToString() {
				if (TokenType.Literal == Type) {
					return "Literal:" + Value;
				}
				if (TokenType.String == Type) {
					return "String:" + Value;
				}
				return string.Format("{0}[{1}]", Type, Children.Select(x => x.ToString()).ConcatString());
			}
		}

		#endregion

		private int _closecount;

		private string _currentvalue;
		private int _opencount;
		private State _state;
		private List<Token> _tokenlist;
	}
}