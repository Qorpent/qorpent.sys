using System.Collections.Generic;
using System.Linq;
using System.Text;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// Формирует расширение и вспомогательные классы для клонирования объектов
	/// </summary>
	public class GenerateResolveTagFacility : CSharpModelGeneratorBase{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			yield return new Production
			{
				FileName = "Extensions/ResolveTagOptions.cs",
				GetContent = () => GenerateResolveTagOptions()
			};
			yield return new Production{
				FileName = "Extensions/ResolveTagFacility.cs",
				GetContent = ()=>GenerateResolveTagFacilityClass()
			};
		}

		private string GenerateResolveTagOptions()
		{
			WriteHeader();
			Using("System");
			Namespace(DefaultNamespce);
			Summary("Options for tag resolution in model");
			Class("ResolveTagOptions");
			Summary("Defult instance");
			Write("public static readonly ResolveTagOptions Default  = new ResolveTagOptions();");
			foreach (var table in Tables.Where(_ => _.ResolveAble))
			{
				foreach (var fld in table.GetOrderedFields().Where(_=>_.Resolve)){
					var name = table.Name + fld.Name;
					Summary(name + " can be used in resolution");
					Write("public bool " + name + " = " + true + ";");
				}	
			}
			Close();
			Close();
			return o.ToString();
		}

		private string GenerateResolveTagFacilityClass(){
			WriteHeader();
			Using("System");
			Using("System.Collections.Generic");
			Using("System.Linq");
			Using("Qorpent.Utils.Extensions");
			Namespace(DefaultNamespce);
				Summary("Extensions for resolvetag model objects");
				ExtensionClass("ResolveTagFacility");
				foreach (var table in Tables.Where(_ => _.ResolveAble)){
					Summary("Resolves all tags for "+table.Name);
					Write("public static string ResolveTag(this " + table.Name + " target, string name, ResolveTagOptions options = null) {");
						IndentLevel++;
						Write("if(null==target)return \"\";");
						Write("if(string.IsNullOrWhitespace(name))return \"\";");
						Write("options = options ?? ResolveTagOptions.Default;");
						Write("var result = string.Empty;");
						foreach (var fld in table.GetOrderedFields().Where(_=>_.ResolveType!=ResolveType.None).OrderBy(_=>_.ResolvePriority)){
							Write("if (options." + table.Name + fld.Name + "){");
							IndentLevel++;
							if (fld.ResolveType == ResolveType.Delegate){
								Write("result = "+fld.Name+".ResolveTag(name);");
							}else if (fld.ResolveType == ResolveType.Tag){
								Write("result = TagHelper.Value ( " + fld.Name + ", name);");
							}
							else if (fld.ResolveType == ResolveType.List){
								Write("result = " + fld.Name + ".SmartSplit().Contains(name)?name:\"\"");
							}
							else if (fld.ResolveType == ResolveType.Dictionary){
								Write("if(" + fld.Name + ".ContainsKey(name) result = " + fld.Name + "[name]'");
							}
							Write("if (!string.IsNullOrWhitespace(result))return result;");
							Close();
						}
						Write("return result??\"\";");

					Close();
				}
				Close();
			Close();

			return o.ToString();
		}
	}
}