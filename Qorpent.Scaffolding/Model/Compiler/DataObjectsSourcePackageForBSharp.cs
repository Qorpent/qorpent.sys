using Qorpent.BSharp;
using Qorpent.IoC;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	///     Поставщик исходни	ков для расширений по генерации данных
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "data.bssrcpkg", ServiceType = typeof (IBSharpSourceCodeProvider))]
	public class DataObjectsSourcePackageForBSharp : ResourceBasedSrcBSharpProvider{
		/// <summary>
		/// </summary>
		public DataObjectsSourcePackageForBSharp(){
			ResourceMarker = "Model.Compile";
		}
	}
}