using System.Net;
using System.Text;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// Фейковый хэндлер
	/// </summary>
	public class NotFoundHandler : RequestHandlerBase
	{


	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="server"></param>
	    /// <param name="request"></param>
	    /// <param name="response"></param>
	    /// <param name="callbackEndPoint"></param>
	    /// <param name="cancel"></param>
	    public override void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint,
	        CancellationToken cancel) {
                response.Finish("command not found","text/plain; charset=utf-8",404);

	    }
	}
}