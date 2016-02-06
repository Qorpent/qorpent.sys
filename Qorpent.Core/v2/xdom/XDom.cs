using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Experiments;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.xdom
{
    public static class XDom
    {
        public static XElement X(string name, params object[] data)
        {
            var result = new XElement(name);
            if (null != data) {
                foreach (var o in data) {
                    if (o is string || o is XElement || o is XAttribute || o is XText ||o.GetType().IsValueType) {
                        result.Add(o);
                    }
                    else {
                        var j = o.jsonifymap();
                        foreach (var p  in j) {
                            if (p.Key == "cls") {
                                setClasses(result, p.Value);
                            }
                            else {
                                result.SetAttributeValue(p.Key,p.Value);
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static void setClasses(this XElement result, object classes ) {
            if (classes is string) {
                result.setClass(classes as string);
            }
            else if (classes is Array) {
                foreach (var s in (classes as Array).OfType<object>().Select(_ => _.ToStr())) {
                    result.setClass(s);
                }
            }
            else if (classes is IDictionary<string, object>) {
                var d = classes as Dictionary<string, object>;
                foreach (var di in d) {
                    if (di.Value.ToBool()) {
                        result.setClass(di.Key);
                    }
                    else {
                        result.removeClassName(di.Key);
                    }
                }
            }
        }

        public static H1Element h1(params object[] content) {
            return new H1Element((object[])content);
        }
        public static H2Element h2(params object[] content)
        {
            return new H2Element((object[])content);
        }
        public static H3Element h3(params object[] content)
        {
            return new H3Element((object[])content);
        }
        public static H4Element h4(params object[] content)
        {
            return new H4Element((object[])content);
        }
    
        public static HtmlElement html(params object[] content)
        {
            return new HtmlElement((object[])content);
        }
        public static SpanElement span(params object[] content)
        {
            return new SpanElement((object[])content);
        }
        public static DivElement div(params object[] content)
        {
            return new DivElement((object[])content);
        }
        public static ThElement th(params object[] content) {
            return new ThElement((object[])content);
        }
        public static TdElement td(params object[] content)
        {
            return new TdElement((object[])content);
        }
        public static TableElement table(params object[] content)
        {
            return new TableElement((object[])content);
        }

        public static X colspan<X>(this X e, int cols) where X : TableCellElement
        {
            e.ColSpan = cols;
            return e;

        }

        public static X rowspan<X>(this X e, int rows) where X:TableCellElement
        {
            e.RowSpan = rows;
            return e;
        }

        public static X appendTo<X>(this X e, XElement target) where X:XElement
        {
            if (target is DomElement) {
                ((DomElement) target).Set(e);
            }
            else {
                target.Add(e);
            }
            return e;
        }
        public static XElement prependTo(this XElement e, XElement target)
        {
            target.AddFirst(e);
            return e;
        }

        public static XElement attr(this XElement e, string name, object value)
        {
            e.SetAttributeValue(name, value);
            return e;
        }

        public static XElement text(this XElement e, object text)
        {
            e.Value = text.ToStr();
            return e;
        }
        public static X setClass<X>(this X e, string classes) where X:XElement
        {
            var current = e.Attr("class");
            e.attr("class", ComplexStringExtension.SetList(current, classes, ' '));
            return e;
        }
        public static XElement removeClassName(this XElement e, string classes)
        {
            var current = e.Attr("class");
            e.attr("class", ComplexStringExtension.Remove(current, classes));
            return e;
        }

    }
}
