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
// PROJECT ORIGIN: Qorpent.Dsl/LogicalExpressionParser.cs
#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.LogicalExpressions {
	/// <summary>
	/// 	simple expression to node tree parser for logical expressions
	/// </summary>
	public class LogicalExpressionParser {
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
			if (LETokenType.Literal == token.Type) {
				current.Children.Add(new LiteralNode {Literal = token.Value, Negative = token.Negation});
				return;
			}
            if (LETokenType.Number == token.Type)
            {
                current.Children.Add(new LiteralNode { Literal = token.Value, IsNumber=true, Negative = token.Negation });
                return;
            }
			if (LETokenType.And == token.Type || LETokenType.Block == token.Type) {
				var conj = new ConjunctionNode {Negative = token.Negation};
				current.Children.Add(conj);
				foreach (var child in token.Children) {
					processTokenToExpression(conj, child);
				}
				return;
			}
			if (LETokenType.Or == token.Type) {
				var disj = new DisjunctionNode {Negative = token.Negation};
				current.Children.Add(disj);
				foreach (var child in token.Children) {
					processTokenToExpression(disj, child);
				}
				return;
			}
			if (0!=(token.Type &  LETokenType.Compare)) {
				var fst = token.Children[0];
				var sec = token.Children[1];
				LogicalExpressionNode n;
			    if (token.Type == LETokenType.Regex) {
			        n = new RegexTestNode {
			            First = fst.Value,
			            Second = sec.Value,
			            FirstIsLiteral = fst.Type == LETokenType.Literal,
			            SecondIsLiteral = sec.Type == LETokenType.Literal
			        };
			    }else if (fst.Type == LETokenType.Number) {
                    var eq = new EqualValueNode();
                    n = eq;
                    eq.Literal = sec.Value;
                    eq.Value = fst.Value;
			        eq.IsNumber = true;
			    }
                else if (sec.Type == LETokenType.Number) {
                    var eq = new EqualValueNode();
                    n = eq;
                    eq.Literal = fst.Value;
                    eq.Value = sec.Value;
                    eq.IsNumber = true;
                }
				else if (sec.Type == LETokenType.Literal) {
					var eq = new EqualNode();
					n = eq;
				    if (fst.Type == LETokenType.String) {
				        var eq2= new EqualValueNode();
                        eq2.Literal = sec.Value;
				        eq2.Value = fst.Value;
				        n = eq2;
				    }
				    else {
				        eq.FirstLiteral = fst.Value;
				        eq.SecondLiteral = sec.Value;
				    }
				}
				else {
					var eq = new EqualValueNode();
					n = eq;
					eq.Literal = fst.Value;
					eq.Value = sec.Value;
				    
				}
				n.Negative = token.Negation;
			    n.Operation = token.Type;
			    if (token.Type != LETokenType.Neq && token.Type != LETokenType.Eq) {
			        n.IsNumber = true;
			    }
				if (LETokenType.Neq == token.Type) {
					n.Negative = !n.Negative;
				}
				current.Children.Add(n);
			}
		}

		private List<Token> Resolveblocks(List<Token> tokens) {
			//remove not sign
			foreach (var t in tokens.ToList()) {
				if (LETokenType.Not == t.Type) {
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
				if (0!=(t.Type & LETokenType.Compare )) {
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
					if (t.Type == LETokenType.OpenBlock) {
						currentopen = t;
						content.Clear();
					}
					else if (t.Type == LETokenType.CloseBlock) {
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
					currentopen.Type = LETokenType.Block;
					ResolveAndAndOr(currentopen.Children);
				}
			}

			ResolveAndAndOr(tokens);

			return tokens;
		}

		private void ResolveAndAndOr(IList<Token> children) {
			var currentoperation = LETokenType.None;
			var arguments = new List<Token>();
			foreach (var a in children.ToArray()) {
				children.Remove(a);
				if (a.Type == LETokenType.And || a.Type == LETokenType.Or) {
					if (a.Type == currentoperation) {
						continue;
					}
					if (LETokenType.None == currentoperation) {
						currentoperation = a.Type;
						continue;
					}
					ProcessBinary(children, currentoperation, arguments);
					currentoperation = a.Type;
					continue;
				}

				arguments.Add(a);
			}
			if (currentoperation == LETokenType.None) {
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

		private static void ProcessBinary(IList<Token> children, LETokenType currentoperation, List<Token> arguments) {
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
		    _skipnext = 0;
		    _idx = -1;
			var idx = 0;
			foreach (var c in expression) {
			    _idx++;
			    if (_skipnext>0) {
			        _skipnext--;
                    continue;
			    }
			    var next = idx<expression.Length-1 ? expression[idx+1]:'\0';
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
						if (State.AfterBinaryOperator != _state && State.None!=_state) {
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
                        if (next == '&')
                        {
                            _skipnext = 1;
                        }
                        _tokenlist.Add(new Token {Type = LETokenType.And});
						_state = State.AfterAndOrOr;
						break;
					case '|':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
                       
                        if (State.AfterLiteral != _state && State.AfterString != _state && State.None != _state) {
							throw new Exception("| is at wrong place " + idx);
						}
                        if (next == '|')
                        {
                            _skipnext = 1;
                        }
                        _tokenlist.Add(new Token {Type = LETokenType.Or});
						_state = State.AfterAndOrOr;
						break;
                    case '~':
                        if (State.InLiteral == _state)
                        {
                            Closeliteral();
                        }

                        if (State.AfterLiteral != _state && State.AfterString != _state && State.None != _state && State.InNeq!=_state)
                        {
                            throw new Exception("~ is at wrong place " + idx);
                        }
                        if (next == '~')
                        {
                            _skipnext = 1;
                        }
                        _tokenlist.Add(new Token { Type = LETokenType.Regex,Negation = _state==State.InNeq});
                        
                        _state = State.AfterBinaryOperator;
                        break;

                    case '=':
                        if (State.InLiteral == _state)
                        {
                            Closeliteral();
                        }
				        
                        if (State.AfterLiteral == _state || State.AfterString==_state) {
							_tokenlist.Add(new Token {Type = LETokenType.Eq});
							_state = State.AfterBinaryOperator;
                            if (next == '=')
                            {
                               _skipnext = 1;
                            }
                            break;
						}
						if (State.InNeq == _state) {
							_tokenlist.Add(new Token {Type = LETokenType.Neq});
							_state = State.AfterBinaryOperator;
							break;
						}
						throw new Exception("= is at wrong place " + idx);
                    case '>':
                        if (State.InLiteral == _state)
                        {
                            Closeliteral();
                        }
                        if (State.AfterLiteral == _state) {
                            next = expression[_idx + 1];
                            if (next == '=') {
                                _tokenlist.Add(new Token { Type = LETokenType.GreaterOrEq });
                                _skipnext = 1;
                            }
                            else {
                                _tokenlist.Add(new Token { Type = LETokenType.Greater });
                            }
                            _state = State.AfterBinaryOperator;
                            break;
                        }
                        
                        throw new Exception("> is at wrong place " + idx);
                    case '<':
                        if (State.InLiteral == _state)
                        {
                            Closeliteral();
                        }
                        if (State.AfterLiteral == _state)
                        {
                            next = expression[_idx + 1];
                            if (next == '=')
                            {
                                _tokenlist.Add(new Token { Type = LETokenType.LowerOrEq });
                                _skipnext = 1;
                            }
                            else
                            {
                                _tokenlist.Add(new Token { Type = LETokenType.Lower });
                            }
                            _state = State.AfterBinaryOperator;
                            break;
                        }

                        throw new Exception("> is at wrong place " + idx);
					case '!':
						if (State.InLiteral == _state) {
							Closeliteral();
						}
						if (State.AfterLiteral == _state) {
							_state = State.InNeq;
							break;
						}
						if (State.None == _state || State.AfterAndOrOr == _state) {
							_tokenlist.Add(new Token {Type = LETokenType.Not});
							break;
						}
						throw new Exception("! is at wrong place " + idx);
					default:
						if (State.None == _state || State.AfterNot == _state || State.AfterAndOrOr == _state ||
						    State.AfterBinaryOperator == _state) {
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
			_tokenlist.Add(new Token {Type = LETokenType.CloseBlock});
			_closecount++;
			_state = State.None;
		}

		private void Openblock() {
			_tokenlist.Add(new Token {Type = LETokenType.OpenBlock});
			_opencount++;
			_state = State.None;
		}

		private void Closeliteral() {
		    if (_currentvalue == "0" || 0 != _currentvalue.ToDecimal()) {
                _tokenlist.Add(new Token { Type = LETokenType.Number, Value = _currentvalue });
		    }
		    else {
                _tokenlist.Add(new Token { Type = LETokenType.Literal, Value = _currentvalue });
		    }
			
			_currentvalue = "";
			_state = State.AfterLiteral;
		}

		private void Closestring() {
			_tokenlist.Add(new Token {Type = LETokenType.String, Value = _currentvalue});
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
			AfterBinaryOperator,
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
			public LETokenType Type { get; set; }

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
			/// 	Returns a <see cref="T:System.Str" /> that represents the current <see cref="T:System.Object" />.
			/// </summary>
			/// <returns> A <see cref="T:System.Str" /> that represents the current <see cref="T:System.Object" /> . </returns>
			/// <filterpriority>2</filterpriority>
			public override string ToString() {
				if (LETokenType.Literal == Type) {
					return "Lit:" + Value;
				}
				if (LETokenType.String == Type) {
					return "Str:" + Value;
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
	    private int _skipnext;
	    private int _idx;
	}
}