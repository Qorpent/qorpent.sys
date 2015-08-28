using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.xdom
{
    public static class XDom
    {
        public static XElement X(string name, params object[] data)
        {
            return new XElement(name, (object[])data);
        }

        public static XElement appendTo(this XElement e, XElement target)
        {
            target.Add(e);
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
        public static XElement addClassName(this XElement e, string classes)
        {
            var current = e.Attr("class");
            e.attr("class", ComplexStringExtension.SetList(current, classes, ' '));
            return e;
        }
    }
}
