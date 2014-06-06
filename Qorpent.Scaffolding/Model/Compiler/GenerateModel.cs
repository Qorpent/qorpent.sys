﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model.Compiler{

	/// <summary>
	/// Формирует вспомогательный класс адаптера для DataReader
	/// </summary>
	public class GenerateModel : CSharpModelGeneratorBase
	{
		

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){

			var genfactory = new Production{
				FileName = "Adapters/Model.cs",
				GetContent = ()=>GenerateModelClass()
			};
			yield return genfactory;
		}

		private string GenerateModelClass(){
			o = new StringBuilder();
			o.AppendLine(Header);
			o.AppendLine("using System;");
			o.AppendLine("#if !NOQORPENT");
			o.AppendLine("using Qorpent.Data;");
			o.AppendLine("#endif");
			o.AppendFormat("namespace {0}.Adapters {{\r\n", DefaultNamespce);
			o.AppendLine("\t///<summary>Model for " + DefaultNamespce + " definition</summary>");
			o.AppendLine("\tpublic partial class Model {");
			GenerateGetAdapterMethods();
			GenerateReferenceBehaviorMarkers();
			o.AppendLine("\t}");
			o.AppendLine("}");
			return o.ToString();
		}
		
		private void GenerateReferenceBehaviorMarkers(){
			var tables = Model.Classes.Values.OrderBy(_ => _.Name);
			foreach (var t in tables){
				foreach (var reference in t.GetOrderedFields().Where(_=>_.IsReference)){
					SetupDirectMarker(t,reference);
					if (reference.IsReverese){
						SetupBackCollectionMarker(t, reference);
					}
				}
				
			}
		}

		private void SetupDirectMarker(PersistentClass t,  Field reference){
			o.AppendLine("\t\t///<summary>Marks active auto foreign key link from " + t.Name + " to " +
			             reference.ReferenceClass.Name + " with " + reference.ReferenceField + " (" +
			             reference.Comment + ")</summary>");
			o.AppendLine("\t\tpublic bool AutoLoad" + t.Name + reference.Name + " = "+reference.IsAutoLoadByDefault.ToString().ToLowerInvariant()+";");
			o.AppendLine("\t\t///<summary>Marks active auto foreign key link from " + t.Name + " to " +
						 reference.ReferenceClass.Name + " with " + reference.ReferenceField + " (" +
						 reference.Comment + ") as LazyLoad </summary>");
			o.AppendLine("\t\tpublic bool Lazy" + t.Name + reference.Name + " = " + reference.IsLazyLoadByDefault.ToString().ToLowerInvariant() + ";");
		}

		private void SetupBackCollectionMarker(PersistentClass t, Field reference){
			o.AppendLine("\t\t///<summary>Marks active auto collection in " + reference.Table.Name + " of " +
						 t.Name + " with " + reference.Name + " (" +
						 reference.Comment + ")</summary>");
			o.AppendLine("\t\tpublic bool AutoLoad" + reference.ReferenceClass.Name + reference.ReverseCollectionName  + "=" + reference.IsAutoLoadReverseByDefault.ToString().ToLowerInvariant() + ";");
			o.AppendLine("\t\t///<summary>Marks active auto collection in " + reference.Table.Name + " of " +
						 t.Name + " with " + reference.Name + " (" +
						 reference.Comment + ") as Lazy</summary>");
			o.AppendLine("\t\tpublic bool Lazy" + reference.ReferenceClass.Name + reference.ReverseCollectionName + "=" + reference.IsLazyLoadReverseByDefault.ToString().ToLowerInvariant() + ";");
		}

		private void GenerateGetAdapterMethods(){
			o.AppendLine("\t\t///<summary>Retrieve data adapter by type (generic)</summary>");
			o.AppendLine("#if NOQORPENT");
			o.AppendLine("\t\tpublic static object GetAdapter<T>(){");
			o.AppendLine("\t\t\treturn GetAdapter(typeof(T));");
			o.AppendLine("#else");
			o.AppendLine("\t\tpublic static IObjectDataAdapter<T> GetAdapter<T>() where T:class,new(){");
			o.AppendLine("\t\t\treturn (IObjectDataAdapter<T>)GetAdapter(typeof(T));");
			o.AppendLine("#endif");
			o.AppendLine("\t\t}");
			o.AppendLine("\t\t///<summary>Retrieve data adapter by type</summary>");
			o.AppendLine("\t\tpublic static object GetAdapter(Type objectType){");
			o.AppendLine("\t\t\tswitch(objectType.Name){");
			foreach (var t in _context.ResolveAll("dbtable").OrderBy(_=>_.Name)){
				var tn = t.Name;
				if (t.Namespace != DefaultNamespce){
					tn = t.FullName;
				}
				o.AppendLine("\t\t\t\tcase \"" + tn + "\": return new " + tn + "DataAdapter();");
			}
			o.AppendLine("\t\t\t}");
			o.AppendLine("\t\t\treturn null;");
			o.AppendLine("\t\t}");
		}
	}
}