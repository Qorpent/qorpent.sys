using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.xdom {
    public class DomElement : XElement {
        private string _elementName = null;

        private DomElement(XName name) : base(name) {
        }

        private DomElement(XName name, object content) : base(name, content) {
        }

        private DomElement(XName name, params object[] content) : base(name, content) {
        }

        protected DomElement(XElement other) : base(other) {
        }

        private DomElement(XStreamingElement other) : base(other) {
        }


        public string Id {
            get { return this.Attr("id"); }
            set { this.SetAttributeValue("id",value);}
        }

        public string Class {
            get { return this.Attr("class"); }
            set { this.SetAttributeValue("class", value); }
        }

        public string Style
        {
            get { return this.Attr("style"); }
            set { SetAttributeValue("style", value); }
        }

        public DomElement ParentElement => Ancestors().OfType<DomElement>().FirstOrDefault();

        public DomElementFlags Flags { get; protected set; }


        protected  string TagName => _elementName ?? (_elementName= this.GetType().Name.Replace("Element", "").ToLowerInvariant());

        protected DomElement(  object[] content, object[] structure = null, string name = null , DomElementFlags flags = DomElementFlags.Default) :base("span") {
            Flags = flags;
            if (!string.IsNullOrWhiteSpace(name)) {
                Name = name;
            }
            else {
                Name = TagName;
            }
            if (null != structure) {
                Add((object[])structure);
            }
       
            if (null != content) {
                foreach (var item in content) {
                    Set(item);
                }
            }

            if (Flags.HasFlag(DomElementFlags.RequireValue) && string.IsNullOrEmpty(this.Value)) {
                this.Add(new XText(""));
            }
        }

        public static readonly IDictionary<string, Type> ElementMap = new Dictionary<string, Type> {
            ["html"] = typeof (HtmlElement),
            ["head"] = typeof (HeadElement),
            ["body"] = typeof (BodyElement),
            ["link"] = typeof (LinkElement),
            ["style"] = typeof (StyleElement),
            ["script"] = typeof (ScriptElement),
            ["meta"] = typeof (MetaElement),
        };

        
        public void Set(object item) {
            if (item == null) {
                Value = string.Empty;
            }
            else if (item is XElement) {
                var e = item as XElement;
                if (Flags.HasFlag(DomElementFlags.AllowElements)) {
                    e = AddaptElement(e);
                    if (!SpecialSetElement(e)) {
                        Add(e);
                    }
                }else { 
                    throw new Exception("this element ("+TagName+") not allows sub-elements");
                }
            }else if (item is XAttribute) {
                var a = item as XAttribute;
                if (!SpecialSetAttrubute(a)) {
                    this.SetAttributeValue(a.Name,a.Value);
                }
            }
            else if (item is string  || item is XText ||
                     item.GetType().IsValueType) {
                if (Flags.HasFlag(DomElementFlags.AllowText)) {
                    if (!SpecialSetTextContent(item)) {
                        Add(item);
                    }
                }else  {
                    throw new Exception("this element (" + TagName + ") not allows text data");
                }
            }
            else if (item is Array) {
                foreach (var aitem  in (item as Array)) {
                    Set(aitem);
                }
            }
            else {
                var dict = item.ToDict();
                foreach (var i in dict) {
                    if (i.Key == "cls" || i.Key == "_class" || i.Key == "class") {
                        this.setClasses(i.Value);
                    }
                    else {
                        SetAttributeValue(i.Key,i.Value);
                    }
                }
            }
        }

        protected virtual bool SpecialSetAttrubute(XAttribute xAttribute) {
            return false;
        }

        protected virtual bool SpecialSetTextContent(object item) {
            return false;
        }

        protected static XElement AddaptElement(XElement e) {
            if (!(e is DomElement)) {
                var tagname = e.Name.LocalName.ToLowerInvariant();
                if (ElementMap.ContainsKey(tagname)) {
                    var domelement = Activator.CreateInstance(ElementMap[tagname], new object[] {null}) as DomElement;
                    var target = domelement;
                    ApplyToDomElement(e, target);
                    e = domelement;
                }
            }
            return e;
        }

        protected static void ApplyToDomElement(XElement e, DomElement target) {
            foreach (var subelement in e.Nodes()) {
                target.Set(subelement);
            }
            foreach (var attribute in e.Attributes()) {
                target.Set(attribute);
            }
        }

        protected virtual bool SpecialSetElement(XElement e) {
            return false;
        }

       
    }
}