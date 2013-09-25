using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        //private int line_number = 1;
        private XElement root, current;
        private ReadMode mode;
        private StringBuilder buf = new StringBuilder(256);
        private CharStack stack = new CharStack();
	    private int symbol_count = 0;
	    private int anon_count = 0;
	    private String value = "";
	    private bool isString = false;
	    //private bool multiLineString = false;
		//private bool escapeCharacter = false;

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

            root = new XElement("root");
	        current = root;
            mode = ReadMode.NewLine;
            foreach (char c in code) {
                map[mode](c);
            }

            return root;
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
					throw new BxlException("not implemented: " + c.ToString());

				case '"':
				case '\'':
					stack.Push((char)ReadMode.AttributeName);
					stack.Push(c);
					mode = ReadMode.SingleLineString;
		            return;
				case ',':
		            return;
				case '=':
					throw new BxlException("unexpected '=' symbol");
                case ' ':
					if (++symbol_count == TAB_SPACE_COUNT) {
			            symbol_count = 0;
						processIndent('\t');
		            }
					return;
				case '\r':
                case '\n':
                    throw new BxlException("unexpected '\\r' or '\\n' symbol");
                case '\t':
                    current = current.Elements().Last();
                    return;
				case '\\':
					throw new BxlException("escape not in string");
                default:
                    buf.Append(c);
                    mode = ReadMode.ElementName;
                    return;
            }
        }

	    private void processNewLine(char c) {
			char s = stack.Peek();
			if (isQuote(s))
				throw new BxlException("new line in regular string");

		    anon_count = 0;
	        current = root;
            switch (c) {
                case ':':
					throw new BxlException("not implemented: " + c.ToString());

				case '"':
				case '\'':
					stack.Push((char)ReadMode.AttributeName);
					stack.Push(c);
					mode = ReadMode.SingleLineString;
					return;
				case ',':
		            return;
				case '=':
					throw new BxlException("unexpected '=' symbol");
                case ' ':
		            symbol_count++;
					mode = ReadMode.Indent;
		            return;
				case '\r':
                case '\n':
                    return;
                case '\t':
                    current = current.Elements().Last();
                    mode = ReadMode.Indent;
                    return;
				case '\\':
					throw new BxlException("escape not in string");
                default:
                    buf.Append(c);
					mode = ReadMode.ElementName;
                    return;
            }
        }

		private void processElementName(char c) {
			switch (c) {
				case ':':
					throw new BxlException("not implemented: " + c.ToString());

				case '"':
				case '\'':
					stack.Push((char)ReadMode.AttributeName);
					stack.Push(c);
					mode = ReadMode.SingleLineString;
					return;
				case ',':
					processElementName(' ');
					return;
				case '=':
					saveAttributeName();
					mode = ReadMode.AttributeValue;
					return;
				case ' ':
					addNode();
					mode = ReadMode.AttributeName;
					return;
				case '\r':
				case '\n':
					addNode();
					mode = ReadMode.NewLine;
					return;
				case '\t':
					processElementName(' ');
					return;
				case '\\':
					throw new BxlException("escape not in string");
				default:
					buf.Append(c);
					return;
			}
		}

		private void processAttributeName(char c) {
			switch (c) {
				case ':':
					throw new BxlException("not implemented: " + c.ToString());

				case '"':
				case '\'':
					addAnonAttribute();
					stack.Push((char)mode);
					stack.Push(c);
					mode = ReadMode.SingleLineString;
					return;
				case ',':
					processAttributeName(' ');
					return;
				case '=':
					saveAttributeName();
					mode = ReadMode.AttributeValue;
					return;
				case ' ':
					saveAttributeName();
					return;
				case '\r':
				case '\n':
					saveAttributeName();
					addAnonAttribute();
					mode = ReadMode.NewLine;
					return;
				case '\t':
					processAttributeName(' ');
					return;
				case '\\':
					throw new BxlException("escape not in string");
				default:
					buf.Append(c);
					addAnonAttribute();
					return;
			}
		}

	    private void processAttributeValue(char c) {
			switch (c) {
				case ':':
					throw new BxlException("not implemented: " + c.ToString());

				case '"':
				case '\'':
					stack.Push((char)mode);
					stack.Push(c);
					mode = ReadMode.SingleLineString;
					return;
				case ',':
					processAttributeValue(' ');
					return;
				case '=':
					throw new BxlException("unexpected '=' symbol");
				case ' ':
					if (buf.Length == 0)
						return;
					addAttributeValue();
					mode = ReadMode.AttributeName;
					return;
				case '\r':
				case '\n':
					if (buf.Length == 0)
						throw new BxlException("empty attribute value");
					addAttributeValue();
					mode = ReadMode.NewLine;
					return;
				case '\t':
					processAttributeValue(' ');
					return;
				case '\\':
					throw new BxlException("escape not in string");
				default:
					buf.Append(c);
					return;
			}
	    }

		private void processSingleLineString(char c) {
			isString = true;
			switch (c) {
				case ':':
					throw new BxlException("not implemented: " + c.ToString());

				case '"':
				case '\'':
					if (stack.Peek() == c) {
						stack.Pop();
						mode = (ReadMode) stack.Pop();
					}
					else
						buf.Append(c);
					return;
				case '\r':
				case '\n':
					throw new BxlException("new line in regular string");
				case '\\':
					stack.Push((char)mode);
					mode = ReadMode.EscapingBackSlash;
					return;
				default:
					buf.Append(c);
					return;
			}
		}

		private void processEscapingBackSlash(char c) {
			buf.Append(c);
			mode = (ReadMode) stack.Pop();
		}

		//		modifying tree

	    private void addNode() {
			String s = buf.ToString().Escape(EscapingType.XmlName);
		    buf.Clear();
			XElement node = new XElement(s);
			current.Add(node);
			current = node;
	    }

	    private void addAnonAttribute() {
		    if (value.Length == 0)
			    return;
		    switch (anon_count) {
			    case 0:
					current.SetAttributeValue(XName.Get("code"), value);
					current.SetAttributeValue(XName.Get("id"), value);
				    break;
				case 1:
					current.SetAttributeValue(XName.Get("name"), value);
				    break;
				default:
				    if (isString) {
					    String name = "_aa" + (current.Attributes().Count() + 1);
						current.SetAttributeValue(XName.Get(name), value);
						isString = false;
				    } else
						current.SetAttributeValue(XName.Get(value.Escape(EscapingType.XmlName)), "1");
				    break;
		    }
			value = "";
		    anon_count++;
	    }

	    private void saveAttributeName() {
			if (buf.Length == 0)
				return;
			value = buf.ToString();
			buf.Clear();
	    }

	    private void addAttributeValue() {
			// checking for empty values must be implemented in invoker !
		    String s = buf.ToString();
			buf.Clear();
			current.SetAttributeValue(XName.Get(value.Escape(EscapingType.XmlName)), s); // s escaped automatically
		    value = "";
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
