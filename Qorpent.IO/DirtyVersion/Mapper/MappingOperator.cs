namespace Qorpent.IO.DirtyVersion.Mapper {
	/// <summary>
	/// 
	/// </summary>
	public class MappingOperator {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		public MappingOperator(MappingInfo info)
		{
			this.MappingInfo = info;
		}
		/// <summary>
		/// Целевая информация по мапингу
		/// </summary>
		public MappingInfo MappingInfo { get; private set; }
	}
}