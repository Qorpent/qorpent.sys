using System;
using System.Security.Principal;
using System.Threading;
using Qorpent.Experiments;
using Qorpent.Host;
using Qorpent.IO.Http;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Log.NewLog;
using qorpent.v2.security.authentication;
using qorpent.v2.security.authorization;
using qorpent.v2.security.user;

namespace qorpent.v2.security.handlers.logon {
	/// <summary>
	///		Обработчик процесса авторизации по токену
	/// </summary>
	[UserOp("logonsec", Secure =true, SuccessLevel = LogLevel.Info, ErrorLevel = LogLevel.Warn, TreatFalseAsError = true)]
	[ContainerComponent(Lifestyle.Singleton, "handler.tokenauthprocess", ServiceType = typeof(ITokenAuthProcessHandler))]
	public class TokenAuthProcessHandler : TokenAuthHandlerBase, ITokenAuthProcessHandler {

		[Inject]
		public IHttpTokenService TokenService { get; set; }
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
			if (user != null) {
				var result = ProcessUserLogin(user, server, context);
				var strResult = result.Result.stringify();
				context.Finish(strResult);
				return;
			}
			context.Finish("false");
		}
		/// <summary>
		///		Произведение авторизации и всех сопутствующих процедур
		/// </summary>
		/// <param name="user">Пользователь</param>
		/// <param name="server">Сервер</param>
		/// <param name="context">Контекст</param>
		/// <returns>HandlerResult</returns>
		private HandlerResult ProcessUserLogin(IUser user, IHostServer server, WebContext context) {
			var identity = new Identity(user) {AuthenticationType = "secure"};
			context.User = new GenericPrincipal(identity, null);
			var logondata = new LogonInfo {
				Identity = identity,
				RemoteEndPoint = context.Request.RemoteEndPoint,
				LocalEndPoint = context.Request.LocalEndPoint,
				UserAgent = context.Request.UserAgent
			};
			var token = TokenService.Create(context.Request);
			TokenService.Store(context.Response, context.Request.Uri, token);
			return new HandlerResult { Result = true, Data = logondata };
		}
	}
}