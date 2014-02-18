using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.BSharp
{
	/// <summary>
	/// Интерфейс модулей исходного кода
	/// </summary>
	public interface IBSharpSourceCodeProvider{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compiler"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		IEnumerable<XElement> GetSources(IBSharpCompiler compiler, IBSharpContext context);
	}
}
