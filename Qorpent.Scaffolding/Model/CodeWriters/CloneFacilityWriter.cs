using System.IO;
using System.Linq;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Generates cloneable facility
	/// </summary>
	public class CloneFacilityWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public CloneFacilityWriter(PersistentModel model, TextWriter output = null)
			: base(model, output){
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun(){
			WriteHeader();
			Using("System");
			Using("System.Collections.Generic");
			Using("System.Linq");
			Namespace(DefaultNamespce);
			Summary("Extensions for cloning model objects");
			ExtensionClass("CloneFacility");
			foreach (PersistentClass table in Tables.Where(_ => _.Cloneable)){
				Indent();
				Summary("Clones " + table.Name);
				Write("public static " + table.Name + " Clone(this " + table.Name + " target, CloneOptions options = null) {");
				IndentLevel++;
				Write("if(null==target)return null;");
				Write("options = options ?? CloneOptions.Default;");
				Write("var result = new " + table.Name + "();");
				foreach (Field fld in table.GetOrderedFields().Where(_ => !_.NoCode)){
					string reoptions = (fld.ReferenceClass == table) ? "options" : "";
					if (fld.IsReference && fld.ReferenceClass.Cloneable){
						Write("if(options." + table.Name + fld.Name + ")result." + fld.Name + " = result." + fld.Name + ".Clone(" +
						      reoptions + ");");
						Write("else result." + fld.Name + " = target." + fld.Name + ";");
					}
					else if (fld.IsCloneByDefault){
						string tp = fld.DataType.CSharpDataType;
						if (tp.StartsWith("IDict")){
							tp = tp.Substring(1);
						}
						Write("if(options." + table.Name + fld.Name + ")result." + fld.Name + " = new " + tp + "(target." + fld.Name +
						      ");");
						Write("else result." + fld.Name + " = target." + fld.Name + ";");
					}
					else{
						Write("result." + fld.Name + " = target." + fld.Name + ";");
					}
				}
				foreach (Field fld in table.GetOrderedReverse().Where(_ => _.IsReverese && !_.NoCode)){
					if (fld.Table.Cloneable){
						Write("if(options." + table.Name + fld.ReverseCollectionName + ") {");
						IndentLevel++;
						Write("result." + fld.ReverseCollectionName + " = target." + fld.ReverseCollectionName +
						      ".Select(_=>_.Clone(options)).ToList();");
						IndentLevel--;
						Indent();
						o.WriteLine("}else{");
						IndentLevel++;
						Write("result." + fld.ReverseCollectionName + " = target." + fld.ReverseCollectionName + ";");
						Close();
					}
					else{
						Write("result." + fld.ReverseCollectionName + " = target." + fld.ReverseCollectionName + ";");
					}
				}
				Write("return result;");

				Close();
			}
			Close();
			Close();
		}
	}
}