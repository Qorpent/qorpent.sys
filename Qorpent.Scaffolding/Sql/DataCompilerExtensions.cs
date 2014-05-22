using Qorpent.BSharp.Builder;
using Qorpent.IoC;
using Qorpent.Scaffolding.Application;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// Расширения компилятора B# для работы с DDL B#
	/// </summary>
	[ContainerComponent(Lifestyle.Transient,"data.bsbext",ServiceType=typeof(IBSharpBuilderExtension))]
	public class DataCompilerExtensions : BSharpBuilderExtensionBase, IBSharpBuilderExtension
	{
		/// <summary>
		/// 
		/// </summary>
		protected override void PrepareTasks()
		{
			Tasks.Add(new GenerateTsqlScriptsTask());
			
		}
	}
}