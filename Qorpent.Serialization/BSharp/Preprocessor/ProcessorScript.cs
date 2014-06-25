#pragma warning disable 649
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	/// <summary>
	///     Описывает скрипт препроцессора
	/// </summary>
	internal class ProcessorScript : PreprocessorCommandBase{
		private readonly IList<PreprocessorCommand> Commands = new List<PreprocessorCommand>();
		private readonly BSharpBuilderPhase _phase;
		public bool Staged;
		private string _file;
		private Regex _fileRegex;

		/// <summary>
		/// </summary>
		public ProcessorScript(IBSharpProject project, XElement e, BSharpBuilderPhase phase) : base(project, e){
			_phase = phase;
		}

		public string File{
			get { return _file; }
			set{
				_file = value;
				if (_file[0] == '/' && _file[_file.Length - 1] == '/'){
					_fileRegex = new Regex(_file.Substring(1, _file.Length - 2));
				}
			}
		}

		protected override void Initialize(){
			base.Initialize();
			Source.Elements("command").DoForEach(_ => Commands.Add(new PreprocessorCommand(_project, _)));
		}

		public override void Execute(XElement e = null){
			_project.Log.Info("Start preprocessor " + Source.Attr("code"));
			Stopwatch sw = Stopwatch.StartNew();
			int srcdegree = Parallel ? Environment.ProcessorCount : 1;
			PreprocessorCommand[] commands = Commands.OrderBy(_ => _.Index).ToArray();
			if (Staged){
				RunStaged(srcdegree, commands);
			}
			else{
				RunNonStaged(srcdegree, commands);
			}
			sw.Stop();
			_project.Log.Info("Finish preprocessor " + Source.Attr("code") + " " + sw.Elapsed);
		}

		private void RunStaged(int srcdegree, IEnumerable<PreprocessorCommand> commands){
			foreach (PreprocessorCommand command in commands){
				GetAllXmlList()
					.Where(IsMatch)
					.ToArray()
					.AsParallel()
					.WithDegreeOfParallelism(srcdegree)
					.ForAll(src => { command.Execute(src); });
			}
		}

		/// <summary>
		/// </summary>
		/// <returns></returns>
		private IEnumerable<XElement> GetAllXmlList(){
			if (_phase == BSharpBuilderPhase.PreProcess){
				return _project.Sources;
			}
			return _project.Context.Get(BSharpContextDataType.Working).Select(_ => _.Compiled).ToArray();
		}

		private void RunNonStaged(int srcdegree, IEnumerable<PreprocessorCommand> commands){
			GetAllXmlList().Where(IsMatch).ToArray().AsParallel().WithDegreeOfParallelism(srcdegree).ForAll(src =>{
				IList<Task> pending = new List<Task>();
				foreach (PreprocessorCommand command in commands){
					if (command.Async){
						pending.Add(command.ExecuteAsync(src));
					}
					else{
						command.Execute(src);
					}
				}
				Task.WaitAll(pending.ToArray());
			});
		}

		private bool IsMatch(XElement arg){
			if (string.IsNullOrWhiteSpace(File)) return true;
			string file = arg.Describe().File;
			if (null != _fileRegex){
				return _fileRegex.IsMatch(file);
			}
			return file.Contains(File);
		}
	}
}