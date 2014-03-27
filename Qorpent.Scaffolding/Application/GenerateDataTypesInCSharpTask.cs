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
	public class GenerateDataTypesInCSharpTask : CodeGeneratorTaskBase{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			var enums = targetclasses.Where(_ => _.Compiled.Attr("enum").ToBool() ).ToArray();
			var structs = targetclasses.Where(_ => _.Compiled.Attr("struct").ToBool()).ToArray();
			var refcache = targetclasses.ToDictionary(_ => _.Name, _=>_);
			foreach (var @enum in enums){
				if(@enum.Compiled.Attr("generate")=="false")continue;
				yield return GenerateEnum(@enum, refcache);
			}

			foreach (var @struct in structs)
			{
				if (@struct.Compiled.Attr("generate") == "false") continue;
				yield return GenerateStruct(@struct, refcache);
			}
		}

		private Production GenerateStruct(IBSharpClass e, Dictionary<string, IBSharpClass> refcache){
			var result = new Production{FileName = e.FullName + ".cs"};
			var sb = new StringBuilder();
			sb.AppendLine(CommonHeader);
			sb.AppendLine("using System;");
			sb.AppendLine("using Qorpent.Serialization;");
			sb.AppendLine("using Qorpent.Mvc.Binding;");
			sb.AppendLine("using System.Xml;");
			sb.AppendLine("using System.Xml.Linq;");
			sb.AppendLine("using System.Collections.Generic;");

			sb.AppendLine("namespace " + e.Namespace + " {");
			sb.AppendLine("\t/// <summary>\r\n\t///\t" + e.Compiled.Attr("name") + "\r\n\t/// </summary>");
			sb.AppendLine("\t[Serialize]");
			sb.AppendLine("\tpublic partial class " + e.Name + " {");
			 
			foreach (var field in e.Compiled.Elements()){
				GenerateField(e, field, refcache,sb);
			}
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			result.Content = sb.ToString();
			return result;
		}
		
		private void GenerateField(IBSharpClass cls, XElement field, Dictionary<string, IBSharpClass> refcache, StringBuilder sb){
			sb.AppendLine();
			var type = field.Name.LocalName;
			if (type == "any"){
				type = "object";
			}else if (type == "xml"){
				type = "XElement";
			}
			var name = field.Attr("code");
			var comment = field.Attr("name");
			

			var attr = "Serialize";
			if (field.Attr("optional").ToBool())
			{
				attr = "SerializeNotNullOnly";
			}
			if (field.Attr("noreturn").ToBool())
			{
				attr = "IgnoreSerialize";
			}

			
			WriteMemberSummary(sb, comment);
			sb.AppendLine("\t\t[" + attr + "]");

			if (type == "dictionary"){
				var prefix = field.Attr("param-prefix",name+".");
				sb.AppendLine("\t\t[Bind(ParameterPrefix=\"" + prefix + "\")]");
				sb.AppendLine(string.Format("\t\tpublic IDictionary<string, string> {0} {{get{{return __{1};}}}}", name,name.ToLower()));
				sb.AppendLine(string.Format("\t\tprivate IDictionary<string, string> __{0} = new Dictionary<string, string>();",
											 name.ToLower()));
			}
			else{
				sb.AppendLine(string.Format("\t\tpublic {0} {1} {{get;set;}}", type, name));
			}

		}

	
		private Production GenerateEnum(IBSharpClass e, Dictionary<string, IBSharpClass> refcache){
			var result = new Production{FileName = e.FullName + ".cs"};
			var sb = new StringBuilder();
			var items = e.Compiled.Elements("item").ToArray();
			var type = "int";
			if (items.Count() > 24){
				type = "long";
			}
			sb.AppendLine(CommonHeader);
			sb.AppendLine("using System;");
			sb.AppendLine("namespace " + e.Namespace + " {");
			var summary = e.Compiled.Attr("name");
			sb.AppendLine("\t/// <summary>\r\n\t///\t" + summary + "\r\n\t/// </summary>");
			sb.AppendLine("\t[Flags]");
			sb.AppendLine("\tpublic enum " + e.Name + " : "+type+" {");
			sb.AppendLine();
			WriteMemberSummary(sb, "Отсутвующее значение");
			sb.AppendLine("\t\tUndefined = 0,");
			var val = 1;
			bool wascustom = false;
			bool wasreserved = false;
			foreach (var item in items ){
				sb.AppendLine();
				var code = item.Attr("code");
				summary = item.Attr("name");
				WriteMemberSummary(sb, summary);
				sb.AppendLine("\t\t" + code+" = "+val+",");
				if (code == "Custom"){
					wascustom = true;
				}else if (code == "Reserved"){
					wasreserved = true;
				}
				val *= 2;
			}
			if (!wascustom){
				sb.AppendLine();
				WriteMemberSummary(sb, "Пользовательский тип");
				sb.AppendLine("\t\tCustom = "+val+",");
				val *= 2;
			}
			if (!wasreserved){
				sb.AppendLine();
				WriteMemberSummary(sb, "Зарезервированное значение");
				sb.AppendLine("\t\tReserved = " + val + ",");
			}
			
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			result.Content = sb.ToString();
			return result;
		}


		/// <summary>
		/// 
		/// </summary>
		public GenerateDataTypesInCSharpTask()
			: base()
		{
			ClassSearchCriteria = "ui-data";
			DefaultOutputName = "CSharp/DataTypes";
		}

		
	}
}