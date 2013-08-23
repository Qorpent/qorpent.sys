using System.Collections.Generic;
using System.Xml.Linq;
using Qorpent.Config;

namespace Qorpent.BSharp {
	/// <summary>
	///     Интерфейс компилятора BxlSharp - на входе у нас набор неких входных документов,
	///     на выходе набор итоговых классов с исключенными абстракциями
	/// </summary>
	public interface IBSharpCompiler {
		/// <summary>
		/// </summary>
		/// <param name="compilerConfig"></param>
		void Initialize(IBSharpConfig compilerConfig);

		/// <summary>
		///     Компилирует источники в перечисление итоговых классов
		/// </summary>
		/// <param name="sources"></param>
		/// <param name="preparedContext"></param>
		/// <returns></returns>
		IBSharpContext Compile(IEnumerable<XElement> sources, IBSharpContext preparedContext = null);

		/// <summary>
		///     Возвращает конфигурацию компилятора
		/// </summary>
		/// <returns></returns>
		IBSharpConfig GetConfig();
		/// <summary>
		/// Возвращает условия компиляции
		/// </summary>
		/// <returns></returns>
		IConfig GetConditions();
	}
}