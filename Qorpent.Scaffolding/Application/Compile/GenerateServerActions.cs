﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application{
	/// <summary>
	/// Создает классы дейтвий
	/// </summary>
	public class GenerateServerActions : CodeGeneratorTaskBase
	{
		/// <summary>
		/// 
		/// </summary>
		public GenerateServerActions(){
			ClassSearchCriteria = "ui-action";
			DefaultOutputName = "CSharp";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			foreach (var targetclass in targetclasses){
                if(targetclass.Compiled.Attr("nocode").ToBool())continue;
				yield return GetActionClass(targetclass);
			}
		}

		private Production GetActionClass(IBSharpClass e){
			var result = new Production { FileName = "Actions/"+ e.FullName + ".cs" };
			result.GetContent = () =>{
				var sb = new StringBuilder();
				sb.AppendLine(CommonHeader);
				sb.AppendLine("using System;");
				sb.AppendLine("using System.Collections.Generic;");
				sb.AppendLine("using Qorpent.Mvc;");
				sb.AppendLine("using Qorpent.Mvc.Binding;");

				var resultclass = new BSharpClassRef(e.Compiled.Attr("Result"));
                if (string.IsNullOrWhiteSpace(resultclass.Name)) {
                    resultclass.Name = "object";
                }
			    if (!string.IsNullOrWhiteSpace(e.Compiled.Attr("resultCSharpClass"))) {
			        resultclass.Name = e.Compiled.Attr("resultCSharpClass");

			    }
				var argumentclass = new BSharpClassRef(e.Compiled.Attr("Arguments"));
				if (resultclass.Namespace != e.Namespace && !string.IsNullOrWhiteSpace(resultclass.Namespace)){
					sb.AppendLine("using " + resultclass.Namespace + ";");
				}
				if (argumentclass.Namespace != e.Namespace && argumentclass.Namespace != resultclass.Namespace &&
				    !string.IsNullOrWhiteSpace(argumentclass.Namespace)){
					sb.AppendLine("using " + argumentclass.Namespace + ";");
				}
                foreach (var u in e.Compiled.Elements("using"))
                {
                    sb.AppendLine("using " + u.Attr("code") + ";");
                }

				if (resultclass.IsArray){
					resultclass.Name = "ICollection<" + resultclass.Name + ">";
				}

				sb.AppendLine("namespace " + e.Namespace + " {");
				sb.AppendLine("\t/// <summary>\r\n\t///\t" + e.Compiled.Attr("name") + "\r\n\t/// </summary>");
				var controller = e.Compiled.Attr("controller");
				var actionname = controller + "." + e.Name.ToLower();
			    if (actionname.StartsWith(".")) { //признак того что для Qweb сделан акшен на HOST
			        var url = e.Compiled.Attr("url").SmartSplit(false,true,'/');
			        actionname = url[0] + "." + url[1];
			    }
				var role = string.Join(",", e.Compiled.Elements("Role").Select(_ => _.Attr("code")));
				if (!string.IsNullOrWhiteSpace(role)) {
					role = ", Role = \"" + role + "\"";
				}
				sb.AppendLine(string.Format("\t[Action(\"{0}\"{1})]", actionname, role));

			    var baseClass = e.Compiled.Attr("baseclass", "ActionBase");
                

				sb.AppendLine("\tpublic partial class " + e.Name + ": "+baseClass+"<" + resultclass.Name + "> {");
				if (!string.IsNullOrWhiteSpace(argumentclass.Name)){
					WriteMemberSummary(sb, "Call argumets due to specification");
					sb.AppendLine("\t\t[Bind]");
					sb.AppendLine("\t\tpublic " + argumentclass.Name + " Args { get; set; }");
				}
				sb.AppendLine("\t}");
				sb.AppendLine("}");
				return sb.ToString();
			};
			return result;
		}
	}
}