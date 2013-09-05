using System.Collections.Generic;
using Qorpent.BSharp.Runtime;

namespace Qorpent.Serialization
{
	/// <summary>
	/// Интерфейс службы конвертации класса B# в граф  DOT
	/// </summary>
	public interface IBSharpDotConverter {
		/// <summary>
		/// Преобразует класс в DOT
		/// </summary>
		/// <param name="runtimeclass"></param>
		/// <returns></returns>
		IDotGraph BuildGraph(IBSharpRuntimeClass runtimeclass);
	}
	/// <summary>
	/// Интерфейс графа DOT
	/// </summary>
	public interface IDotGraph {
		/// <summary>
		/// Получить узлы
		/// </summary>
		/// <returns></returns>
		IEnumerable<IDotNode> GetNodes();
		/// <summary>
		/// Получить узлы
		/// </summary>
		/// <returns></returns>
		IEnumerable<IDotSubgraph> GetSubgraphs();
	}
	/// <summary>
	/// 
	/// </summary>
	public interface IDotSubgraph {
		
	}

	/// <summary>
	/// Узел дота
	/// </summary>
	public interface IDotNode {
	
	}
}
