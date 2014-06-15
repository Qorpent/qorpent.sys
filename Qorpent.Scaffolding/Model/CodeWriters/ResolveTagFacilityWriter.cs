using System.IO;
using System.Linq;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Generates ResolveTagable facility
	/// </summary>
	public class ResolveTagFacilityWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public ResolveTagFacilityWriter(PersistentModel model, TextWriter output = null)
			: base(model, output){
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun(){
			WriteHeader();
			Using("System");
			Using("System.Collections.Generic");
			Using("System.Linq");
			Using("Qorpent.Utils.Extensions");
			Namespace(DefaultNamespce);
			Summary("Extensions for resolvetag model objects");
			ExtensionClass("ResolveTagFacility");
			foreach (PersistentClass table in Tables.Where(_ => _.ResolveAble)){
				Summary("Resolves all tags for " + table.Name);
				Write("public static string ResolveTag(this " + table.Name +
				      " target, string name, ResolveTagOptions options = null) {");
				IndentLevel++;
				Write("if(null==target)return \"\";");
				Write("if(string.IsNullOrWhiteSpace(name))return \"\";");
				Write("options = options ?? ResolveTagOptions.Default;");
				Write("var result = string.Empty;");
				foreach (
					Field fld in
						table.GetOrderedFields()
						     .Where(_ => _.ResolveType != ResolveType.None && !_.NoCode)
						     .OrderBy(_ => _.ResolvePriority)){
					Write("if (options." + table.Name + fld.Name + "){");
					IndentLevel++;
					if (fld.ResolveType == ResolveType.Delegate){
						Write("result = target." + fld.Name + ".ResolveTag(name);");
					}
					else if (fld.ResolveType == ResolveType.Tag){
						Write("result = TagHelper.Value ( target." + fld.Name + ", name);");
					}
					else if (fld.ResolveType == ResolveType.List){
						Write("result = target." + fld.Name + ".SmartSplit().Contains(name)?name:\"\";");
					}
					else if (fld.ResolveType == ResolveType.Dictionary){
						Write("if(target." + fld.Name + ".ContainsKey(name)) result = ((target." + fld.Name +
						      "[name])??string.Empty).ToString();");
					}
					Write("if (!string.IsNullOrWhiteSpace(result))return result;");
					Close();
				}
				Write("return result??\"\";");

				Close();
			}
			Close();
			Close();
		}
	}
}