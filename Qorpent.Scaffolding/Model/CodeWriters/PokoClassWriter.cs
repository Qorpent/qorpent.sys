using System.Collections.Generic;
using System.IO;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Generates С# poko class for PersistentClass
	/// </summary>
	public class PokoClassWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="cls"></param>
		/// <param name="output"></param>
		public PokoClassWriter(PersistentClass cls, TextWriter output = null) : base(cls, output){
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun(){
			WriteStartClass();
			GenerateOwnFields();
			GenerateIncomeReferences();
			WriteEndClass();
		}


		private void GenerateIncomeReferences(){
			Field[] incomerefs = Cls.GetOrderedReverse().Where(_ => _.IsReverese && !_.NoCode).ToArray();
			foreach (Field incomeref in incomerefs){
				string name = incomeref.ReverseCollectionName;
				IBSharpClass cls = incomeref.Table.TargetClass;
				string clsname = cls.Name;
				if (cls.Namespace != Cls.Namespace){
					clsname = cls.FullName;
				}
				GenerateField(incomeref, "ICollection<" + clsname + ">", name, "(" + incomeref.Name + "  привязка )",
				              "?? (Native" + name + " = new List<" + clsname + ">())");
			}
		}

		private void GenerateOwnFields(){
			IList<string> interfaces = Cls.CSharpInterfaces;
			foreach (Field of in Cls.GetOrderedFields().Where(_ => !_.NoCode)){
				string hideCondition = of.Definition.GetSmartValue("qorpent-hide","hide");
				string qorpentOverride = of.Definition.Attr("override");
				bool hide = interfaces.Contains(hideCondition);
				if (hide) continue;
				bool over = interfaces.Contains(qorpentOverride);
				
				string name = of.Name;
				if (of.IsReference){
					GenerateRef(of);
				}
				else{
					GenerateField(of, "", name, over: over);
				}
				
			}
		}


		private void GenerateRef(Field f){
			string dtype;
			GenerateField(f, "", f.Name + f.ReferenceField, "(Идентификатор)");
			IBSharpClass cls = f.ReferenceClass.TargetClass;
			string nsname = cls.Namespace;
			if (nsname == f.Table.TargetClass.Namespace){
				dtype = cls.Name;
			}
			else{
				dtype = cls.FullName;
			}
			GenerateField(f, dtype);
		}

		private void GenerateField(Field fld, string dtype = "", string name = "", string subcomment = "", string init = null,
		                           bool over = false){
			if (string.IsNullOrWhiteSpace(dtype)){
				dtype = fld.DataType.CSharpDataType;
			}
			if (string.IsNullOrWhiteSpace(name)){
				name = fld.Name;
			}

			string sermode = fld.Definition.Attr("serialize");
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

			o.WriteLine("\t\t///<summary>");
			o.WriteLine("\t\t///" + fld.Comment + " " + subcomment);
			o.WriteLine("\t\t///</summary>");
			if (fld.Definition.Attr("comment").ToBool()){
				IList<string> commentline = fld.Definition.Attr("comment").SmartSplit(false, true, '\r', '\n');
				if (commentline.Count > 0){
					if (commentline.Count == 1){
						o.WriteLine("\t\t///<remarks>" + fld.Definition.Attr("comment") + "</remarks>");
					}
					else{
						o.WriteLine("\t\t///<remarks>");
						foreach (string cl in commentline){
							o.WriteLine("\t\t///" + cl);
						}
						o.WriteLine("\t\t///</remarks>");
					}
				}
			}
			if (null != serattribute){
				o.WriteLine("#if !NOQORPENT");
				o.WriteLine("\t\t[" + serattribute + "]");
				o.WriteLine("#endif");
			}
			string ltype = dtype.ToLower();
			if (over){
				
				if ((ltype == fld.DataType.CSharpDataType.ToLower()) || IsNonLazyType(ltype)){
					o.WriteLine(
						"\t\tpublic override {0} {1} {{get {{return Native{1}{2};}} set{{Native{1}=value;}}}}\r\n", dtype, name,
						init);
				}
				else{
					o.WriteLine(
						"\t\tpublic override {0} {1} {{get {{return ((null!=Native{1}{2} as {0}.Lazy )?( Native{1}{2} = (({0}.Lazy) Native{1}{2} ).GetLazy(Native{1}{2}) ): Native{1}{2} );}} set{{Native{1}=value;}}}}\r\n",
						dtype, name,
						init);
				}
				
			}
			else{
				if ((ltype == fld.DataType.CSharpDataType.ToLower()) || IsNonLazyType(ltype)){
					o.WriteLine("\t\tpublic virtual {0} {1} {{get {{return Native{1}{2};}} set{{Native{1}=value;}}}}\r\n", dtype, name,
					            init);
				}
				else{
					o.WriteLine(
						"\t\tpublic virtual {0} {1} {{get {{return ((null!=Native{1}{2} as {0}.Lazy )?( Native{1}{2} = (({0}.Lazy) Native{1}{2} ).GetLazy(Native{1}{2}) ): Native{1}{2} );}} set{{Native{1}=value;}}}}\r\n",
						dtype, name,
						init);
				}
			}
			o.WriteLine("\t\t///<summary>Direct access to " + name + "</summary>");
			o.WriteLine("\t\tprotected {0} Native{1};\r\n", dtype, name);
			o.WriteLine();
		}

		private static bool IsNonLazyType(string ltype){
			return ltype == "int32" || ltype == "int" || ltype == "string" || ltype == "decimal" || ltype == "bool" ||
			       ltype == "boolean" || ltype == "datetime" || ltype.Contains("icollection");
		}

		private void WriteEndClass(){
			o.WriteLine("\t}");
			o.WriteLine("}");
		}

		private void WriteStartClass(){
			WriteHeader();
			o.WriteLine("using System;");
			o.WriteLine("using System.Collections.Generic;");
			o.WriteLine("#if !NOQORPENT");
			o.WriteLine("using Qorpent.Serialization;");
			o.WriteLine("using Qorpent.Model;");
			o.WriteLine("#endif");
			o.Write("namespace {0} {{\r\n", Cls.Namespace);
			o.WriteLine("\t///<summary>");
			o.WriteLine("\t///" + Cls.Comment);
			o.WriteLine("\t///</summary>");
			o.WriteLine("#if !NOQORPENT");
			o.WriteLine("\t[Serialize]");
			o.WriteLine("#endif");
			o.Write("\tpublic partial class {0} ", Cls.Name);
			IList<string> interfaces = Cls.CSharpInterfaces;
			bool fst = true;
			foreach (string i in interfaces){
				if (fst){
					o.Write(": ");
					fst = false;
				}
				else{
					o.Write(", ");
				}
				o.Write(i);
			}
			o.WriteLine(" {");
			o.WriteLine("\t\t///<summary>Lazy load nest type</summary>");
			o.WriteLine("\t\tpublic class Lazy:" + Cls.Name + "{");
			o.WriteLine("\t\t\t///<summary>Function to get lazy</summary>");
			o.WriteLine("\t\t\tpublic Func<" + Cls.Name + "," + Cls.Name + "> GetLazy;");
			o.WriteLine("\t\t}");
		}
	}
}