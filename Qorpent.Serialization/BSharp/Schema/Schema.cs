using System;
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
		/// Общая политика поведения по элементам
		/// </summary>
		public ElementPolicy ElementPolicy { get; set; }

	}
	/// <summary>
	/// Правило элемента
	/// </summary>
	public class ElementRule {
		
	}

	/// <summary>
	/// Политика поведения внутренних элементов
	/// </summary>
	public enum ElementPolicy {
		/// <summary>
		/// Все что не запрещено - разрешено
		/// </summary>
		Free,
		/// <summary>
		/// Все что не разрешено - запрещено
		/// </summary>
		Strict,
	}
}
