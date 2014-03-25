using System.Collections.Generic;

namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// ¬ыполн€ет какую-то работу при мерже
	/// </summary>
	public interface ICustomMerger{
		/// <summary>
		/// ќбработать дельту и вернуть ту, котора€ действительно нуждаетс€ в прокачке
		/// </summary>
		/// <param name="basedelta"></param>
		/// <returns></returns>
		IEnumerable<MetaFileRegistryDelta> Merge(IEnumerable<MetaFileRegistryDelta> basedelta);
	}
}