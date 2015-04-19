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
	    /// <param name="response"></param>
	    /// <param name="callbackEndPoint"></param>
	    /// <param name="cancel"></param>
	    /// <param name="request"></param>
	    void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint, CancellationToken cancel);
	}
}