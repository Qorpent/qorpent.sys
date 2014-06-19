using System.Collections.Generic;
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
			DefaultOutputName = "CSharp/Actions";
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			foreach (var targetclass in targetclasses){
				yield return GetActionClass(targetclass);
			}
		}

		private Production GetActionClass(IBSharpClass e){
			var result = new Production { FileName = e.FullName + ".cs" };
			result.GetContent = () =>{
				var sb = new StringBuilder();
				sb.AppendLine(CommonHeader);
				sb.AppendLine("using System;");
				sb.AppendLine("using System.Collections.Generic;");
				sb.AppendLine("using Qorpent.Mvc;");
				sb.AppendLine("using Qorpent.Mvc.Binding;");

				var resultclass = new BSharpClassRef(e.Compiled.Attr("Result"));
				var argumentclass = new BSharpClassRef(e.Compiled.Attr("Arguments"));
				if (resultclass.Namespace != e.Namespace && !string.IsNullOrWhiteSpace(resultclass.Namespace)){
					sb.AppendLine("using" + resultclass.Namespace + ";");
				}
				if (argumentclass.Namespace != e.Namespace && argumentclass.Namespace != resultclass.Namespace &&
				    !string.IsNullOrWhiteSpace(argumentclass.Namespace)){
					sb.AppendLine("using" + argumentclass.Namespace + ";");
				}

				if (resultclass.IsArray){
					resultclass.Name = "IList<" + resultclass.Name + ">";
				}

				sb.AppendLine("namespace " + e.Namespace + " {");
				sb.AppendLine("\t/// <summary>\r\n\t///\t" + e.Compiled.Attr("name") + "\r\n\t/// </summary>");
				var controller = e.Compiled.Attr("controller");
				var actionname = controller + "." + e.Name.ToLower();
				sb.AppendLine(string.Format("\t[Action(\"{0}\")]", actionname));


				sb.AppendLine("\tpublic partial class " + e.Name + ": ActionBase<" + resultclass.Name + "> {");
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