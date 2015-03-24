using System;
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
			var enums = targetclasses.Where(_ => _.Compiled.Attr("enum").ToBool() && !_.Compiled.Attr("nocode").ToBool()  ).ToArray();
            var structs = targetclasses.Where(_ => _.Compiled.Attr("struct").ToBool() && !_.Compiled.Attr("nocode").ToBool() && !_.Compiled.ContainsElement("interface")).ToArray();

		    var interfaces =
		        _context.Get(BSharpContextDataType.Working).Where(_ => _.Compiled.ContainsElement("interface")).ToArray();
            
			var refcache = targetclasses.ToDictionary(_ => _.Name, _=>_);

            


			foreach (var @enum in enums){
				if(@enum.Compiled.Attr("generate")=="false")continue;
				if(@enum.Compiled.Attr("generate-app-cs")=="false")continue;
				yield return GenerateEnum(@enum, refcache);
			}

			foreach (var @struct in structs)
			{
				if (@struct.Compiled.Attr("generate") == "false") continue;
                if (@struct.Compiled.Attr("generate-app-cs") == "false") continue;
				yield return GenerateStruct(@struct, refcache);
			}

           
            foreach (var @interface in interfaces)
            {
                if (@interface.Compiled.Attr("generate") == "false") continue;
                yield return GenerateInterface(@interface, refcache);
            }
		}

	    private Production GenerateInterface(IBSharpClass @interface, Dictionary<string, IBSharpClass> refcache) {
	        var filename = @interface.Compiled.Element("interface").Attr("folder") + "/I" + @interface.Name + ".cs";
	        return new Production {FileName = filename, GetContent = () => GenerateInterfaceInternal(@interface,refcache)};
	    }

	    private string GenerateInterfaceInternal(IBSharpClass e, Dictionary<string, IBSharpClass> refcache) {
            var sb = new StringBuilder();
            sb.AppendLine(CommonHeader);
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            foreach (var u in e.Compiled.Elements("using"))
            {
                if (u.Attr("code") != e.Namespace) {
                    sb.AppendLine("using " + u.Attr("code") + ";");
                }
            }
            sb.AppendLine("namespace " + e.Namespace + " {");
            sb.AppendLine("\t/// <summary>\r\n\t///\t" + e.Compiled.Attr("name") + "\r\n\t/// </summary>");
            var implements = string.Join(", ", e.Compiled.Elements("implements").Select(_ => _.Attr("code")).Where(_=>_!="I"+e.Name));
            if (!string.IsNullOrWhiteSpace(implements))
            {
                implements = " : " + implements;
            }
            sb.AppendLine("\tpublic partial interface I" + e.Name + implements + " {");

            foreach (var field in e.Compiled.Elements())
            {
                if (field.Name.LocalName == "using") continue;
                if (field.Name.LocalName == "implements") continue;
                if (field.Name.LocalName == "interface") continue;
                GenerateInterfaceField(e, field, refcache, sb);
            }
            sb.AppendLine("\t}");
            sb.AppendLine("}");
            return sb.ToString();
	    }

	    private Production GenerateStruct(IBSharpClass e, Dictionary<string, IBSharpClass> refcache) {
	        var usenames = this.Project.Definition.Attr("NoFullNames").ToBool();
			var result = new Production{FileName = "DataTypes/" + (usenames?e.Name:e.FullName) + ".cs", GetContent = () => GenerateInternal(e, refcache)};
			return result;
		}

		private string GenerateInternal(IBSharpClass e, Dictionary<string, IBSharpClass> refcache){
			var sb = new StringBuilder();
			sb.AppendLine(CommonHeader);
			sb.AppendLine("using System;");
			sb.AppendLine("using Qorpent.Serialization;");
			sb.AppendLine("using Qorpent.Mvc.Binding;");
			sb.AppendLine("using System.Xml;");
			sb.AppendLine("using System.Xml.Linq;");
			sb.AppendLine("using System.Collections.Generic;");
		    foreach (var u in e.Compiled.Elements("using")) {
                
                sb.AppendLine("using "+u.Attr("code")+";");
		    }
			sb.AppendLine("namespace " + e.Namespace + " {");
			sb.AppendLine("\t/// <summary>\r\n\t///\t" + e.Compiled.Attr("name") + "\r\n\t/// </summary>");
			sb.AppendLine("\t[Serialize]");
		    var implements = string.Join(", ", e.Compiled.Elements("implements").Select(_ => _.Attr("code")));
		    if (!string.IsNullOrWhiteSpace(implements)) {
		        implements = " : " + implements;
		    }
			sb.AppendLine("\tpublic partial class " + e.Name + implements +" {");

			foreach (var field in e.Compiled.Elements()){
                if (field.Name.LocalName == "using") continue;
                if (field.Name.LocalName == "implements") continue;
                if (field.Name.LocalName == "interface") continue;
				GenerateField(e, field, refcache, sb);
			}
			sb.AppendLine("\t}");
			sb.AppendLine("}");
			return sb.ToString();
		}

        private void GenerateInterfaceField(IBSharpClass cls, XElement field, Dictionary<string, IBSharpClass> refcache, StringBuilder sb)
        {
            sb.AppendLine();
            var type = field.Name.LocalName;
            if (type == "any")
            {
                type = "object";
            }
            else if (type == "xml")
            {
                type = "System.Xml.XElement";
            }
            var name = field.Attr("code");
            var comment = field.Attr("name");   
            WriteMemberSummary(sb, comment);
            if (type == "datetime")
            {
                type = "DateTime";
            }
            if (type == "dictionary")
            {
                sb.AppendLine(string.Format("\t\tIDictionary<string, string> {0} {{get;}}", name, name.ToLower()));
            }
            else if (type == "map")
            {
                sb.AppendLine(string.Format("\t\tIDictionary<string, object> {0} {{get;}}", name, name.ToLower()));
            }
            else
            {
                sb.AppendLine(string.Format("\t\t{0} {1} {{get;set;}}", type, name));
            }

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
			

			var attr = "SerializeNotNullOnly";
			if (field.Attr("required").ToBool())
			{
				attr = "Serialize";
			}
			if (field.Attr("noreturn").ToBool())
			{
				attr = "IgnoreSerialize";
			}

			
			WriteMemberSummary(sb, comment);
			sb.AppendLine("\t\t[" + attr + "]");
		    if (type == "datetime") {
		        type = "DateTime";
		    }
			if (type == "text") type = "string";
			 if (type == "dictionary"){
				var prefix = field.Attr("param-prefix",name+".");
				sb.AppendLine("\t\t[Bind(ParameterPrefix=\"" + prefix + "\")]");
				sb.AppendLine(string.Format("\t\tpublic IDictionary<string, string> {0} {{get{{return __{1};}}}}", name,name.ToLower()));
				sb.AppendLine(string.Format("\t\tprivate IDictionary<string, string> __{0} = new Dictionary<string, string>();",
											 name.ToLower()));
			}
            else if (type == "map")
            {
                var prefix = field.Attr("param-prefix", name + ".");
                sb.AppendLine("\t\t[Bind(ParameterPrefix=\"" + prefix + "\")]");
                sb.AppendLine(string.Format("\t\tpublic IDictionary<string, object> {0} {{get{{return __{1};}}}}", name, name.ToLower()));
                sb.AppendLine(string.Format("\t\tprivate IDictionary<string, object> __{0} = new Dictionary<string, object>();",
                                             name.ToLower()));
            }
			else{
				sb.AppendLine(string.Format("\t\tpublic {0} {1} {{get;set;}}", type, name));
			}

		}

	
		private Production GenerateEnum(IBSharpClass e, Dictionary<string, IBSharpClass> refcache){
			var result = new Production{FileName = "DataTypes/"+ e.FullName + ".cs", GetContent = () => GenerateInternal(e)};
			return result;
		}

		private static string GenerateInternal(IBSharpClass e){
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
			sb.AppendLine("\tpublic enum " + e.Name + " : " + type + " {");
			sb.AppendLine();
			WriteMemberSummary(sb, "Отсутвующее значение");
			sb.AppendLine("\t\tUndefined = 0,");
			var val = 1;
			bool wascustom = false;
			bool wasreserved = false;
			foreach (var item in items){
				sb.AppendLine();
			    var selfValue = item.Value;
			    if (string.IsNullOrWhiteSpace(selfValue)) {
			        selfValue = val.ToString();
			        val *= 2;
			    }
				var code = item.Attr("code");
				summary = item.Attr("name");
				WriteMemberSummary(sb, summary);
				sb.AppendLine("\t\t" + code + " = " + selfValue + ",");
				if (code == "Custom"){
					wascustom = true;
				}
				else if (code == "Reserved"){
					wasreserved = true;
				}
			}
			if (!wascustom){
				sb.AppendLine();
				WriteMemberSummary(sb, "Пользовательский тип");
				sb.AppendLine("\t\tCustom = " + val + ",");
				val *= 2;
			}
			if (!wasreserved){
				sb.AppendLine();
				WriteMemberSummary(sb, "Зарезервированное значение");
				sb.AppendLine("\t\tReserved = " + val + ",");
			}

			sb.AppendLine("\t}");
			sb.AppendLine("}");
			return sb.ToString();
		}


		/// <summary>
		/// 
		/// </summary>
		public GenerateDataTypesInCSharpTask()
			: base()
		{
			ClassSearchCriteria = "ui-data;attr:struct";
			DefaultOutputName = "CSharp";
		}

		
	}
}