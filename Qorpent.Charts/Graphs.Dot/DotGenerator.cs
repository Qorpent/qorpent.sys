using System;
using Qorpent.Mvc.Renders;

namespace Qorpent.Graphs.Dot {
	/// <summary>
	/// Генератор языка DOT
	/// </summary>
	public class DotGenerator {
	    private DotRender _render;


		/// <summary>
		/// Формирует программу на языке DOT
		/// </summary>
		/// <param name="subGraph"></param>
		/// <returns></returns>
		public string Generate(SubGraph subGraph)
		{
		    Graph g;
		    if (subGraph is Graph)
		    {
		        g =(Graph) subGraph;
		    }
		    else
		    {
		        g = new Graph();
		        g.AddSubGraph(subGraph);
		    }
		    return GraphRender.Create(g).GenerateGraphScript();
		}
	}
}