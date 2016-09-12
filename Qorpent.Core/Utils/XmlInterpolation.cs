using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

namespace Qorpent.Utils {
    /// <summary>
    ///     Выполняет интерполяцию внутри XML-элемента,
    ///     источником является набор атрибутов на каждом уровне иерархии
    ///     что позволяет осуществить их мерж
    /// </summary>
    public class XmlInterpolation {
        private readonly StringInterpolation _stringInterpolation = new StringInterpolation();
        private ILogicalExpressionEvaluator le;
        
        /// <summary>
        /// </summary>
        public XmlInterpolation() {
            StopAttribute = "stopinterpolate";
            UseExtensions = true;
        }

        /// <summary>
        ///     Установка символа начала интерполяции
        /// </summary>
        public char AncorSymbol
        {
            get { return _stringInterpolation.AncorSymbol; }
            set { _stringInterpolation.AncorSymbol = value; }
        }

        /// <summary>
        ///     Стопер при обходе элементов - блокирует интерполяцию
        ///     значения - 1,true - ниже, all - что либо другое - этот и ниже
        /// </summary>
        public string StopAttribute { get; set; }

        /// <summary>
        /// </summary>
        public IDictionary<string, object> SecondSource { get; set; }

        /// <summary>
        ///     Обработка только атрибута code
        /// </summary>
        public bool CodeOnly { get; set; }

        public bool UseXiXml { get; set; } = true;

        /// <summary>
        ///     Уровень обхода
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        ///     Использование расширенной интерполяции (новые функции)
        /// </summary>
        public bool UseExtensions { get; set; }
        public IXmlIncludeProvider XmlIncludeProvider { get; set; }

        /// <summary>
        ///     Выполняет интерполяцию атрибутов в строки по Xml элементу
        ///     рекурсивно по всей подветке
        /// </summary>
        /// <returns></returns>
        public XElement Interpolate(XElement source, object baseconfig) {
            var level = Level;
            if (level <= 0) {
                level = int.MaxValue;
            }
            baseconfig = baseconfig ?? new Scope();
            XElement result;
            if (baseconfig is IScope) {
                result = InternalInterpolate(source, (IScope) baseconfig, level);
            }
            else {
                result= InternalInterpolate(source, new Scope(baseconfig.ToDict()), level);
            }
            Postprocess(result,Level);
            return result;
        }

        private void Postprocess(XElement result,int level) {
            if (0 == level) {
                if (UseXiXml) {
                    ApplyXIXml(result);
                }
            }
        }

        /// <summary>
        ///     Выполняет интерполяцию атрибутов в строки по Xml элементу
        ///     рекурсивно по всей подветке
        /// </summary>
        /// <returns></returns>
        public XElement Interpolate(XElement source, IScope baseconfig = null, XElement[] elements =null) {
            var level = Level;
            if (level <= 0) {
                level = int.MaxValue;
            }
            var result = InternalInterpolate(source, baseconfig, level, elements);
            Postprocess(result,Level);
            return result;
        }

        private XElement InternalInterpolate(XElement source, IScope parentconfig, int level, XElement[] elements=null) {
            var datasource = PrepareDataSource(source, parentconfig);
            XElement changed = null;
            
            var processchild = (null!=elements) || InterpolateDataToElement(source, datasource, out changed);
            source = changed ?? source;
            if (processchild && level >= 1) {
                level--;
                if (null != elements) {
                    foreach (var e in elements)
                    {
                        InternalInterpolate(e, datasource, level, null);
                    }
                }
                else {
                    foreach (var e in source.Elements().ToArray()) {
                        InternalInterpolate(e, datasource, level, null);
                    }
                }
            }
            if (UseExtensions && source.Attr("xi-delete").ToBool()) {
                return null;
            }
          
            return source;
        }

        private void ApplyXIXml(XElement source) {
            foreach (var element in source.DescendantsAndSelf().Reverse().ToArray()) {
                if (element.Name.LocalName == "xi-xml") {
                    var xml = GetXmlFromText(element.Value);
                    element.Value = "";
                    element.Add(xml.Elements());
                }
                var xixml = element.Attribute("xi-xml");
                if (null != xixml) {
                    var text = xixml.Value;
                    var xml = GetXmlFromText(text);
                    element.Add(xml.Nodes());
                    xixml.Remove();
                }
                if (element.Name.LocalName == "xi-xml") {
                    element.ReplaceWith(element.Nodes());
                }
            }
        }

