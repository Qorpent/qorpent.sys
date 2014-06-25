#pragma warning disable 649
using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	internal abstract class PreprocessorCommandBase{
		private readonly XElement _e;
		public bool Async;
		public string Code;
		public int Index;
		public bool Parallel;
		protected IBSharpProject _project;

		/// <summary>
		/// </summary>
		public PreprocessorCommandBase(IBSharpProject project, XElement e){
			_e = e;
			_project = project;
			Initialize();
		}

		public XElement Source{
			get { return _e; }
		}

		protected virtual void Initialize(){
			_e.Apply(this);
		}

		/// <summary>
		///     выполняет скрипт асинхронно
		/// </summary>
		/// <returns></returns>
		public Task ExecuteAsync(XElement e = null){
			return Task.Run(() => Execute(e));
		}

		/// <summary>
		///     Выполняет сам скрип
		/// </summary>
		public abstract void Execute(XElement e);
	}
}