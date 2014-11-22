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
	public class GenerateDataTypesInJavaScriptTask : CodeGeneratorTaskBase{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses) {
		    var modulename = Project.ModuleName;
		    if (string.IsNullOrWhiteSpace(modulename)) {
		        modulename = Project.ProjectName;
		    }
			var production = new Production{
				FileName = modulename + "_types" + ".js",
				GetContent = () => GenerateInternal(targetclasses)
			};
			yield return production;
		}

		private string GenerateInternal(IBSharpClass[] targetclasses){
			var sb = new StringBuilder();
			sb.AppendLine("/*"+Production.AUTOGENERATED_MASK+"*/"+"// Type definitions for " + Project.ProjectName);
			sb.AppendLine();
			sb.AppendLine("define ([], function() {");
			sb.AppendLine("\tvar result = {};");
			var enums = targetclasses.Where(_ => _.Compiled.Attr("enum").ToBool()).ToArray();
			var structs = targetclasses.Where(_ => _.Compiled.Attr("struct").ToBool()).ToArray();
			var refcache = targetclasses.ToDictionary(_ => _.Name, _ => _);
			foreach (var @enum in enums){
				sb.AppendLine();
				if (@enum.Compiled.Attr("generate") == "false") continue;
				GenerateEnum(@enum, refcache, sb);
			}

			foreach (var @struct in structs){
				sb.AppendLine();
				if (@struct.Compiled.Attr("generate") == "false") continue;
				GenerateStruct(@struct, refcache, sb);
			}
			sb.AppendLine("\treturn result;");
			sb.AppendLine("});");
			return sb.ToString();
		}

		private string GenerateStruct(IBSharpClass e, Dictionary<string, IBSharpClass> refcache, StringBuilder sb){

			sb.AppendLine("\t// " + e.Compiled.Attr("name"));
			sb.AppendLine("\tvar "+e.Name+"= result." + e.Name + " = function(args){");
			sb.AppendLine("\t\targs=args||{}");
			sb.AppendLine("\t\tthis.__getClassInfo=function(){return {name:\""+e.FullName+"\"}};");
			foreach (var field in e.Compiled.Elements()) {
			    if (field.Name.LocalName == "using")continue;
			    if (field.Name.LocalName == "implements")continue;
				GenerateField(e, field, refcache,sb);
			}
			sb.AppendLine("\t};");
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
			bool isarray = false;
			bool isenum = false;

			if (type.EndsWith("*")){
				isarray = true;
				type = type.Substring(0, type.Length - 1);
			}
			if (refcache.ContainsKey(type)){
				var tp = refcache[type];
				if (tp.Compiled.Attr("enum").ToBool()){
					isenum = true;
				}
			}
			var name = field.Attr("code");
			var comment = field.Attr("name");
			var isFunction = type == "function";
			
			
			WriteMemberSummary(sb,comment,"js");


			var val = field.Value;

			if (string.IsNullOrWhiteSpace(val)){
				if (isarray){
					val = "[]";
				}
				else if (isenum){
					val = type + ".Undefined";
				}
				else{
					if (type == "string"){
						val = "\"\"";
					}
					else if (type == "any" ||type == "map" || type=="dictionary"){
						val = "{}";
					}
					else if (type == "int" || type == "decimal"){
						val = "0";
					}
					else if (type == "bool"){
						val = "false";
					}
					else if (type == "datetime"){
						val = "new Date(1900,0,1)";
					}
                
					else{
						val = "new result." + type + "()";
					}
				}
			}
			else{
				if(val.StartsWith("(")&& val.EndsWith(")")){
					val = "\"" + val.Substring(1,val.Length-2).Escape(EscapingType.JsonValue).Replace("\\'", "'") + "\"";
				}
			}
			if (!val.StartsWith("\"") && !val.All(_=>_=='.'||char.IsLetter(_)) && !isFunction){
				val = "(" + val + ")";
			}

			if (isFunction) {
				sb.AppendLine(string.Format("\t\tthis.{0} = function() {{\n\t\t\t{1}\n\t\t}}", name, val.Trim()));
			} else {
				sb.AppendLine(string.Format("\t\tthis.{0} = args.hasOwnProperty(\"{0}\") ? args.{0} : ( args.hasOwnProperty(\"{2}\") ? args.{2} : {1}) ;", name, val, name.ToLower()));
			}

		}

	
		private string GenerateEnum(IBSharpClass e, Dictionary<string, IBSharpClass> refcache, StringBuilder sb){
		
			var items = e.Compiled.Elements("item").ToArray();
			var summary = e.Compiled.Attr("name");
			sb.AppendLine("\t//" + summary );
			var sb2 = new StringBuilder();
			var basis = "\tvar "+e.Name+" = result." + e.Name;
			sb.AppendLine(basis +" = {");
			WriteMemberSummary(sb, "Отсутвующее значение = 0", "js");
			sb.AppendLine("\t\tUndefined : 'Undefined',");
			sb2.AppendLine("\t\t\tUndefined : 0,");

			var val = 1;
			bool wascustom = false;
			bool wasreserved = false;
			foreach (var item in items ){
				var code = item.Attr("code");
				var name = item.Attr("name");
				var _val = val;
				if (!string.IsNullOrWhiteSpace(item.Value)){
					_val = item.Value.ToInt();
				}
				WriteEnumMember(sb,sb2, name, _val, basis, code);
				if (code == "Custom"){
					wascustom = true;
				}else if (code == "Reserved"){
					wasreserved = true;
				}
				val *= 2;
			}
			if (!wascustom){
				WriteEnumMember(sb,sb2, "Пользоватеьский тип", val, basis, "Custom");
				val *= 2;
			}
			if (!wasreserved){
				WriteEnumMember(sb,sb2, "Зарезервированное значение", val, basis, "Reserved");
			}

			sb.AppendLine("\t\t__Values : {");
			sb.AppendLine(sb2.ToString());
			sb.AppendLine("\t\t\t__TERMINAL : null");
			sb.AppendLine("\t\t}");
			
			sb.AppendLine("\t};");
			return sb.ToString();
		}

		private static void WriteEnumMember(StringBuilder sb, StringBuilder sb2, string name, int val, string basis, string code){
			string summary = name + " = " + val;
			sb.AppendLine();
			WriteMemberSummary(sb, summary, "js");
			sb.AppendLine("\t\t"+code+" : '"+code+"',");
			sb2.AppendLine("\t\t\t"+ code + " : " + val + ",");
		}


		/// <summary>
		/// 
		/// </summary>
		public GenerateDataTypesInJavaScriptTask()
			: base()
		{
			ClassSearchCriteria = "ui-data";
			DefaultOutputName = "Js";
		}

		
	}
}