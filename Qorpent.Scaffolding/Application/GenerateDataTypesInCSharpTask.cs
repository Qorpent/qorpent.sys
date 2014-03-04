using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.SqlGeneration;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Application{
	/// <summary>
	/// Формирует типы данных для приложения на C#
	/// </summary>
	public class GenerateDataTypesInCSharpTask : CodeGeneratorTaskBase{
		
		private const string CommonHeader = @"
//////////////////////////////////////////////////////////////////////
////       AUTO-GENERATED WITH  GenerateDataTypesInCSharpTask     ////
//////////////////////////////////////////////////////////////////////
";

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
			sb.AppendLine("using System.Xml;");
			sb.AppendLine("using System.Xml.Linq;");
			sb.AppendLine("using System.Collection.Generic;");

			sb.AppendLine("namespace " + e.Namespace + " {");
			sb.AppendLine("\t///<summary>\r\n\t///\t" + e.Compiled.Attr("name") + "\r\n\t///</summary>");
			sb.AppendLine("\t[Serialize]");
			sb.AppendLine("\tpublic class " + e.Name + " {");
			foreach (var field in e.Compiled.Elements()){
				GenerateField(e, field, refcache,sb);
			}
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			result.Content = sb.ToString();
			return result;
		}
		
		private void GenerateField(IBSharpClass cls, XElement field, Dictionary<string, IBSharpClass> refcache, StringBuilder sb){
			var type = field.Name.LocalName;
			if (type == "any"){
				type = "object";
			}else if (type == "xml"){
				type = "XElement";
			}
			var name = field.Attr("code");
			var comment = field.Attr("name");
			WriteMemberSummary(sb,comment);
			sb.AppendLine("\t\t[Serialize]");
			sb.AppendLine(string.Format("\t\tpublic {0} {1} {{get;set;}}", type, name));
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
			sb.AppendLine("\t///<summary>\r\n\t///\t" + summary + "\r\n\t///</summary>");
			sb.AppendLine("\t[Flags]");
			sb.AppendLine("\tpublic enum " + e.Name + " : "+type+" {");
			var val = 1;
			foreach (var item in items ){
				summary = item.Attr("name");
				WriteMemberSummary(sb, summary);
				sb.AppendLine("\t\t" + item.Attr("code")+" = "+val+",");
				val *= 2;
			}
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			result.Content = sb.ToString();
			return result;
		}

		private static void WriteMemberSummary(StringBuilder sb, string summary){
			sb.AppendLine("\t\t///<summary>\r\n\t\t///\t" + summary + "\r\n\t\t///</summary>");
		}


		/// <summary>
		/// 
		/// </summary>
		public GenerateDataTypesInCSharpTask()
			: base()
		{
			ClassSearchCriteria = "ui-data";
			DefaultOutputName = "CSharp";
		}

		
	}
}