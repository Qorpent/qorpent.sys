using System.Text;
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
			Tasks.Add(new GenerateServerActions());
			Tasks.Add(new GenerateDataTypesInTypeScriptTask());
			Tasks.Add(new GenerateDataTypesInJavaScriptTask());
			Tasks.Add(new GenerateJsonUiSpecification());
			Tasks.Add(new GenerateActionsInJavaScriptTask());
		}
	}
}