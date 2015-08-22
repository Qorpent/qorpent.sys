using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Bxl;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;
using Qorpent.Wiki;

namespace Qorpent.Utils {
	/// <summary>
	/// Выполняет интерполяцию внутри XML-элемента, 
	/// источником является набор атрибутов на каждом уровне иерархии
	/// что позволяет осуществить их мерж
	/// </summary>
	public class XmlInterpolation {
		private readonly StringInterpolation _stringInterpolation = new StringInterpolation();
		/// <summary>
		/// 
		/// </summary>
		public XmlInterpolation() {
			StopAttribute = "stopinterpolate";
		    UseExtensions = true;
		}
        /// <summary>
        /// Выполняет интерполяцию атрибутов в строки по Xml элементу
        /// рекурсивно по всей подветке
        /// </summary>
        /// <returns></returns>
        public XElement Interpolate(XElement source, object baseconfig)
        {
            var level = Level;
            if (level <= 0)
            {
                level = int.MaxValue;
            }
            baseconfig = baseconfig ?? new Scope();
            if (baseconfig is IScope) {
                return InternalInterpolate(source, (IScope)baseconfig, level);
            }
            return InternalInterpolate(source, new Scope(baseconfig.ToDict()), level);
        }
		/// <summary>
		/// Выполняет интерполяцию атрибутов в строки по Xml элементу
		/// рекурсивно по всей подветке
		/// </summary>
		/// <returns></returns>
		public XElement Interpolate(XElement source, IScope baseconfig= null){
			var level = Level;
			if (level <= 0){
				level = int.MaxValue;
			}
			return InternalInterpolate(source, baseconfig,level);
		}

