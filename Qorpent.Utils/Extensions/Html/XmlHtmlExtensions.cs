using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Qorpent.Utils.Extensions.Html
{
    /// <summary>
    /// Расширения для работы с XML как с HTML
    /// </summary>
    public static class XmlHtmlExtensions {
        private const string DefaultStylesId = "__qorpent_default_styles";

        /// <summary>
        /// Стили по умолчанию
        /// </summary>
        public static string DefaultStyles = @"
table {
    border-collapse : collapse;
}
td,  th {
    padding : 2px;
    border : solid 1px gray;
}
th {
    background-color : #999999;
    color: white;
}

";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static XElement CreateNewHtml()
        {
            return XElement.Parse("<html><head></head><body></body></html>");
        }
        /// <summary>
        /// Установить заголовок документа
        /// </summary>
        /// <param name="html"></param>
        /// <param name="title"></param>
        /// <returns>Исходный документ</returns>
        public static XElement HtmlSetDocumentTitle(this XElement html, string title) {
            var titleElement = HtmlGetHead(html).EnsureSingleElement("title");
            titleElement.Value = title;
            return html;
        }
        /// <summary>
        /// Получить заголовок HTMl
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static XElement HtmlGetHead(XElement html) {
            return FindHtmlElement(html).EnsureSingleElement("head");
        }
        /// <summary>
        /// Получить тело HTML
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static XElement HtmlGetBody(XElement html)
        {
            return FindHtmlElement(html).EnsureSingleElement("body");
        }

        /// <summary>
        /// Создает заголовок первого уровня
        /// </summary>
        /// <param name="html"></param>
        /// <param name="text"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static XElement HtmlAddHead1(this XElement html, string text = null, object attributes = null)
        {
            return HtmlAddHead(html, 1, text, attributes);
        }
        /// <summary>
        /// Добавляет в документ стандартные стили
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static XElement HtmlAddDefaultStyles(this XElement html) {
            var head = HtmlGetHead(html);
            var existed = head.Elements("style").FirstOrDefault(_ => _.Attr("id") == DefaultStylesId);
            if (null == existed) {
                head.AddElement("style", DefaultStyles, new {id = DefaultStylesId, type = "text/css"});
            }
            return html;
        }

        /// <summary>
        /// Установить класс элемента
        /// </summary>
        /// <param name="htmlElement"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static XElement HtmlSetClassName(this XElement htmlElement, string className) {
            var existed = htmlElement.Attribute("class");
            if (null == existed || string.IsNullOrWhiteSpace(existed.Value)) {
                htmlElement.SetAttributeValue("class",className);
            }
            else {
                var current = existed.Value;
                if (current.Split(' ').All(_ => _ != className)) {
                    current += " " + className;
                }
                existed.Value = current;
            }
            return htmlElement;
        }

        /// <summary>
        /// Добавляет элемент таблицы
        /// </summary>
        /// <param name="html"></param>
        /// <param name="cls"></param>
        /// <param name="attributes"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        public static XElement HtmlAddTable(this XElement html, string cls, object attributes= null, object head=null) {
            var tableElement = html.HtmlAddElement("table", attributes: attributes);
            var headElement = tableElement.EnsureSingleElement("thead");
            tableElement.EnsureSingleElement("tbody");
            if (!string.IsNullOrWhiteSpace(cls)) {
                tableElement.HtmlSetClassName(cls);
            }
            if (null != head) {
                var headRow = headElement.HtmlAddTableRow();
                foreach (var col in head.ToDict()) {
                    HtmlAddTableCell(headRow, col.Value, col.Key);
                }
            }

            return tableElement;
        }
        /// <summary>
        /// Добавдение ячейки в строку
        /// </summary>
        /// <param name="current"></param>
        /// <param name="cls"></param>
        /// <param name="text"></param>
        /// <param name="headcellattributes"></param>
        public static XElement HtmlAddTableCell(this XElement current, string cls = null, string text = null, object headcellattributes = null) {
            var rowelement = FindElement(current, new[] {"tr"}, new[] {"thead", "tbody"}, "tr");
            var cell = rowelement.AddElement("td", text, headcellattributes);
            if (!string.IsNullOrWhiteSpace(cls)) {
                cell.HtmlSetClassName(cls);
            }
            return cell;
        }

        /// <summary>
        /// Находит ближайший соответствующий элемент в качестве контейнера строки таблицы и добавляет ее туда
        /// </summary>
        /// <param name="html"></param>
        /// <param name="attributes"></param>
        /// <param name="cells"></param>
        /// <returns>Элемент строки</returns>
        public static XElement HtmlAddTableRow(this XElement html, object attributes = null, object[] cells = null) {
            var parent = FindTableRowContainer(html);
            var row = parent.AddElement("tr",attributes:attributes);
            if (null != cells) {
                foreach (var cell in cells) {
                    HtmlAddTableCell(row, cell);
                }
            }
            return row;
        }
        /// <summary>
        /// Добавляет ячейку к строке таблицы
        /// </summary>
        /// <param name="row"></param>
        /// <param name="val"></param>
        /// <param name="cls"></param>
        /// <returns></returns>
        public static XElement HtmlAddTableCell(this XElement row, object val = null, string cls = null) {
            var txt = val as string;
            IDictionary<string, object> attr = null;
            if (null == val) {
                txt = "";
            }
            else if (val is string) {
                txt = val as string;
            }
            else if (val.GetType().IsValueType) {
                txt = val.ToStr();
            }
            else {
                attr = val.ToDict();
                if (attr.ContainsKey("cls")) {
                    cls = attr["cls"].ToString();
                    attr.Remove("cls");
                }
                if (attr.ContainsKey("text")) {
                    txt = attr["text"].ToString();
                    attr.Remove("text");
                }
            }
            return row.HtmlAddTableCell(cls, txt, attr);
        }

        /// <summary>
        /// Ищет ближайший подходящий контейнер для строки (thead,tbody)
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static XElement FindTableRowContainer(XElement html) {
            return FindElement(html, RowContainerFindTags,new[]{"table"}, "tbody");
        }

        /// <summary>
        /// Создает заголвок указанного уровня
        /// </summary>
        /// <param name="html"></param>
        /// <param name="i"></param>
        /// <param name="text"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private static XElement HtmlAddHead(XElement html, int i, string text = null, object attributes = null)
        {
            var tagname = "h" + i;
            return HtmlAddElement(html, tagname, text, attributes);
        }
        /// <summary>
        /// Создает элемент контента
        /// </summary>
        /// <param name="html"></param>
        /// <param name="tagname"></param>
        /// <param name="text"></param>
        /// <param name="attributes"></param>
        /// <returns>Полученный элемент контента</returns>
        /// <remarks>Если находится в html то переходит в body</remarks>
        private static XElement HtmlAddElement(this XElement html, string tagname, string text = null, object attributes = null)
        {
            var parent = html;
            if (html.Name.LocalName == "html") {
                parent = HtmlGetBody(html);
            }
            return parent.AddElement(tagname, text, attributes);
        }

        private static readonly string[] HtmlFindTags = new[] {"html"};
        private static readonly string[] RowContainerFindTags = new[] { "tbody","thead" };
        /// <summary>
        /// Получить корневой элемент документа
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static XElement FindHtmlElement(XElement html) {
            return FindElement(html, HtmlFindTags);
        }
        /// <summary>
        /// Ищет ближайший соответствующий схеме элемент вверх и вниз в случае нахождение рутов
        /// </summary>
        /// <param name="current"></param>
        /// <param name="elementsToFind"></param>
        /// <param name="availRoots"></param>
        /// <param name="defaultInRoot"></param>
        /// <returns></returns>
        public static XElement FindElement(XElement current, string[] elementsToFind, string[] availRoots = null,
                                           string defaultInRoot=null) {
            if(null==current)throw new ArgumentNullException("current");
            if(null==elementsToFind)throw new ArgumentNullException("elementsToFind");
            if (-1 != Array.IndexOf(elementsToFind, current.Name.LocalName)) {
                return current;
            }
            if (null != availRoots && -1 != Array.IndexOf(availRoots, current.Name.LocalName)) {
                if (string.IsNullOrWhiteSpace(defaultInRoot)) {
                    throw new ArgumentNullException("defaultInRoot");
                }
                return current.EnsureSingleElement(defaultInRoot);
            }
            if(null==current.Parent)throw new Exception("suggested elements not found");
            return FindElement(current.Parent,elementsToFind,availRoots,defaultInRoot);
        }
    }
}
