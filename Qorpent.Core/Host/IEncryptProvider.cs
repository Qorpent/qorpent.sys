namespace Qorpent.Host{
	/// <summary>
	/// </summary>
	public interface IEncryptProvider{
		/// <summary>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		string Encrypt(string text);

		/// <summary>
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		string Encrypt(byte[] data);

		/// <summary>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		string Decrypt(string text);

		/// <summary>
		/// </summary>
		/// <param name="server"></param>
		void Initialize(IHostServer server);

		/// <summary>
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		byte[] DecryptData(string text);
	}
}