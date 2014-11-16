using System.IO;
using System.Linq;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	/// </summary>
	public class BaseModelWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public BaseModelWriter(PersistentModel model, TextWriter output = null) : base(model, output){
		}

		///
		protected override void InternalRun(){
			WriteHeader();
			o.WriteLine("using System;");
			o.WriteLine("#if !NOQORPENT");
			o.WriteLine("using Qorpent.Data;");
			o.WriteLine("#endif");
			o.Write("namespace {0}.Adapters {{\r\n", DefaultNamespce);
			o.WriteLine("\t///<summary>Model for " + DefaultNamespce + " definition</summary>");
			o.WriteLine("\tpublic partial class Model {");
			GenerateGetAdapterMethods();
			GenerateReferenceBehaviorMarkers();
			o.WriteLine("\t}");
			o.WriteLine("}");
		}

		private void GenerateReferenceBehaviorMarkers(){
			IOrderedEnumerable<PersistentClass> tables = Model.Classes.Values.OrderBy(_ => _.Name);
			foreach (PersistentClass t in tables){
                if(t.NoSql)continue;
				foreach (Field reference in t.GetOrderedFields().Where(_ => _.IsReference)){
					if (null == reference.ReferenceClass.TargetClass) continue; //reference to outer
					SetupDirectMarker(t, reference);
					if (reference.IsReverese ){
						SetupBackCollectionMarker(t, reference);
					}
				}
			}
		}

		private void SetupDirectMarker(PersistentClass t, Field reference){
			o.WriteLine("\t\t///<summary>Marks active auto foreign key link from " + t.Name + " to " +
			            reference.ReferenceClass.Name + " with " + reference.ReferenceField + " (" +
			            reference.Comment + ")</summary>");
			o.WriteLine("\t\tpublic bool AutoLoad" + t.Name + reference.Name + " = " +
			            reference.IsAutoLoadByDefault.ToString().ToLowerInvariant() + ";");
			o.WriteLine("\t\t///<summary>Marks active auto foreign key link from " + t.Name + " to " +
			            reference.ReferenceClass.Name + " with " + reference.ReferenceField + " (" +
			            reference.Comment + ") as LazyLoad </summary>");
			o.WriteLine("\t\tpublic bool Lazy" + t.Name + reference.Name + " = " +
			            reference.IsLazyLoadByDefault.ToString().ToLowerInvariant() + ";");
		}

		private void SetupBackCollectionMarker(PersistentClass t, Field reference){
			o.WriteLine("\t\t///<summary>Marks active auto collection in " + reference.Table.Name + " of " +
			            t.Name + " with " + reference.Name + " (" +
			            reference.Comment + ")</summary>");
			o.WriteLine("\t\tpublic bool AutoLoad" + reference.ReferenceClass.Name + reference.ReverseCollectionName + "=" +
			            reference.IsAutoLoadReverseByDefault.ToString().ToLowerInvariant() + ";");
			o.WriteLine("\t\t///<summary>Marks active auto collection in " + reference.Table.Name + " of " +
			            t.Name + " with " + reference.Name + " (" +
			            reference.Comment + ") as Lazy</summary>");
			o.WriteLine("\t\tpublic bool Lazy" + reference.ReferenceClass.Name + reference.ReverseCollectionName + "=" +
			            reference.IsLazyLoadReverseByDefault.ToString().ToLowerInvariant() + ";");
		}

		private void GenerateGetAdapterMethods(){
			o.WriteLine("\t\t///<summary>Retrieve data adapter by type (generic)</summary>");
			o.WriteLine("#if NOQORPENT");
			o.WriteLine("\t\tpublic static object GetAdapter<T>(){");
			o.WriteLine("\t\t\treturn GetAdapter(typeof(T));");
			o.WriteLine("#else");
			o.WriteLine("\t\tpublic static IObjectDataAdapter<T> GetAdapter<T>() where T:class,new(){");
			o.WriteLine("\t\t\treturn (IObjectDataAdapter<T>)GetAdapter(typeof(T));");
			o.WriteLine("#endif");
			o.WriteLine("\t\t}");
			o.WriteLine("\t\t///<summary>Retrieve data adapter by type</summary>");
			o.WriteLine("#if NOQORPENT");
			o.WriteLine("\t\tpublic static object GetAdapter(Type objectType){");
			o.WriteLine("#else");
			o.WriteLine("\t\tpublic static IObjectDataAdapter GetAdapter(Type objectType){");
			o.WriteLine("#endif");
			o.WriteLine("\t\t\tswitch(objectType.Name){");
			foreach (PersistentClass t in Tables){
				string tn = t.Name;
				if (t.Namespace != DefaultNamespce){
					tn = t.TargetClass.FullName;
				}
				o.WriteLine("\t\t\t\tcase \"" + tn + "\": return new " + tn + "DataAdapter();");
			}
			o.WriteLine("\t\t\t}");
			o.WriteLine("\t\t\treturn null;");
			o.WriteLine("\t\t}");
		}
	}
}