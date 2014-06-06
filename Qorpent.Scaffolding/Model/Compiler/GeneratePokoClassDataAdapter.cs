﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// Формирует вспомогательный класс адаптера для DataReader
	/// </summary>
	public class GeneratePokoClassDataAdapter : CSharpModelGeneratorBase
	{
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			foreach (var targetclass in Model.Classes.Values){
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
		private string GenerateSingleClass(PersistentClass targetclass){
			
			WriteStartClass(targetclass);
			WriteGetTableQuery(targetclass);
			WriteGetSelectQuery(targetclass);
			WriteSingleRecordProcessor(targetclass);
			WriteEnumerableReaderProcessor(targetclass);
			WriteEndClass();
			return o.ToString();
		}

		private void WriteGetTableQuery(PersistentClass targetclass){
			o.AppendLine("\t\t///<summary>Implementation of GetTableName</summary>");
			o.AppendLine("\t\tpublic string GetTableName(object options = null) {");
			o.AppendLine("\t\t\treturn \""+targetclass.FullSqlName+"\";");
			o.AppendLine("\t\t}");
		}

		private void WriteSingleRecordProcessor(PersistentClass targetclass){
			o.AppendLine("\t\t///<summary>Implementation of ProcessRecord</summary>");
			o.AppendLine("\t\tpublic "+targetclass.Name+" ProcessRecord(IDataReader reader, bool nativeorder = false) {");
			o.AppendLine("\t\t\tvar result = new " + targetclass.Name + "();");
			o.AppendLine("\t\t\tif ( nativeorder ) {");
			var i = 0;
			foreach (var ormField in targetclass.GetOrderedFields()){
				var type = ormField.DataType.ReaderCSharpDataType;
				var name = ormField.Name;
				if (ormField.IsReference){ //ref
					name+= ormField.ReferenceField;
				}
				if (name == "Idx"){
					name = "Index";
				}
				var cast = "";
				if (type != ormField.DataType.CSharpDataType){
					cast = "(" + ormField.DataType.CSharpDataType + ")";
				}
				o.AppendLine("\t\t\t\tresult." + name + " = "+cast+"reader.Get" + type + "(" + i + ");");
				i++;
			}
			o.AppendLine("\t\t\t}else{");
			o.AppendLine("\t\t\t\tfor(var i=0;i<reader.FieldCount;i++){");
			o.AppendLine("\t\t\t\t\tvar name = reader.GetName(i).ToLowerInvariant();");
			o.AppendLine("\t\t\t\t\tvar value = reader.GetValue(i);");
			o.AppendLine("\t\t\t\t\tif(value is DBNull)continue;");
			o.AppendLine("\t\t\t\t\tswitch(name){");
			foreach (var ormField in targetclass.GetOrderedFields()){
				var type = ormField.DataType.ReaderCSharpDataType;
				var name = ormField.Name;
				if (ormField.IsReference)
				{
					name += ormField.ReferenceField;
				}
				if (name == "Idx"){
					name = "Index";
				}
				if (type != ormField.DataType.CSharpDataType){
					o.AppendLine("\t\t\t\t\t\tcase \"" + name.ToLower() + "\": result." + name + " = ("+ormField.DataType.CSharpDataType+")" +
							 "value;break;");
				}
				else{
					o.AppendLine("\t\t\t\t\t\tcase \"" + name.ToLower() + "\": result." + name + " = Convert.To" + type +
					             "(value);break;");
				}
				i++;
			}
			o.AppendLine("\t\t\t\t\t}");
			o.AppendLine("\t\t\t\t}");
			o.AppendLine("\t\t\t}");
			o.AppendLine("\t\t\treturn result;");
			o.AppendLine("\t\t}");
		}

		private void WriteEnumerableReaderProcessor(PersistentClass targetclass)
		{
			o.AppendLine("\t\t///<summary>Implementation of ProcessRecordSet</summary>");
			o.AppendLine("\t\tpublic IEnumerable<" + targetclass.Name + "> ProcessRecordSet(IDataReader reader, bool nativeorder = false) {");
			o.AppendLine("\t\t\twhile(reader.Read()){");
			o.AppendLine("\t\t\t\tyield return ProcessRecord(reader,nativeorder);");
			o.AppendLine("\t\t\t}");
			o.AppendLine("\t\t}");
		}

		private void WriteGetSelectQuery(PersistentClass targetclass){
			o.AppendLine("\t\t///<summary>Implementation of PrepareSelectQuery</summary>");
			o.AppendLine("\t\tpublic string PrepareSelectQuery(object conditions = null, object hints = null) {");
			o.AppendLine("\t\t\tvar sb = new StringBuilder();");
			o.Append("\t\t\tsb.Append(\"select ");
			bool fst = true;
			foreach (var of in targetclass.GetOrderedFields()){
				if (fst){
					fst = false;
				}
				else{
					o.Append(", ");
				}
				var n = of.Name;
				
				var rn = n;
				if (of.IsReference){
					rn += of.ReferenceField;
				}
				if (n == "Idx"){
					rn = "\\\"Index\\\"";
				}
				o.Append(n);
				if (rn != n){
					o.Append(" as " + rn+"");
				}

			}
			o.Append(" from " + targetclass.FullSqlName+" \");");
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

		private void WriteStartClass(PersistentClass targetclass)
		{
			WriteHeader();
			o.AppendLine("using System;");
			o.AppendLine("using System.Collections.Generic;");
			o.AppendLine("using System.Text;");
			o.AppendLine("using System.Data;");
			o.AppendLine("#if !NOQORPENT");
			o.AppendLine("using Qorpent.Data;");
			o.AppendLine("#endif");
			o.AppendFormat("namespace {0}.Adapters {{\r\n", targetclass.Namespace);
			o.AppendLine("\t///<summary>");
			o.AppendLine("\t/// Data Adapter for " + targetclass.Name);
			o.AppendLine("\t///</summary>");
			o.AppendLine("#if !NOQORPENT");
			o.AppendFormat("\tpublic partial class {0}DataAdapter : IObjectDataAdapter<{0}> {{\r\n", targetclass.Name);
			o.AppendLine("#else");
			o.AppendFormat("\tpublic partial class {0}DataAdapter {{\r\n", targetclass.Name);
			o.AppendLine("#endif");
			
		}
	}
}