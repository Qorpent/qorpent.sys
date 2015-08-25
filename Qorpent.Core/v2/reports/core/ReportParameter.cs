using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using qorpent.v2.model;
using Qorpent.Core.Tests.Experiments;
using Qorpent.Experiments;
using Qorpent.Model;
using Qorpent.Utils;
using Qorpent.Utils.Extensions;

namespace qorpent.v2.reports.core {
    public class ReportParameter : Item, IWithIndex {
        public ReportParameter() {
            List= new List<IItem>();
        }
        public string Default { get; set; }
        public int Idx { get; set; }
        public string IfCondition { get; set; }
        public IList<IItem> List { get; set; }

        protected override void WriteJsonInternal(JsonWriter jw, string mode) {
            base.WriteJsonInternal(jw, mode);
            jw.WriteProperty("default",Default);
            jw.WriteProperty("ngif",IfCondition,true);
            jw.WriteProperty("list",List.OfType<object>().ToArray());
        }

        protected override void ReadFromXml(XElement xml) {
            base.ReadFromXml(xml);
            Default = xml.AttrOrValue("default");
            IfCondition = xml.AttrOrValue("ng-if");
            var listattr = xml.Attr("list");
            if (!string.IsNullOrWhiteSpace(listattr)) {
                ReadStringList(listattr);
            }
            var listitems = xml.Elements("item");
            foreach (var listitem in listitems) {
                List.Add(Create<Item>(listitem));
            }
        }

        protected override void LoadFromJson(object jsonsrc) {
            base.LoadFromJson(jsonsrc);
            var j = jsonsrc.nestorself("_source");
            Default = j.str("default");
            IfCondition = j.str("ngif");
            var list = j.get("list");
            ReadList(list);
        }

        private void ReadList(object list) {
            if (null != list) {
                var strlist = list as string;
                if (null != strlist) {
                    ReadStringList(strlist);
                }
                var arrlist = list as Array;
                if (null != arrlist) {
                    ReadArrayList(arrlist);
                }
                var dictlist = list as IDictionary<string, object>;
                if (null != dictlist) {
                    ReadDictionaryList(dictlist);
                }
            }
        }

        private void ReadDictionaryList(IDictionary<string, object> dictlist) {
            foreach (var i in dictlist) {
                var item = new Item();
                if (i.Value is IDictionary<string, object>) {
                    item.Read(i.Value);
                    if (string.IsNullOrWhiteSpace(item.Id)) {
                        item.Id = i.Key;
                    }
                    else if (item.Id == "_empty_" || item.Id == "''" || item.Id == "null" || item.Id == "\"\"") {
                        item.Id = "";
                    }
                }
                else {
                    item.Id = i.Key;
                    item.Name = i.Value.ToStr();
                }
                List.Add(item);
            }
        }

        private void ReadArrayList(Array arrlist) {
            foreach (var i in arrlist) {
                if (null == i) continue;
                var si = i as string;
                if (null != si) {
                    List.Add(new Item {Id = si, Name = si});
                    continue;
                }
                var di = i as IDictionary<string, object>;
                if (null != di) {
                    List.Add(Create<Item>(di));
                    continue;
                }
                List.Add(new Item {Id = i.ToStr(), Name = i.ToStr()});
            }
        }

        private static readonly ComplexStringHelper listreader = new ComplexStringHelper {
            ItemPrefix = "",
            ItemSuffix = "",
            ItemDelimiter = "|",
            ValueDelimiter = ":",
            NotExistedValue = "",
            EmptyValue = "",
            ItemDelimiterSubstitution = "`",
            ValueDelimiterSubstitution = "~"
        };

        private void ReadStringList(string strlist) {
            var dict = listreader.Parse(strlist);
            foreach (var p in dict) {
                var i = new Item();
                i.Id = p.Key;
                i.Name = p.Value;
                if (string.IsNullOrWhiteSpace(i.Name)) {
                    i.Name = i.Id;
                }
                else if (i.Name == "_empty_" || i.Name == "''" || i.Name == "null" || i.Name == "\"\"") {
                    i.Name = "";
                }
                List.Add(i);
            }
        }
    }
}