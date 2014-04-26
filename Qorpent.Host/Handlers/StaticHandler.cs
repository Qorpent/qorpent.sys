using System.Net;
using System.Text;
using System.Threading;

namespace Qorpent.Host.Handlers
{
	/// <summary>
	/// Возвращает статический контет
	/// </summary>
	public class StaticHandler:IRequestHandler
	{
		private string _content;
		private string _mime;
		private int _status;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		/// <param name="mimetype"></param>
		/// <param name="status"></param>
		public StaticHandler(string content, string mimetype = "text/plain; charset=utf-8", int status = 200)
		{
			this._content = content;
			this._mime = mimetype;
			this._status = status;
		}

		/// <summary>
		/// Выполняет указанный запрос
		/// </summary>
		/// <param name="server"></param>
		/// <param name="callcontext"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel)
		{
			callcontext.Response.ContentEncoding = Encoding.UTF8;
			callcontext.Finish(_content,_mime,_status);
		}
	}
}