using System;
using System.Threading;
using Qorpent.Host;
using Qorpent.IO.Http;

namespace pksp.kb.web
{
    public class ActionContext
    {
        public IHostServer Server;
        public WebContext WebContext;
        public CancellationToken Cancel;
        public object Result;
        public Exception Error;
        public string MimeType = "application/json";
        public int State = 200;
        public RequestParameters Parameters;
        public string RenderMode;
    }
}