﻿using System.Threading.Tasks;
using System.Xml.Linq;
using Qorpent.BSharp.Builder;
using Qorpent.Utils.Extensions;

namespace Qorpent.Scaffolding.Sql{
	internal abstract class PreprocessorCommandBase{
		protected IBSharpProject _project;
		public int Index;
		public bool Async;
		public bool Parallel;
		protected XElement _e;

		/// <summary>
		/// 
		/// </summary>
		public PreprocessorCommandBase(IBSharpProject project,XElement e){
			_e = e;
			_project = project;
			Initialize();
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