using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Bxl;
using Qorpent.IoC;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor
{
	/// <summary>
	/// Поставщик исходни	ков для расширений по генерации данных
	/// </summary>
	[ContainerComponent(Lifestyle.Transient, Name = "preprocessor.bssrcpkg", ServiceType = typeof(IBSharpSourceCodeProvider))]
	public class PreprocessorSourcePackageForBSharp : ResourceBasedSrcBSharpProvider{

		
		/// <summary>
		/// 
		/// </summary>
		public PreprocessorSourcePackageForBSharp()
		{
			ResourceMarker = "Preprocessor";
		}

		
	}
}
