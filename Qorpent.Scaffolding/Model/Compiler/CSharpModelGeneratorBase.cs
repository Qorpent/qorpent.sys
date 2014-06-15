using System.Linq;
using Qorpent.BSharp;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	///     Базовый класс генераторов модели
	/// </summary>
	public abstract class CSharpModelGeneratorBase : CodeGeneratorTaskBase{
		/// <summary>
		/// </summary>
		public CSharpModelGeneratorBase(){
			ClassSearchCriteria = "dbtable";
		}

		/// <summary>
		/// </summary>
		protected PersistentModel Model { get; set; }

		/// <summary>
		/// </summary>
		protected int IndentLevel { get; set; }

		/// <summary>
		/// </summary>
		protected PersistentClass[] Tables { get; set; }

		/// <summary>
		/// </summary>
		/// <param name="context"></param>
		public override void Execute(IBSharpContext context){
			Model = (PersistentModel) context.ExtendedData[PrepareModelTask.DefaultModelName];
			Tables = Model.Classes.Values.OrderBy(_ => _.Name).ToArray();
			base.Execute(context);
		}
	}
}