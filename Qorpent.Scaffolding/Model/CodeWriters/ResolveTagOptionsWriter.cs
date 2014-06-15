using System.IO;
using System.Linq;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Generates options class for ResolveTagable facility
	/// </summary>
	public class ResolveTagOptionsWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public ResolveTagOptionsWriter(PersistentModel model, TextWriter output = null)
			: base(model, output){
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun(){
			WriteHeader();
			Using("System");
			Namespace(DefaultNamespce);
			Summary("Options for tag resolution in model");
			Class("ResolveTagOptions");
			Summary("Defult instance for full resolution");
			Write("public static readonly ResolveTagOptions Default  = new ResolveTagOptions();");
			Summary("Default instance for resolution on only one level");
			Write("public static readonly ResolveTagOptions SelfOnly = new ResolveTagOptions{");
			IndentLevel++;
			foreach (PersistentClass table in Tables.Where(_ => _.ResolveAble)){
				foreach (Field fld in table.GetOrderedFields().Where(_ => _.Resolve && _.IsReference && !_.NoCode)){
					string name = table.Name + fld.Name;
					Write(name + "=false,");
				}
			}
			IndentLevel--;
			Write("};");
			foreach (PersistentClass table in Tables.Where(_ => _.ResolveAble)){
				foreach (Field fld in table.GetOrderedFields().Where(_ => _.Resolve && !_.NoCode)){
					string name = table.Name + fld.Name;
					Summary(name + " can be used in resolution");
					Write("public bool " + name + " = true;");
				}
			}
			Close();
			Close();
		}
	}
}