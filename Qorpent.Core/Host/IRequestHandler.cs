using System.Net;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host{
	/// <summary>
	///     Хэндлер запросов
	/// </summary>
	public interface IRequestHandler{
	    /// <summary>
	    ///     Выполняет указанный запрос
	    /// </summary>
	    /// <param name="server"></param>
	    /// <param name="callbackEndPoint"></param>
	    /// <param name="cancel"></param>
	    void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel);
	}
}