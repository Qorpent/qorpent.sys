﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Uson;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application {
    /// <summary>
    /// Формирует типы данных для приложения на C#
    /// </summary>
    public class GenerateControllersTask : CodeGeneratorTaskBase {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetclasses"></param>
        /// <returns></returns>
        protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses) {
            var filename = Project.ProjectName + "_controllers.js";
            var production = new Production{
	            FileName = filename,
	            GetContent = () => GenerateInternal(targetclasses),
            };
	        yield return production;
        }

	    private string GenerateInternal(IBSharpClass[] targetclasses){
		    var result = "";
		    result += GetHeader(targetclasses);
		    foreach (var targetclass in targetclasses){
			    result += GenerateSingleController(targetclass);
		    }
		    result += GetFooter(targetclasses);
		    return result;
	    }

	    private string GetFooter(IBSharpClass[] targetclasses) {
            return ";\r\n});";
        }

        private string GetHeader(IBSharpClass[] targetclasses) {
            var services =
                targetclasses.SelectMany(_ => _.Compiled.Elements("service"))
                             .Select(_ => _.ChooseAttr("type","code"))
                             .Distinct()
                             .ToList();
            if (targetclasses.SelectMany(_=>_.Compiled.Elements("item")).Any(_ => !string.IsNullOrWhiteSpace(_.Attr("persistentCode"))) && services.All(_ => _ != "settings")) {
               services.Add("settings");
            }
            return "/*" + Production.AUTOGENERATED_MASK + "*/" + string.Format(@"define(['{0}_api','{0}_types','angular',{1}], function (Api, Types) {{
    angular.module('{0}_controllers',[{1}])", Project.ProjectName, "'" + string.Join("','", services) + "'");
        }

        private string GenerateSingleController(IBSharpClass targetclass) {
            var xml = targetclass.Compiled;
            var code = xml.Attr("fullcode").Split('.').Last();
            var sb = new StringBuilder();
            var deps = new Dictionary<string, XElement>();
            var advanced = new StringBuilder();
            if (xml.Elements("item").Any(_=>!string.IsNullOrWhiteSpace(_.Attr("persistentCode"))) && deps.All(_ => _.Value.Attr("type") != "settings")) {
                deps["settings"] = new XElement("service", new XAttribute("type", "settings"), new XAttribute("before","true"));
            }
            foreach (var s in xml.Elements("service")) {
                var scode = s.Attr("code");
                deps[scode] = s;
            }
            sb.Append(string.Format(@"
        .controller('{3}_{0}', ['$scope','$http','$rootScope','{1}', function ($scope, $http, $rootScope, {2}) {{ 
                $scope.api = Api($http); 
                $scope.view = '{0}.html';
", code, string.Join("','", deps.Values.Select(_ => _.ChooseAttr("type", "code")).Distinct()), string.Join(",", deps.Values.Select(_ => _.ChooseAttr("type", "code")).Distinct()), Project.ProjectName));

            var items = xml.Elements("item");
            foreach (var e in deps.Where(_ => _.Value.Attr("before").ToBool())) {
                var type = e.Value.ChooseAttr("type", "code");
                if (type == "refresh") {
                    SetupRefresh(targetclass, e, advanced);
                }
                sb.AppendLine("\t\t\t\t" + type + "($scope," + e.Value.ToJson() + ");");
            }
            foreach (var item in items) {
                var type = item.Attr("type");
                var typestr = "{}";
                var watcher = "";
                if (!string.IsNullOrWhiteSpace(type)) {
                    var persistent = item.Attr("persistentCode");
                   
                    dynamic ext = new UObj();
                    var parameters = item.Element("parameters");
                    if(null!=parameters)
                    {
                        foreach (var a in parameters.Attributes()) {
                            ext[a.Name.LocalName] = a.Value;
                        }
                    }
                    string ctor = ext.ToJson(UObjSerializeMode.Javascript);
                    if (!string.IsNullOrWhiteSpace(persistent)) {
                        ctor = "$.extend("+ctor+",$scope.settings.get('" + persistent + "'))";
                    }
                    typestr = "new Types." + type.Split('.').Last()+"("+ctor+")";
                    watcher = "\t\t\t\t$scope.$watch('" + item.Attr("code") + "',function(n,o){$scope.settings.set('" + persistent +
                              "',n)},true);";

                }
                sb.AppendLine("\t\t\t\t$scope."+item.Attr("code")+" = "+typestr+";");
                if (!string.IsNullOrWhiteSpace(watcher)) {
                    sb.AppendLine(watcher);
                }
            }
         
            foreach (var e in deps.Where(_=>!_.Value.Attr("before").ToBool())) {
                var type = e.Value.ChooseAttr("type", "code");
                if (type=="refresh") {
                    SetupRefresh(targetclass, e,advanced);
                }
                sb.AppendLine("\t\t\t\t" + type + "($scope," + e.Value.ToJson() + ");");
            }
            sb.AppendLine(advanced.ToString());
    
            sb.AppendLine("\t\t}])");
            
            
            //sb.AppendLine();
            return sb.ToString();
        }

        private void SetupRefresh(IBSharpClass targetclass, KeyValuePair<string, XElement> el, StringBuilder advanced) {
            var target = el.Value.Attr("target");
            var action = el.Value.Attr("action");
            var args = el.Value.Attr("args");
            if (string.IsNullOrWhiteSpace(args) && !string.IsNullOrWhiteSpace(target)) {
                args = targetclass.Compiled.Elements("item").First(_ => _.Attr("code") == target).Attr("args");
            }
            if (target.ToBool() && !action.ToBool()) {
                var item = targetclass.Compiled.Elements("item").Where(_ => _.Attr("code") == target).FirstOrDefault();
                if (null != item) {
                    var actionName = item.Attr("action");
                    if (actionName.ToBool()) {
                        el.Value.SetAttr("action", "javascript://$scope.api." + actionName.Split('.').Last());
                    }
                }
                if (!string.IsNullOrWhiteSpace(args)) {
                    el.Value.SetAttr("args", "javascript://$scope." + args);
                    advanced.AppendLine("\t\t\t\t$scope.$watch('" + args + "', function(n,o){$scope." + target +
                                        "_refresh.refresh.run();},true);");
					
                }
	            var subscribe = el.Value.Attr("subscribe");
				if (!string.IsNullOrWhiteSpace(subscribe)){
					advanced.AppendLine("\t\t\t\t$rootScope.$on('" + subscribe + "', function(n,o){$scope." + target +
					                    "_refresh.refresh.run();});");
				}
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public GenerateControllersTask()
            : base() {
            ClassSearchCriteria = "ui-controller";
            DefaultOutputName = "Js";
        }


    }
}