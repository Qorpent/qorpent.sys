using System.Collections.Generic;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	///     Формирует вспомогательный класс адаптера для DataReader
	/// </summary>
	public class GenerateModel : CSharpModelGeneratorBase{
		/// <summary>
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			var genfactory = new Production{
				FileName = "Adapters/Model.cs",
				GetContent = () => new BaseModelWriter(Model).ToString()
			};
			yield return genfactory;
		}
	}
}