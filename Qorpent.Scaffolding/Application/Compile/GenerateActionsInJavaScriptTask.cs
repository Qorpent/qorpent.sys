﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Serialization;
using Qorpent.Uson;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application{
	/// <summary>
	/// Формирует типы данных для приложения на C#
	/// </summary>
	public class GenerateActionsInJavaScriptTask : CodeGeneratorTaskBase{
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
            var modulename = Project.ModuleName;
            if (string.IsNullOrWhiteSpace(modulename))
            {
                modulename = Project.ProjectName;
            }
			var production = new Production{
				FileName = modulename + "_api" + ".js",
				GetContent = () => GenerateInternal(targetclasses)
			};
			yield return production;
		}

		private string GenerateInternal(IBSharpClass[] targetclasses){
			var sb = new StringBuilder();
			sb.AppendLine("//" + Production.AUTOGENERATED_MASK);
			sb.AppendLine("// Type definitions for " + Project.ProjectName);
			sb.AppendLine();
            var modulename = Project.ModuleName;
            if (string.IsNullOrWhiteSpace(modulename))
            {
                modulename = Project.ProjectName;
            }
			sb.AppendLine("define ([\"" + modulename + "_types\",\"the\"], function(types,$the) {");
			sb.AppendLine("\treturn  function  ($http, $emitter) {");
			for (var i = 0; i < targetclasses.Length; i++){
				var _cls = targetclasses[i];
				sb.AppendLine("\t\tvar __" + _cls.Name + " = null;");
			}
			sb.AppendLine("\t\tvar __result = {};");
			sb.AppendLine();
			
			for (var i = 0; i < targetclasses.Length; i++){
				var cls = targetclasses[i];
				var name = cls.Name;
				if (!string.IsNullOrWhiteSpace(cls.Compiled.Attr("controller"))){
					name = cls.Compiled.Attr("controller") + "_" + cls.Name;
				}
				sb.AppendLine("\t\tObject.defineProperty(__result,'" + name + "',{get : function(){  return __"+name+"||(__"+name+" = $the.action({");
				var attrs =
					cls.Compiled.Attributes().Where(_ => _.Name.LocalName != "prototype" && _.Name.LocalName != "fullcode").ToArray();
				for (var j = 0; j < attrs.Length; j++){
					var a = attrs[j];
					var aname = a.Name.LocalName;
					if (aname == "prototype" || aname == "fullcode") continue;
					var val = a.Value;
					if (aname == "Url"){
						aname = "url";
					}
                    if (aname == "SupportsAsync") {
                        val = "supportsAsync";
                    }
					if (aname == "Result" || aname == "Arguments")
					{
						if (val.EndsWith("*"))
						{
							val = val.Substring(0, val.Length - 1);
							sb.AppendLine("\t\t\t\tisarray : true,");
						}
						val = "types." + _context.Get(val).Name;
					}
					else{
						val = val.Escape(EscapingType.JsonValue);
						if (!(val == "0" || val.ToDecimal(true) != 0 || val == "true" || val == "null" || val == "false")){
							val = "\"" + val + "\"";
						}
					}
					sb.AppendLine("\t\t\t" + aname.ToLower() + " : " + val + ",");
				}
                var emits = cls.Compiled.Elements("emit");
				foreach (var xElement in emits){
					if (string.IsNullOrWhiteSpace(xElement.Attr("code"))){
						xElement.SetAttributeValue("code",cls.Name.ToUpperInvariant());
					}
				}
                if (emits.Any()) {

                    sb.AppendLine("\t\t\temits: " + "['" + string.Join("','", emits.OrderBy(_=>_.Attr("code")).Select(_=>_.Attr("code"))) + "'],");
                }
				var parameters = cls.Compiled.Element("Parameters");
				if (null != parameters){
					sb.AppendLine("\t\t\tparameters: " + parameters.ToJson());
				}
				else{
					sb.AppendLine("\t\t\tparameters : null");
				}
				sb.AppendLine("\t\t},null,$emitter))},writeable:false});");
				sb.AppendLine();
			}


			sb.AppendLine("\t\treturn __result;");
			sb.AppendLine("\t}");
			sb.AppendLine("});");
			
			return sb.ToString();
		}


		/// <summary>
		/// 
		/// </summary>
		public GenerateActionsInJavaScriptTask()
			: base()
		{
			ClassSearchCriteria = "ui-action|ui-api";
			DefaultOutputName = "Js";
		}

		
	}
}