		private XElement InternalInterpolate(XElement source, IScope parentconfig,int level) {
		    
			var datasource = PrepareDataSource(source, parentconfig);
		    XElement changed;
			var processchild = InterpolateDataToElement(source, datasource, out changed);
		    source = changed;
			if (processchild && level>=1){
				level--;
				foreach (var e in source.Elements().ToArray()) {
					InternalInterpolate(e, datasource,level);
				}
			}
		    if (UseExtensions && source.Attr("xi-delete").ToBool()) {
		        return null;
		    }
			return source;
		}
		/// <summary>
		/// Интерполирует исходный элемет в целевой
		/// </summary>
		/// <param name="source"></param>
		/// <param name="baseelement"></param>
		/// <returns></returns>
		public XElement Interpolate(XElement source, XElement baseelement){

			var datasources = baseelement.AncestorsAndSelf().Reverse().ToArray();
			Scope cfg = null;
			foreach (var element in datasources) {
				cfg = new Scope(cfg);
				foreach (var attribute in element.Attributes()) {
					cfg.Set(attribute.Name.LocalName,attribute.Value);
				}
			}
			return Interpolate(source, cfg);
		}
		/// <summary>
		/// Установка символа начала интерполяции
		/// </summary>
		public char AncorSymbol {
			get { return _stringInterpolation.AncorSymbol; }
			set { _stringInterpolation.AncorSymbol = value; }
		}
		/// <summary>
		/// Стопер при обходе элементов - блокирует интерполяцию
		/// значения - 1,true - ниже, all - что либо другое - этот и ниже
		/// </summary>
		public string StopAttribute { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public IDictionary<string,object> SecondSource { get; set; }
		/// <summary>
		/// Обработка только атрибута code
		/// </summary>
		public bool CodeOnly { get; set; }
		/// <summary>
		/// Уровень обхода
		/// </summary>
		public int Level { get; set; }
        /// <summary>
        /// Использование расширенной интерполяции (новые функции)
        /// </summary>
	    public bool UseExtensions { get; set; }

        public IIncludeXmlProvider IncludeXmlProvider { get; set; }

	    private bool InterpolateDataToElement(XElement source, IScope datasource, out XElement result) {
            result = source;
            if (UseExtensions) {

	           
	            if (!MatchCondition(source, datasource,"if")) return false;

	            IncludeXmlProvider = IncludeXmlProvider ?? new IncludeXmlProvider();

	            var globalreplace = source.Attr("xi-replace");
	            if (!string.IsNullOrWhiteSpace(globalreplace)) {
	                _stringInterpolation.Interpolate(globalreplace);
	                var xml = IncludeXmlProvider.GetXml(globalreplace, source,datasource);
	                if (null == xml) {
	                    xml = XElement.Parse("<span class='no-ex replace'>no replace "+globalreplace+"</span>");
	                }
                    result = xml;
	                if (null != source.Parent) {
	                    source.ReplaceWith(xml);
	                }
	                source = xml;
                }

              

	            if (source.Attr("xi-repeat").ToBool()) {
	                var replace = ProcessRepeat(source, datasource);
	                if (null == replace) {
	                    source.Remove();
	                }
	                else {
	                    source.ReplaceWith(replace);
	                }
	                return false;
	            }

                var include = source.Attr("xi-include");
                if (!string.IsNullOrWhiteSpace(include))
                {
                    _stringInterpolation.Interpolate(include);
                    var xml = IncludeXmlProvider.GetXml(include, source, datasource);
                    if (null == xml)
                    {
                        xml = XElement.Parse("<span class='no-ex include'>no replace " + include + "</span>");
                    }
                    source.ReplaceAll(xml);
                }


            }

			bool processchild = true;
			if (!string.IsNullOrWhiteSpace(StopAttribute)) {
				var stopper = source.Attribute(StopAttribute);
				if (null != stopper) {
					if (stopper.Value != "0") {
						if(stopper.Value=="all")	return false;
						processchild = false;
					}
				}
			}
			if (CodeOnly){
				foreach (var a in source.Attributes())
				{
					if (a.Name.LocalName == "code" || a.Name.LocalName == "id"){
						var val = a.Value;
						if (val.Contains(_stringInterpolation.AncorSymbol) && val.Contains(_stringInterpolation.StartSymbol)){
							val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
							a.Value = val;
							datasource.Set(a.Name.LocalName, val);
						}
					}
				}
			}
			else{

				bool changed = true;
				while (changed){
					changed = false;
					foreach (var a in source.Attributes().OrderBy(_ => {
					    if (_.Value.Contains("${") && _.Value.Contains("(")) {
					        return 1000;
					    }
					    return 0;
					}))
					{
						var val = a.Value;
						if (val.Contains(_stringInterpolation.AncorSymbol) && val.Contains(_stringInterpolation.StartSymbol))
						{
							val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
							changed = changed || (val != a.Value);
							a.Value = val;
							datasource.Set(a.Name.LocalName, val);
						}
					}
				}
				changed = true;
				while (changed){
					changed = false;
					foreach (var t in source.Nodes().OfType<XText>())
					{

						var val = t.Value;
						if (val.Contains(_stringInterpolation.AncorSymbol) && val.Contains(_stringInterpolation.StartSymbol))
						{
							val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
							changed = changed || val != t.Value;
							t.Value = val;
						}
					}
				}
				

			}
			return processchild;
		}

	    private ILogicalExpressionEvaluator le = null;

	    private  bool MatchCondition(XElement e, IScope ds, string suffix) {
	        var attrname = "xi-" + suffix;
	        le = le ?? new LogicalExpressionEvaluator();
            if (e.Attr(attrname).ToBool())
            {
                var ifname = e.Attr(attrname);
	            var ismatch =  le.Eval(ifname, ds);
	            
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

	    private XmlInterpolation repeater = null;
	    private XElement[] ProcessRepeat(XElement source, IScope datasource) {
	        object[] a = GetDataSource(source, datasource);
	        if (0 == a.Length) return null;



	        var result = new List<XElement>();

            var scope = source.Attr("xi-scope");
	        var scope2 = "";
	        
	        var expand = source.Attr("xi-expand").ToBool();
            var rep = source.Attr("xi-repeat");
            if (rep.Contains(" in "))
            {
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
                if (!string.IsNullOrWhiteSpace(scope2))
                {
                    cfg[scope2] = dict;
                }
                if (expand) {
                    foreach (var p in dict) {
                        cfg[p.Key] = p.Value;
                    }
                }

                var clone = new XElement(source);
                cfg.Set("this",clone);
                cfg.Set("parent",source);
                cfg.SetParent(datasource);
                cfg.Set("_idx", i);
                cfg.Set("_num", i + 1);
                cfg.Set("_i", dict);
                if (!MatchCondition(clone, cfg,"where")) continue;
	            
                clone.SetAttributeValue("xi-repeat",null);
                clone.SetAttributeValue("xi-scope",null);
                clone.SetAttributeValue("xi-expand",null);
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
	            name = Regex.Replace(name,@"^[\s\S]+?\s+in\s+", "");
	        }
	        if (name.StartsWith("$")) {
	            name = name.Substring(1);
	            var ds = source.XPathSelectElement("//xi-dataset[@code='" + name + "']");
	            if (null != ds) {
	                result = new List<object>();
	                foreach (var e in ds.Elements( )) {
	                    var dict = new Dictionary<string, object>();
	                    foreach (var attribute in e.Attributes()) {
	                        dict[attribute.Name.LocalName] = attribute.Value;
	                    }
	                    ((IList<object>)result).Add(dict);
	                }
	            }
	        }
	        else {
	            result = datasource.Get<object>(name) as IEnumerable;
	        }
	        if (null == result) return new object[]{};
	        return result.OfType<object>().ToArray();

	    }

	    private IScope PrepareDataSource(XElement source, IScope parent) {
			var result = new Scope();
			if (null != parent) {
				result.SetParent(parent);
			}
			foreach (var a in source.Attributes()) {
				
				result.Set(a.Name.LocalName,a.Value);
				
			}
            result.Set("this",source);
			return result;


		}
	}

    public interface IIncludeXmlProvider {
        XElement GetXml(string path, XElement current, IScope scope);
    }

    public class IncludeXmlProvider : IIncludeXmlProvider {
        public IncludeXmlProvider() {
            IncludeCache = new Dictionary<string, XElement>();
        }
        IDictionary<string, XElement> IncludeCache { get; set; }

        public XElement GetXml(string path, XElement current, IScope scope) {

            var filename = path.Contains("@") ? EnvironmentInfo.ResolvePath(path) : path;
            if (!Path.IsPathRooted(filename))
            {
                var dir = Path.GetDirectoryName(current.Describe().File);
                filename = Path.Combine(dir, filename);
            }
            if (!File.Exists(filename)) return null;
            if (!IncludeCache.ContainsKey(filename))
            {
                XElement result = null;
                if (filename.Contains(".bxl"))
                {
                    result = IncludeCache[filename] = new BxlParser().Parse(File.ReadAllText(filename), filename,
                        BxlParserOptions.ExtractSingle);

                }
                else
                {
                    result = IncludeCache[filename] = XElement.Load(filename);
                }
                Preprocess(result,current,scope);
                return result;
            }
            return IncludeCache[filename];
        }

        public virtual void Preprocess(XElement result,XElement current,IScope scope) {
          
        }
    }


    
}