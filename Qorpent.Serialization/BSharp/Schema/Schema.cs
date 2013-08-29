using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.BSharp.Schema
{
	/// <summary>
	/// Структура описания требований и авткоррекции схемы
	/// </summary>
	public class Schema
	{
		/// <summary>
		/// Запросы схемы на выполнение
		/// </summary>
		public ClassQuery[] Queries { get; set; }
		/// <summary>
		/// Общая политика поведения по элементам
		/// </summary>
		public ElementPolicy ElementPolicy { get; set; }
		/// <summary>
		/// Общее правило для всех элементов
		/// </summary>
		public ElementRule AllElementsRule { get; set; }
		/// <summary>
		/// Правило для корневого элемента
		/// </summary>
		public ElementRule RootElementRule { get; set; }
		/// <summary>
		/// Правило для всех элементов кроме корня
		/// </summary>
		public ElementRule BodyElementRule { get; set; }
		/// <summary>
		/// Правило для отдельног элмента
		/// </summary>
		public ElementRule[] NamedElementRules { get; set; }

	}
}
