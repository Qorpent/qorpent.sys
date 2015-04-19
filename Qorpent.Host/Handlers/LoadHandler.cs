using System.IO;
using System.Net;
using System.Threading;
using System.Web.UI;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers {
    /// <summary>
    /// Производит сохранение и загрузку данных в кэш приложения
    /// </summary>
    public class LoadHandler : RequestHandlerBase
    {
       

        public override void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint,
            CancellationToken cancel) {
                var data = RequestParameters.Create(request);

                var name = data.Get("name");


                var root = EnvironmentInfo.ResolvePath("@repos@/.appdata");

                Directory.CreateDirectory(root);
                var fileName = Path.Combine(root, name);
                var contentType = "text/plain";
                if (fileName.EndsWith(".json"))
                {
                    contentType = "application/json";
                }
                var content = "";
                if (File.Exists(fileName))
                {
                    content = File.ReadAllText(fileName);
                }
                response.Finish(content, contentType);
        }
    }
}