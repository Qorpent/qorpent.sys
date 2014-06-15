using System.IO;
using System.Linq;

namespace Qorpent.Scaffolding.Model.CodeWriters{
	/// <summary>
	///     Generates options class for cloneable facility
	/// </summary>
	public class CloneOptionsWriter : CodeWriterBase{
		/// <summary>
		/// </summary>
		/// <param name="model"></param>
		/// <param name="output"></param>
		public CloneOptionsWriter(PersistentModel model, TextWriter output = null) : base(model, output){
		}

		/// <summary>
		/// </summary>
		protected override void InternalRun(){
			WriteHeader();
			Using("System");
			Namespace(DefaultNamespce);
			Summary("Options for cloning in model");
			Class("CloneOptions");
			Summary("Defult instance");
			Write("public static readonly CloneOptions Default = new CloneOptions();");
			foreach (PersistentClass table in Tables.Where(_ => _.Cloneable)){
				foreach (
					Field fld in
						table.GetOrderedFields()
						     .Where(_ => (_.IsReference && _.ReferenceClass.Cloneable && !_.NoCode) || _.IsCloneByDefault)){
					string name = table.Name + fld.Name;
					Summary(name + " must be cloned");
					Write("public bool " + name + " = " + fld.IsCloneByDefault.ToString().ToLowerInvariant() + ";");
				}
				foreach (Field fld in table.GetOrderedReverse().Where(_ => _.IsReverese && _.Table.Cloneable && !_.NoCode)){
					string name = table.Name + fld.ReverseCollectionName;
					Summary(name + " must be cloned");
					Write("public bool " + name + " = " + fld.IsReverseCloneByDefault.ToString().ToLowerInvariant() + ";");
				}
			}
			Close();
			Close();
		}
	}
}