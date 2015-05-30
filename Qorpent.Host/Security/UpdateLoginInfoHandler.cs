using System.Threading;
using Qorpent.Events;
using Qorpent.IO.Http;
using Qorpent.Security;

namespace Qorpent.Host.Security {
    /// <summary>
    /// Обновляет информацию о пользователе
    /// </summary>
    public class UpdateLoginInfoHandler:IRequestHandler {
        private IHostServer _server;
        private IRoleResolver _roles;
        private ILoginSourceProvider _logins;

        public UpdateLoginInfoHandler(IHostServer server) {
            _server = server;
            _roles = _server.Container.Get<IRoleResolver>();
            _logins = _server.Container.Get<ILoginSourceProvider>();
        }

        public void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
            var data = RequestParameters.Create(context);
            var request =  UpdateLoginInfoRequest.Create(context,data,_roles,_logins);
            request.Authorize();
            if (!request.HasDelta()) {
                context.Finish("\"no updates required\"",status:201);
            };
            request.Update();
            if (request.resetrequired) {
            //    ((DefaultHostAuthenticationProvider) _server.Auth).Reset(new ResetEventData(true));
            }
            var info = request.GetInfo();
            _logins.Save(info,request.Forced);
            context.Finish("\"updated\"");
        }

        
    }
}