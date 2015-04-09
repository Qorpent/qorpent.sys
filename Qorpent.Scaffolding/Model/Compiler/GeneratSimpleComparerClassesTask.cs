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
	public class GeneratSimpleComparerClassesTask : CSharpModelGeneratorBase{
		/// <summary>
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses){
            foreach (var cls in Tables) {

                var fields = cls.Fields.Values.Where(_ => _.IsHash).ToArray();
                if (fields.Length == 0) continue;

                yield return new Production{
				       FileName = "SimpleComparers/" + cls.Name + "Comparer.cs",
				       GetContent = () => new SimpleComparerClassWriter(cls).ToString()
			       };

            }

            //return from cls in Tables
                   //select 
		}
	}
}