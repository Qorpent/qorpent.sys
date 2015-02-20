using System;
using System.IO;
using System.Net;
using System.Threading;
using Qorpent.Host.Utils;
using Qorpent.Experiments;
namespace Qorpent.Host.Handlers {
    /// <summary>
    /// Производит сохранение и загрузку данных в кэш приложения
    /// </summary>
    public class SaveHandler : IRequestHandler {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="callcontext"></param>
        /// <param name="callbackEndPoint"></param>
        /// <param name="cancel"></param>
        public void Run(IHostServer server, HttpListenerContext callcontext, string callbackEndPoint, CancellationToken cancel) {
            var data = new RequestDataRetriever(callcontext.Request).GetRequestData();
            var name = data.Get("name");
            var content = data.Get("content");
            if (data.PostData.StartsWith("{")) {
                var json = Experiments.Json.Parse(data.PostData);
                name = (string)Experiments.Json.Get(json, "name");
                content = (string)Experiments.Json.Get(json, "content");
            }
            if (name.StartsWith("/") || name.Contains("..")) {
                throw new Exception("wrong and not-secure path "+name );
            }
            var root = EnvironmentInfo.ResolvePath("@repos@/.appdata");       
            var fileName = Path.Combine(root, name);
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            File.WriteAllText(fileName, content);
            callcontext.Finish("OK");
        }
    }
}