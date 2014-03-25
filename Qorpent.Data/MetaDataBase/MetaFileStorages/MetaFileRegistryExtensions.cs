namespace Qorpent.Data.MetaDataBase{
	/// <summary>
	/// 
	/// </summary>
	public static class MetaFileRegistryExtensions{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		/// <param name="code"></param>
		/// <param name="revision"></param>
		/// <returns></returns>
		public static MetaFileDescriptor Checkout(this IMetaFileRegistry registry, string code, string revision){
			return registry.Checkout(new MetaFileDescriptor{Code = code, Revision = revision});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="registry"></param>
		/// <param name="code"></param>
		/// <param name="revision"></param>
		/// <returns></returns>
		public static MetaFileDescriptor GetByRevision(this IMetaFileRegistry registry, string code, string revision)
		{
			return registry.GetByRevision(new MetaFileDescriptor { Code = code, Revision = revision });
		}
	}
}