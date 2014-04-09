using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace Qorpent.Embed.Bxl{
	public class BxlParser{
		private const String Namespace = "namespace::";
		private const String Code = "code";
		private const String Id = "id";
		private const String Name = "name";

		private const int TabSpaceCount = 4;
		private const String AnonPrefix = "_aa";
		private const String AnonValue = "1";
		private readonly XName _infoFile = "_file";
		private readonly XName _infoLine = "_line";
		private readonly XName _rootName = "root";
		private readonly StringBuilder _buf = new StringBuilder(256);
		private readonly CharStack _stack = new CharStack();
		private readonly StateOperation[] _map;
		private String _anonCode;
		private String _anonId;
		private String _anonName;
		private String _defaultNsPrefix;
		private List<Stats> _anon;

		private XElement _current;
		private int _defNsCount;
		private LexInfo _info;
		private bool _isExpression;
		private bool _isString;
		private char _last;
		private int _level;
		private ReadMode _mode;
		private BxlParserOptions _options;
		private String _prefix;
		private XElement _root;
		private int _symbolCount;
		private int _tabIgnore;
		private int _tabs;
		private String _value;

		/// <summary>
		/// </summary>
		public BxlParser(){
			_map = new StateOperation[]{
				ProcessStart,
				ProcessElementName,
				ProcessAttributeName,
				ProcessAttributeValue,
				ProcessSingleLineString,
				ProcessMultiLineString,
				ProcessIndent,
				ProcessNewLine,
				ProcessQuoting1,
				ProcessQuoting2,
				ProcessUnquoting,
				ProcessEscapingBackSlash,
				ProcessExpression,
				ProcessTextContent,
				ProcessWaitingForNl,
				ProcessNamespaceName,
				ProcessNamespaceValue,
				ProcessColon,
				ProcessCommentary
			};
		}

		/// <summary>
		///     Parses source code into Xml
		/// </summary>
		/// <param name="code"></param>
		/// <param name="filename"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public XElement Parse(string code = null, string filename = "code.bxl",
		                      BxlParserOptions options = BxlParserOptions.None){
			if (string.IsNullOrWhiteSpace(code)){
				if (string.IsNullOrWhiteSpace(filename))
					return new XElement("root");
				code = File.ReadAllText(filename);
			}

			if (string.IsNullOrWhiteSpace(filename)){
				filename = "code.bxl";
			}
			Init(filename, options);
			foreach (char c in code){
				_info.CharIndex++;
				_info.Column++;
				if (c == '\t'){
					_info.Column += 3;
				}
				_map[(int) _mode](c);

				if (c == '\r' || c == '\n' && _last != '\r'){
					_info.Line++;
					_info.Column = 0;
				}
				_last = c;
			}

			if (code.Last() != '\r' && code.Last() != '\n')
				_map[(int) _mode]('\n');

			if (_options.HasFlag(BxlParserOptions.ExtractSingle) && _root.Elements().Count() == 1){
				_current = _root.Elements().First();
				_current.Remove();
				// explicit copy namespaces from _root to _current ?
				_root = _current;
			}

			if (0 != _stack.Count)
				throw new BxlException("invalid quotes or braces", _info.Clone());
			if (!options.HasFlag(BxlParserOptions.NoLexData)){
				_root.SetAttr("_file", filename);
			}

			return _root;
		}

		private void Init(String filename, BxlParserOptions options){
			_options = options;
			_info = new LexInfo(filename, 1);
			_defaultNsPrefix = Namespace + filename + "_";
			String __ = "";
			if (_options.HasFlag(BxlParserOptions.SafeAttributeNames)){
				__ = "__";
			}
			_anonCode = __ + Code;
			_anonId = __ + Id;
			_anonName = __ + Name;

			_level = -1;
			_anon = new List<Stats>{new Stats()};
			_symbolCount = 0;
			_tabIgnore = 0;
			_tabs = 0;
			_defNsCount = 0;
			_value = "";
			_prefix = "";
			_isString = false;
			_isExpression = false;
			_last = (char) 0;
			_buf.Clear();
			_stack.Clear();

			_root = new XElement(_rootName);
			_current = _root;
			_mode = ReadMode.Start;
		}

		//		processing current state

		private void ProcessIndent(char c){
			switch (c){
				case ':':
					_mode = ReadMode.TextContent;
					return;
				case '"':
					_stack.Push((char) ReadMode.AttributeName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char) ReadMode.AttributeName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case ',':
					return;
				case '=':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case ' ':
					if (++_symbolCount == TabSpaceCount){
						_symbolCount = 0;
						ProcessIndent('\t');
					}
					return;
				case '\r':
				case '\n':
					_mode = ReadMode.NewLine;
					return;
				case '\t':
					if (_tabs == 0){
						_level++;
						if (_level >= _anon.Count)
							_anon.Add(new Stats());
						_current = _current.LastNode as XElement ?? _current;
					}
					else
						_tabs--;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					_mode = ReadMode.ElementName;
					return;
			}
		}

		private void ProcessNewLine(char c){
			char s = _stack.Peek();
			if (s == '\'' || s == '"')
				throw new BxlException("new line in regular string", _info.Clone());

			_symbolCount = 0;
			_tabs = _tabIgnore;
			_current = _root;
			_level = -1;
			switch (c){
				case ':':
					_mode = ReadMode.TextContent;
					return;
				case '"':
					_stack.Push((char) ReadMode.AttributeName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char) ReadMode.AttributeName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case ',':
					return;
				case '=':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case ' ':
					_symbolCount++;
					_mode = ReadMode.Indent;
					return;
				case '\r':
				case '\n':
					return;
				case '\t':
					_mode = ReadMode.Indent;
					_map[(int) _mode](c);
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					_mode = ReadMode.ElementName;
					return;
			}
		}

		private void ProcessElementName(char c){
			switch (c){
				case ':':
					SaveValue();
					_stack.Push((char) _mode);
					_mode = ReadMode.Colon;
					return;
				case '"':
					if (_value.Length != 0)
						AddNode();
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					_stack.Push((char) ReadMode.AttributeName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					if (_value.Length != 0)
						AddNode();
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					_stack.Push((char) ReadMode.AttributeName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					SaveValue();
					_mode = ReadMode.AttributeValue;
					return;
				case ',':
				case '\t':
				case ' ':
					SaveValue();
					return;
				case '\r':
				case '\n':
					SaveValue();
					AddNode();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					if (_value.Length != 0){
						AddNode();
						_mode = ReadMode.AttributeName;
					}
					_buf.Append(c);
					return;
			}
		}

		private void ProcessAttributeName(char c){
			switch (c){
				case ':':
					SaveValue();
					_stack.Push((char) _mode);
					_mode = ReadMode.Colon;
					return;
				case '"':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					AddAnonAttribute();
					_stack.Push((char) _mode);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					AddAnonAttribute();
					_stack.Push((char) _mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					if (_isExpression)
						throw new BxlException("can not assign to expression", _info.Clone());
					SaveValue();
					_isString = false;
					_isExpression = false;
					_mode = ReadMode.AttributeValue;
					return;
				case '\t':
				case ',':
				case ' ':
					SaveValue();
					return;
				case '\r':
				case '\n':
					SaveValue();
					AddAnonAttribute();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
					AddAnonAttribute();
					if (_buf.Length == 0){
						_stack.Push((char) _mode);
						_stack.Push(c);
						_mode = ReadMode.Expression;
					}
					_buf.Append(c);
					return;
				case ')':
					if (_buf.Length != 0)
						_buf.Append(c);
					return;
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					AddAnonAttribute();
					return;
			}
		}

		private void ProcessAttributeValue(char c){
			switch (c){
				case ':':
					if (_buf.Length == 0 && !_isString)
						return;
					AddAttributeValue();
					_mode = ReadMode.TextContent;
					return;
				case '"':
					_stack.Push((char) _mode);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char) _mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '\t':
				case ',':
				case ' ':
					if (_buf.Length == 0 && !_isString)
						return;
					AddAttributeValue();
					_mode = ReadMode.AttributeName;
					return;
				case '\r':
				case '\n':
					if (_buf.Length == 0 && !_isString)
						throw new BxlException("empty attribute value", _info.Clone());
					AddAttributeValue();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
					if (_buf.Length == 0){
						_stack.Push((char) _mode);
						_stack.Push(c);
						_mode = ReadMode.Expression;
					}
					_buf.Append(c);
					return;
				case ')':
					if (_buf.Length != 0)
						_buf.Append(c);
					return;
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void ProcessSingleLineString(char c){
			_isString = true;
			switch (c){
				case '"':
				case '\'':
					if (_stack.Peek() == c){
						_stack.Pop();
						_mode = (ReadMode) _stack.Pop();
						_map[(int) _mode](' ');
					}
					else
						_buf.Append(c);
					return;
				case '\r':
				case '\n':
					throw new BxlException("new line in regular string", _info.Clone());
				case '\\':
					_stack.Push((char) _mode);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void ProcessEscapingBackSlash(char c){
			_buf.Append(c);
			_mode = (ReadMode) _stack.Pop();
		}

		private void ProcessQuoting1(char c){
			_isString = true;
			switch (c){
				case '"':
					_mode = ReadMode.Quoting2;
					return;
				case '\n':
				case '\r':
					throw new BxlException("new line in regular string", _info.Clone());
				case '\\':
					_stack.Push('"');
					_stack.Push((char) ReadMode.SingleLineString);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append(c);
					_stack.Push('"');
					_mode = ReadMode.SingleLineString;
					return;
			}
		}

		private void ProcessQuoting2(char c){
			switch (c){
				case '"':
					_mode = ReadMode.MultiLineString;
					return;
				default:
					_map[_stack.Pop()](c);
					return;
			}
		}

		private void ProcessUnquoting(char c){
			switch (c){
				case '"':
					_symbolCount++;
					if (_symbolCount == 3){
						_mode = (ReadMode) _stack.Pop();
						_symbolCount = 0;
						_map[(int) _mode](' ');
					}
					return;
				case '\\':
					_buf.Append('"', _symbolCount);
					_stack.Push((char) ReadMode.MultiLineString);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append('"', _symbolCount);
					_buf.Append(c);
					_mode = ReadMode.MultiLineString;
					return;
			}
		}

		private void ProcessMultiLineString(char c){
			switch (c){
				case '"':
					_symbolCount = 1;
					_mode = ReadMode.Unquoting;
					return;
				case '\\':
					_stack.Push((char) _mode);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void ProcessExpression(char c){
			_isExpression = true;
			switch (c){
				case '\\':
					_stack.Push((char) _mode);
					_mode = ReadMode.EscapingBackSlash;
					return;
				case '(':
					_buf.Append(c);
					_stack.Push(c);
					return;
				case ')':
					_buf.Append(c);
					_stack.Pop();
					if (_stack.Peek() != '(')
						_mode = (ReadMode) _stack.Pop();
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void ProcessTextContent(char c){
			switch (c){
				case ':':
				case '=':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case ',':
				case ' ':
				case '\t':
					AddTextContent();
					return;
				case '"':
					_stack.Push((char) _mode);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char) _mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '\r':
				case '\n':
					AddTextContent();
					_mode = ReadMode.NewLine;
					return;
				case '(':
					if (_buf.Length == 0){
						_stack.Push((char) _mode);
						_stack.Push(c);
						_mode = ReadMode.Expression;
					}
					_buf.Append(c);
					return;
				case ')':
					if (_buf.Length != 0)
						_buf.Append(c);
					return;
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void ProcessWaitingForNl(char c){
			switch (c){
				case ' ':
				case ',':
				case '\t':
					return;
				case '\n':
				case '\r':
					_mode = ReadMode.NewLine;
					return;
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					throw new BxlException("unexpected symbol " + c, _info.Clone());
			}
		}

		private void ProcessStart(char c){
			switch (c){
				case ':':
					_mode = ReadMode.TextContent;
					return;
				case ',':
				case ')':
					return;
				case '=':
				case '(':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '"':
					_stack.Push((char) ReadMode.NamespaceName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char) ReadMode.NamespaceName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case ' ':
					_symbolCount++;
					if (_symbolCount == TabSpaceCount){
						_symbolCount = 0;
						ProcessStart('\t');
					}
					return;
				case '\n':
				case '\r':
					_symbolCount = 0;
					_tabIgnore = 0;
					return;
				case '\t':
					_tabIgnore++;
					return;
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					_mode = ReadMode.NamespaceName;
					return;
			}
		}

		private void ProcessNamespaceName(char c){
			switch (c){
				case ':':
					SaveValue();
					_stack.Push((char) ReadMode.ElementName);
					_mode = ReadMode.Colon;
					return;
				case '\t':
				case ' ':
				case ',':
					SaveValue();
					return;
				case '=':
					SaveValue();
					_mode = ReadMode.NamespaceValue;
					return;
				case ')':
					return;
				case '(':
					if (_value.Length != 0){
						AddNode();
						_stack.Push((char) ReadMode.AttributeName);
						_stack.Push(c);
						_mode = ReadMode.Expression;
					}
					else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '"':
					if (_value.Length != 0){
						AddNode();
						_stack.Push((char) ReadMode.AttributeName);
						_mode = ReadMode.Quoting1;
					}
					else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\'':
					if (_value.Length != 0){
						AddNode();
						_stack.Push((char) ReadMode.AttributeName);
						_stack.Push(c);
						_mode = ReadMode.SingleLineString;
					}
					else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\n':
				case '\r':
					SaveValue();
					AddNode();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escaping not in string", _info.Clone());
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					if (_value.Length != 0){
						AddNode();
						_mode = ReadMode.AttributeName;
					}
					return;
			}
		}

		private void ProcessNamespaceValue(char c){
			switch (c){
				case ':':
				case ',':
				case '=':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '"':
					if (_buf.Length == 0){
						_stack.Push((char) _mode);
						_mode = ReadMode.Quoting1;
					}
					else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\'':
					if (_buf.Length == 0){
						_stack.Push((char) _mode);
						_stack.Push(c);
						_mode = ReadMode.SingleLineString;
					}
					else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\t':
				case ' ':
					if (_buf.Length != 0){
						AddNamespace();
						_mode = ReadMode.WaitingForNl;
					}
					return;
				case '\n':
				case '\r':
					if (_buf.Length != 0){
						AddNamespace();
						_mode = ReadMode.Start;
					}
					else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\\':
					throw new BxlException("escaping not in string", _info.Clone());
				case '(':
					if (_buf.Length == 0){
						_stack.Push((char) _mode);
						_stack.Push(c);
						_mode = ReadMode.Expression;
					}
					_buf.Append(c);
					return;
				case ')':
					if (_buf.Length != 0)
						_buf.Append(c);
					return;
				case '#':
					_map[(int) _mode]('\n');
					_stack.Push((char) _mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void ProcessColon(char c){
			switch (c){
				case ':':
					if (_value.Length == 0)
						throw new BxlException("wrong namespace", _info.Clone());
					_prefix = _value;
					_value = "";
					_mode = (ReadMode) _stack.Pop();
					return;
				default:
					_mode = (ReadMode) _stack.Pop();
					if (_mode == ReadMode.ElementName)
						AddNode();
					else if (_mode == ReadMode.AttributeName)
						AddAnonAttribute();
					_mode = ReadMode.TextContent;
					_map[(int) _mode](c);
					return;
			}
		}

		private void ProcessCommentary(char c){
			if (c == '\n' || c == '\r')
				_mode = (ReadMode) _stack.Pop();
		}

		//		modifying tree

		private void AddNode(){
			String s = _value.Escape(EscapingType.XmlName);

			String ns = ResolveNamespace();
			var node = new XElement(XName.Get(s, ns));
			if (!_options.HasFlag(BxlParserOptions.NoLexData)){
				node.Add(new XAttribute(_infoFile, _info.File));
				node.Add(new XAttribute(_infoLine, _info.Line));
			}
			_current.Add(node);
			_current = node;

			ProcessIndent('\t');
			_anon[_level].Reset();

			_isExpression = false;
			_isString = false;
			_value = "";
		}

		private void AddAnonAttribute(){
			if (_value.Length == 0)
				return;
			if (_current == _root)
				throw new BxlException("adding attribute to root not allowed", _info.Clone());

			if (!_anon[_level].HasCodeId){
				if (_prefix.Length != 0)
					_prefix += "::";
				if (!_options.HasFlag(BxlParserOptions.OnlyIdAttibute)){
					_current.SetAttributeValue(XName.Get(_anonCode), _prefix + _value);
					_anon[_level].Count++;
				}
				if (!_options.HasFlag(BxlParserOptions.OnlyCodeAttribute)){
					if (_current.Attribute("id") == null || _current.Attribute("id").Value == ""){
						_current.SetAttributeValue(XName.Get(_anonId), _prefix + _value);
					}

					_anon[_level].Count++;
				}
				_anon[_level].HasCodeId = true;
			}
			else if (!_anon[_level].HasName){
				if (_prefix.Length != 0)
					_prefix += "::";
				_current.SetAttributeValue(XName.Get(_anonName), _prefix + _value);
				_anon[_level].Count++;
				_anon[_level].HasName = true;
			}
			else{
				if (_isString || _isExpression){
					// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
					_anon[_level].Count++;
					String name = AnonPrefix + (_anon[_level].Count);

					// namespace not needed because this anonymous string attribute can not be declared with namespace prefix
					_current.SetAttributeValue(XName.Get(name), _value);
				}
				else
					_current.SetAttributeValue(XName.Get(_value.Escape(EscapingType.XmlName), ResolveNamespace()), AnonValue);
			}

			_isExpression = false;
			_isString = false;
			_value = "";
			_prefix = "";
		}

		private void SaveValue(){
			if (_buf.Length == 0)
				return;
			_value = _buf.ToString();
			_buf.Clear();
		}

		private void AddAttributeValue(){
			// checking for empty _buf must be implemented in invoker !
			if (_value.Length == 0)
				throw new BxlException("empty attribute name", _info.Clone());
			if (_current == _root)
				throw new BxlException("adding attribute to root not allowed", _info.Clone());

			String ns = ResolveNamespace();
			String s = _buf.ToString();
			_buf.Clear();
			_current.SetAttributeValue(XName.Get(_value.Escape(EscapingType.XmlName), ns), s);
			_isExpression = false;
			_isString = false;
			_value = "";
		}

		private void AddTextContent(){
			if (_buf.Length == 0 && !_isString)
				return;
			String s = _buf.ToString();
			_buf.Clear();
			_isExpression = false;
			_isString = false;

			_current.Add(new XText(s));
			_mode = ReadMode.WaitingForNl;
		}

		private String ResolveNamespace(){
			String ns = "";
			if (_prefix.Length != 0){
				XNamespace xns = _root.GetNamespaceOfPrefix(_prefix);
				if (xns == null){
					
					ns = ns == "" ? AddDefaultNamespace(_prefix) : ns;
				}
				else
					ns = xns.NamespaceName;
				_prefix = "";
			}
			return ns;
		}

		private void AddNamespace(){
			// checking for empty _buf must be implemented in invoker !
			if (_value.Length == 0)
				throw new BxlException("empty namespace name", _info.Clone());
			String s = _value.Escape(EscapingType.XmlName);
			_root.SetAttributeValue(XNamespace.Xmlns + s, _buf.ToString());
			_isExpression = false;
			_isString = false;
			_value = "";
			_buf.Clear();
		}

		private String AddDefaultNamespace(String prefix){
			String s = prefix.Escape(EscapingType.XmlName);
			String ns = _defaultNsPrefix + new string('X', ++_defNsCount);
			_root.SetAttributeValue(XNamespace.Xmlns + s, ns);
			return ns;
		}

		/// <summary>
		///     modified method Pop and Peak - if stack is empty they return 0, not excption
		/// </summary>
		private class CharStack : Stack<char>{
			public new char Pop(){
				if (Count != 0)
					return base.Pop();
				return (char) 0;
			}

			public new char Peek(){
				if (Count != 0)
					return base.Peek();
				return (char) 0;
			}
		}

		private class Stats{
			public int Count;
			public bool HasCodeId;
			public bool HasName;

			public void Reset(){
				HasCodeId = HasName = false;
				Count = 0;
			}
		}

		private delegate void StateOperation(char c);
	}


	/// <summary>
	///     Set of flags to configure bxl parsing process
	/// </summary>
	[Flags]
	public enum BxlParserOptions{
		/// <summary>
		///     default zero option
		/// </summary>
		None = 0,

		/// <summary>
		///     do not generate lexinfo data in result XML (result size-optimization and readablitiy)
		/// </summary>
		NoLexData = 1,

		/// <summary>
		///     use '__' prefixed standard attributes (code,name,id) instead of direct names
		/// </summary>
		SafeAttributeNames = 2,

		/// <summary>
		///     prevent creation of 'code' attribute in id-code pare
		/// </summary>
		OnlyIdAttibute = 4,

		/// <summary>
		///     prevent creation of 'id' attribute in id-code pare
		/// </summary>
		OnlyCodeAttribute = 8,


		/// <summary>
		///     Forces remove ROOT element if only one child element
		/// </summary>
		ExtractSingle = 32,
	}

	/// <summary>
	/// </summary>
	internal static class Extensions{
		/// <summary>
		///     Устанавливает атрибут, если значение не null
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public static XElement SetAttr(this XElement parent, string name, object value){
			if (value != null){
				if (value is string){
					if (String.IsNullOrWhiteSpace(value as string)){
						return parent;
					}
				}

				parent.SetAttributeValue(name, value);
			}

			return parent;
		}
	}

	/// <summary>
	///     Срабатывает на любые проблемы, случившиеся в процессе обработки BXL
	/// </summary>
	[Serializable]
	internal class BxlException : Exception{
		/// <summary>
		///     Ошибка источника позиции исходного файла
		/// </summary>
		public readonly LexInfo LexInfo;

		/// <summary>
		///     Создает новый экземпляр ошибки
		/// </summary>
		/// <param name="message"> Пользовательское сообщение </param>
		/// <param name="inner"> inner wrapped exception </param>
		/// <param name="lexinfo"> Позиция исходного файла вызвавшего ошибку </param>
		public BxlException(string message = "", LexInfo lexinfo = null, Exception inner = null)
			: base(message + (lexinfo ?? new LexInfo()), inner){
			LexInfo = lexinfo ?? new LexInfo();
		}

		/// <summary>
		///     При переопределении в производном классе задает сведения об исключении для
		///     <see
		///         cref="T:System.Runtime.Serialization.SerializationInfo" />
		///     .
		/// </summary>
		public override void GetObjectData(SerializationInfo info, StreamingContext context){
			base.GetObjectData(info, context);
			info.AddValue("_LexInfo", LexInfo);
		}
	}

	internal enum ReadMode{
		Start = 0,
		ElementName,
		AttributeName,
		AttributeValue,
		SingleLineString,
		MultiLineString,
		Indent,
		NewLine,
		Quoting1,
		Quoting2,
		Unquoting,
		EscapingBackSlash,
		Expression,
		TextContent,
		WaitingForNl,
		NamespaceName,
		NamespaceValue,
		Colon,
		Commentary
	}

	/// <summary>
	///     Описывает информацию о позиции исходных файлов в BXL файле
	/// </summary>
	internal class LexInfo{
		/// <summary>
		///     ****************************************Not-lined char index in whole file
		/// </summary>
		public int CharIndex;

		/// <summary>
		///     Колонка количества символов в строке
		/// </summary>
		public int Column;

		/// <summary>
		///     Дополнительный контекст
		/// </summary>
		public string Context;

		/// <summary>
		///     Имя исходного файла
		/// </summary>
		public string File;

		/// <summary>
		///     Длина описанного элемента кода
		/// </summary>
		public int Length;

		/// <summary>
		///     Номер строки, описываемый элементом код
		/// </summary>
		public int Line;

		/// <summary>
		/// </summary>
		public LexInfo(){
		}

		/// <summary>
		///     Создает новый экземпляр BxlLexInfo
		/// </summary>
		/// <param name="filename"> имя исходного файла </param>
		/// <param name="line"> номер строки </param>
		/// <param name="col"> номер колонки </param>
		/// <param name="charindex"> глобальный индекс символа </param>
		/// <param name="length"> длина элемента </param>
		/// <param name="context"></param>
		public LexInfo(string filename = "", int line = 0, int col = 0, int charindex = 0, int length = 0,
		               string context = null){
			File = filename;
			Line = line;
			Column = col;
			Length = length;
			CharIndex = charindex;
			Context = context;
		}

		/// <summary>
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		protected bool Equals(LexInfo other){
			return Column == other.Column && string.Equals(File, other.File) && Line == other.Line;
		}

		/// <summary>
		///     Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		///     A hash code for the current <see cref="T:System.Object" />.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		public override int GetHashCode(){
			unchecked{
				// ReSharper disable NonReadonlyFieldInGetHashCode
				int hashCode = Column;

				hashCode = (hashCode*397) ^ (File != null ? File.GetHashCode() : 0);

				hashCode = (hashCode*397) ^ Line;
				return hashCode;
				// ReSharper restore NonReadonlyFieldInGetHashCode
			}
		}

		/// <summary>
		///     Determines whether the specified <see cref="T:System.Object" /> is equal to the current <see cref="T:System.Object" />.
		/// </summary>
		/// <returns>
		///     true if the specified object  is equal to the current object; otherwise, false.
		/// </returns>
		/// <param name="obj">The object to compare with the current object. </param>
		/// <filterpriority>2</filterpriority>
		public override bool Equals(object obj){
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((LexInfo) obj);
		}

		/// <summary>
		///     Создает читаемую строку lexinfo
		/// </summary>
		/// <returns> </returns>
		public override string ToString(){
			return " at " + (File ?? "") + " : " + Line + ":" + Column +
			       (string.IsNullOrWhiteSpace(Context) ? "" : " : " + Context);
		}

		/// <summary>
		///     Создает копию текущего lexinfo
		/// </summary>
		/// <returns> </returns>
		public LexInfo Clone(){
			return (LexInfo) MemberwiseClone();
		}
	}

	/// <summary>
	///     Univarsal character escaper
	/// </summary>
	internal static class Escaper{
		private static readonly XmlName Xname = new XmlName();

		/// <summary>
		///     Escape all symbols for given type
		/// </summary>
		/// <param name="str"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static String Escape(this String str, EscapingType type){
			if (string.IsNullOrEmpty(str)){
				return string.Empty;
			}

			var sb = new StringBuilder(str.Length);

			if (!IsLiteral(str[0], type, true)){
				sb.Append(EscapeFirst(str[0]));
			}
			else{
				sb.Append(str[0]);
			}

			for (int i = 1; i < str.Length; i++)
				if (!IsLiteral(str[i], type)){
					sb.Append(EscapeCommon(str[i]));
				}
				else{
					sb.Append(str[i]);
				}

			return sb.ToString();
		}


		/// <summary>
		/// </summary>
		/// <param name="c"></param>
		/// <param name="type"></param>
		/// <param name="first"></param>
		/// <returns></returns>
		public static bool IsLiteral(this char c, EscapingType type, bool first = false){
			return !(first && Xname.GetFirst().ContainsKey(c)
			         || Xname.GetCommon().ContainsKey(c)
			         || Xname.NeedEscapeUnicode(c));
		}

		/// <summary>
		/// </summary>
		/// <param name="str"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsLiteral(this String str, EscapingType type){
			if (Xname.GetFirst().ContainsKey(str[0]))
				return false;
			return str.All(c => !Xname.GetCommon().ContainsKey(c));
		}

		private static String EscapeFirst(char c){
			String r;
			if (Xname.GetFirst().TryGetValue(c, out r))
				return r;
			return EscapeCommon(c);
		}

		private static String EscapeCommon(char c){
			String r;
			if (Xname.GetCommon().TryGetValue(c, out r))
				return r;
			return EscapeUnicode(c);
		}

		private static String EscapeUnicode(char c){
			// escaping not defined
			if (Xname.GetUnicodePattern() == null)
				return c.ToString(CultureInfo.InvariantCulture);

			if (!Xname.NeedEscapeUnicode(c))
				return c.ToString(CultureInfo.InvariantCulture);

			return Xname.GetUnicodePattern().Replace(" ", ((int) c).ToString("X4"));
		}


		/// <summary>
		///     Unscape all symbols for given type
		/// </summary>
		/// <param name="str"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static String Unescape(this String str, EscapingType type){
			var res = new StringBuilder(str.Length);

			int offset = 0;
			foreach (var i in Xname.GetFirst()){
				if (str.StartsWith(i.Value)){
					res.Append(i.Key);
					offset = i.Value.Length;
					break;
				}
			}

			for (int i = offset; i < str.Length; i++){
				String s = CheckSuffix(str, offset, i);
				if (s != null){
					res.Append(s);
					offset = i + 1;
				}
			}

			return res + str.Substring(offset);
		}

		private static String CheckSuffix(String str, int start, int end){
			String s;
			// the length of escaping code not constant so check all variants
			foreach (int j in Xname.GetUnescape().KeysLength){
				if (j > end - start + 1)
					continue;

				s = str.Substring(end - j + 1, j);
				s = UnescapeEntity(s);
				if (s != null)
					return str.Substring(start, end - start - j + 1) + s;
			}

			// check unicode
			if (Xname.GetUnicodePattern() == null)
				return null;

			int p = Xname.GetUnicodePattern().Length + 3;
			if (p > end - start + 1)
				return null;

			s = str.Substring(end - p + 1, p);
			s = UnescapeUnicode(s);
			if (s != null)
				return str.Substring(start, end - start - p + 1) + s;

			return null;
		}

		private static String UnescapeEntity(this String s){
			char r;
			if (Xname.GetUnescape().TryGetValue(s, out r))
				return r.ToString(CultureInfo.InvariantCulture);

			return null;
		}

		private static String UnescapeUnicode(this String s){
			String p = Xname.GetUnicodePattern();
			if (p == null)
				return null;
			if (p.Length + 3 == s.Length){
				String hex = s.Substring(p.IndexOf(' '), 4);
				ushort value;
				if (ushort.TryParse(hex, NumberStyles.HexNumber, null, out value))
					return ((char) value).ToString(CultureInfo.InvariantCulture);
			}

			return null;
		}

		/// <summary>
		///     Возвращает строку в PascalCase
		/// </summary>
		/// <param name="s"></param>
		/// <param name="escapeWhiteSpaces"></param>
		/// <returns></returns>
		public static string PascalCase(string s, bool escapeWhiteSpaces = true){
			if (string.IsNullOrEmpty(s)) return "";
			if (escapeWhiteSpaces){
				if (s.Contains(" ")){
					var res = new StringBuilder();
					foreach (string n in s.Split(' ')){
						res.Append(PascalCase(n));
					}
					return res.ToString();
				}
			}
			return (s[0].ToString(CultureInfo.InvariantCulture).ToUpper() + s.Substring(1)).Replace(" ", "_");
		}
	}

	/// <summary>
	/// </summary>
	[Flags]
	public enum EscapingType{
		/// <summary>
		/// </summary>
		XmlName = 1,
	}

	/// <summary>
	///     strores information about length of keys
	/// </summary>
	internal class OptimizedEscapeDictionary : Dictionary<string, char>{
		private readonly HashSet<int> _len = new HashSet<int>();

		public HashSet<int> KeysLength{
			get { return _len; }
		}

		public new void Add(string k, char v){
			base.Add(k, v);

			_len.Add(k.Length);
		}
	}

	/// <summary>
	///     <see cref="EscapingType.XmlName" />
	/// </summary>
	internal class XmlName{
		private static readonly Dictionary<char, string> Common = new Dictionary<char, string>{
			{'+', "__PLUS__"},
			{'?', "__ASK__"},
			{'!', "__EXC__"},
			{'~', "__TILD__"},
			{'@', "__AT__"},
			{'*', "__STAR__"},
			{'$', "__USD__"},
			{'^', "__UP__"},
			{'&', "__AMP__"},
			{'/', "__DIV__"},
			{':', "__DBL__"},
			{'%', "__PERC__"},
			{'(', "__LBRACE__"},
			{')', "__RBRACE__"},
			{'[', "__LINDEX__"},
			{']', "__RINDEX__"},
			{'{', "__LBLOCK__"},
			{'}', "__RBLOCK__"},
			{'|', "__VLINE__"},
			{';', "__PERIOD__"},
			{'<', "__LT__"},
			{'>', "__GT__"},
			{'=', "__EQ__"},
			{',', "__COMMA__"},
			{'"', "__QUOT__"},
			{'\'', "__APOS__"},
			{' ', "__SPACE__"},
			{'\t', "__TAB__"},
			{'\n', "__NLINE__"},
		};

		private static readonly Dictionary<char, string> First = new Dictionary<char, string>{
			{'-', "__MINUS__"},
			{'.', "__DOT__"},
			{'0', "_0"},
			{'1', "_1"},
			{'2', "_2"},
			{'3', "_3"},
			{'4', "_4"},
			{'5', "_5"},
			{'6', "_6"},
			{'7', "_7"},
			{'8', "_8"},
			{'9', "_9"},
		};

		private static readonly OptimizedEscapeDictionary Unescape = new OptimizedEscapeDictionary{
			{"__PLUS__", '+'},
			{"__MINUS__", '-'},
			{"__ASK__", '?'},
			{"__EXC__", '!'},
			{"__TILD__", '~'},
			{"__AT__", '@'},
			{"__STAR__", '*'},
			{"__USD__", '$'},
			{"__UP__", '^'},
			{"__AMP__", '&'},
			{"__DIV__", '/'},
			{"__DBL__", ':'},
			{"__PERC__", '%'},
			{"__LBRACE__", '('},
			{"__RBRACE__", ')'},
			{"__LINDEX__", '['},
			{"__RINDEX__", ']'},
			{"__LBLOCK__", '{'},
			{"__RBLOCK__", '}'},
			{"__VLINE__", '|'},
			{"__PERIOD__", ';'},
			{"__LT__", '<'},
			{"__GT__", '>'},
			{"__DOT__", '.'},
			{"__EQ__", '='},
			{"__COMMA__", ','},
			{"__QUOT__", '"'},
			{"__APOS__", '\''},
			{"__SPACE__", ' '},
			{"__TAB__", '\t'},
			{"__NLINE__", '\n'},
		};

		public Dictionary<char, string> GetFirst(){
			return First;
		}

		public Dictionary<char, string> GetCommon(){
			return Common;
		}

		public OptimizedEscapeDictionary GetUnescape(){
			return Unescape;
		}

		public string GetUnicodePattern(){
			return "__0x __";
		}

		public bool NeedEscapeUnicode(char c){
			// standard ASCII exclude control characters
			if (c >= 32 && c <= 127)
				return false;
			// Russian
			if (c >= 0x0410 && c <= 0x044f || c == 0x0401 || c == 0x0451)
				return false;

			return true;
		}
	}
}