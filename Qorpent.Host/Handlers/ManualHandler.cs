using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers
{
    /// <summary>
    /// Handler with manually set response, usable for test proposes
    /// </summary>
    public class ManualHandler : IRequestHandler
    {
        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            if (null!=Override) {
                if (Override(this, server, context)) {
                    return;
                }
            }

            var error = GetError?.Invoke(this, server, context) ?? Error;
            if (null != error) {
                throw error;
            }

            var status = GetStatus?.Invoke(this, server, context) ?? Status;
            var mime = GetMime?.Invoke(this, server, context) ?? Mime;
            var content = GetContent?.Invoke(this, server, context) ?? Content;
            var timeout = GetTimeout?.Invoke(this, server, context) ?? Timeout;

            if (0 < timeout) {
                Thread.Sleep(timeout);
            }

            context.Finish(content,mime,status);
        }

        public const string DefaultMime = "application/json";
        public const int DefaultStatus = 200;
        public const string DefaultContent = "";
        public const int DefaultTimeout = 0;
        public string Mime { get; set; } = DefaultMime;
        public int Status { get; set; } = DefaultStatus;
        public Exception Error { get; set; }
        public object Content { get; set; } = DefaultContent;
        public int Timeout { get; set; } = DefaultTimeout;
        public string CallPoint { get; set; }
        public Func<ManualHandler,IHostServer,WebContext,object> GetContent { get; set; }
        public Func<ManualHandler, IHostServer, WebContext, int> GetStatus { get; set; }
        public Func<ManualHandler, IHostServer, WebContext, int> GetTimeout { get; set; }
        public Func<ManualHandler, IHostServer, WebContext, string> GetMime { get; set; }
        public Func<ManualHandler, IHostServer, WebContext, Exception> GetError { get; set; }
        public Func<ManualHandler, IHostServer, WebContext,bool> Override { get; set; }

        public void Reset() {
            Mime = DefaultMime;
            Status = DefaultStatus;
            Content = DefaultContent;
            Timeout = DefaultTimeout;
            Error = null;
            GetContent = null;
            GetStatus = null;
            GetTimeout = null;
            GetMime = null;
            GetError = null;
            Override = null;
        }
        
        
    }
}
