#pragma warning disable 649
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Qorpent.BSharp.Builder;

namespace Qorpent.BSharp.Preprocessor{
	internal class PreprocessorCommand : PreprocessorCommandBase{
		/// <summary>
		/// 
		/// </summary>
		public PreprocessorCommand(IBSharpProject project,XElement e) : base(project, e){
		}
		/// <summary>
		/// Селектор
		/// </summary>
		public string For;
		/// <summary>
		/// Выполняет сам скрип
		/// </summary>
		public override void Execute(XElement e =null ){
		//	_project.Log.Trace("Start command " + _e.Attr("code"));
			var srcdegree = Parallel ? Environment.ProcessorCount : 1;
			var selector = GetElements(e).ToArray();
			selector.AsParallel().WithDegreeOfParallelism(srcdegree).ForAll(el =>{
				foreach (var operation in Operations){
					operation.Execute(el);
				}
			});
		//	_project.Log.Trace("Finish command " + _e.Attr("code"));
		}
		protected override void Initialize()
		{
			base.Initialize();
			foreach (var element in Source.Elements()){
				var operation = PreprocessOperation.Create(_project,element);
				if (null != operation){
					Operations.Add(operation);
				}
			}
		}

		private IEnumerable<XElement> GetElements(XElement src){
			if ("ROOTELEMENTS" == For) return src.Elements();
			if (string.IsNullOrWhiteSpace(For) || "ALLELEMENTS" == For) return src.Descendants();
			return src.XPathSelectElements(For);
		}

		public readonly IList<PreprocessOperation> Operations = new List<PreprocessOperation>();
	}
}