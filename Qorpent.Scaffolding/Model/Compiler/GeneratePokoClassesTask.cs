using System.Collections.Generic;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Model.Compiler{
	/// <summary>
	///     Задача по генерации Poko-классов модели на основе DataMapping,
	///     данные классы являются PurePOKO, то есть даже не наследуют никаких интерфейсов
	/// </summary>
	/// <remarks>Единственная опциональная завязка на Qorpent в этой генерации - отключаемые атрибуты Serialize</remarks>
	public class GeneratePokoClassesTask : CSharpModelGeneratorBase{
		/// <summary>
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
			return from cls in Tables
			       select new Production{
				       FileName = "DataTypes/" + cls.Name + ".cs",
				       GetContent = () => new PokoClassWriter(cls).ToString()
			       };
		}
	}
}