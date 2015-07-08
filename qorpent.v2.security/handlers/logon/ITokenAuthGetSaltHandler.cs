using Qorpent.Host;

namespace qorpent.v2.security.handlers.logon {
	/// <summary>
	///		Интерфейс обработчика получения соли для токена
	/// </summary>
	public interface ITokenAuthGetSaltHandler : IRequestHandler { }
}