        private static XElement GetXmlFromText(string text) {
            try {
                if (text.StartsWith("![")) {
                    text = text.Substring(1).Replace("[", "<").Replace("]", ">");
                }
                text = "<div>" + text + "</div>";
                var xml = XElement.Parse(text);
                return xml;
            }
            catch (Exception e) {
                return new XElement("div",new XAttribute("error",e.Message),text);
            }
        }

        /// <summary>
        ///     Интерполирует исходный элемет в целевой
        /// </summary>
        /// <param name="source"></param>
        /// <param name="baseelement"></param>
        /// <returns></returns>
        public XElement Interpolate(XElement source, XElement baseelement) {
            var datasources = baseelement.AncestorsAndSelf().Reverse().ToArray();
            Scope cfg = null;
            foreach (var element in datasources) {
                cfg = new Scope(cfg);
                foreach (var attribute in element.Attributes()) {
                    cfg.Set(attribute.Name.LocalName, attribute.Value);
                }
                var selftext = string.Join(Environment.NewLine, element.Nodes().OfType<XText>().Select(_ => _.Value));
                if (!string.IsNullOrWhiteSpace(selftext) && !cfg.ContainsOwnKey("__value")) {
                    cfg.Set("__value", selftext);
                }
            }
            return Interpolate(source, cfg);
        }

        private bool InterpolateDataToElement(XElement source, IScope datasource, out XElement result) {
            result = source;
            if (UseExtensions) {
                if (!MatchCondition(source, datasource, "if")) {
                    return false;
                }
#if !EMBEDQPT
                XmlIncludeProvider = XmlIncludeProvider ?? new XmlIncludeProvider();
#endif
                var globalreplace = source.Attr("xi-replace");
                if (!string.IsNullOrWhiteSpace(globalreplace)) {
                    _stringInterpolation.Interpolate(globalreplace);
                    var xml = XmlIncludeProvider.GetXml(globalreplace, source, datasource);
                    if (null == xml) {
                        xml = XElement.Parse("<span class='no-ex replace'>no replace " + globalreplace + "</span>");
                    }
                    result = xml;
                    if (null != source.Parent) {
                        source.ReplaceWith(xml);
                    }
                    source = xml;
                }


                if (source.Attr("xi-repeat").ToBool()) {
                    XElement[] replace = ProcessRepeat(source, datasource);
                    if (null == replace) {
                        source.Remove();
                    }
                    else {
                        // ReSharper disable once CoVariantArrayConversion
                        source.ReplaceWith(replace);
                    }
                    return false;
                }

                var include = source.Attr("xi-include");
                if (!string.IsNullOrWhiteSpace(include)) {
                    _stringInterpolation.Interpolate(include);
                    var xml = XmlIncludeProvider.GetXml(include, source, datasource);
                    if (null == xml) {
                        xml = XElement.Parse("<span class='no-ex include'>no replace " + include + "</span>");
                    }
                    source.ReplaceAll(xml);
                }
            }

            var processchild = true;
            if (!string.IsNullOrWhiteSpace(StopAttribute)) {
                var stopper = source.Attribute(StopAttribute);
                if (null != stopper) {
                    if (stopper.Value != "0") {
                        if (stopper.Value == "all") {
                            return false;
                        }
                        processchild = false;
                    }
                }
            }
            if (CodeOnly) {
                foreach (var a in source.Attributes()) {
                    if (a.Name.LocalName == "code" || a.Name.LocalName == "id") {
                        var val = a.Value;
                        if (val.Contains(_stringInterpolation.AncorSymbol) &&
                            val.Contains(_stringInterpolation.StartSymbol)) {
                            val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
                            a.Value = val;
                            datasource.Set(a.Name.LocalName, val);
                        }
                    }
                }
            }
            else {
                var changed = true;
                while (changed) {
                    changed = false;
                    foreach (var a in source.Attributes().OrderBy(_ => {
                        if (_.Value.Contains(AncorSymbol+"{") && _.Value.Contains("(")) {
                            return 1000;
                        }
                        return 0;
                    })) {
                        var val = a.Value;
                        if (val.Contains(_stringInterpolation.AncorSymbol) &&
                            val.Contains(_stringInterpolation.StartSymbol)) {
                            val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
                            changed = changed || (val != a.Value);
                            a.Value = val;
                            datasource.Set(a.Name.LocalName, val);
                        }
                    }
                }
                changed = true;
                while (changed) {
                    changed = false;
                    foreach (var t in source.Nodes().OfType<XText>()) {
                        var val = t.Value;
                        if (val.Contains(_stringInterpolation.AncorSymbol) &&
                            val.Contains(_stringInterpolation.StartSymbol)) {
                            val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
                            changed = changed || val != t.Value;
                            t.Value = val;
                        }
                    }
                }
            }
            return processchild;
        }

