using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	///     Интерфейс генератора исходного кода для препроцессора
	/// </summary>
	public interface ISourceCodeGenerator{
		/// <summary>
		///     Выполняет трансформацию определения генератора в узлы для замены в вызывающем контексте
		/// </summary>
		/// <param name="project"></param>
		/// <param name="definition"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		IEnumerable<XNode> Execute(IBSharpProject project, XElement definition, object context = null);
	}
}