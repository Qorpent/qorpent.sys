using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.Config;
using Qorpent.Dsl.LogicalExpressions;
using Qorpent.LogicalExpressions;
using Qorpent.Utils.Extensions;
using Qorpent.Utils.LogicalExpressions;

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
            baseconfig = baseconfig ?? new ConfigBase();
            if (baseconfig is IConfig) {
                return InternalInterpolate(source, (IConfig)baseconfig, level);
            }
            return InternalInterpolate(source, new ConfigBase(baseconfig.ToDict()), level);
        }
		/// <summary>
		/// Выполняет интерполяцию атрибутов в строки по Xml элементу
		/// рекурсивно по всей подветке
		/// </summary>
		/// <returns></returns>
		public XElement Interpolate(XElement source, IConfig baseconfig= null){
			var level = Level;
			if (level <= 0){
				level = int.MaxValue;
			}
			return InternalInterpolate(source, baseconfig,level);
		}

		private XElement InternalInterpolate(XElement source, IConfig parentconfig,int level) {
			var datasource = PrepareDataSource(source, parentconfig);
			var processchild = InterpolateDataToElement(source, datasource);
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
			ConfigBase cfg = null;
			foreach (var element in datasources) {
				cfg = new ConfigBase(cfg);
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

	    private bool InterpolateDataToElement(XElement source, IConfig datasource) {
	        if (UseExtensions) {
	            if (!MatchCondition(source, datasource,"if")) return false;
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
					foreach (var a in source.Attributes())
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

	    private  bool MatchCondition(XElement e, IConfig ds, string suffix) {
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
	    private XElement[] ProcessRepeat(XElement source, IConfig datasource) {
	        object[] a = GetDataSource(source, datasource);
	        if (0 == a.Length) return null;



	        var result = new List<XElement>();

            var scope = source.Attr("xi-scope");
            foreach (var o in a) {
                var dict = o.ToDict();
                if (!string.IsNullOrWhiteSpace(scope)) {
                    dict = dict.ToDictionary(_ => scope + "." + _.Key, _ => _.Value);
                }
                var cfg = new ConfigBase(dict);
                
                var clone = new XElement(source);
                cfg.SetParent(datasource);
                if (!MatchCondition(clone, cfg,"where")) continue;
	            
                clone.SetAttributeValue("xi-repeat",null);
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
            }
	        return result.ToArray();
	    }

	    private static object[] GetDataSource(XElement source, IConfig datasource) {
	        IEnumerable result = null;
	        var name = source.Attr("xi-repeat");
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

	    private IConfig PrepareDataSource(XElement source, IConfig parent) {
			var result = new ConfigBase();
			if (null != parent) {
				result.SetParent(parent);
			}
			foreach (var a in source.Attributes()) {
				
				result.Set(a.Name.LocalName,a.Value);
				
			}
			return result;


		}
	}
}