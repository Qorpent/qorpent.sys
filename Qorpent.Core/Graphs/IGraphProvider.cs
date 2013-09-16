namespace Qorpent.Graphs
{
	/// <summary>
	/// Интерфейс генератора графов
	/// </summary>
	public interface IGraphProvider {
		/// <summary>
		/// Формирует SVG по скрипту
		/// </summary>
		/// <param name="script"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		object Generate(string script, GraphOptions options);
		
	}
}
