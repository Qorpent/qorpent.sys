using Qorpent.BSharp.Builder;
using Qorpent.IoC;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	///     Расширения компилятора B# для работы с DDL B#
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, "data.bsbext", ServiceType = typeof (IBSharpBuilderExtension))]
	public class DataCompilerExtensions : BSharpBuilderExtensionBase, IBSharpBuilderExtension{
		/// <summary>
		/// </summary>
		protected override void PrepareTasks(){
			Tasks.Add(new PrepareModelTask());
			Tasks.Add(new GenerateTsqlScriptsTask());
			Tasks.Add(new GeneratePokoClassesTask());
			Tasks.Add(new GeneratePokoClassDataAdapter());
			Tasks.Add(new GenerateModel());
			Tasks.Add(new GenerateExtendedCachedModel());
			Tasks.Add(new GenerateCloneableFacility());
			Tasks.Add(new GenerateResolveTagFacility());
			Tasks.Add(new GenerateTableStructureFileTask());
		}
	}
}