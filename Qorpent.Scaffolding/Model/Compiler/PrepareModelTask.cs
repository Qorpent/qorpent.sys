using System.Linq;
using System.Security.Policy;
using Qorpent.BSharp;
using Qorpent.BSharp.Builder;
using Qorpent.Data;
using Qorpent.Utils.Extensions;

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
				var gopts = new GenerationOptions();
				gopts.IncludeSqlObjectTypes = SqlObjectType.All;
				gopts.ExcludeSqlObjectTypes = SqlObjectType.None;

				var conds = context.Compiler.GetConditions();
				if (conds.ContainsKey("EX_ALL")){
					gopts.ExcludeSqlObjectTypes = SqlObjectType.All;
				}
				if (conds.Any(_=>_.Key.StartsWith("DO_")))
				{
					gopts.IncludeSqlObjectTypes = SqlObjectType.None;
					gopts.ExcludeSqlObjectTypes = SqlObjectType.All;
				}
				if (conds.ContainsKey("DO_DYN")) {
					gopts.IncludeSqlObjectTypes = SqlObjectType.Function|SqlObjectType.ClrTrigger|SqlObjectType.Procedure;
					gopts.ExcludeSqlObjectTypes = SqlObjectType.All;
				}
				foreach (var c in context.Compiler.GetConditions())
				{
					if (c.Key.StartsWith("DO_")||c.Key.StartsWith("EX_")){
						if(c.Key=="EX_ALL")continue;
						if(c.Key=="DO_DYN")continue;
						var type = c.Key.Substring(3);
						var tp = type.To<SqlObjectType>(true);
						if (tp != SqlObjectType.None){
							if (c.Key.StartsWith("DO_")){
								gopts.IncludeSqlObjectTypes |= tp;
							}
							else{
								gopts.ExcludeSqlObjectTypes |= tp;
							}
						}
					}
				}
				PersistentModel model = new PersistentModel{GenerationOptions = gopts}.Setup(context);
				context.ExtendedData[DefaultModelName] = model;
			}
		}
	}
}