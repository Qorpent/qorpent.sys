using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Dsl;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bxl2 {
    /// <summary>
    /// 
    /// </summary>
    public class BxlParser2 : IBxlParser {
		private const String NAMESPACE = "namespace::";
		private const String CODE = "code";
		private const String ID = "id";
		private const String NAME = "name";

		private const int TAB_SPACE_COUNT = 4;
		private readonly XName ROOT_NAME = "root";
		private readonly XName INFO_FILE = "_file";
		private readonly XName INFO_LINE = "_line";
		private const String ANON_PREFIX = "_aa";
		private const String ANON_VALUE = "1";
	    private String DEFAULT_NS_PREFIX;
		private String ANON_CODE;
		private String ANON_ID;
		private String ANON_NAME;

	    private BxlParserOptions _options;
        private XElement _root, _current;
        private ReadMode _mode;
        private readonly StringBuilder _buf = new StringBuilder(256);
        private readonly CharStack _stack = new CharStack();
	    private int _symbolCount;
	    private int _tabIgnore;
	    private int _tabs;
	    private int _anonCount;
	    private int _defNsCount;
	    private String _value;
	    private String _prefix;
	    private bool _isString;
	    private bool _isExpression;
	    private char _last;
	    private LexInfo _info;

        private delegate void stateOperation(char c);
        private readonly stateOperation[] map;

        /// <summary>
        /// 
        /// </summary>
        public BxlParser2() {
            map = new stateOperation[] {
				processStart,
				processElementName,
				processAttributeName,
				processAttributeValue,
				processSingleLineString,
				processMultiLineString,
				processIndent,
				processNewLine,
				processQuoting1,
				processQuoting2,
				processUnquoting,
				processEscapingBackSlash,
				processExpression,
				processTextContent,
				processWaitingForNL,
				processNamespaceName,
				processNamespaceValue,
				processColon,
				processCommentary
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="filename"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public XElement Parse(string code = null, string filename = "code.bxl", BxlParserOptions options = BxlParserOptions.None) {
            if (code.IsEmpty()) {
                if (filename.IsEmpty())
                    return new XElement("root");
                code = File.ReadAllText(filename);
            }

			init(filename, options);
            foreach (char c in code) {
	            _info.CharIndex++;
	            _info.Column++;

                map[(int)_mode](c);

				if (c == '\r' || c == '\n' && _last != '\r') {
					_info.Line++;
					_info.Column = 0;
				}
	            _last = c;
            }

	        if (code.Last() != '\r' && code.Last() != '\n')
		        map[(int)_mode]('\n');

	        if (_options.HasFlag(BxlParserOptions.ExtractSingle) && _root.Elements().Count() == 1) {
		        _current = _root.Elements().First();
				_current.Remove();
				// explicit copy namespaces from _root to _current ?
		        _root = _current;
	        }

	        if (_stack.IsNotEmpty())
				throw new BxlException("invalid quotes or braces");
            return _root;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcexml"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public string Generate(XElement sourcexml, BxlGeneratorOptions options = null) {
            return "";
        }

	    private void init(String filename, BxlParserOptions options) {
			_options = options;
			_info = new LexInfo(filename, 1);
			DEFAULT_NS_PREFIX = NAMESPACE + filename + "_";
			String __ = "";
			if (_options.HasFlag(BxlParserOptions.SafeAttributeNames)) {
				__ = "__";
			}
			ANON_CODE = __ + CODE;
			ANON_ID = __ + ID;
			ANON_NAME = __ + NAME;

			_symbolCount = 0;
			_tabIgnore = 0;
			_tabs = 0;
			_anonCount = 0;
			_defNsCount = 0;
			_value = "";
			_prefix = "";
			_isString = false;
			_isExpression = false;
			_last = (char)0;
			_buf.Clear();
			_stack.Clear();

			_root = new XElement(ROOT_NAME);
			_current = _root;
			_mode = ReadMode.Start;
	    }

		//		processing current state

        private void processIndent(char c) {
            switch (c) {
                case ':':
		            _mode = ReadMode.TextContent;
		            return;
				case '"':
					_stack.Push((char)ReadMode.AttributeName);
					_mode = ReadMode.Quoting1;
		            return;
				case '\'':
					_stack.Push((char)ReadMode.AttributeName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
		            return;
				case ',':
		            return;
				case '=':
					throw new BxlException("unexpected symbol " + c, _info);
                case ' ':
					if (++_symbolCount == TAB_SPACE_COUNT) {
			            _symbolCount = 0;
						processIndent('\t');
		            }
					return;
				case '\r':
                case '\n':
					_mode = ReadMode.NewLine;
		            return;
                case '\t':
		            if (_tabs == 0)
			            _current = _current.LastNode as XElement ?? _current;
					else
						_tabs--;
                    return;
				case '\\':
					throw new BxlException("escape not in string", _info);
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c, _info);
				case '#':
		            map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
		            return;
                default:
                    _buf.Append(c);
                    _mode = ReadMode.ElementName;
                    return;
            }
        }

	    private void processNewLine(char c) {
			char s = _stack.Peek();
			if (s == '\'' || s == '"')
				throw new BxlException("new line in regular string", _info);

		    _anonCount = 0;
		    _tabs = _tabIgnore;
	        _current = _root;
            switch (c) {
                case ':':
		            _mode = ReadMode.TextContent;
		            return;
				case '"':
					_stack.Push((char)ReadMode.AttributeName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char)ReadMode.AttributeName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case ',':
		            return;
				case '=':
					throw new BxlException("unexpected symbol " + c, _info);
                case ' ':
		            _symbolCount++;
					_mode = ReadMode.Indent;
		            return;
				case '\r':
                case '\n':
                    return;
                case '\t':
					if (_tabs == 0)
						_current = _current.Elements().Last();
					else
						_tabs--;
                    _mode = ReadMode.Indent;
                    return;
				case '\\':
					throw new BxlException("escape not in string", _info);
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c, _info);
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
                default:
                    _buf.Append(c);
					_mode = ReadMode.ElementName;
                    return;
            }
        }

		private void processElementName(char c) {
			switch (c) {
				case ':':
					saveValue();
					_stack.Push((char)_mode);
					_mode = ReadMode.Colon;
					return;
				case '"':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info);
					_stack.Push((char)ReadMode.AttributeName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info);
					_stack.Push((char)ReadMode.AttributeName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					saveValue();
					_mode = ReadMode.AttributeValue;
					return;
				case ',':
				case '\t':
				case ' ':
					saveValue();
					addNode();
					_mode = ReadMode.AttributeName;
					return;
				case '\r':
				case '\n':
					saveValue();
					addNode();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info);
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void processAttributeName(char c) {
			switch (c) {
				case ':':
					saveValue();
					_stack.Push((char)_mode);
					_mode = ReadMode.Colon;
					return;
				case '"':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info);
					addAnonAttribute();
					_stack.Push((char)_mode);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info);
					addAnonAttribute();
					_stack.Push((char)_mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					if (_isExpression)
						throw new BxlException("can not assign to expression", _info);
					saveValue();
					_mode = ReadMode.AttributeValue;
					return;
				case '\t':
				case ',':
				case ' ':
					saveValue();
					return;
				case '\r':
				case '\n':
					saveValue();
					addAnonAttribute();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info);
				case '(':
					addAnonAttribute();
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
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
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					addAnonAttribute();
					return;
			}
		}

	    private void processAttributeValue(char c) {
			switch (c) {
				case ':':
					if (_buf.Length == 0 && !_isString)
						return;
					addAttributeValue();
					_mode = ReadMode.TextContent;
					return;
				case '"':
					_stack.Push((char)_mode);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char)_mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					throw new BxlException("unexpected symbol " + c, _info);
				case '\t':
				case ',':
				case ' ':
					if (_buf.Length == 0)
						return;
					addAttributeValue();
					_mode = ReadMode.AttributeName;
					return;
				case '\r':
				case '\n':
					if (_buf.Length == 0 && !_isString)
						throw new BxlException("empty attribute value", _info);
					addAttributeValue();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info);
				case '(':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
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
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					return;
			}
	    }

		private void processSingleLineString(char c) {
			_isString = true;
			switch (c) {
				case '"':
				case '\'':
					if (_stack.Peek() == c) {
						_stack.Pop();
						_mode = (ReadMode) _stack.Pop();
						map[(int)_mode](' ');
					}
					else
						_buf.Append(c);
					return;
				case '\r':
				case '\n':
					throw new BxlException("new line in regular string", _info);
				case '\\':
					_stack.Push((char)_mode);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void processEscapingBackSlash(char c) {
			_buf.Append(c);
			_mode = (ReadMode) _stack.Pop();
		}

		private void processQuoting1(char c) {
			_isString = true;
			switch (c) {
				case '"':
					_mode = ReadMode.Quoting2;
					return;
				case '\n':
				case '\r':
					throw new BxlException("new line in regular string", _info);
				case '\\':
					_stack.Push('"');
					_stack.Push((char)ReadMode.SingleLineString);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append(c);
					_stack.Push('"');
					_mode = ReadMode.SingleLineString;
					return;
			}
		}

		private void processQuoting2(char c) {
			switch (c) {
				case '"':
					_mode = ReadMode.MultiLineString;
					return;
				default:
					map[_stack.Pop()](c);
					return;
			}
		}

		private void processUnquoting(char c) {
			switch (c) {
				case '"':
					_symbolCount++;
					if (_symbolCount == 3) {
						_mode = (ReadMode) _stack.Pop();
						_symbolCount = 0;
						map[(int)_mode](' ');
					}
					return;
				case '\\':
					_buf.Append('"', _symbolCount);
					_stack.Push((char)ReadMode.MultiLineString);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append('"', _symbolCount);
					_buf.Append(c);
					_mode = ReadMode.MultiLineString;
					return;
			}
		}

		private void processMultiLineString(char c) {
			switch (c) {
				case '"':
					_symbolCount = 1;
					_mode = ReadMode.Unquoting;
					return;
				case '\\':
					_stack.Push((char)_mode);
					_mode = ReadMode.EscapingBackSlash;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

	    private void processExpression(char c) {
		    _isExpression = true;
		    switch (c) {
			    case '\\':
					_stack.Push((char)_mode);
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

	    private void processTextContent(char c) {
		    switch (c) {
			    case ':':
				case '=':
					throw new BxlException("unexpected symbol " + c, _info);
				case '\\':
					throw new BxlException("escape not in string", _info);
				case ',':
				case ' ':
				case '\t':
				    addTextContent();
				    return;
				case '"':
					_stack.Push((char)_mode);
					_mode = ReadMode.Quoting1;
				    return;
				case '\'':
					_stack.Push((char)_mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '\r':
				case '\n':
					addTextContent();
					_mode = ReadMode.NewLine;
				    return;
				case '(':
				    if (_buf.Length == 0) {
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
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
				    return;
		    }
	    }

	    private void processWaitingForNL(char c) {
		    switch (c) {
			    case ' ':
				case ',':
				case '\t':
				    return;
				case '\n':
				case '\r':
					_mode = ReadMode.NewLine;
				    return;
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					throw new BxlException("unexpected symbol " + c, _info);
		    }
	    }

		private void processStart(char c) {
			switch (c) {
				case ':':
					_mode = ReadMode.TextContent;
					return;
				case ',':
				case ')':
					return;
				case '=':
				case '(':
					throw new BxlException("unexpected symbol " + c, _info);
				case '\\':
					throw new BxlException("escape not in string", _info);
				case '"':
					_stack.Push((char)ReadMode.NamespaceName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					_stack.Push((char)ReadMode.NamespaceName);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case ' ':
					_symbolCount++;
					if (_symbolCount == TAB_SPACE_COUNT) {
						_symbolCount = 0;
						processStart('\t');
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
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					_mode = ReadMode.NamespaceName;
					return;
			}
		}

		private void processNamespaceName(char c) {
			switch (c) {
				case ':':
					saveValue();
					_stack.Push((char)ReadMode.ElementName);
					_mode = ReadMode.Colon;
					return;
				case '\t':
				case ' ':
				case ',':
					saveValue();
					return;
				case '=':
					saveValue();
					_mode = ReadMode.NamespaceValue;
					return;
				case ')':
					return;
				case '(':
					if (_value.Length != 0) {
						addNode();
						_stack.Push((char)ReadMode.AttributeName);
						_stack.Push(c);
						_mode = ReadMode.Expression;
					} else
						throw new BxlException("unexpected symbol " + c, _info);
					return;
				case '"':
					if (_value.Length != 0) {
						addNode();
						_stack.Push((char)ReadMode.AttributeName);
						_mode = ReadMode.Quoting1;
					} else
						throw new BxlException("unexpected symbol " + c, _info);
					return;
				case '\'':
					if (_value.Length != 0) {
						addNode();
						_stack.Push((char)ReadMode.AttributeName);
						_stack.Push(c);
						_mode = ReadMode.SingleLineString;
					} else
						throw new BxlException("unexpected symbol " + c, _info);
					return;
				case '\n':
				case '\r':
					saveValue();
					addNode();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escaping not in string", _info);
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					if (_value.Length != 0) {
						addNode();
						_mode = ReadMode.AttributeName;
					}
					return;
			}
		}

		private void processNamespaceValue(char c) {
			switch (c) {
				case ':':
				case ',':
				case '=':
					throw new BxlException("unexpected symbol " + c, _info);
				case '"':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
						_mode = ReadMode.Quoting1;
					} else
						throw new BxlException("unexpected symbol " + c, _info);
					return;
				case '\'':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
						_stack.Push(c);
						_mode = ReadMode.SingleLineString;
					} else
						throw new BxlException("unexpected symbol " + c, _info);
					return;
				case '\t':
				case ' ':
					if (_buf.Length != 0) {
						addNamespace();
						_mode = ReadMode.WaitingForNL;
					}
					return;
				case '\n':
				case '\r':
					if (_buf.Length != 0) {
						addNamespace();
						_mode = ReadMode.Start;
					} else
						throw new BxlException("unexpected symbol " + c, _info);
					return;
				case '\\':
					throw new BxlException("escaping not in string", _info);
				case '(':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
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
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
				default:
					_buf.Append(c);
					return;
			}
		}

		private void processColon(char c) {
			switch (c) {
				case ':':
					if (_value.Length == 0)
						throw new BxlException("wrong namespace", _info);
					_prefix = _value;
					_value = "";
					_mode = (ReadMode)_stack.Pop();
					return;
				default:
					_mode = (ReadMode)_stack.Pop();
					if (_mode == ReadMode.ElementName)
						addNode();
					else if (_mode == ReadMode.AttributeName)
						addAnonAttribute();
					_mode = ReadMode.TextContent;
					map[(int)_mode](c);
					return;
			}
		}

		private void processCommentary(char c) {
			if (c == '\n' || c == '\r')
				_mode = (ReadMode) _stack.Pop();
		}

		//		modifying tree

	    private void addNode() {
			String s = _value.Escape(EscapingType.XmlName);

		    String ns = resolveNamespace();
		    XElement node = new XElement(XName.Get(s, ns));
		    if (!_options.HasFlag(BxlParserOptions.NoLexData)) {
			    node.Add(new XAttribute(INFO_FILE, _info.File));
				node.Add(new XAttribute(INFO_LINE, _info.Line));
		    }
		    _current.Add(node);
			_current = node;
			_isExpression = false;
			_isString = false;
			_value = "";
	    }

	    private void addAnonAttribute() {
		    if (_value.Length == 0)
			    return;
			if (_current == _root) 
				throw new BxlException("adding attribute to root not allowed", _info);

		    switch (_anonCount) {
			    case 0:
				    if (_prefix.Length != 0)
					    _prefix += "::";
					if (!_options.HasFlag(BxlParserOptions.OnlyIdAttibute))
						_current.SetAttributeValue(XName.Get(ANON_CODE), _prefix + _value);
					if (!_options.HasFlag(BxlParserOptions.OnlyCodeAttribute))
						_current.SetAttributeValue(XName.Get(ANON_ID), _prefix + _value);
				    _prefix = "";
				    break;
				case 1:
					if (_prefix.Length != 0)
						_prefix += "::";
					_current.SetAttributeValue(XName.Get(ANON_NAME), _prefix + _value);
				    _prefix = "";
				    break;
				default:
				    if (_isString || _isExpression) {
					    int att_count = _current.Attributes().Count();
					    if (!_options.HasFlag(BxlParserOptions.NoLexData))
						    att_count -= 2;
					    String name = ANON_PREFIX + (att_count + 1);

						// namespace not needed because this anonymous string attribute can not be declared with namespace prefix
						_current.SetAttributeValue(XName.Get(name), _value);
				    } else
						_current.SetAttributeValue(XName.Get(_value.Escape(EscapingType.XmlName), resolveNamespace()), ANON_VALUE);
				    break;
		    }
		    _isExpression = false;
			_isString = false;
		    _value = "";
		    _anonCount++;
	    }

	    private void saveValue() {
			if (_buf.Length == 0)
				return;
			_value = _buf.ToString();
			_buf.Clear();
	    }

	    private void addAttributeValue() {
			// checking for empty _buf must be implemented in invoker !
			if (_value.Length == 0)
				throw new BxlException("empty attribute name", _info);
			if (_current == _root)
				throw new BxlException("adding attribute to root not allowed", _info);

		    String ns = resolveNamespace();
		    String s = _buf.ToString();
			_buf.Clear();
			_current.SetAttributeValue(XName.Get(_value.Escape(EscapingType.XmlName), ns), s);
			_isExpression = false;
			_isString = false;
		    _value = "";
	    }

	    private void addTextContent() {
		    if (_buf.Length == 0 && !_isString)
			    return;
		    String s = _buf.ToString();
		    _buf.Clear();
			_isExpression = false;
			_isString = false;

		    _current.Add(new XText(s));
			_mode = ReadMode.WaitingForNL;
	    }

	    private String resolveNamespace() {
		    String ns = "";
			if (_prefix.Length != 0) {
				XNamespace xns = _root.GetNamespaceOfPrefix(_prefix);
				if (xns == null) {
					foreach (var i in QorpentConst.Xml.WellKnownNamespaces)
						if (i.Value == _prefix) {
							ns = i.Key;
							_root.SetAttributeValue(XNamespace.Xmlns + _prefix, ns);
							break;
						}
					ns = ns == "" ? addDefaultNamespace(_prefix) : ns;
				} else 
					ns = xns.NamespaceName;
				_prefix = "";
			}
		    return ns;
	    }

		private void addNamespace() {
			// checking for empty _buf must be implemented in invoker !
			if (_value.Length == 0)
				throw new BxlException("empty namespace name", _info);
			String s = _value.Escape(EscapingType.XmlName);
			_root.SetAttributeValue(XNamespace.Xmlns + s, _buf.ToString());
			_isExpression = false;
			_isString = false;
			_value = "";
			_buf.Clear();
		}

		private String addDefaultNamespace(String prefix) {
			String s = prefix.Escape(EscapingType.XmlName);
			String ns = DEFAULT_NS_PREFIX + new string('X', ++_defNsCount);
			_root.SetAttributeValue(XNamespace.Xmlns + s, ns);
			return ns;
		}

		/// <summary>
		/// modified method Pop and Peak - if stack is empty they return 0, not excption
		/// </summary>
		private class CharStack : Stack<char> {
			
			public new char Pop() {
				if (this.IsNotEmpty())
					return base.Pop();
				return (char)0;
			}

			public new char Peek() {
				if (this.IsNotEmpty())
					return base.Peek();
				return (char) 0;
			}
		}
    }
}
