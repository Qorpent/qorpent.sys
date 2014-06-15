using Qorpent.BSharp;
using Qorpent.BSharp.Builder;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	/// </summary>
	public class PrepareModelTask : BSharpBuilderTaskBase{
		/// <summary>
		/// </summary>
		public const string DefaultModelName = "PrepareModelTask-model";

		/// <summary>
		/// </summary>
		public PrepareModelTask(){
			Phase = BSharpBuilderPhase.PostBuild;
		}

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context){
			if (!context.ExtendedData.ContainsKey(DefaultModelName)){
				PersistentModel model = new PersistentModel().Setup(context);
				context.ExtendedData[DefaultModelName] = model;
			}
		}
	}
}