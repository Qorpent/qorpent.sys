using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.Serialization;
using Qorpent.Dsl;
using Qorpent.Utils.Extensions;

#if !EMBEDQPT
using Qorpent.BSharp;
using Qorpent.Events;
using Qorpent.IoC;
using Qorpent.Utils;
#endif

namespace Qorpent.Bxl {
	/// <summary>
	/// 
	/// </summary>
#if !EMBEDQPT
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof(IBxlParser))]
	[ContainerComponent(Lifestyle.Transient, ServiceType = typeof(ISpecialXmlParser), Name = "bxl.xml.parser")]
#endif
	public class BxlParser : IBxlParser, ISpecialXmlParser

    {
		private const string NAMESPACE = "namespace::";
		private const string CODE = "code";
		private const string ID = "id";
		private const string NAME = "name";

		private const int TAB_SPACE_COUNT = 4;
		private readonly XName ROOT_NAME = "root";
		private readonly XName INFO_FILE = "_file";
		private readonly XName INFO_LINE = "_line";
		private const string ANON_PREFIX = "_aa";
		private const string ANON_VALUE = "1";
		private string DEFAULT_NS_PREFIX;
		private string ANON_CODE;
		private string ANON_ID;
		private string ANON_NAME;

		private BxlParserOptions _options;
		private XElement _root, _current;
		private ReadMode _mode;
		private readonly StringBuilder _buf = new StringBuilder(256);
		private readonly CharStack _stack = new CharStack();
		private int _symbolCount;
		private int _tabIgnore;
		private int _tabs;
		private int _defNsCount;
		private string _value;
		private string _prefix;
		private bool _isString;
		private bool _isExpression;
	    private char _next = '\0';
	    private LexInfo _info;
		private readonly CharStack _expStack = new CharStack();

		private int _level;
		private List<Stats> _anon;

		private delegate void stateOperation(char c);
		private readonly stateOperation[] map;

		/// <summary>
		/// 
		/// </summary>
		public BxlParser() {
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
				processCommentary,
				processMultilineCommentary
            };
		}

		/// <summary>
		/// 	Parses source code into Xml
		/// </summary>
		/// <param name="code"></param>
		/// <param name="filename"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public XElement Parse(string code = null, string filename = "code.bxl", BxlParserOptions options = BxlParserOptions.None) {
			if (string.IsNullOrWhiteSpace(code)) {
				if (string.IsNullOrWhiteSpace(filename))
					return new XElement("root");
				code = File.ReadAllText(filename);
			}

			if (string.IsNullOrWhiteSpace(filename)){
				filename = "code.bxl";
			}
			init(filename, options);
		    int l = code.Length;
			for (var i=0;i<l;i++) {
			    if (_skip > 0) {
			        _skip--;
                    continue;
			    }
			    var c = code[i];
			    if (i < code.Length - 1) {
			        _next = code[i + 1];
			    }
			    else {
			        _next = '\0';
			    }
				_info.CharIndex++;
				_info.Column++;
			    if (c == 160) {
			        c = ' ';
			    }
				if (c == '\t'){
					_info.Column += 3;
				}
			    if (c == '\r') {
			        continue;
			    }
				map[(int)_mode](c);

				if (c == '\n' ) {
					_info.Line++;
					_info.Column = 0;
				}
			}

			if ( code.Last() != '\n')
				map[(int)_mode]('\n');

			if (_options.HasFlag(BxlParserOptions.ExtractSingle) && _root.Elements().Count() == 1) {
				_current = _root.Elements().First();
				_current.Remove();
				// explicit copy namespaces from _root to _current ?
				_root = _current;
			}

			if (_stack.IsNotEmpty())
				throw new BxlException("invalid quotes or braces",_info.Clone());
			if (!options.HasFlag(BxlParserOptions.NoLexData)){
				_root.SetAttr("_file", filename);
			}

            if (options.HasFlag(BxlParserOptions.PerformInterpolation)) {
			    _root = _root.Interpolate();
			}

			return _root;
		}

		/// <summary>
		///		Generates BXL code from XML with given settings
		/// </summary>
		/// <param name="sourcexml"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public string Generate(XElement sourcexml, BxlGeneratorOptions options = null) {
			return new BxlGenerator().Convert(sourcexml, options);
		}

		XElement ISpecialXmlParser.ParseXml(string srccode) {
			return Parse(srccode, "isxp");
		}

		private void init(string filename, BxlParserOptions options) {
			_options = options;
			_info = new LexInfo(filename, 1);
            _expStack.Clear();
			DEFAULT_NS_PREFIX = NAMESPACE + filename + "_";
			string __ = "";
			if (_options.HasFlag(BxlParserOptions.SafeAttributeNames)) {
				__ = "__";
			}
			ANON_CODE = __ + CODE;
			ANON_ID = __ + ID;
			ANON_NAME = __ + NAME;
		    _skip = 0;
			_level = -1;
			_anon = new List<Stats>() { new Stats() };
			_symbolCount = 0;
			_tabIgnore = 0;
			_tabs = 0;
			_defNsCount = 0;
			_value = "";
			_prefix = "";
			_isString = false;
			_isExpression = false;
		    _buf.Clear();
			_stack.Clear();
		    _next = '\0';

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
					throw new BxlException("unexpected symbol " + c, _info.Clone());
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
					if (_tabs == 0) {
						_level++;
						if (_level >= _anon.Count)
							_anon.Add(new Stats());
						_current = _current.LastNode as XElement ?? _current;
					} else
						_tabs--;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
                case '/':
                    if (_next == '*')
                    {
                        _stack.Push((char)_mode);
                        _mode = ReadMode.MultilineCommentary;
                        mlcdepth = 1;
                        _skip = 1;
                        return;
                    }
                    goto default;
                default:
					_buf.Append(c);
					_mode = ReadMode.ElementName;
					return;
			}
		}

		private void processNewLine(char c) {
			if (char.IsWhiteSpace(c))
			{
				_expStack.Clear();
			}
			char s = _stack.Peek();
			if (s == '\'' || s == '"')
				throw new BxlException("new line in regular string", _info.Clone());

			_symbolCount = 0;
			_tabs = _tabIgnore;
			_current = _root;
			_level = -1;
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
					map[(int)_mode](c);
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
				case ')':
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
                case '/':
                    if (_next == '*')
                    {
                        _stack.Push((char)_mode);
                        _mode = ReadMode.MultilineCommentary;
                        mlcdepth = 1;
                        _skip = 1;
                        return;
                    }
                    goto default;
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
					if (_value.Length != 0)
						addNode();
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					_stack.Push((char)ReadMode.AttributeName);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					if (_value.Length != 0)
						addNode();
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
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
					return;
				case '\r':
				case '\n':
					saveValue();
					addNode();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
                case '/':
                    if (_next == '*')
                    {
                        saveValue();
                        map[(int)_mode](' ');
                        _stack.Push((char)_mode);
                        _mode = ReadMode.MultilineCommentary;
                        mlcdepth = 1;
                        _skip = 1;
                        return;
                    }
                    goto default;
                default:
					if (_value.Length != 0) {
						addNode();
						_mode = ReadMode.AttributeName;
						if (c == '('){
							_stack.Push((char)_mode);
							_stack.Push('(');
							_mode = ReadMode.Expression;
							_expStack.Clear();
							_expStack.Push('(');
						}
					}
					_buf.Append(c);
					return;
			}
		}

		private void processAttributeName(char c) {
			if (!char.IsWhiteSpace(c) && c!='=' && c!=',' && c!=' ' && c!='\t' && c!='#'){
				if (_expStack.Count != 0){
				    if (!(_expStack.Count == 1 && _expStack.Peek() == '\0')) {
				        throw new BxlException("not terminated expression", lexinfo: _info.Clone());
				    }
				}
			}
			
			switch (c) {
				case ':':
					saveValue();
					_stack.Push((char)_mode);
					_mode = ReadMode.Colon;
					return;
				case '"':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					addAnonAttribute();
					_stack.Push((char)_mode);
					_mode = ReadMode.Quoting1;
					return;
				case '\'':
					if (_prefix.Length != 0)
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					addAnonAttribute();
					_stack.Push((char)_mode);
					_stack.Push(c);
					_mode = ReadMode.SingleLineString;
					return;
				case '=':
					if (_isExpression)
						throw new BxlException("can not assign to expression", _info.Clone());
					saveValue();
					_isString = false;
					_isExpression = false;
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
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
					addAnonAttribute();
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
						_stack.Push(c);
						_mode = ReadMode.Expression;
						_expStack.Clear();
						_expStack.Push(c);
					}
					_buf.Append(c);
					return;
				case ')':
					if(_expStack.Count!=0)throw new BxlException("invalid expression finishing");
					if (_buf.Length != 0)
						_buf.Append(c);
					return;
				case '#':
					saveValue();
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
                case '/':
                    if (_next == '*')
                    {
                        saveValue();
                        map[(int)_mode](' ');
                        _stack.Push((char)_mode);
                        _mode = ReadMode.MultilineCommentary;
                        mlcdepth = 1;
                        _skip = 1;
                        return;
                    }
                    goto default;
                default:
					_buf.Append(c);
					addAnonAttribute();
					return;
			}
		}

		private void processAttributeValue(char c) {
			if (char.IsWhiteSpace(c)){
				_expStack.Clear();
			}
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
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '\t':
				case ',':
				case ' ':
					if (_buf.Length == 0 && !_isString)
						return;
					addAttributeValue();
					_mode = ReadMode.AttributeName;
					return;
				case '\r':
				case '\n':
					if (_buf.Length == 0 && !_isString)
						throw new BxlException("empty attribute value", _info.Clone());
					addAttributeValue();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
				case '(':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
						_stack.Push(c);
						_mode = ReadMode.Expression;
						_expStack.Clear();
						_expStack.Push(c);
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
						_mode = (ReadMode)_stack.Pop();
						map[(int)_mode](' ');
					} else
						_buf.Append(c);
					return;
				case '\r':
				case '\n':
					throw new BxlException("new line in regular string", _info.Clone());
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
			if (c == 't'){
				c = '\t';
			}
			else if(c=='n'){
				c = '\n';
			}else if (c == 'r'){
				c = '\r';
			}
			_buf.Append(c);
			_mode = (ReadMode)_stack.Pop();
		}

		private void processQuoting1(char c) {
			_isString = true;
			switch (c) {
				case '"':
					_mode = ReadMode.Quoting2;
					return;
				case '\n':
				case '\r':
					throw new BxlException("new line in regular string", _info.Clone());
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
						_mode = (ReadMode)_stack.Pop();
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
			char last;
			switch (c) {
				case '\\':
					_stack.Push((char)_mode);
					_mode = ReadMode.EscapingBackSlash;
					return;
				case '(':
					_expStack.Push(c);
					_buf.Append(c);
					_stack.Push('(');
					return;
				case ')':
					if (0 == _expStack.Count) throw new BxlException("exp stack finished before needed ", _info.Clone());
			        last = _expStack.Pop();
					if (last != '(') throw new BxlException("non matched expression ", _info.Clone());
					_buf.Append(c);
					_stack.Pop();
					if (_expStack.Count == 0){
						_expStack.Push('\0');
					}
					if (_stack.Peek() != '('){
						
						_mode = (ReadMode) _stack.Pop();
					}
					return;
				case '[':
					_expStack.Push(c);
					_buf.Append(c);
					_stack.Push('(');
					return;
				case ']':
					if (0 == _expStack.Count) throw new BxlException("exp stack finished before needed ", _info.Clone());
			        last = _expStack.Pop();
					if (last != '[') throw new BxlException("non matched expression ", _info.Clone());
					if (_expStack.Count == 0)
					{
						_expStack.Push('\0');
					}
					_buf.Append(c);
					_stack.Pop();
					return;
				case '{':
					_expStack.Push(c);
					_buf.Append(c);
					_stack.Push('(');
					return;
				case '}':
					if (0 == _expStack.Count) throw new BxlException("exp stack finished before needed ", _info.Clone());
			        last = _expStack.Pop();
					if (last != '{') throw new BxlException("non matched expression ", _info.Clone());
					if (_expStack.Count == 0)
					{
						_expStack.Push('\0');
					}
					_buf.Append(c);
					_stack.Pop();
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
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
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
						_stack.Push((char)_mode);
						_stack.Push(c);

						_mode = ReadMode.Expression;
						_expStack.Clear();
						_expStack.Push('(');
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
                case '/':
                    if (_next == '*')
                    {
                        _stack.Push((char)_mode);
                        _mode = ReadMode.MultilineCommentary;
                        mlcdepth = 1;
                        _skip = 1;
                        return;
                    }
                    goto default;
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
                case '/':
                    if (_next == '*')
                    {
                        _stack.Push((char)_mode);
                        _mode = ReadMode.MultilineCommentary;
                        mlcdepth = 1;
                        _skip = 1;
                        return;
                    }
                    goto default;
                default:
					throw new BxlException("unexpected symbol " + c, _info.Clone());
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
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '\\':
					throw new BxlException("escape not in string", _info.Clone());
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
                case '/':
			        if (_next == '*') {
			            _stack.Push((char) _mode);
			            _mode = ReadMode.MultilineCommentary;
			            mlcdepth = 1;
			            _skip = 1;
			            return;
			        }
			        goto default;
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
						_buf.Append(c);
						_mode = ReadMode.Expression;
						_expStack.Clear();
						_expStack.Push(c);
					} else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '"':
					if (_value.Length != 0) {
						addNode();
						_stack.Push((char)ReadMode.AttributeName);
						_mode = ReadMode.Quoting1;
					} else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\'':
					if (_value.Length != 0) {
						addNode();
						_stack.Push((char)ReadMode.AttributeName);
						_stack.Push(c);
						_mode = ReadMode.SingleLineString;
					} else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\n':
				case '\r':
					saveValue();
					addNode();
					_mode = ReadMode.NewLine;
					return;
				case '\\':
					throw new BxlException("escaping not in string", _info.Clone());
				case '#':
					map[(int)_mode]('\n');
					_stack.Push((char)_mode);
					_mode = ReadMode.Commentary;
					return;
                case '/':
                    if (_next == '*')
                    {
                        map[(int)_mode](' ');
                        _stack.Push((char)_mode);
                        _mode = ReadMode.MultilineCommentary;
                        mlcdepth = 1;
                        _skip = 1;
                        return;
                    }
                    goto default;
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
					throw new BxlException("unexpected symbol " + c, _info.Clone());
				case '"':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
						_mode = ReadMode.Quoting1;
					} else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\'':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
						_stack.Push(c);
						_mode = ReadMode.SingleLineString;
					} else
						throw new BxlException("unexpected symbol " + c, _info.Clone());
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
						throw new BxlException("unexpected symbol " + c, _info.Clone());
					return;
				case '\\':
					throw new BxlException("escaping not in string", _info.Clone());
				case '(':
					if (_buf.Length == 0) {
						_stack.Push((char)_mode);
						_stack.Push(c);
						_mode = ReadMode.Expression;
						_expStack.Clear();
						_expStack.Push(c);
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
						throw new BxlException("wrong namespace", _info.Clone());
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
				_mode = (ReadMode)_stack.Pop();
		}
	    private int mlcdepth;
	    private int _skip;

	    private void processMultilineCommentary(char c)
        {
            if (c == '/') {
                if (_next == '*') {
                    mlcdepth++;
                    _skip = 1;
                }
            }else if (c == '*') {
                if (_next == '/') {
                    mlcdepth--;
                    _skip = 1;
                    if (mlcdepth == 0) {
                        _mode = (ReadMode)_stack.Pop();
                    }
                }
            }
        }

        //		modifying tree

        private void addNode() {
			string s = _value.Escape(EscapingType.XmlName);

			string ns = resolveNamespace();
			XElement node = new XElement(XName.Get(s, ns));
			if (!_options.HasFlag(BxlParserOptions.NoLexData)) {
				node.Add(new XAttribute(INFO_FILE, _info.File));
				node.Add(new XAttribute(INFO_LINE, _info.Line));
			}
			_current.Add(node);
			_current = node;

			processIndent('\t');
			_anon[_level].reset();

			_isExpression = false;
			_isString = false;
			_value = "";
		}

		private void addAnonAttribute() {
			if (_value.Length == 0)
				return;
			if (_current == _root){
				if(_options.HasFlag(BxlParserOptions.PreventRootAttributes))
					throw new BxlException("adding attribute to root not allowed  if strict PreventRootAttributes mode set on", _info.Clone());
			}
			if (!_anon[_level].hasCodeId) {
				if (_prefix.Length != 0)
					_prefix += "::";
				if (!_options.HasFlag(BxlParserOptions.OnlyIdAttibute)) {
					_current.SetAttributeValue(XName.Get(ANON_CODE), _prefix + _value);
					_anon[_level].count++;
				}
				if (!_options.HasFlag(BxlParserOptions.OnlyCodeAttribute)) {
					if (_current.Attribute("id") == null || _current.Attribute("id").Value == ""){
						_current.SetAttributeValue(XName.Get(ANON_ID), _prefix + _value);
					}
					
					_anon[_level].count++;
				}
				_anon[_level].hasCodeId = true;
			}
			else if (!_anon[_level].hasName && !_current.Attributes().Any(_ => _.Name.LocalName != "id" && _.Name.LocalName != "code" && _.Name.LocalName != "_file" && _.Name.LocalName != "_line"))
			{
				if (_prefix.Length != 0)
					_prefix += "::";
				_current.SetAttributeValue(XName.Get(ANON_NAME), _prefix + _value);
				_anon[_level].count++;
				_anon[_level].hasName = true;
			} else {
				if (_isString || _isExpression) {
					// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
					_anon[_level].count++;
					string name = ANON_PREFIX + (_anon[_level].count);

					// namespace not needed because this anonymous string attribute can not be declared with namespace prefix
					_current.SetAttributeValue(XName.Get(name), _value);
				} else
					_current.SetAttributeValue(XName.Get(_value.Escape(EscapingType.XmlName), resolveNamespace()), ANON_VALUE);
			}

			_isExpression = false;
			_isString = false;
			_value = "";
			_prefix = "";
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
				throw new BxlException("empty attribute name", _info.Clone());
			if (_current == _root){
				if (_options.HasFlag(BxlParserOptions.PreventRootAttributes)){
					throw new BxlException("adding attribute to root not allowed  if strict PreventRootAttributes mode set on", _info.Clone());
				}
			}
				

			string ns = resolveNamespace();
			string s = _buf.ToString();
			_buf.Clear();
			var name = XName.Get(_value.Escape(EscapingType.XmlName), ns);
			if (_options.HasFlag(BxlParserOptions.PreventDoubleAttributes)){
				if (name != "id" && name != "code" && name != "name"){
					if (null != _current.Attribute(name)){
						throw new BxlException("cannot add doubled attributes if strict PreventDoubleAttributes mode set on" );
					}
				}
			}
			if (name != "id" && name != "code" && name != "name" &&  _level!=-1){

				_anon[_level].count += 3;
				_anon[_level].hasCodeId= true;
				_anon[_level].hasName = true;
			}
			_current.SetAttributeValue(name, s);
			_isExpression = false;
			_isString = false;
			_value = "";
		}

		private void addTextContent() {
			if (_buf.Length == 0 && !_isString)
				return;
			string s = _buf.ToString();
			_buf.Clear();
			_isExpression = false;
			_isString = false;

			_current.Add(new XText(s));
			_mode = ReadMode.WaitingForNL;
		}

		private string resolveNamespace() {
			string ns = "";
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
				throw new BxlException("empty namespace name", _info.Clone());
			string s = _value.Escape(EscapingType.XmlName);
			_root.SetAttributeValue(XNamespace.Xmlns + s, _buf.ToString());
			_isExpression = false;
			_isString = false;
			_value = "";
			_buf.Clear();
		}

		private string addDefaultNamespace(string prefix) {
			string s = prefix.Escape(EscapingType.XmlName);
			string ns = DEFAULT_NS_PREFIX + new string('X', ++_defNsCount);
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
				return (char)0;
			}
		}

		private class Stats {
			public bool hasCodeId;
			public bool hasName;
			public int count;

			public void reset() {
				hasCodeId = hasName = false;
				count = 0;
			}
		}
	}
}
