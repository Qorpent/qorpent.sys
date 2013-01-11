using System;

namespace Qorpent.Mvc.Binding {
	/// <summary>
	/// Интерфейс специального атрибута биндинга для перекрытия стандартного поведения
	/// </summary>
	public interface ICustomBindConverter {
		/// <summary>
		/// Конвертирует строку в нужное значение контроллера
		/// </summary>
		/// <param name="action"></param>
		/// <param name="val"></param>
		/// <param name="context"></param>
		/// <param name="directsetter"> </param>
		void SetConverted(object action, string val, IMvcContext context,Action<object,object> directsetter);
	}
}