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
        private Stack<char> stack = new Stack<char>();
	    private int symbol_count = 0;
	    private int anon_count = 0;
	    private String att_name = "";

        private delegate void stateOperation(char c);
        private Dictionary<ReadMode, stateOperation> map;

        /// <summary>
        /// 
        /// </summary>
        public BxlParser2() {
            map = new Dictionary<ReadMode, stateOperation>() {
                {ReadMode.ElementName,		processElementName},
				{ReadMode.AttributeName,	processAttributeName},
				{ReadMode.AttributeValue,	processAttributeValue},
                {ReadMode.Indent,			processIndent},
                {ReadMode.NewLine,			processNewLine},
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
                if (c == '\r')
                    continue;
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
                case '"':
                case '\'':
					throw new BxlException("not implemented: " + c.ToString());

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
                case '\n':
                    throw new BxlException("unexpected '\\n' symbol");
                case '\t':
                    current = current.Elements().Last();
                    return;
                default:
                    buf.Append(c);
                    mode = ReadMode.ElementName;
                    return;
            }
        }

        private void processNewLine(char c) {
	        anon_count = 0;
	        current = root;
            switch (c) {
                case ':':
                case '"':
                case '\'':
					throw new BxlException("not implemented: " + c.ToString());

				case ',':
		            return;
				case '=':
					throw new BxlException("unexpected '=' symbol");
                case ' ':
		            symbol_count++;
					mode = ReadMode.Indent;
		            return;
                case '\n':
                    return;
                case '\t':
                    current = current.Elements().Last();
                    mode = ReadMode.Indent;
                    return;
                default:
                    buf.Append(c);
					mode = ReadMode.ElementName;
                    return;
            }
        }

		private void processElementName(char c) {
			switch (c) {
				case ':':
				case '"':
				case '\'':
					throw new BxlException("not implemented: " + c.ToString());

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
				case '\n':
					addNode();
					mode = ReadMode.NewLine;
					return;
				case '\t':
					processElementName(' ');
					return;
				default:
					buf.Append(c);
					return;
			}
		}

		private void processAttributeName(char c) {
			switch (c) {
				case ':':
				case '"':
				case '\'':
					throw new BxlException("not implemented: " + c.ToString());

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
				case '\n':
					saveAttributeName();
					addAnonAttribute();
					mode = ReadMode.NewLine;
					return;
				case '\t':
					processAttributeName(' ');
					return;
				default:
					buf.Append(c);
					addAnonAttribute();
					return;
			}
		}

	    private void processAttributeValue(char c) {
			switch (c) {
				case ':':
				case '"':
				case '\'':
					throw new BxlException("not implemented: " + c.ToString());

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
				case '\n':
					if (buf.Length == 0)
						throw new BxlException("empty attribute value");
					addAttributeValue();
					mode = ReadMode.NewLine;
					return;
				case '\t':
					processAttributeValue(' ');
					return;
				default:
					buf.Append(c);
					return;
			}
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
		    if (att_name.Length == 0)
			    return;
		    String s = att_name;
		    att_name = "";
		    switch (anon_count) {
			    case 0:
					current.SetAttributeValue(XName.Get("code"), s);
					current.SetAttributeValue(XName.Get("id"), s);
				    break;
				case 1:
					current.SetAttributeValue(XName.Get("name"), s);
				    break;
				default:
					current.SetAttributeValue(XName.Get(s), "1");
				    break;
		    }
		    anon_count++;
	    }

	    private void saveAttributeName() {
			if (buf.Length == 0)
				return;
			String s = buf.ToString().Escape(EscapingType.XmlName);
			buf.Clear();
		    att_name = s;
	    }

	    private void addAttributeValue() {
			// checking for empty values must be implemented in invoker !
			String s = buf.ToString().Escape(EscapingType.XmlAttribute);
			buf.Clear();
			current.SetAttributeValue(XName.Get(att_name), s);
		    att_name = "";
	    }
       
    }
}
