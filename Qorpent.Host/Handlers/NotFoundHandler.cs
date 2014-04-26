using System.Net;
using System.Text;
using System.Threading;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// Фейковый хэндлер
	/// </summary>
	public class NotFoundHandler : IRequestHandler
	{
		/// <summary>
		/// Выполняет указанный запрос
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callcontext"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel)
		{
			callcontext.Response.StatusCode = 404;
			callcontext.Response.ContentType = "text/plain; charset=utf-8";
			var buffer = Encoding.UTF8.GetBytes("command not found");
			callcontext.Response.OutputStream.Write(buffer,0,buffer.Length);
			callcontext.Response.Close();
		}
	}
}