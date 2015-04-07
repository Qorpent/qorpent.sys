using Qorpent.BSharp.Builder;
using Qorpent.IoC;
using Qorpent.Scaffolding.Model.CodeWriters;

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
		    if (!Project.NoOutput) {
		        Tasks.Add(new GenerateTsqlScriptsTask());
		        Tasks.Add(new GeneratePokoClassesTask());
                Tasks.Add(new GeneratSimpleComparerClassesTask());
		        Tasks.Add(new GeneratePokoClassDataAdapter());
		        Tasks.Add(new GenerateModel());
		        Tasks.Add(new GenerateExtendedCachedModel());
		        Tasks.Add(new GenerateCloneableFacility());
		        Tasks.Add(new GenerateResolveTagFacility());
		        if (!string.IsNullOrWhiteSpace(EnvironmentInfo.GetExecutablePath("dot"))) {
		            Tasks.Add(new GenerateTableStructureFileTask());
		        }
		    }

		}
	}
}