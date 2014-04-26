using System.Net;
using System.Threading;

namespace Qorpent.Host
{
	/// <summary>
	/// Хэндлер запросов
	/// </summary>
	public interface IRequestHandler
	{
		/// <summary>
		/// Выполняет указанный запрос
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callcontext"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel);
	}
}