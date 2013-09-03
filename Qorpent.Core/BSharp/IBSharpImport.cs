using System.Xml.Linq;
using Qorpent.Config;

namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс записи об импорте
	/// </summary>
	public interface IBSharpImport {
		/// <summary>
		/// </summary>
		IBSharpClass Target { get; set; }

		/// <summary>
		///     Тип импорта
		/// </summary>
		string Condition { get; set; }

		/// <summary>
		///     Код цели
		/// </summary>
		string TargetCode { get; set; }

		/// <summary>
		///     Признак неразрешенного импорта
		/// </summary>
		bool Orphaned { get; set; }

		/// <summary>
		/// Исходное определение
		/// </summary>
		XElement Source { get; set; }

		/// <summary>
		/// Проверяет условные импорты
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		bool Match(IConfig config);
	}
}