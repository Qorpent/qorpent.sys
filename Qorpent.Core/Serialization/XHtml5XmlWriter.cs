using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Qorpent.Serialization {
    /// <summary>
    ///������� XML Writer, ����������� � XHTML
    /// </summary>
    /// <remarks>
    ///  ������������ ����������� ���� ���� ������ ���������m, ��������� ��� ����
    /// ������� IMG;
    /// ����� � ������ ���� ������� ����� �� �������� - ��������� ������� "���������" �
    /// ���� �������� HTML
    /// </remarks>
    public class XHtml5XmlWriter : XmlTextWriter {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        public XHtml5XmlWriter(TextWriter w)
            : base(w) {
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="w"></param>
        /// <param name="encoding"></param>
        public XHtml5XmlWriter(Stream w, Encoding encoding)
            : base(w, encoding) {
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        public XHtml5XmlWriter(string fileName, Encoding encoding)
            : base(fileName, encoding) {
        }
        /// <summary>
        /// ������ ���� ��������� HTML
        /// </summary>
        public static readonly string[] Html5Elements = new[] {
            "a",
            "abbr",
            "address",
            "area",
            "article",
            "aside",
            "audio",
            "b",
            "base",
            "bdi",
            "bdo",
            "blockquote",
            "body",
            "br",
            "button",
            "canvas",
            "caption",
            "cite",
            "code",
            "col",
            "colgroup",
            "command",
            "data",
            "datagrid",
            "datalist",
            "dd",
            "del",
            "details",
            "dfn",
            "div",
            "dl",
            "dt",
            "em",
            "embed",
            "eventsource",
            "fieldset",
            "figcaption",
            "figure",
            "footer",
            "form",
            "h1",
            "h2",
            "h3",
            "h4",
            "h5",
            "h6",
            "head",
            "header",
            "hgroup",
            "hr",
            "html",
            "i",
            "iframe",
            "img",
            "input",
            "ins",
            "kbd",
            "keygen",
            "label",
            "legend",
            "li",
            "link",
            "mark",
            "map",
            "menu",
            "meta",
            "meter",
            "nav",
            "noscript",
            "object",
            "ol",
            "optgroup",
            "option",
            "output",
            "p",
            "param",
            "pre",
            "progress",
            "q",
            "ruby",
            "rp",
            "rt",
            "s",
            "samp",
            "script",
            "section",
            "select",
            "small",
            "source",
            "span",
            "strong",
            "style",
            "sub",
            "summary",
            "sup",
            "table",
            "tbody",
            "td",
            "textarea",
            "tfoot",
            "th",
            "thead",
            "time",
            "title",
            "tr",
            "track",
            "u",
            "ul",
            "var",
            "video",
            "wbr"
        };

        /// <summary>
        /// ������ ���������, ������� ������ ������������������ � ������������� ������������ �����
        /// </summary>
        public static readonly string[] CloseableXhtmlElements = new[] {"img","br","hr","meta"};

        private bool _preventFullClosing;
        private bool _autoStylesWrote;
        private bool _htmlElmentWasWrote;
        private bool _doctypeWasWrote;
        private bool _generateHtmlWrapper = true;
        private bool _generateStylesForNonHtmlElements = true;
        private bool _requireCloser;
        private int _level;

        /// <summary>
        /// ���������� ����� - ���������� ����, ��� DocType �������� (�������� � ������ � ������ ��������������� ���������)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pubid"></param>
        /// <param name="sysid"></param>
        /// <param name="subset"></param>
        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            base.WriteDocType(name, pubid, sysid, subset);
            _doctypeWasWrote = true;
        }
        /// <summary>
        /// ����� ��������������� ��������� HTML5, �� ��������� ��������
        /// </summary>
        public bool GenerateHtmlWrapper {
            get { return _generateHtmlWrapper; }
            set { _generateHtmlWrapper = value; }
        }
        /// <summary>
        /// ����� ��������� ��������������� ����� ��� �����
        /// </summary>
        public bool GenerateStylesForNonHtmlElements {
            get { return _generateStylesForNonHtmlElements; }
            set { _generateStylesForNonHtmlElements = value; }
        }

        readonly IList<string> _autoStyleElements = new List<string>();
        private bool _noCheckStyles;

        /// <summary>
        ///���������� ������ - ��������� ������������� ������� ������ <see cref="CloseableXhtmlElements"/>
        /// </summary>
        /// <param name="prefix">The namespace prefix of the element. </param><param name="localName">The local name of the element. </param><param name="ns">The namespace URI to associate with the element. If this namespace is already in scope and has an associated prefix then the writer automatically writes that prefix also. </param><exception cref="T:System.InvalidOperationException">The writer is closed. </exception>
        public override void WriteStartElement(string prefix, string localName, string ns) {
            if (GenerateHtmlWrapper && !_htmlElmentWasWrote) {
                _htmlElmentWasWrote = true;
                if (!_doctypeWasWrote) {
                    WriteRaw("<!DOCTYPE html>");
                }   
                if (string.IsNullOrWhiteSpace(ns) && localName.ToUpper() != "HTML") {
                    WriteStartElement("html");
                    WriteStartElement("head");
                    WriteStartElement("meta");
                    WriteAttributeString("name","","generator");
                    WriteAttributeString("value", "", "XHtml5XmlWriter");
                    WriteEndElement();
                    WriteEndElement();
                    WriteStartElement("body");
                    _requireCloser = true;
                    _level = 0;
                }
                
               
            }
            base.WriteStartElement(prefix, localName, ns);
            if (GenerateHtmlWrapper && GenerateStylesForNonHtmlElements) {
                if (string.IsNullOrWhiteSpace(ns)) {
                    var tag = localName.ToLowerInvariant();
                    if (-1 == Array.IndexOf(Html5Elements, tag)) {
                        if (!_autoStyleElements.Contains(tag)) {
                            _autoStyleElements.Add(tag);
                        }
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(ns) && -1 != Array.IndexOf(CloseableXhtmlElements, localName.ToLowerInvariant())) {
                _preventFullClosing = true;
            }
            else {
                _preventFullClosing = false;
            }
            _level++;

        }
     

        /// <summary>
        /// ���������� ������ �������� �������� - ��������� ������� ������������ ���� ���� ��� ������ ��������� ����� ����� ������ 
        ///<see cref="CloseableXhtmlElements"/>
        /// </summary>
        public override void WriteEndElement() {
            if (_preventFullClosing) {
                base.WriteEndElement();
                ProcessWrapperChecking();
            }
            else {
                WriteFullEndElement();
            }
            
        }

        private void ProcessWrapperChecking() {      
            _preventFullClosing = false;
            _level--;
            if (_level <= 2) {
                CheckStyles();
                CheckCloser();
            }
        }

        /// <summary>
        /// ��������� ��� �������� ��� ����������
        /// </summary>
        public override void WriteFullEndElement()
        {
            base.WriteFullEndElement();
            ProcessWrapperChecking();
        }

        private void CheckStyles() {
            if (_noCheckStyles) return;
//��� ��������� � ��������� ������ ����������
            if (GenerateStylesForNonHtmlElements && GenerateHtmlWrapper && !_autoStylesWrote) {
                // � �������� �������� ��������
                if (_requireCloser && 0 == _level || (!_requireCloser && 2 == _level)) {
                    if (_autoStyleElements.Any()) {
                        WriteStartElement("style");
                        WriteAttributeString("type", "text/css");
                        foreach (var e in _autoStyleElements) {
                            WriteRaw(string.Format("{0} {{border:solid 1px black; padding:1px; margin:1px;}}",e));
                            WriteRaw(string.Format("{0}:before {{content: \"[{0}:\" attr(code) \"] \"}}",e));
                        }
                        _noCheckStyles = true;
                        WriteEndElement();
                        _noCheckStyles = false;
                        _autoStylesWrote = true;
                    }
                    
                }
                    
            }
            
        }

        private void CheckCloser() {
            
            if (_level == 0) {
                if (_requireCloser) {
                    WriteEndElement();
                    WriteEndElement();
                }
            }
        }
    }
}