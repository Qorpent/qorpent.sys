using Qorpent.BSharp.Builder;
using Qorpent.IoC;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	///     Расширения компилятора B# для работы с DDL B#
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, "preprocessor.bsbext", ServiceType = typeof (IBSharpBuilderExtension))]
	public class DataCompilerExtensions : BSharpBuilderExtensionBase, IBSharpBuilderExtension{
		/// <summary>
		/// </summary>
		protected override void PrepareTasks(){
			Tasks.Add(new ExtendedPreprocessorTask(BSharpBuilderPhase.PreProcess));
			Tasks.Add(new ExtendedPreprocessorTask(BSharpBuilderPhase.PostProcess));
		}
	}
}