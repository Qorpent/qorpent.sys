﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;

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
				foreach (var e in source.Elements()) {
					InternalInterpolate(e, datasource,level);
				}
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

		private bool InterpolateDataToElement(XElement source, IConfig datasource) {
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
				foreach (var a in source.Attributes()){
					var val = a.Value;
					if (val.Contains(_stringInterpolation.AncorSymbol) && val.Contains(_stringInterpolation.StartSymbol)){
						val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
						a.Value = val;
						datasource.Set(a.Name.LocalName, val);
					}
				}

				foreach (var t in source.Nodes().OfType<XText>()){

					var val = t.Value;
					if (val.Contains(_stringInterpolation.AncorSymbol) && val.Contains(_stringInterpolation.StartSymbol)){
						val = _stringInterpolation.Interpolate(val, datasource, SecondSource);
						t.Value = val;
					}
				}

			}
			return processchild;
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