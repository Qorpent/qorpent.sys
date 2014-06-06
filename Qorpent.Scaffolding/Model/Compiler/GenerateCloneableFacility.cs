using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// Формирует расширение и вспомогательные классы для клонирования объектов
	/// </summary>
	public class GenerateCloneableFacility : CSharpModelGeneratorBase{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			yield return new Production{
				FileName = "Extensions/CloneOptions.cs",
				GetContent = () => GenerateCloneOptions()
			};
			yield return new Production{
				FileName = "Extensions/CloneFacility.cs",
				GetContent = ()=>GenerateCloneableFacilityClass()
			};
		}

		private string GenerateCloneOptions(){
			WriteHeader();
			Using("System");
			Namespace(DefaultNamespce);
			Summary("Options for cloning in model");
				Class( "CloneOptions");
				Summary("Defult instance");
				Write("public static readonly CloneOptions Default = new CloneOptions();");
				foreach (var table in Tables.Where(_ => _.Cloneable)){
					foreach (var fld in table.GetOrderedFields().Where(_ => _.IsReference && _.ReferenceClass.Cloneable)){
						var name = table.Name + fld.Name;
						Summary(name + " must be cloned");
						Write("public bool " + name + " = " + fld.IsCloneByDefault.ToString().ToLowerInvariant() + ";");
					}
					foreach (var fld in table.GetOrderedReverse().Where(_ => _.IsReverese && _.Table.Cloneable)){
						var name = table.Name + fld.ReverseCollectionName;
						Summary(name + " must be cloned");
						Write("public bool " + name + " = " + fld.IsReverseCloneByDefault.ToString().ToLowerInvariant() + ";");
					}
				}
			Close();
			Close();
			return o.ToString();
		}

		private string GenerateCloneableFacilityClass(){
			WriteHeader();
			Using("System");
			Using("System.Collections.Generic");
			Using("System.Linq");
			Namespace(DefaultNamespce);
				Summary("Extensions for cloning model objects");
				ExtensionClass("CloneFacility");
				foreach (var table in Tables.Where(_ => _.Cloneable)){
					Indent();
					Summary("Clones "+table.Name);
					Write("public static " + table.Name + " Clone(this " + table.Name + " target, CloneOptions options = null) {");
						IndentLevel++;
						Write("if(null==target)return null;");
						Write("options = options ?? CloneOptions.Default;");
						Write("var result = new " + table.Name + "();");
						foreach (var fld in table.GetOrderedFields()){
							
							var reoptions = (fld.ReferenceClass == table) ? "options" : "";
							if (fld.IsReference && fld.ReferenceClass.Cloneable){
								Write("if(options." + table.Name+fld.Name + ")result." + fld.Name + " = result." + fld.Name + ".Clone("+reoptions+");");
								Write("else result." + fld.Name + " = target." + fld.Name + ";");
							}
							else{
								var name = fld.Name;
								if (name == "Idx")
								{
									name = "Index";
								}
								Write("result." + name + " = target." + name + ";");
							}
						}
						foreach (var fld in table.GetOrderedReverse().Where(_=>_.IsReverese )){
							if ( fld.Table.Cloneable)
							{
								Write("if(options." + table.Name+ fld.ReverseCollectionName + ") {");
								IndentLevel++;
								Write("result." + fld.ReverseCollectionName + " = target." + fld.ReverseCollectionName +
								             ".Select(_=>_.Clone(options)).ToList();");
								IndentLevel--;
								Indent();
								o.AppendLine("}else{");
								IndentLevel++;
								Write("result." + fld.ReverseCollectionName + " = target." + fld.ReverseCollectionName + ";");
								Close();
							}
							else
							{
								Write("result." + fld.ReverseCollectionName + " = target." + fld.ReverseCollectionName + ";");
							}
						}
						Write("return result;");

					Close();
				}
				Close();
			Close();

			return o.ToString();
		}
	}
}