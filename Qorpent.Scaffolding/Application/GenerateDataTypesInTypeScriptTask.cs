using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application{
	/// <summary>
	/// Формирует типы данных для приложения на C#
	/// </summary>
	public class GenerateDataTypesInTypeScriptTask : CodeGeneratorTaskBase{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			var production = new Production{
				FileName = Project.ProjectName + ".d.ts"
			};
			var sb = new StringBuilder();
			sb.AppendLine("// Type definitions for " + Project.ProjectName);
			sb.AppendLine();
			sb.AppendLine("declare module " + Project.ProjectName + " {");

			var enums = targetclasses.Where(_ => _.Compiled.Attr("enum").ToBool() ).ToArray();
			var structs = targetclasses.Where(_ => _.Compiled.Attr("struct").ToBool()).ToArray();
			var refcache = targetclasses.ToDictionary(_ => _.Name, _=>_);
			foreach (var @enum in enums){
				sb.AppendLine();
				if(@enum.Compiled.Attr("generate")=="false")continue;
				GenerateEnum(@enum, refcache,sb);
			}

			foreach (var @struct in structs)
			{
				sb.AppendLine();
				if (@struct.Compiled.Attr("generate") == "false") continue;
				GenerateStruct(@struct, refcache,sb);
			}
			sb.AppendLine("}");
			production.Content = sb.ToString();
			yield return production;
		}

		private string GenerateStruct(IBSharpClass e, Dictionary<string, IBSharpClass> refcache, StringBuilder sb){

			sb.AppendLine("\t\t// " + e.Compiled.Attr("name"));
			sb.AppendLine("\t\tinterface I" + e.Name + " {");
			 
			foreach (var field in e.Compiled.Elements()){
				GenerateField(e, field, refcache,sb);
			}
			sb.AppendLine("\t\t}");
			return sb.ToString();
		}
		
		private void GenerateField(IBSharpClass cls, XElement field, Dictionary<string, IBSharpClass> refcache, StringBuilder sb){

			if (field.Attr("noreturn").ToBool())
			{
				return;
			}
			
			var type = field.Name.LocalName;
			if (type == "object" || type=="dictionary"){
				type = "any";
			}else if (type == "xml"){
				type = "XElement";
			}
			var name = field.Attr("code");
			var comment = field.Attr("name");
			
			if (type != type.ToLower()){
				var foreign = refcache[type];
				if (null != foreign){
					if (foreign.Compiled.Attr("struct").ToBool()){
						type = "I" + type;
					}
				}
			}

			
			WriteMemberSummary(sb,comment,"ts");
				sb.AppendLine(string.Format("\t\t\t{0}: {1};",  name, type));
			

		}

	
		private string GenerateEnum(IBSharpClass e, Dictionary<string, IBSharpClass> refcache, StringBuilder sb){
			
			var items = e.Compiled.Elements("item").ToArray();
			var summary = e.Compiled.Attr("name");
			sb.AppendLine("\t\t//" + summary );
			sb.AppendLine("\t\tenum " + e.Name +" {");
			sb.AppendLine();
			WriteMemberSummary(sb, "Отсутвующее значение = 0","ts");
			sb.AppendLine("\t\t\tUndefined,");
			var val = 1;
			bool wascustom = false;
			bool wasreserved = false;
			foreach (var item in items ){
				sb.AppendLine();
				var code = item.Attr("code");
				summary = item.Attr("name")+ " = "+val;
				WriteMemberSummary(sb, summary, "ts");
				sb.AppendLine("\t\t\t" + code+",");
				if (code == "Custom"){
					wascustom = true;
				}else if (code == "Reserved"){
					wasreserved = true;
				}
				val *= 2;
			}
			if (!wascustom){
				sb.AppendLine();
				WriteMemberSummary(sb, "Пользовательский тип" + " = " + val,"ts");
				sb.AppendLine("\t\t\tCustom,");
				val *= 2;
			}
			if (!wasreserved){
				sb.AppendLine();
				WriteMemberSummary(sb, "Зарезервированное значение" + " = " + val,"ts");
				sb.AppendLine("\t\t\tReserved,");
			}
			
			sb.AppendLine("\t\t}");
			return sb.ToString();
		}


		/// <summary>
		/// 
		/// </summary>
		public GenerateDataTypesInTypeScriptTask()
			: base()
		{
			ClassSearchCriteria = "ui-data";
			DefaultOutputName = "";
		}

		
	}
}