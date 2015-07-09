using System;
using System.Threading;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IO.Http;
using Qorpent.IoC;
using Qorpent.Log;
using qorpent.v2.security.authorization;

namespace qorpent.v2.security.handlers.logon {
	/// <summary>
	///		Обработчик процесса авторизации по токену
	/// </summary>
	[UserOp("tokenauthprocess", SuccessLevel = LogLevel.Info, Secure = false)]
	[ContainerComponent(Lifestyle.Singleton, "handler.tokenauthprocess", ServiceType = typeof(ITokenAuthProcessHandler))]
	public class TokenAuthProcessHandler : HandlerBase, ITokenAuthProcessHandler {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="context"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public override void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
			var container = server.Container;
			if (container == null) {
				throw new Exception("Cannot access container");
			}
			var caProxy = container.Get<ICaWrapper>();
			if (caProxy == null) {
				throw new Exception("Cannot access CA proxy");
			}
			if (context.PreparedParameters == null) {
				context.PreparedParameters = RequestParameters.Create(context);
			}
			var certId = context.PreparedParameters.Get("cert");
			var message = context.PreparedParameters.Get("message");
			if (string.IsNullOrWhiteSpace(certId)) {
				throw new ArgumentException("Empty certificate fingerprint");
			}
			if (string.IsNullOrWhiteSpace(message)) {
				throw new ArgumentException("Empty encrypted message");
			}
			var user = caProxy.ProcessAuth(certId, message);
			var strUser = user.stringify();
			context.Finish(strUser);
		}
	}
}