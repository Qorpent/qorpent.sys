using System.IO;
using System.Net;
using System.Threading;
using System.Web.UI;
using Qorpent.Host.Utils;

namespace Qorpent.Host.Handlers {
    /// <summary>
    /// Производит сохранение и загрузку данных в кэш приложения
    /// </summary>
    public class LoadHandler : IRequestHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="callcontext"></param>
        /// <param name="callbackEndPoint"></param>
        /// <param name="cancel"></param>
        public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel)
        {
            var data = new RequestDataRetriever(callcontext.Request).GetRequestData();
            
            var name = data.Get("name");
            
            
            var root = EnvironmentInfo.ResolvePath("@repos@/.appdata");

            Directory.CreateDirectory(root);
            var fileName = Path.Combine(root, name);
            var contentType = "text/plain";
            if (fileName.EndsWith(".json")) {
                contentType = "application/json";
            }
            var content = "";
            if (File.Exists(fileName)) {
                content = File.ReadAllText(fileName);
            }
            callcontext.Finish(content,contentType);
        }
    }
}