using Qorpent.Model;

namespace Qorpent{
	/// <summary>
	/// Интерфейс абстрактных фильтров
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IFilter<T>:IWithIndex{
		/// <summary>
		/// Проверяет применимость фильтра к целевому объекту и контексту
		/// </summary>
		/// <param name="target"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		bool IsMatch(T target, IScope context = null);
		/// <summary>
		/// Применяет фильтр к целевому объекту в заданном контексте
		/// </summary>
		/// <param name="target"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		FilterState Apply(T target, IScope context = null);
	}
}