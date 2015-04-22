using System.Net;
using System.Text;
using System.Threading;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers
{
    /// <summary>
	/// Возвращает статический контет
	/// </summary>
	public class StaticHandler:RequestHandlerBase
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

		

        public override void Run(IHostServer server, WebContext context, string callbackEndPoint,
            CancellationToken cancel) {
                context.ContentEncoding = Encoding.UTF8;
                context.Finish(_content, _mime, _status);
        }
    }
}