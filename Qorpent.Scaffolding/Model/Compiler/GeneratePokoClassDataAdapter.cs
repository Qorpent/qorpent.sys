using System.Collections.Generic;
using System.Linq;
using Qorpent.BSharp;
using Qorpent.Scaffolding.Model.CodeWriters;

namespace Qorpent.Scaffolding.Model.Compiler
{
	/// <summary>
	///     Формирует вспомогательный класс адаптера для DataReader
	/// </summary>
	public class GeneratePokoClassDataAdapter : CSharpModelGeneratorBase
	{
		/// <summary>
		/// </summary>
		/// <param name="targetclasses"></param>
		/// <returns></returns>
		protected override IEnumerable<Production> InternalGenerate(IBSharpClass[] targetclasses)
		{
			foreach (var cls in Model.Classes.Values.Where(_=>!_.NoSql).ToArray())
			{
				yield return new Production
				{
					FileName = "Adapters/" + cls.Name + "DataAdapter.cs",
					GetContent = () => new PokoAdapterWriter(cls).ToString()
				};
				yield return new Production
				{
					FileName = "ObjectCaches/" + cls.Name + "DataCache.cs",
					GetContent = () => new PokoObjectCacheWriter(cls).ToString()
				};
			}
		}
	}
}