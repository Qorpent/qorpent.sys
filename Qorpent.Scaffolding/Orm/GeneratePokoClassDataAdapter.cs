﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Orm{
	/// <summary>
	/// Формирует вспомогательный класс адаптера для DataReader
	/// </summary>
	public class GeneratePokoClassDataAdapter : CodeGeneratorTaskBase
	{
		private StringBuilder o;

		/// <summary>
		/// 
		/// </summary>
		public GeneratePokoClassDataAdapter()
			: base()
		{
			ClassSearchCriteria = "dbtable";
			DefaultOutputName = "Orm";
		}

		/// <summary>
		/// 
		/// </summary>
		private const string Header = "/*" + Production.AUTOGENERATED_MASK + "*/\r\n" + @"
//////////////////////////////////////////////////////////////////////
////       AUTO-GENERATED WITH  GeneratePokoClassDataAdapter     ////
//////////////////////////////////////////////////////////////////////
";
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			foreach (var targetclass in targetclasses){
				var prod = new Production{
					FileName = "Adapters/" +targetclass.Name + "DataAdapter.cs",
					GetContent = () => GenerateSingleClass(targetclass)
				};
				yield return prod;
			}
		}

		/// <summary>
		/// Формирует POKO класс на основе мапинга
		/// </summary>
		/// <param name="targetclass"></param>
		/// <returns></returns>
		private string GenerateSingleClass(IBSharpClass targetclass){
			o = new StringBuilder();
			var x = targetclass.Compiled;
			WriteStartClass(targetclass, x);
			WriteGetSelectQuery(targetclass, x);
			WriteSingleRecordProcessor(targetclass, x);
			WriteEnumerableReaderProcessor(targetclass, x);
			WriteEndClass();
			return o.ToString();
		}

		private void WriteSingleRecordProcessor(IBSharpClass targetclass, XElement xElement){
			o.AppendLine("\t\t///<summary>Implementation of ProcessRecord</summary>");
			o.AppendLine("\t\tpublic "+targetclass.Name+" ProcessRecord(IDataReader reader, bool nativeorder = false) {");
			o.AppendLine("\t\t\tvar result = new " + targetclass.Name + "();");
			o.AppendLine("\t\t\tif ( nativeorder ) {");
			var i = 0;
			foreach (var ormField in targetclass.GetOrmFields()){
				var type = ormField.Item3;
				var name = ormField.Item2.Attr("code");
				if (string.IsNullOrWhiteSpace(ormField.Item3)){ //ref
					type = "Int32";
					name += "Id";
					if (ormField.Item2.Attr("to").EndsWith(".Code")){
						type = "String";
						name += "Code";
					}
				}
				o.AppendLine("\t\t\t\tresult." + name + " = reader.Get" + type + "(" + i + ");");
				i++;
			}
			o.AppendLine("\t\t\t}else{");
			o.AppendLine("\t\t\t\tfor(var i=0;i<reader.FieldCount;i++){");
			o.AppendLine("\t\t\t\t\tvar name = reader.GetName(i).ToLowerInvariant();");
			o.AppendLine("\t\t\t\t\tvar value = reader.GetValue(i);");
			o.AppendLine("\t\t\t\t\tif(value is DBNull)continue;");
			o.AppendLine("\t\t\t\t\tswitch(name){");
			foreach (var ormField in targetclass.GetOrmFields())
			{
				var type = ormField.Item3;
				var name = ormField.Item2.Attr("code");
				if (string.IsNullOrWhiteSpace(ormField.Item3))
				{ //ref
					type = "Int32";
					name += "Id";
					if (ormField.Item2.Attr("to").EndsWith(".Code"))
					{
						type = "String";
						name += "Code";
					}
				}
				o.AppendLine("\t\t\t\t\t\tcase \"" + name.ToLower() + "\": result."+name+" = Convert.To"+type+"(val);break;");
				i++;
			}
			o.AppendLine("\t\t\t\t\t}");
			o.AppendLine("\t\t\t\t}");
			o.AppendLine("\t\t\t}");
			o.AppendLine("\t\t\treturn result;");
			o.AppendLine("\t\t}");
		}

		private void WriteEnumerableReaderProcessor(IBSharpClass targetclass, XElement xElement)
		{
			o.AppendLine("\t\t///<summary>Implementation of ProcessRecordSet</summary>");
			o.AppendLine("\t\tpublic IEnumerable<" + targetclass.Name + "> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {");
			o.AppendLine("\t\t\twhile(reader.Read()){");
			o.AppendLine("\t\t\t\tyield return ProcessRecord(reader,nativeorder);");
			o.AppendLine("\t\t\t}");
			o.AppendLine("\t\t}");
		}

		private void WriteGetSelectQuery(IBSharpClass targetclass, XElement x){
			o.AppendLine("\t\t///<summary>Implementation of PrepareSelectQuery</summary>");
			o.AppendLine("\t\tpublic string PrepareSelectQuery(object conditions = null, object hints = null) {");
			o.AppendLine("\t\t\tvar sb = new StringBuilder();");
			o.Append("\t\t\tsb.Append(\"select ");
			bool fst = true;
			foreach (var of in targetclass.GetOrmFields()){
				if (fst){
					fst = false;
				}
				else{
					o.Append(", ");
				}
				var n = of.Item2.Attr("code");
				
				var rn = n;
				if (of.Item2.Name.LocalName == "ref"){
					var sfx = "Id";
					if (of.Item2.Attr("to").EndsWith("Code")){
						sfx = "Code";
					}
					rn += sfx;
				}
				if (n == "Idx"){
					rn = "\\\"Index\\\"";

				}
				o.Append(n);
				if (rn != n){
					o.Append(" as " + rn+"");
				}

			}
			o.Append(" from " + targetclass.Compiled.Attr("fullname")+" \");");
			o.AppendLine();
			o.AppendLine("\t\t\tvar where = conditions as string;");
			o.AppendLine("\t\t\tif ( null != where ) {");
			o.AppendLine("\t\t\t\tsb.Append(\" where \");");
			o.AppendLine("\t\t\t\tsb.Append(where);");
			o.AppendLine("\t\t\t}");
			o.AppendLine("\t\t\treturn sb.ToString();");
			o.AppendLine("\t\t}");
		}

		private void WriteEndClass()
		{
			o.AppendLine("\t}");
			o.AppendLine("}");
		}

		private void WriteStartClass(IBSharpClass targetclass, XElement x)
		{
			o.AppendLine(Header);
			o.AppendLine("using System;");
			o.AppendLine("using System.Collections.Generic;");
			o.AppendLine("using System.Text;");
			o.AppendLine("#if !NOQORPENT");
			o.AppendLine("using Qorpent.Serialization;");
			o.AppendLine("using Qorpent.Model;");
			o.AppendLine("using Qorpent.Data;");
			o.AppendLine("#endif");
			o.AppendFormat("namespace {0}.Adapters {{\r\n", targetclass.Namespace);
			o.AppendLine("\t///<summary>");
			o.AppendLine("\t/// Data Adapter for " + x.Attr("name"));
			o.AppendLine("\t///</summary>");
			o.AppendLine("#if !NOQORPENT");
			o.AppendFormat("\tpublic partial class {0}DataAdapter : IObjectDataAdapter<{0}> {{\r\n", targetclass.Name);
			o.AppendLine("#else");
			o.AppendFormat("\tpublic partial class {0}DataAdapter {{\r\n", targetclass.Name);
			o.AppendLine("#endif");
			
		}
	}
}