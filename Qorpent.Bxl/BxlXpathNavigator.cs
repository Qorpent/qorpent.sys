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
// PROJECT ORIGIN: Qorpent.Bxl/BxlXpathNavigator.cs
#endregion
using System;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Qorpent.Bxl {
	/// <summary>
	/// 	Bxl Xpath Navigator implementation for BxlTree
	/// </summary>
	internal class BxlXpathNavigator : XPathNavigator {
		public BxlXpathNavigator(BxlTree bxlTree) {
			_tree = bxlTree;
			_current = new BxlToken(BxlTokenType.Element) {Value = ""};
			foreach (var token in _tree.Tokens) {
				_current.Elements.Add(token);
			}
		}

		public override string Value {
			get {
				if (_inattribute) {
					return null != _currentAttribute.Right ? _currentAttribute.Right.Value : _currentAttribute.Value;
				}
				return null == _current.ElementValue ? "" : _current.ElementValue.Value;
			}
		}

		public override XmlNameTable NameTable {
			get { return new NameTable(); }
		}

		public override XPathNodeType NodeType {
			get { return _inattribute ? XPathNodeType.Attribute : XPathNodeType.Element; }
		}

		public override string LocalName {
			get {
				if (_inattribute) {
					if (null != _currentAttribute.Left) {
						return _currentAttribute.Left.Value;
					}
					if (0 == _currentAttibuteIndex) {
						return "code";
					}
					if (1 == _currentAttibuteIndex) {
						return "name";
					}
					return "anonym" + _currentAttibuteIndex;
				}
				return _current.Value;
			}
		}

		public override string Name {
			get { return LocalName; }
		}

		public override string NamespaceURI {
			get { return ""; }
		}

		public override string Prefix {
			get { return ""; }
		}

		public override string BaseURI {
			get { return ""; }
		}

		public override bool IsEmptyElement {
			get { return _current.Elements.Count == 0; }
		}

		public override XPathNavigator Clone() {
			return (XPathNavigator) MemberwiseClone();
		}

		public override bool MoveToFirstAttribute() {
			_currentAttibuteIndex = 0;
			if (_current.Attributes.Count > 0) {
				_inattribute = true;
				_currentAttribute = _current.Attributes[0];
				return true;
			}
			return false;
		}

		public override bool MoveToNextAttribute() {
			_currentAttibuteIndex++;
			if (_current.Attributes.Count > _currentAttibuteIndex) {
				_currentAttribute = _current.Attributes[_currentAttibuteIndex];
				return true;
			}
			return false;
		}

		public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope) {
			return false;
		}

		public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope) {
			return false;
		}

		public override bool MoveToNext() {
			if (null != _siblings) {
				_siblingIndex++;
				_inattribute = false;
				if (_siblings.Length > _siblingIndex) {
					_current = _siblings[_siblingIndex];
					_currentAttibuteIndex = 0;
					_currentElementIndex = 0;
					return true;
				}
			}
			return false;
		}


		public override bool MoveToPrevious() {
			if (null != _siblings) {
				_siblingIndex--;
				_inattribute = false;
				if (_siblings.Length > _siblingIndex) {
					_current = _siblings[_siblingIndex];
					_currentAttibuteIndex = 0;
					_currentElementIndex = 0;
					return true;
				}
			}
			return false;
		}

		public override bool MoveToFirstChild() {
			if (0 != _current.Elements.Count) {
				_inattribute = false;
				_currentAttibuteIndex = 0;
				_currentElementIndex = 0;
				_siblingIndex = 0;
				_siblings = _current.Elements.ToArray();
				_current = _current.Elements[0];
				return true;
			}
			return false;
		}

		public override bool MoveToParent() {
			if (null != _current.Parent) {
				_inattribute = false;
				_currentAttibuteIndex = 0;
				_currentElementIndex = 0;
				_siblingIndex = 0;
				_siblings = _current.Parent.Elements.ToArray();
				_current = _current.Parent;
			}
			return false;
		}

		public override bool MoveTo(XPathNavigator other) {
			var o = (BxlXpathNavigator) other;
			_current = o._current;
			_currentAttribute = o._currentAttribute;
			_currentAttibuteIndex = o._currentAttibuteIndex;
			_currentElementIndex = o._currentElementIndex;
			_siblings = o._siblings;
			_siblingIndex = o._siblingIndex;
			return true;
		}

		public override bool MoveToId(string id) {
			throw new NotImplementedException();
		}

		public override bool IsSamePosition(XPathNavigator other) {
			var o = (BxlXpathNavigator) other;
			if (_current != o._current) {
				return false;
			}
			if (_currentAttribute != o._currentAttribute) {
				return false;
			}
			if (_currentAttibuteIndex != o._currentAttibuteIndex) {
				return false;
			}
			if (_currentElementIndex != o._currentElementIndex) {
				return false;
			}
			if (_siblingIndex != o._siblingIndex) {
				return false;
			}
			return true;
		}

		private readonly BxlTree _tree;
		private BxlToken _current;
		private int _currentAttibuteIndex;
		private BxlToken _currentAttribute;
		private int _currentElementIndex;
		private bool _inattribute;
		private int _siblingIndex;
		private BxlToken[] _siblings;
	}
}