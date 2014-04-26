namespace Qorpent.Host{
	/// <summary>
	/// 
	/// </summary>
	public interface IHostServerStaticResolver{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		void Initialize(IHostServer server);

	    /// <summary>
	    /// Возвращает дескриптор статического файла
	    /// </summary>
	    /// <param name="name"></param>
	    /// <param name="context"></param>
	    /// <param name="withextensions"></param>
	    /// <returns></returns>
	    StaticContentDescriptor Get(string name, object context = null, bool withextensions =false);

		/// <summary>
		/// Сброс кэша
		/// </summary>
		void DropCache();
	}
}