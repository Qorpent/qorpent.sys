﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Serialization;
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
			var production = new Production{
				FileName = Project.ProjectName+"_api" + ".js"
			};
			var sb = new StringBuilder();
			sb.AppendLine("// Type definitions for " + Project.ProjectName);
			sb.AppendLine();
			sb.AppendLine("define ([\""+Project.ProjectName+"_types\",\"actionBuilder\"], function(types,actionBuilder) {");
			sb.AppendLine("\treturn  function  ($http, siteroot) {");
			sb.AppendLine("\t\treturn {");

			for (var i = 0; i < targetclasses.Length; i++){
				var cls = targetclasses[i];
				var name = cls.Compiled.Attr("controller") + "_" + cls.Name;
				sb.AppendLine("\t\t\t" + name + ": actionBuilder($http,siteroot,{");
				var attrs = cls.Compiled.Attributes().ToArray();
				for (var j = 0; j < attrs.Length;j++ ){
					var a = attrs[j];
					var aname = a.Name.LocalName;
					if (aname == "prototype" || aname == "fullcode") continue;
					var val = a.Value;
					if (aname == "Result" || aname == "Arguments")
					{
						if (val.EndsWith("*"))
						{
							val = val.Substring(0, val.Length - 1);
							sb.AppendLine("\t\t\t\tisarray : true,");
						}
						val = "types." + _context.Get(val).Name;

					}
					else
					{
						val = val.Escape(EscapingType.JsonValue);
						if (!(val == "0" || val.ToDecimal(true) != 0 || val == "true" || val == "null" || val == "false"))
						{
							val = "\"" + val + "\"";
						}

					}
					sb.AppendLine("\t\t\t\t" + aname.ToLower() + " : " + val + (j==attrs.Length-1? ",":""));
				}
	
				if (i == targetclasses.Length - 1){
					sb.AppendLine("\t\t\t})");
				}
				else{
					sb.AppendLine("\t\t\t}),");
				}
				
			}


				sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("});");
			production.Content = sb.ToString();
			yield return production;
		}

		

		/// <summary>
		/// 
		/// </summary>
		public GenerateActionsInJavaScriptTask()
			: base()
		{
			ClassSearchCriteria = "ui-action";
			DefaultOutputName = "";
		}

		
	}
}