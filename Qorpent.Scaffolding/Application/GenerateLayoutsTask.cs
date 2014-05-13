﻿using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// Формирует типы данных для приложения на C#
    /// </summary>
    public class GenerateLayoutsTask : CodeGeneratorTaskBase {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetclasses"></param>
        /// <returns></returns>
        protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses) {

            foreach (var targetclass in targetclasses) {
                var filename = Project.ProjectName+"_"+ targetclass.Name + ".html";
                if (targetclass.Compiled.Attr("filename").IsNotEmpty()) {
                    filename = targetclass.Compiled.Attr("filename") + ".html";
                }
                var production = new Production{
	                FileName = filename,
	                GetContent = () => GenerateSingleLayout(targetclass)
                };
	            yield return production;
            }
        }

	    private string GenerateSingleLayout(IBSharpClass targetclass){
		    var xml = targetclass.Compiled;
		    PrepareElement(xml);
			var rootelement = new XElement("div")
                .SetAttr("ng-controller", "LayoutController")
                .SetAttr("class", "layout__container");
	        var subroot = new XElement("layout",new XAttribute("root","1"),new XAttribute("layout-item","1"),new XAttribute("height","max"),new XAttribute("width","max"));
            foreach (var a in targetclass.Compiled.Attributes()) {
                if(a.Name.LocalName=="code")continue;
                if(a.Name.LocalName=="prototype")continue;
                if(a.Name.LocalName=="filename")continue;
                if(a.Name.LocalName=="fullcode")continue;
                subroot.SetAttributeValue(a.Name,a.Value);
            }
                 rootelement.Add(subroot);
	        
		    foreach (var e in xml.Elements()){
				
			    GenerateSingleElement(subroot, e);
		    }
			return "<!--" + Production.AUTOGENERATED_MASK + "-->" + rootelement;
	    }

	    private void PrepareElement(XElement xml){
		    if (null == xml.Attribute("orientation")){
			    if (xml.Attr("name") == "vertical"){
				    xml.SetAttributeValue("orientation","vertical");
			    }
				if (xml.Attr("name") == "horizontal")
				{
					xml.SetAttributeValue("orientation", "horizontal");
				}
				var attr = xml.Attribute("vertical");
				if (null != attr)
				{
					xml.SetAttributeValue("orientation", "vertical");
					attr.Remove();
				}
			    attr = xml.Attribute("horizontal");
				if (null != attr){
					xml.SetAttributeValue("orientation", "horizontal");
					attr.Remove();
				}
				
		    }
	    }

	    private void GenerateSingleElement(XElement r, XElement e){
			PrepareElement(e);
		    if (e.Name.LocalName != "widget" ){
			    GenerateBlock(r, e);
		    }else if (e.Name.LocalName == "widget"){
			    GenerateWidget(r,e);
		    }
	    }

	    private void GenerateWidget(XElement root, XElement e){
			if (e.HasElements){
				var outerel = new XElement("layout", new XAttribute("height", "max"), new XAttribute("width", "max")).SetAttr("id", e.Attr("code"));
				outerel.SetAttr("ng-controller", GetControllerName(e.Attr("code")))
					  .SetAttr("class", e.Attr("class"))
					  .SetAttr("orientation","vertical");
				if (e.Attr("name") == "horizontal" || e.Attr("horizontal").ToBool() || e.Attr("orientation") == "horizontal"){
					outerel.SetAttr("orientation", "horizontal");
				}
				outerel.SetAttributeValue("layout-item", "1");
				foreach (var v in e.Elements().OrderBy(_=>_.Attr("order").ToInt())){


					var ctrlel = new XElement("widget", new XAttribute("height", "max"), new XAttribute("width", "max"),
					                          new XAttribute("orientation", "vertical"));

					foreach (var a in v.Attributes()){
						if (a.Name.LocalName == "code") continue;
						if (a.Name.LocalName == "controller") continue;
						if (a.Name.LocalName == "withtitle") continue;
						if (a.Name.LocalName == "titlelevel") continue;
						if (a.Name.LocalName == "name") continue;
						ctrlel.SetAttributeValue(a.Name, a.Value);
					}
					ctrlel.SetAttributeValue("layout-item", "1");
					var include = new XElement("ng-include").SetAttr("src", "view");
					if (null != v.Attribute("code")){
						include.SetAttr("src", "'" + v.Attr("code") + ".html'");
					}
					if (!string.IsNullOrWhiteSpace(v.Attr("name"))){
						var outer = new XElement("div", new XAttribute("ng-init", "title='" + v.Attr("name") + "'"));
						outer.Add(include);
						include = outer;
					}
					ctrlel.Add(include);
					outerel.Add(ctrlel);
				}

				root.Add(outerel);
			}
			else{
				var ctrlel = new XElement("widget", new XAttribute("height", "max"), new XAttribute("width", "max"), new XAttribute("orientation", "vertical"));
				ctrlel.SetAttr("ng-controller", GetControllerName(e.Attr("code")))
					  .SetAttr("class", e.Attr("class"));
				foreach (var a in e.Attributes())
				{
					if (a.Name.LocalName == "code") continue;
					if (a.Name.LocalName == "controller") continue;
					if (a.Name.LocalName == "withtitle") continue;
					if (a.Name.LocalName == "titlelevel") continue;
					if (a.Name.LocalName == "name") continue;
					ctrlel.SetAttributeValue(a.Name, a.Value);
				}
				ctrlel.SetAttributeValue("layout-item", "1");
				var include = new XElement("ng-include").SetAttr("src", "view");
				if (null != e.Attribute("view"))
				{
					include.SetAttr("src", "'" + e.Attr("view") + ".html'");
				}
				if (!string.IsNullOrWhiteSpace(e.Attr("name")))
				{
					var outer = new XElement("div", new XAttribute("ng-init", "title='" + e.Attr("name") + "'"));
					outer.Add(include);
					include = outer;
				}
				ctrlel.Add(include);
				root.Add(ctrlel);
			}
            
	    }

	    private void GenerateBlock(XElement root, XElement e){
            var zoneel = new XElement(e.Name, new XAttribute("height", "max"), new XAttribute("width", "max")).SetAttr("id", e.Attr("code"));
		foreach (var attr in e.Attributes())
				{
					zoneel.SetAttr(attr.Name.ToStr(), attr.Value);
				}

        zoneel.SetAttributeValue("layout-item", "1");
			root.Add(zoneel);
		    foreach (var c in e.Elements()){
				GenerateSingleElement(zoneel,c);
		    }
	    }


        /// <summary>
        /// 
        /// </summary>
        public GenerateLayoutsTask()
            : base() {
            ClassSearchCriteria = "ui-layout";
            DefaultOutputName = "View";
        }


        private string GetControllerName(string actionName) {
            if (actionName.Contains(".")) {
                return Project.ProjectName + "_" + actionName.Split('.').Last();
            }
            return actionName;
        }
    }
}