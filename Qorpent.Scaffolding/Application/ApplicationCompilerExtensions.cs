using Qorpent.BSharp.Builder;
using Qorpent.Integration.BSharp.Builder.Tasks;
using Qorpent.IoC;

namespace Qorpent.Scaffolding.Application{
	/// <summary>
	/// Расширения компилятора B# для работы с DDL B#
	/// </summary>
	[ContainerComponent(Lifestyle.Transient,"app.bsbext",ServiceType=typeof(IBSharpBuilderExtension))]
	public class ApplicationCompilerExtensions : BSharpBuilderExtensionBase, IBSharpBuilderExtension
	{
		/// <summary>
		/// 
		/// </summary>
		protected override void PrepareTasks()
		{
			Tasks.Add(new GenerateDataTypesInCSharpTask());
		}
	}
}