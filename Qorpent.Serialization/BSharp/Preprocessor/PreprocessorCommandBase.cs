using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.BSharp.Preprocessor{
	internal abstract class PreprocessorCommandBase{
		protected IBSharpProject _project;
		public int Index;
		public bool Async;
		public bool Parallel;
		private XElement _e;

		/// <summary>
		/// 
		/// </summary>
		public PreprocessorCommandBase(IBSharpProject project,XElement e){
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
		/// выполняет скрипт асинхронно
		/// </summary>
		/// <returns></returns>
		public Task ExecuteAsync(XElement e=null){
			return Task.Run(()=>Execute(e));
		}

		/// <summary>
		/// Выполняет сам скрип
		/// </summary>
		public abstract void Execute(XElement e);
	}
}