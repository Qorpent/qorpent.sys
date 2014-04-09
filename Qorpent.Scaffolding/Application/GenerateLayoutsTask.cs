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
                var filename = targetclass.Name + ".html";
                if (targetclass.Compiled.Attr("filename").IsNotEmpty()) {
                    filename = targetclass.Compiled.Attr("filename") + ".html";
                }
                var production = new Production() {
                    FileName = filename
                };
                production.Content = GenerateSingleLayout(targetclass);
                yield return production;
            }
        }

        private string GenerateSingleLayout(IBSharpClass targetclass) {
            var xml = targetclass.Compiled;
            var rootelement = new XElement("div").SetAttr("layout", 1).SetAttr("class", "layout");
            foreach (var zone in xml.Elements()) {
                var zoneel = new XElement(zone.Name).SetAttr("id", zone.Attr("code"));
                var zoneroot = zoneel;
                if (zone.Attr("withcontainer").ToBool()) {
                    var container = new XElement("ng-container");
                    container.SetAttr("class", zone.Attr("row"))
                             .SetAttr("dynamicheight", zone.Attr("dynamicheight"))
                             .SetAttr("split", zone.Attr("split"))
                             .SetAttr("horizontal", zone.Attr("horizontal"))
                             .SetAttr("collapsible", zone.Attr("collapsible"));
                    foreach (var attr in zone.Attributes().Where(x => !x.Name.ToStr().IsIn(new[] { "dynamicheight", "split", "horizontal", "collapsible" }))) {
                        zoneel.SetAttr(attr.Name.ToStr(), attr.Value);
                    }
                    zoneel.Add(container);
                    zoneroot = container;
                } else {
                    foreach (var attr in zone.Attributes()) {
                        zoneel.SetAttr(attr.Name.ToStr(), attr.Value);
                    }  
                }
                foreach (var ctrl in zone.Elements()) {
                    var ctrlel = new XElement("div");
                    ctrlel.SetAttr("ng-controller", GetControllerName(ctrl.Attr("code")))
                          .SetAttr("class", ctrl.Attr("class"));
                    if (ctrl.Attr("withtitle").ToBool()) {
                        var title = new XElement("h" + (ctrl.Attr("titlelevel") ?? "3"));
                        title.SetValue(ctrl.Attr("name"));
                        ctrlel.Add(title);
                    }
                    ctrlel.Add(new XElement("ng-include").SetAttr("src", "view"));
                    zoneroot.Add(ctrlel);
                }
                rootelement.Add(zoneel);
            }
            return rootelement.ToString();
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