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
// PROJECT ORIGIN: Qorpent.Bxl/BxlTree.cs
#endregion
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Tree-like, XPath navigable representation of BxlToken parse result
	/// </summary>
	public class BxlTree : IXPathNavigable {
		/// <summary>
		/// 	Creates bxltree from given tokens
		/// </summary>
		/// <param name="tokens"> source token list </param>
		public BxlTree(BxlToken[] tokens) {
			IDictionary<int, BxlToken> lastsforlevel = new Dictionary<int, BxlToken>();
			BxlToken currentElement = null;
			foreach (var t in tokens) {
				BxlToken parent;
				if (BxlTokenType.Element == t.Type) {
					var level = t.Level;
					currentElement = t;
					lastsforlevel[level] = t;
					if (level > 0) {
						parent = lastsforlevel[level - 1];
						parent.Elements.Add(t);
						t.Parent = parent;
					}
				}
				else if (BxlTokenType.Attribute == t.Type || BxlTokenType.Literal == t.Type || BxlTokenType.String == t.Type ||
				         BxlTokenType.Expression == t.Type) {
					parent = t.Level > 0 ? lastsforlevel[t.Level - 1] : currentElement;
					if (parent != null) {
						parent.Attributes.Add(t);
						t.Parent = parent;
					}
				}
				else if (BxlTokenType.Value == t.Type) {
					if (currentElement != null) {
						currentElement.ElementValue = t;
						t.Parent = currentElement;
					}
				}
			}
			_tokens = tokens.Where(x => x.Type == BxlTokenType.Element && x.Level == 0).ToArray();
		}

		/// <summary>
		/// 	Tokens list acessor
		/// </summary>
		public BxlToken[] Tokens {
			get { return _tokens; }
		}


		/// <summary>
		/// 	Returns a new <see cref="T:System.Xml.XPath.XPathNavigator" /> object.
		/// </summary>
		/// <returns> An <see cref="T:System.Xml.XPath.XPathNavigator" /> object. </returns>
		public XPathNavigator CreateNavigator() {
			return new BxlXpathNavigator(this);
		}

		private readonly BxlToken[] _tokens;
	}
}