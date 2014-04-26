using Qorpent.BSharp;
using Qorpent.BSharp.Builder;

namespace Qorpent.Integration.BSharp.Builder.Tasks.WriteTasks {
	/// <summary>
	/// 
	/// </summary>
	public class WriteWorkingOutputTask : WriteTaskBase {
	    /// <summary>
		/// 
		/// </summary>
		public WriteWorkingOutputTask() {
			Phase = BSharpBuilderPhase.PostProcess;
			Index = TaskConstants.WriteWorkingOutputTaskIndex;
            IncludeFlag = BSharpBuilderOutputAttributes.IncludeWork;
            DataType = BSharpContextDataType.Working;
	
	    }

		/// <summary>
		///     Выполнение операции в контексте
		/// </summary>
		/// <param name="context">Контекст</param>
		public override void Execute(IBSharpContext context)
		{
			base.Execute(context);
			Project.Set("WroteTargets",WriteManager.Targets);
		}
	}
}