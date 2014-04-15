using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
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

		private Regex _fileRegex = null;

		public string File{
			get { return _file; }
			set{
				_file = value;
				if (_file[0] == '/' && _file[_file.Length - 1] == '/'){
					_fileRegex = new Regex(_file.Substring(1,_file.Length-2));
				}
			}
		}

		readonly IList<PreprocessorCommand> Commands =new List<PreprocessorCommand>();
		private string _file;

		public override void Execute(XElement e=null){
			_project.Log.Info("Start preprocessor "+_e.Attr("code"));
			var sw = Stopwatch.StartNew();
			var srcdegree = Parallel ? Environment.ProcessorCount : 1;
			var commands = Commands.OrderBy(_ => _.Index).ToArray();
			_project.Sources.Where(IsMatch).AsParallel().WithDegreeOfParallelism(srcdegree).ForAll(src =>
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
			sw.Stop();
			_project.Log.Info("Finish preprocessor " + _e.Attr("code")+" "+sw.Elapsed );
		}

		private bool IsMatch(XElement arg){
			if (string.IsNullOrWhiteSpace(File)) return true;
			var file = arg.Describe().File;
			if (null != _fileRegex){
				return _fileRegex.IsMatch(file);
			}
			return file.Contains(File);
		}
	}
}