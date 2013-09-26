using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Bxl2 {
    /// <summary>
    /// 
    /// </summary>
    public class BxlParser2 : IBxlParser {
	    private const int TAB_SPACE_COUNT = 4;
	    private const String ROOT_NAME = "root";
	    private const String ANON_CODE = "code";
	    private const String ANON_NAME = "name";
	    private const String ANON_ID = "id";
	    private const String ANON_PREFIX = "_aa";
	    private const String ANON_VALUE = "1";
	    private String _default_namespace_prefix = "namespace::";

        //private int line_number = 1;
        private XElement _root, _current;
        private ReadMode _mode;
        private readonly StringBuilder _buf = new StringBuilder(256);
        private readonly CharStack _stack = new CharStack();
	    private int _symbolCount;
	    private int _anonCount;
	    private String _value;
	    private bool _isString;
	    private bool _isExpression;

        private delegate void stateOperation(char c);
        private Dictionary<ReadMode, stateOperation> map;

        /// <summary>
        /// 
        /// </summary>
        public BxlParser2() {
            map = new Dictionary<ReadMode, stateOperation>() {
                {ReadMode.ElementName,			processElementName},
				{ReadMode.AttributeName,		processAttributeName},
				{ReadMode.AttributeValue,		processAttributeValue},
                {ReadMode.Indent,				processIndent},
                {ReadMode.NewLine,				processNewLine},
				{ReadMode.SingleLineString,		processSingleLineString},
				{ReadMode.EscapingBackSlash,	processEscapingBackSlash},
				{ReadMode.Quoting1,				processQuoting1},
				{ReadMode.Quoting2,				processQuoting2},
				{ReadMode.Unquoting,			processUnquoting},
				{ReadMode.MultiLineString,		processMultiLineString},
				{ReadMode.Expression,			processExpression},
				{ReadMode.TextContent,			processTextContent},
				{ReadMode.WaitingForNL,			processWaitingForNL},
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

	        _default_namespace_prefix += filename + "_";
			_symbolCount = 0;
			_anonCount = 0;
			_value = "";
			_isString = false;
			_isExpression = false;
	        _buf.Clear();
			_stack.Clear();

            _root = new XElement(ROOT_NAME);
	        _current = _root;
            _mode = ReadMode.NewLine;
            foreach (char c in code) {
                map[_mode](c);
            }

	        if (code.Last() != '\r' && code.Last() != '\n')
		        map[_mode]('\n');

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
					throw new BxlException("unexpected symbol " + c);
                case ' ':
					if (++_symbolCount == TAB_SPACE_COUNT) {
			            _symbolCount = 0;
						processIndent('\t');
		            }
					return;
				case '\r':
                case '\n':
                    throw new BxlException("unexpected '\\r' or '\\n' symbol");
                case '\t':
                    _current = _current.Elements().Last();
                    return;
				case '\\':
					throw new BxlException("escape not in string");
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c);
                default:
                    _buf.Append(c);
                    _mode = ReadMode.ElementName;
                    return;
            }
        }

	    private void processNewLine(char c) {
			char s = _stack.Peek();
			if (isQuote(s))
				throw new BxlException("new line in regular string");

		    _anonCount = 0;
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
					throw new BxlException("unexpected symbol " + c);
                case ' ':
		            _symbolCount++;
					_mode = ReadMode.Indent;
		            return;
				case '\r':
                case '\n':
                    return;
                case '\t':
                    _current = _current.Elements().Last();
                    _mode = ReadMode.Indent;
                    return;
				case '\\':
					throw new BxlException("escape not in string");
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c);
                default:
                    _buf.Append(c);
					_mode = ReadMode.ElementName;
                    return;
            }
        }

		private void processElementName(char c) {
			switch (c) {
				case ':':
					addNode();
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
				case '=':
					saveAttributeName();
					_mode = ReadMode.AttributeValue;
					return;
				case ',':
				case '\t':
				case ' ':
					addNode();
					_mode = ReadMode.AttributeName;
					return;
				case '\r':
				case '\n':
					addNode();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string");
				default:
					_buf.Append(c);
					return;
			}
		}

		private void processAttributeName(char c) {
			switch (c) {
				case ':':
					saveAttributeName();
					addAnonAttribute();
					_mode = ReadMode.TextContent;
					return;
				case '"':
					addAnonAttribute();
					_stack.Push((char)_mode);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					addAnonAttribute();
					_stack.Push((char)_mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					if (_isExpression)
						throw new BxlException("can not assign to expression");
					saveAttributeName();
					_mode = ReadMode.AttributeValue;
					return;
				case '\t':
				case ',':
				case ' ':
					saveAttributeName();
					return;
				case '\r':
				case '\n':
					saveAttributeName();
					addAnonAttribute();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string");
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
					throw new BxlException("unexpected symbol " + c);
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
						throw new BxlException("empty attribute value");
					addAttributeValue();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string");
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
						map[_mode](' ');
					}
					else
						_buf.Append(c);
					return;
				case '\r':
				case '\n':
					throw new BxlException("new line in regular string");
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
					throw new BxlException("new line in regular string");
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
					map[(ReadMode)_stack.Pop()](c);
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
						map[_mode](' ');
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
					throw new BxlException("unexpected symbol " + c);
				case '\\':
					throw new BxlException("escape not in string");
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
				default:
					throw new BxlException("unexpected symbol " + c);
		    }
	    }

		//		modifying tree

	    private void addNode() {
			String s = _buf.ToString().Escape(EscapingType.XmlName);
		    _buf.Clear();
			XElement node = new XElement(s);
			_current.Add(node);
			_current = node;
	    }

	    private void addAnonAttribute() {
		    if (_value.Length == 0)
			    return;
		    switch (_anonCount) {
			    case 0:
					_current.SetAttributeValue(XName.Get(ANON_CODE), _value);
					_current.SetAttributeValue(XName.Get(ANON_ID), _value);
				    break;
				case 1:
					_current.SetAttributeValue(XName.Get(ANON_NAME), _value);
				    break;
				default:
				    if (_isString) {
					    String name = ANON_PREFIX + (_current.Attributes().Count() + 1);
						_current.SetAttributeValue(XName.Get(name), _value);
				    } else
						_current.SetAttributeValue(XName.Get(_value.Escape(EscapingType.XmlName)), ANON_VALUE);
				    break;
		    }
		    _isExpression = false;
			_isString = false;
			_value = "";
		    _anonCount++;
	    }

	    private void saveAttributeName() {
			if (_buf.Length == 0)
				return;
			_value = _buf.ToString();
			_buf.Clear();
	    }

	    private void addAttributeValue() {
			// checking for empty values must be implemented in invoker !
		    String s = _buf.ToString();
			_buf.Clear();
			_current.SetAttributeValue(XName.Get(_value.Escape(EscapingType.XmlName)), s);
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

		//		utils

		private static bool isQuote(char c) {
			return c == '\'' || c == '"';
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
