using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	/// <summary>
	/// Описывает скрипт препроцессора
	/// </summary>
	internal class PreprocessorScript : PreprocessorCommandBase{
		/// <summary>
		/// 
		/// </summary>
		public PreprocessorScript(IBSharpProject project,XElement e) : base(project, e){
		}
		protected override void Initialize()
		{
			base.Initialize();
			_e.Elements("command").DoForEach(_ => Commands.Add(new PreprocessorCommand(_project,_)));
		}

		readonly IList<PreprocessorCommand> Commands =new List<PreprocessorCommand>();
		public override void Execute(XElement e=null){
		//	_project.Log.Trace("Start preprocessor "+_e.Attr("code"));
			var srcdegree = Parallel ? Environment.ProcessorCount : 1;
			var commands = Commands.OrderBy(_ => _.Index).ToArray();
			_project.Sources.AsParallel().WithDegreeOfParallelism(srcdegree).ForAll(src =>
			{
				IList<Task> pending = new List<Task>();
				foreach (var command in commands){
					if (command.Async){
						pending.Add(command.ExecuteAsync(src));
					}
					else{
						command.Execute(src);
					}
				}
				Task.WaitAll(pending.ToArray());
			});
	//		_project.Log.Trace("Finish preprocessor " + _e.Attr("code"));
		}
	}
}