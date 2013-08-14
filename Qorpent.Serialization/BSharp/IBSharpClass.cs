using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.Config;

namespace Qorpent.BSharp {
	/// <summary>
	/// Интерфейс класса в BSharp
	/// </summary>
	public interface IBSharpClass {
		/// <summary>
		/// </summary>
		string Name { get; set; }

		/// <summary>
		///     Расширение имени, пакет, используется при конфликтующем разрешении имен
		///     по умолчанию если классы указаны в Namespace резолюция ведется только в рамках
		///     этого namespace, если без namespace, то глобально (RootNs)
		/// </summary>
		string Namespace { get; set; }

		/// <summary>
		///     Полное имя
		/// </summary>
		string FullName { get; }

		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		IBSharpClass DefaultImport { get; set; }

		/// <summary>
		///     Код первичного класса импорта
		/// </summary>
		string DefaultImportCode { get; set; }

		/// <summary>
		///     Явные импорты
		/// </summary>
		IList<BSharpImport> SelfImports { get; }

		/// <summary>
		///     Определение сводимых элементов
		/// </summary>
		IList<BSharpElement> SelfElements { get; }

		/// <summary>
		/// </summary>
		XElement Source { get; set; }

		/// <summary>
		///     Компилированная версия класса
		/// </summary>
		XElement Compiled { get; set; }

		/// <summary>
		///     Элемент хранящий данные об индексе параметров
		/// </summary>
		IConfig ParamSourceIndex { get; set; }

		/// <summary>
		///     Сведенный словарь параметров
		/// </summary>
		IConfig ParamIndex { get; set; }

		/// <summary>
		/// Список всех определений мержа
		/// </summary>
		List<BSharpElement> AllElements { get; }

		/// <summary>
		/// Текущая задача на построение
		/// </summary>
		Task BuildTask { get; set; }

		/// <summary>
		/// Ошибка компиляции
		/// </summary>
		Exception Error { get; set; }

		/// <summary>
		/// Для расширений - имя целевого класса
		/// </summary>
		string TargetClassName { get; set; }

		/// <summary>
		///     Возвращает полное перечисление импортируемых классов в порядке их накатывания
		/// </summary>
		/// <value></value>
		IEnumerable<IBSharpClass> AllImports { get; }

		/// <summary>
		///     Полная проверка статуса Orphan
		/// </summary>
		/// <value></value>
		bool IsOrphaned { get; }

		/// <summary>
		/// Возвращает true при наличии флага
		/// </summary>
		/// <param name="attribute"></param>
		/// <returns></returns>
		bool Is(BSharpClassAttributes attribute);

		/// <summary>
		/// Устанавливает определенные флаги
		/// </summary>
		/// <param name="flags"></param>
		void Set(BSharpClassAttributes flags);

		/// <summary>
		/// Снимает определенные флаги
		/// </summary>
		/// <param name="flags"></param>
		void Remove(BSharpClassAttributes flags);
	}
}