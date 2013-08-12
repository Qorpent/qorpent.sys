using System.Collections.Generic;
using System.Xml.Linq;

namespace Qorpent.ObjectXml {
	/// <summary>
	/// Интерфейс компилятора ObjectXml - на входе у нас набор неких входных документов,
	/// на выходе набор итоговых классов с исключенными абстракциями
	/// </summary>
	public interface IObjectXmlCompiler {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="compilerConfig"></param>
		void Initialize(IObjectXmlCompilerConfig compilerConfig);
		/// <summary>
		/// Компилирует источники в перечисление итоговых классов 
		/// </summary>
		/// <param name="sources"></param>
		/// <returns></returns>
		ObjectXmlCompilerIndex Compile(IEnumerable<XElement> sources);

		/// <summary>
		/// Возвращает конфигурацию компилятора
		/// </summary>
		/// <returns></returns>
		IObjectXmlCompilerConfig GetConfig();
	}
}