using System;
using System.IO;
using System.Net;
using System.Threading;
using Qorpent.Experiments;
using Qorpent.IO.Http;

namespace Qorpent.Host.Handlers {
    /// <summary>
    /// Производит сохранение и загрузку данных в кэш приложения
    /// </summary>
    public class SaveHandler : RequestHandlerBase {
        

        public override void Run(IHostServer server, HttpRequestDescriptor request, HttpResponseDescriptor response, string callbackEndPoint,
            CancellationToken cancel) {
                var data = RequestParameters.Create(request);
                var name = data.Get("name");
                var content = data.Get("content");
                if (data.PostData.StartsWith("{"))
                {
                    var json = Experiments.Json.Parse(data.PostData);
                    name = (string)Experiments.Json.Get(json, "name");
                    content = (string)Experiments.Json.Get(json, "content");
                }
                if (name.StartsWith("/") || name.Contains(".."))
                {
                    throw new Exception("wrong and not-secure path " + name);
                }
                var root = EnvironmentInfo.ResolvePath("@repos@/.appdata");
                var fileName = Path.Combine(root, name);
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                File.WriteAllText(fileName, content);
                response.Finish("OK");
        }
    }
}