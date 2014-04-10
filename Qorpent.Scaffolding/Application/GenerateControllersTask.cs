﻿using System.Collections.Generic;
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

            var production = new Production() {
                FileName = filename,
                Content = ""
            };
            production.Content += GetHeader(targetclasses);
            foreach (var targetclass in targetclasses) {
               
                production.Content += GenerateSingleController(targetclass);
            }
            production.Content += GetFooter(targetclasses);
            yield return production;
        }

        private string GetFooter(IBSharpClass[] targetclasses) {
            return ";\r\n});";
        }

        private string GetHeader(IBSharpClass[] targetclasses) {
            var services =
                targetclasses.SelectMany(_ => _.Compiled.Elements("service"))
                             .Select(_ => _.Attr("code"))
                             .Distinct()
                             .ToArray();
            return string.Format(@"define(['{0}_api','angular',{1}], function (apictor) {{
    angular.module('{0}_controllers',[{1}])", Project.ProjectName, "'" + string.Join("','", services) + "'");
        }

        private string GenerateSingleController(IBSharpClass targetclass) {
            var xml = targetclass.Compiled;
            var code = xml.Attr("fullcode").Split('.').Last();
            var sb = new StringBuilder();
            var deps = xml.Elements("service").ToDictionary(_ => _.Attr("code"), _ => _);
            sb.Append(string.Format(@"
        .controller('{3}_{0}', ['$scope','$http','{1}', function ($scope, $http, {2}) {{ 
                var api = apictor($http); 
                $scope.view = '{0}.html';
", code, string.Join("','", deps.Keys), string.Join(",", deps.Keys), Project.ProjectName));
            var datael = xml.Elements().FirstOrDefault(x => x.Attr("code") == "data");
            if (null != datael) {
                sb.AppendLine("\t\t\t\t$scope.data = {};");
                if (!deps.ContainsKey("refresh")) {
                    sb.AppendLine(string.Format("api.{0}(function(_) {{$scope.data = _}});", datael.Attr("action").Split('.').Last()));
                }
            }
            foreach (var e in deps) {
                if (e.Key == "refresh") {
                    SetupRefresh(targetclass, e.Value);
                }
                sb.AppendLine("\t\t\t\t" + e.Key + "($scope," + e.Value.ToJson() + ");");
            }
    
            sb.AppendLine("\t\t}])");
            
            
            //sb.AppendLine();
            return sb.ToString();
        }

        private void SetupRefresh(IBSharpClass targetclass, XElement value) {
            var target = value.Attr("target");
            var action = value.Attr("action");
            if (target.ToBool() && !action.ToBool()) {
                var item = targetclass.Compiled.Elements("item").Where(_ => _.Attr("code") == target).FirstOrDefault();
                if (null != item) {
                    var actionName = item.Attr("action");
                    if (actionName.ToBool()) {
                        value.SetAttr("action", "javascript://api." + actionName.Split('.').Last());
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