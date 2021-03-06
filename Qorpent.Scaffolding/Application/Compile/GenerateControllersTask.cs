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
	        production = new Production{
		        FileName = Project.ProjectName + "-root.js",
                OnlyIfAutogenrate = true,
		        GetContent = () => GenerateRootController()
	        };
	        yield return production;
        }

	    private string GenerateRootController() {
	        var rootservices = _context.ResolveAll("ui-service");
		    var mainlayout = Project.Context.ResolveAll("ui-layout").FirstOrDefault();
		    if (null == mainlayout) return string.Empty;
		    var fname = mainlayout.Compiled.Attr("filename");
			if (string.IsNullOrWhiteSpace(fname)){
				fname = Project.ProjectName + "_" + mainlayout.Name;
			}
		    var sb = new StringBuilder();
	        var servicenames = rootservices.Select(_ => _.Compiled.Attr("code"));
			sb.AppendLine("//" + Production.AUTOGENERATED_MASK);
			sb.AppendLine("// Type definitions for " + Project.ProjectName);
			sb.AppendLine();
		    sb.AppendFormat(@"define(
    ['angular','errorcatcher','{0}_types','{0}_api','{0}_controllers','layout','menu',{1}], function(angular,errorcatcher,types,apictor){{
        angular.module('app',['ErrorCatcher','{0}_controllers','Layout','Menu',{1}])
            .controller('root',function($scope,$http,{2}){{
                $scope.api = apictor($http);
                $scope.layout = '{3}.html';
", Project.ProjectName, "'" + String.Join("','", servicenames) + "'", String.Join(",", servicenames), fname);
	        foreach (var service in rootservices.Select(_ => _.Compiled)) {
                sb.AppendLine("\t\t\t\t" + service.Attr("code") + "($scope," + service.ToJson() + ");");
	        }
		    sb.AppendLine("\t\t});");
		    sb.AppendLine("\t}");
		    sb.AppendLine(");");
		    return sb.ToString();
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
            if (targetclasses.SelectMany(_=>_.Compiled.Elements("item")).Any(_ => _.GetSmartValue("persistent","persistentCode").ToBool()) && services.All(_ => _ != "settings")) {
               services.Add("settings");
            }
	        var svcall = string.Join("','", services.OrderBy(_=>_));
			if (!string.IsNullOrWhiteSpace(svcall)){
				svcall = ", '" + svcall + "'";
			}
            return "/*" + Production.AUTOGENERATED_MASK + "*/" + string.Format(@"define(['{0}_api','{0}_types','angular'{1}], function (Api, Types) {{
    angular.module('{0}_controllers',[{1}])", Project.ProjectName, svcall);
        }

        private string GenerateSingleController(IBSharpClass targetclass) {
            var xml = targetclass.Compiled;
            var code = xml.Attr("fullcode").Split('.').Last();
            var sb = new StringBuilder();
            var deps = new Dictionary<string, XElement>();
            var advanced = new StringBuilder();
			if (xml.Elements().Any(_ => _.GetSmartValue("persistent", "persistentCode").ToBool()) && deps.All(_ => _.Value.Attr("type") != "settings"))
			{
                deps["settings"] = new XElement("service", new XAttribute("type", "settings"), new XAttribute("before","true"));
            }
            foreach (var s in xml.Elements("service")) {
                var scode = s.Attr("code");
                deps[scode] = s;
            }
			var deplist = string.Join("','", deps.Values.Select(_ => _.ChooseAttr("type", "code")).Distinct().OrderBy(_ => _));
			if (!string.IsNullOrWhiteSpace(deplist)){
				deplist = ", '" + deplist + "'";
			}
	        var calllist = string.Join(",", deps.Values.Select(_ => _.ChooseAttr("type", "code")).Distinct().OrderBy(_=>_));
			if (!string.IsNullOrWhiteSpace(calllist)){
				calllist = ", " + calllist;
			}
	        sb.Append(string.Format(@"
        .controller('{3}_{0}', ['$scope','$http','$rootScope'{1},'$element', function ($scope, $http, $rootScope{2},$element) {{ 
                $scope.api = Api($http, $rootScope);
				$scope.$services = {{}};
				$scope.$services.$element = $element;
				$scope.$services.$http = $http;
				$scope.$services.$rootScope = $rootScope;
", code, deplist, calllist, Project.ProjectName));
	        foreach (var dep in deps){
		        sb.Append("\t\t\t\t$scope.$services." + dep.Key + "=" + dep.Key + ";");
		        sb.AppendLine();
	        }
			sb.AppendFormat(
               @"				$scope.{2} = '{0}.html';
				$scope.title= '{1}';
",
 targetclass.Compiled.ChooseAttr("view", "code"),
 targetclass.Compiled.ChooseAttr("name", "code"),
 (targetclass.Compiled.Element("menu")==null)?"view":"_view"
 );
 
 

            var items = xml.Elements("item");
            foreach (var e in deps.Where(_ => _.Value.Attr("before").ToBool()).OrderBy(_=>_.Key)) {
                var type = e.Value.ChooseAttr("type", "code");
				UnifyPersistentCode(targetclass, e);
	            if (type == "refresh") {
                    SetupRefresh(targetclass, e, advanced);
                }
	            sb.AppendLine("\t\t\t\t" + type + "($scope," + e.Value.ToJson() + ");");
            }
            foreach (var item in items.OrderBy(_=>_.Attr("code"))) {
                var type = item.Attr("type");
                var typestr = "{}";
                var watcher = "";
                if (!string.IsNullOrWhiteSpace(type)) {
                    var persistent = item.GetSmartValue("persistent","persistentCode");
					if (persistent == "1"){
						persistent = Project.ProjectName + "-" + xml.Attr("code")+"-"+ item.Attr("code");
					}
                   
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

                }else if (item.HasElements){
	                var uson = new UObj();
	                foreach (var e in item.Elements()){
		                var u = e.XmlToUson();
		                u.Properties["itemtype"] = e.Name.LocalName;
		                uson.push(u);
	                }

	                typestr = uson.ToJson(UObjSerializeMode.Javascript);
                }


                sb.AppendLine("\t\t\t\t$scope."+item.Attr("code")+" = "+typestr+";");
                if (!string.IsNullOrWhiteSpace(watcher)) {
                    sb.AppendLine(watcher);
                }
            }
         
            foreach (var e in deps.Where(_=>!_.Value.Attr("before").ToBool()).OrderBy(_=>_.Key)) {
                var type = e.Value.ChooseAttr("type", "code");
				UnifyPersistentCode(targetclass, e);
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

	    private void UnifyPersistentCode(IBSharpClass targetclass, KeyValuePair<string, XElement> e){
		    var pc = e.Value.GetSmartValue("persistent", "persistentCode");
		    if (pc == "1"){
			    pc = Project.ProjectName + "-" + targetclass.Name + "-" + e.Value.Attr("target")+"-refresh";
		    }
		    if (pc.ToBool()){
			    e.Value.SetAttr("persistent", pc);
			    e.Value.SetAttr("persistentCode", pc);
		    }
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
                var subscribtions = el.Value.Elements("subscribe").ToArray();
				if (subscribtions.Length > 0) {
                    foreach (var s in subscribtions.OrderBy(_=>_.Attr("code"))) {
                        advanced.AppendLine("\t\t\t\t$rootScope.$on('" + s.Attr("code") + "', function(n,o){$scope." + target +
                                           "_refresh.refresh.run();});");   
				    }
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