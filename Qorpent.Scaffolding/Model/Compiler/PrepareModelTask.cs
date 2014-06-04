using Qorpent.BSharp.Builder;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// 
	/// </summary>
	public class PrepareModelTask : BSharpBuilderTaskBase{
		/// <summary>
		/// 
		/// </summary>
		public PrepareModelTask(){
			this.Phase = BSharpBuilderPhase.PostBuild;
		}
		/// <summary>
		/// 
		/// </summary>
		public const string DefaultModelName = "PrepareModelTask-model";
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(BSharp.IBSharpContext context)
		{
			if (!context.ExtendedData.ContainsKey(DefaultModelName)){
				var model = new PersistentModel().Setup(context);
				context.ExtendedData[DefaultModelName] = model;
			}
		}
	}
}