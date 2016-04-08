using Qorpent.IoC;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	///     Поставщик исходни	ков для расширений по генерации данных
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "preprocessor.bssrcpkg",
		ServiceType = typeof (IBSharpSourceCodeProvider))]
	public class PreprocessorSourcePackageForBSharp : ResourceBasedSrcBSharpProvider{
		/// <summary>
		/// </summary>
		public PreprocessorSourcePackageForBSharp(){
			ResourceMarker = "(?i)preprocessor";
		}
	}
}