using System.Collections.Generic;
using Qorpent.BSharp;
using Qorpent.Graphs;
using Qorpent.Graphs.Dot;
using Qorpent.Graphs.Dot.Builder;
using Qorpent.Graphs.Dot.Types;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// 
	/// </summary>
	public class GenerateTableStructureFileTask:CSharpModelGeneratorBase{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetclasses"></param>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			yield return new Production{
				FileName = Project.ProjectName + "_db_structure.svg",
				GetContent = () => GenerateSvg()
			};
		}

		private string GenerateSvg(){
			var g = new Graph();
			g.ColorScheme = "svg";
			g.RankDir = RankDirType.LR;
			foreach (var cls in Model.Classes.Values){
				if (cls.TargetClass.Compiled.Attr("nosvg").ToBool()) continue;
				var n = new Node{Code = cls.Name, Label = cls.Name, Tooltip = cls.Comment, Shape = NodeShapeType.Box3d};
				var dotcolor = cls.TargetClass.Compiled.Attr("dot-color");
				if (!string.IsNullOrWhiteSpace(dotcolor)){
					n.Style = NodeStyleType.Filled;
					n.FillColor = ColorAttribute.Single(Color.Create(dotcolor));
				}
				g.AddNode(n);
				foreach (var r in cls.GetReferences()){
					if (r.NoSql) continue;
					if (null == r.ReferenceClass.TargetClass) continue;
					if (r.ReferenceClass.TargetClass.Compiled.Attr("nosvg").ToBool()) continue;
					var e = new Edge { From = cls.Name, To = r.ReferenceClass.Name, ArrowHead = new Arrow { MainType = ArrowType.Normal } };
					if (r.IsReverese){
						e.ArrowTail = new Arrow{MainType = ArrowType.Diamond};
					}
					g.AddEdge(e);
				}
			}
			var provider = new DotGraphProvider();
			var render = GraphRender.Create(g);
			var script = render.GenerateGraphScript();

			var svg = provider.Generate(script, new GraphOptions{Format = "svg"});
			
            return svg.ToString();
		}
	}
}