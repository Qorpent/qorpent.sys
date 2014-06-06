﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// Задача по генерации Poko-классов модели на основе DataMapping,
	/// данные классы являются PurePOKO, то есть даже не наследуют никаких интерфейсов
	/// </summary>
	/// <remarks>Единственная опциональная завязка на Qorpent в этой генерации - отключаемые атрибуты Serialize</remarks>
	public class GeneratePokoClasses: CSharpModelGeneratorBase
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			foreach (var cls in Tables){
				var prod = new Production{
					FileName = "DataTypes/" +cls.Name + ".cs",
					GetContent = () => GenerateSingleClass(cls)
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
			o = new StringBuilder();
			WriteStartClass(targetclass);	
			GenerateOwnFields(targetclass);
			GenerateIncomeReferences(targetclass);
			WriteEndClass();
			return o.ToString();
		}

		private void GenerateIncomeReferences(PersistentClass targetclass)
		{
			var incomerefs = targetclass.ReverseFields.Values.Where(_=>_.IsReverese ).OrderBy(_ => _.Idx).ThenBy(_ => _.Name).ToArray();
			foreach (var incomeref in incomerefs){
				var name = incomeref.ReverseCollectionName;
				var cls = incomeref.Table.TargetClass;
				var clsname = cls.Name;
				if (cls.Namespace != targetclass.Namespace){
					clsname = cls.FullName;
				}
				GenerateField(incomeref, "ICollection<" + clsname + ">", name, "(" + incomeref.Name + "  привязка )",
				              "?? (Native" + name + " = new List<" + clsname + ">())");
			}
		}

		private void GenerateOwnFields(PersistentClass targetclass){
			var interfaces = targetclass.CSharpInterfaces;
			foreach (var of in targetclass.GetOrderedFields()){
				var qorpentHide = of.Definition.Attr("qorpent-hide");
				var qorpentOverride = of.Definition.Attr("override");
				var noqorpent = interfaces.Contains(qorpentHide);
				var over = interfaces.Contains(qorpentOverride);
				if (noqorpent){
					o.AppendLine("#if NOQORPENT");
				}
				var name = of.Name;
				if (of.IsReference)
				{
					GenerateRef(of);
					
				}
				else
				{
					if (name == "Idx")
					{
						name = "Index";
					}
					GenerateField(of, "", name,over:over);
				}
				if (noqorpent)
				{
					o.AppendLine("#endif");
				}
			}

		}

		

		

		private void GenerateRef(Field f){
			string dtype;
			GenerateField(f, "", f.Name +f.ReferenceField, "(Идентификатор)");
			var cls = f.ReferenceClass.TargetClass;
			string nsname = cls.Namespace;
			if (nsname == f.Table.TargetClass.Namespace){
				dtype = cls.Name;
			}
			else{
				dtype = cls.FullName;
			}
			GenerateField(f, dtype);
		}

		private void GenerateField(Field fld, string dtype = "", string name="", string subcomment = "", string init = null, bool over = false)
		{
			if (string.IsNullOrWhiteSpace(dtype)){
				dtype = fld.DataType.CSharpDataType;
			}
			if (string.IsNullOrWhiteSpace(name)){
				name = fld.Name;
			}

			var sermode = fld.Definition.Attr("serialize");
			string serattribute = null;
			if (!string.IsNullOrWhiteSpace(sermode)){
				if ("ignore" == sermode){
					serattribute = "IgnoreSerialize";
				}
				else if ("notnull" == sermode){
					serattribute = "SerializeNotNullOnly";
				}
				else{
					serattribute = "Serialize";
				}
			}

			o.AppendLine("\t\t///<summary>");
			o.AppendLine("\t\t///" +fld.Comment+" "+subcomment);
			o.AppendLine("\t\t///</summary>");
			if (fld.Definition.Attr("comment").ToBool()){
				var commentline = fld.Definition.Attr("comment").SmartSplit(false, true, '\r', '\n');
				if (commentline.Count > 0){
					if (commentline.Count == 1){
						o.AppendLine("\t\t///<remarks>" + fld.Definition.Attr("comment") + "</remarks>");
					}
					else{
						o.AppendLine("\t\t///<remarks>");
						foreach (var cl in commentline){
							o.AppendLine("\t\t///" + cl);
						}
						o.AppendLine("\t\t///</remarks>");
					}
				}
				
			}
			if (null != serattribute){
				o.AppendLine("#if !NOQORPENT");
				o.AppendLine("\t\t[" + serattribute + "]");
				o.AppendLine("#endif");
			}
			var ltype = dtype.ToLower();
			if (over){
				o.AppendLine("#if NOQORPENT");
				o.AppendFormat("\t\tpublic virtual {0} {1} {{get {{return Native{1}{2};}} set{{Native{1}=value;}}}}\r\n", dtype, name, init);
				o.AppendLine("#else");

				if ((ltype==fld.DataType.CSharpDataType.ToLower()) || IsNonLazyType(ltype))
				{
					o.AppendFormat(
						"\t\tpublic override {0} {1} {{get {{return Native{1}{2};}} set{{Native{1}=value;}}}}\r\n", dtype, name,
						init);
				}
				else{
					o.AppendFormat(
						"\t\tpublic override {0} {1} {{get {{return ((null!=Native{1}{2} as {0}.Lazy )?( Native{1}{2} = (({0}.Lazy) Native{1}{2} ).GetLazy(Native{1}{2}) ): Native{1}{2} );}} set{{Native{1}=value;}}}}\r\n", dtype, name,
						init);
				}
				o.AppendLine("#endif");
			}
			else{
				if ((ltype == fld.DataType.CSharpDataType.ToLower()) || IsNonLazyType(ltype))
				{
					o.AppendFormat("\t\tpublic virtual {0} {1} {{get {{return Native{1}{2};}} set{{Native{1}=value;}}}}\r\n", dtype, name, init);
				
				}
				else
				{
					o.AppendFormat(
						"\t\tpublic virtual {0} {1} {{get {{return ((null!=Native{1}{2} as {0}.Lazy )?( Native{1}{2} = (({0}.Lazy) Native{1}{2} ).GetLazy(Native{1}{2}) ): Native{1}{2} );}} set{{Native{1}=value;}}}}\r\n", dtype, name,
						init);
				}
				
			}
			o.AppendLine("\t\t///<summary>Direct access to " + name + "</summary>");
			o.AppendFormat("\t\tprotected {0} Native{1};\r\n", dtype, name);
			o.AppendLine();
		}

		private static bool IsNonLazyType(string ltype){
			return ltype == "int32" || ltype=="int" || ltype == "string" || ltype == "decimal" || ltype == "bool" || ltype == "boolean" || ltype == "datetime" || ltype.Contains("icollection");
		}

		private void WriteEndClass(){
			o.AppendLine("\t}");
			o.AppendLine("}");
		}

		private void WriteStartClass(PersistentClass targetclass){
			o.AppendLine(Header);
			o.AppendLine("using System;");
			o.AppendLine("using System.Collections.Generic;");
			o.AppendLine("#if !NOQORPENT");
			o.AppendLine("using Qorpent.Serialization;");
			o.AppendLine("using Qorpent.Model;");
			o.AppendLine("#endif");
			o.AppendFormat("namespace {0} {{\r\n", targetclass.Namespace);
			o.AppendLine("\t///<summary>");
			o.AppendLine("\t///" +targetclass.Comment);
			o.AppendLine("\t///</summary>");
			o.AppendLine("#if !NOQORPENT");
			o.AppendLine("\t[Serialize]");
			o.AppendFormat("\tpublic partial class {0} ",targetclass.Name);
			var interfaces = targetclass.CSharpInterfaces;
			var fst = true;
			foreach (var i in interfaces){
				if (fst){
					o.Append(": ");
					fst = false;
				}
				else{
					o.Append(", ");
				}
				o.Append(i);
			}
			o.AppendLine(" {");
			o.AppendLine("#else");
			o.AppendFormat("\tpublic partial class {0} {{\r\n", targetclass.Name);

			o.AppendLine("#endif");
			o.AppendLine("\t\t///<summary>Lazy load nest type</summary>");
			o.AppendLine("\t\tpublic class Lazy:" + targetclass.Name + "{");
			o.AppendLine("\t\t\t///<summary>Function to get lazy</summary>");
			o.AppendLine("\t\t\tpublic Func<" + targetclass.Name + "," + targetclass.Name + "> GetLazy;");
			o.AppendLine("\t\t}");
		}
	}
}