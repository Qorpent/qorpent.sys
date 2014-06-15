using System.Collections.Generic;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	///     Формирует расширение и вспомогательные классы для клонирования объектов
	/// </summary>
	public class GenerateCloneableFacility : CSharpModelGeneratorBase{
		/// <summary>
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			yield return new Production{
				FileName = "Extensions/CloneOptions.cs",
				GetContent = () => new CloneOptionsWriter(Model).ToString()
			};
			yield return new Production{
				FileName = "Extensions/CloneFacility.cs",
				GetContent = () => new CloneFacilityWriter(Model).ToString()
			};
		}
	}
}