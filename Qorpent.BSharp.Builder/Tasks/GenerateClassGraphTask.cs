using System.Collections.Generic;
using System.IO;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Graphs;
using Qorpent.Graphs.Dot;
using Qorpent.Graphs.Dot.Types;

namespace Qorpent.Integration.BSharp.Builder.Tasks
{
    /// <summary>
    /// Публикует наглядную схему сборки проекта
    /// </summary>
    public class GenerateClassGraphTask : BSharpBuilderTaskBase
    {
        /// <summary>
		/// 
		/// </summary>
        public GenerateClassGraphTask()
        {
			Phase = BSharpBuilderPhase.PostProcess;
			Index = TaskConstants.GenerateClassGraphTaskIndex;
		}

        /// <summary>
        /// Выполнение цели
        /// </summary>
        /// <param name="context"></param>
        public override void Execute(IBSharpContext context) {
            Project.Log.Info("Start GenerateClassGraphTask");
            var graph = BuildGraph(context);
            var dot = GraphRender.Create(graph).GenerateGraphScript();
            var dotfile = Path.Combine(Project.GetOutputDirectory(), "project.dot");
            File.WriteAllText(dotfile, dot);
            var dotr = new DotGraphProvider();
            var svg = dotr.Generate(dot,new GraphOptions());
            var svgfile = Path.Combine(Project.GetOutputDirectory(), "project.svg");
            File.WriteAllText(svgfile,(string)svg);
            Project.Log.Info("Finish GenerateClassGraphTask");
        }

        private Graph BuildGraph(IBSharpContext context) {
            
            var result = new Graph {
                RankDir = RankDirType.RL,
                Label = "Графическая структура проекта",
                DefaultNode = new Node {
                    FontName = "Consolas", 
                    Shape = NodeShapeType.Box3d,
                    FontSize = 8,
                    Style = NodeStyleType.Filled
                },
                DefaultEdge = new Node {
                    FontName = "Consolas",
                    FontSize = 8
                },
            };
            var visited= new List<IBSharpClass>();
            if (null != ((BSharpContext) context).Dictionaries) {
                foreach (var e in ((BSharpContext) context).Dictionaries) {
                    var node = new Node {Code = "d" + e.Key, Label = e.Key,SubgraphCode = "dicts",Shape = NodeShapeType.Mcircle};
                    result.AddNode(node);
                    foreach (var i in e.Value) {
                        result.AddEdge(new Edge {From = i.cls.FullName, To = "d" + e.Key, ArrowHead = ArrowType.Curve});
                    }
                }
            }
            foreach (var c in context.Get(BSharpContextDataType.Working)) {
                BuildClass(result,context, (BSharpClass)c, visited);
            }

            result.AutoTune();
            return result;
        }

        private void BuildClass(Graph g, IBSharpContext ctx, BSharpClass cls, IList<IBSharpClass> visited) {
            if (visited.Contains(cls)) return;
            visited.Add(cls);
            var label = cls.Name;
            if (!string.IsNullOrWhiteSpace(cls.Prototype)) {
                label = "[" + cls.Prototype + "]\r\n" + cls.Name;
            }
            var n = new Node {Code = cls.FullName, Label = label};
            if (!string.IsNullOrWhiteSpace(cls.Namespace)) {
                n.SubgraphCode  = (cls.Namespace.Replace(".", "__")) + "__";
            }
            CheckoutNamespace(g,cls.Namespace);
            if (cls.Is(BSharpClassAttributes.Abstract)) {
                n.Shape = NodeShapeType.Box;
                n.FillColor = Color.Yellow;
            }
            g.AddNode(n);
            if (null != cls.DefaultImport) {
                g.AddEdge(new Edge {From = cls.FullName, To = cls.DefaultImport.FullName,Label = "0"});
                BuildClass(g,ctx,(BSharpClass)cls.DefaultImport,visited);
            }
            int idx = 1;
            foreach (var i in cls.SelfImports) {
                g.AddEdge(new Edge {From = cls.FullName, To = i.Target.FullName,Label = (idx++).ToString()});
                BuildClass(g, ctx, (BSharpClass)i.Target, visited);
            }
            foreach (var i in cls.IncludedClasses) {
                g.AddEdge(new Edge { From = cls.FullName, To = i.FullName, ArrowHead =ArrowType.Diamond});
                BuildClass(g, ctx, (BSharpClass)i, visited);
            }
            foreach (var i in cls.ReferencedClasses)
            {
                g.AddEdge(new Edge { From = cls.FullName, To = i.FullName, ArrowHead = ArrowType.Vee,Color = Color.Blue});
                BuildClass(g, ctx, (BSharpClass)i, visited);
            }
            foreach (var i in cls.LateIncludedClasses)
            {
                g.AddEdge(new Edge { From = cls.FullName, To = i.FullName, ArrowHead = ArrowType.ODiamond, Color = Color.Blue });
                BuildClass(g, ctx, (BSharpClass)i, visited);
            }

            foreach (var i in cls.ReferencedDictionaries) {
                g.AddEdge(new Edge { From = cls.FullName, To = "d"+i, ArrowHead = ArrowType.Inv, Color = Color.Blue });
            }
            
        }

        private void CheckoutNamespace(Graph graph, string ns) {
            if (string.IsNullOrWhiteSpace(ns)) return;
            var parts = ns.Split('.');
            var fullns = "";
            SubGraph parent = graph;
            foreach (var p in parts) {
                fullns += p + "__";
                SubGraph ex = null;
                if (null == (ex=parent.ResolveSubgraph(fullns))) {
                    parent.SubGraphs.Add((ex=new SubGraph{Code = fullns,Label = p}));
                }
                parent = ex;
            }
        }
    }
}