        private bool MatchCondition(XElement e, IScope ds, string suffix) {
            var attrname = "xi-" + suffix;
            le = le ?? new LogicalExpressionEvaluator();
            if (e.Attr(attrname).ToBool()) {
                var ifname = e.Attr(attrname);
                var ismatch = le.Eval(ifname, ds);

                if (!ismatch) {
                    if (e.Parent != null) {
                        e.Remove();
                    }
                    else {
                        e.SetAttributeValue("xi-delete", true);
                    }
                    return false;
                }
                e.SetAttributeValue(attrname, null);
            }
            return true;
        }

        private XElement[] ProcessRepeat(XElement source, IScope datasource) {
            var a = GetDataSource(source, datasource);
            if (0 == a.Length) {
                return null;
            }


            var result = new List<XElement>();

            var scope = source.Attr("xi-scope");
            var scope2 = "";

            var expand = source.Attr("xi-expand").ToBool();
            var rep = source.Attr("xi-repeat");
            if (rep.Contains(" in ")) {
                scope2 = Regex.Match(rep, @"^([\s\S]+?)\s+in").Groups[1].Value;
                if (scope2.EndsWith("+")) {
                    expand = true;
                    scope2 = scope2.Substring(0, scope2.Length - 1);
                }
            }
            
            var i = 0;

            foreach (var o in a) {
                var dict = o.ToDict();
                var cfg = new Scope();
                if (!string.IsNullOrWhiteSpace(scope)) {
                    cfg[scope] = dict;
                }
                if (!string.IsNullOrWhiteSpace(scope2)) {
                    cfg[scope2] = dict;
                }
                if (expand) {
                    foreach (var p in dict) {
                        cfg[p.Key] = p.Value;
                    }
                }

                var clone = new XElement(source);
                cfg.Set("this", clone);
                cfg.Set("parent", source);
                cfg.SetParent(datasource);
                cfg.Set("_idx", i);
                cfg.Set("_num", i + 1);
                cfg.Set("_i", dict);
                if (!MatchCondition(clone, cfg, "where")) {
                    continue;
                }

                clone.SetAttributeValue("xi-repeat", null);
                clone.SetAttributeValue("xi-scope", null);
                clone.SetAttributeValue("xi-expand", null);
                clone = Interpolate(clone, cfg);
                if (null != clone) {
                    if (clone.Attr("xi-body").ToBool()) {
                        foreach (var element in clone.Elements()) {
                            result.Add(element);
                        }
                    }
                    else {
                        result.Add(clone);
                    }
                }
                i++;
            }
            return result.ToArray();
        }

        private static object[] GetDataSource(XElement source, IScope datasource) {
            IEnumerable result = null;
            var name = source.Attr("xi-repeat");
            if (name.Contains(" in ")) {
                name = Regex.Replace(name, @"^[\s\S]+?\s+in\s+", "");
            }
            if (name.StartsWith("$")) {
                name = name.Substring(1);
                var ds = source.XPathSelectElement("//xi-dataset[@code='" + name + "']");
                if (null != ds) {
                    result = new List<object>();
                    foreach (var e in ds.Elements()) {
                        var dict = new Dictionary<string, object>();
                        foreach (var attribute in e.Attributes()) {
                            dict[attribute.Name.LocalName] = attribute.Value;
                        }
                        ((IList<object>) result).Add(dict);
                    }
                }
            }
            else {
                result = datasource.Get<object>(name) as IEnumerable;
            }
            if (null == result) {
                return new object[] {};
            }
            return result.OfType<object>().ToArray();
        }

        private IScope PrepareDataSource(XElement source, IScope parent) {
            var result = new Scope();
            if (null != parent) {
                result.SetParent(parent);
            }
            foreach (var a in source.Attributes()) {
                result.Set(a.Name.LocalName, a.Value);
            }
            result.Set("this", source);
            return result;
        }
    }
}