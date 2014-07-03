using System.IO;
using Qorpent.Config;

namespace Qorpent.Report{
	/// <summary>
	///		Интерфейс элемента отчета
	/// </summary>
	public interface IContentItem : IConfig {
		/// <summary>
		///		Родительский образующий объект
		/// </summary>
		object Native { get; set; }
		/// <summary>
		///		Отрисовывает контент в поток
		/// </summary>
		/// <param name="output">Райтер для записи отрендеренного отчёта</param>
		void Render(TextWriter output);
	}
}