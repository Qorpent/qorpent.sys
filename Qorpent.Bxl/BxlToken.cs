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
// Original file : BxlToken.cs
// Project: Qorpent.Bxl
// 
// ALL MODIFICATIONS MADE TO FILE MUST BE DOCUMENTED IN SVN

#endregion

using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Dsl;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	represents low and high level token of BXL document
	/// </summary>
	public class BxlToken {
		/// <summary>
		/// 	Creates new token of given type
		/// </summary>
		/// <param name="type"> </param>
		public BxlToken(BxlTokenType type) {
			Type = type;
		}

		/// <summary>
		/// 	creates empty token
		/// </summary>
		public BxlToken() {}

		/// <summary>
		/// 	Next token in document flow
		/// </summary>
		public BxlToken Next {
			get { return _next; }
			set {
				if (_next == value) {
					return;
				}
				if (null != _next) {
					_next._prev = null;
				}
				if (null != value) {
					value._prev = this;
				}
				_next = value;
			}
		}


		/// <summary>
		/// 	points to previous token, parsed from file
		/// </summary>
		public BxlToken Prev {
			get { return _prev; }
			set {
				if (null != _prev) {
					_prev._next = null;
				}
				if (null != value) {
					value._next = this;
				}
				_prev = value;
			}
		}

		/// <summary>
		/// 	Collection for child Element's tokens
		/// </summary>
		public IList<BxlToken> Elements {
			get { return _childElements ?? (_childElements = new List<BxlToken>()); }
		}

		/// <summary>
		/// 	Collection for child Attrubute's tokens
		/// </summary>
		public IList<BxlToken> Attributes {
			get { return _childAttributes ?? (_childAttributes = new List<BxlToken>()); }
		}

		/// <summary>
		/// 	Returns next token with some skips
		/// </summary>
		/// <param name="count"> count of skip tokens </param>
		/// <returns> token after skip tokens </returns>
		public BxlToken GetNext(int count) {
			var current = Next;
			var c = count;
			while (current != null && c > 0) {
				c--;
				current = current.Next;
			}
			return current;
		}

		/// <summary>
		/// 	Returns previous token with some skips
		/// </summary>
		/// <param name="count"> count of skip tokens </param>
		/// <returns> token previous to skip tokens </returns>
		public BxlToken GetPrev(int count) {
			var current = Prev;
			var c = count;
			while (current != null && c > 0) {
				c--;
				current = current.Prev;
			}
			return current;
		}

		/// <summary>
		/// 	Returns name, adapted o be XName, performs pseudo chars processing and namespace resolution
		/// </summary>
		/// <param name="namespaces"> dictionary of declared namespaces in context (to resolve prefixes) </param>
		/// <returns> valid XName for element/attribute </returns>
		/// <exception cref="BxlException"></exception>
		public XName GetAdaptedName(IDictionary<string, string> namespaces = null) {
			if (null == _adaptedname) {
				if (Name.Contains("::")) {
					if (Name.EndsWith("::")) {
						throw new BxlException("illegal namespace declaration", lexinfo: LexInfo);
					}
					var nameparts = Name.SmartSplit(false, true, ':');
					if (nameparts.Count == 2) {
						var localname = nameparts[1].ConvertToXNameCompatibleString();
						var prefix = nameparts[0];
						var ns = "";
						if (namespaces != null) {
							if (namespaces.ContainsKey(prefix)) {
								ns = namespaces[prefix];
							}
						}
						if (ns.IsEmpty()) {
							foreach (var pair in QorpentConst.Xml.WellKnownNamespaces) {
								if (pair.Value == prefix) {
									ns = pair.Key;
									if (namespaces != null) {
										namespaces[pair.Value] = pair.Key;
									}
								}
							}
							if (ns.IsEmpty() && namespaces != null) {
								if (namespaces.ContainsKey("IMPLICIT")) {
									var autogen = namespaces["IMPLICIT"] + "X";
									namespaces[prefix] = autogen;
									namespaces["IMPLICIT"] = autogen;
									ns = autogen;
								}
							}
							if (ns.IsEmpty()) {
								throw new BxlException("unknown ns prefix", lexinfo: LexInfo);
							}
						}
						else {}
						_adaptedname = "{" + ns + "}" + localname;
					}
					else {
						_adaptedname = Name.ConvertToXNameCompatibleString();
					}
				}
				else {
					_adaptedname = Name.ConvertToXNameCompatibleString();
				}
			}
			return _adaptedname;
		}


		/// <summary>
		/// 	Return string representation of token + some tokens next
		/// </summary>
		/// <param name="length"> </param>
		/// <returns> </returns>
		public string ToString(int length) {
			var result = Value;
			var c = length;
			var current = Next;
			while (c > 0 && current != null) {
				c--;
				result += current.Value;
				current = current.Next;
			}
			return result;
		}

		/// <summary>
		/// 	Creates standalone copy of token excluding ieratrchy and flow info
		/// </summary>
		/// <returns> </returns>
		public BxlToken Standalone() {
			var copy = new BxlToken
				{
					Type = Type,
					Value = Value,
					LexInfo = LexInfo.Clone(),
					NestLevel = NestLevel,
				};
			return copy;
		}

		/// <summary>
		/// 	Attribute Value for attribute's token
		/// </summary>
		public string AttrValue;

		/// <summary>
		/// 	Flag that colon symbol was in text before this token
		/// </summary>
		public bool ColonPrefixed;

		/// <summary>
		/// 	Element Value for element's token
		/// </summary>
		public BxlToken ElementValue;

		/// <summary>
		/// 	Mark of token to be ignored in process
		/// </summary>
		public bool Ignore;

		/// <summary>
		/// 	Internal low-level token for high-level token
		/// </summary>
		public BxlToken Impl;

		/// <summary>
		/// 	Marks that token is assign pare (attribute candidate)
		/// </summary>
		public bool IsAssigner;

		/// <summary>
		/// 	Left token for assign token
		/// </summary>
		public BxlToken Left;

		/// <summary>
		/// 	Level of tabbing
		/// </summary>
		public int Level;

		/// <summary>
		/// 	Lex info for token
		/// </summary>
		public LexInfo LexInfo;

		/// <summary>
		/// 	Name for named tokens (attributes, elements)
		/// </summary>
		public string Name;

		/// <summary>
		/// 	Level in ierarchy
		/// </summary>
		public int NestLevel;

		/// <summary>
		/// 	Tab counter for level tokens
		/// </summary>
		public decimal Number;

		/// <summary>
		/// 	Parent token up to ierarchy
		/// </summary>
		public BxlToken Parent;

		/// <summary>
		/// 	Right token for assign tokens
		/// </summary>
		public BxlToken Right;

		/// <summary>
		/// 	Wrap type for line-by-line parsed tokens
		/// </summary>
		public BxlTokenType SubType; //for opened constructions

		/// <summary>
		/// 	Char class type of token
		/// </summary>
		public BxlTokenType Type;

		/// <summary>
		/// 	Native Value of token, real Value of element|attribute tokens
		/// </summary>
		public string Value;

		private XName _adaptedname;

		private IList<BxlToken> _childAttributes;
		private IList<BxlToken> _childElements;
		private BxlToken _next;

		private BxlToken _prev;
	}
}