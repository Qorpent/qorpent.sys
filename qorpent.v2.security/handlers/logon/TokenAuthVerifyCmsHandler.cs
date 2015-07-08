using System;
using System.Threading;
using Qorpent.Host;
using Qorpent.IO;
using Qorpent.IO.Http;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Security.SslCa;

namespace qorpent.v2.security.handlers.logon {
	/// <summary>
	///		Обработчик процесса проверки CMS
	/// </summary>
	[UserOp("tokenauthverifycms", SuccessLevel = LogLevel.Info, Secure = false)]
	[ContainerComponent(Lifestyle.Singleton, "handler.tokenauthverifycms", ServiceType = typeof(ITokenAuthVerifyCmsHandler))]
	public class TokenAuthVerifyCmsHandler : HandlerBase, ITokenAuthVerifyCmsHandler {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="server"></param>
		/// <param name="context"></param>
		/// <param name="callbackEndPoint"></param>
		/// <param name="cancel"></param>
		public override void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
			if (context.PreparedParameters == null) {
				context.PreparedParameters = RequestParameters.Create(context);
			}
			var preparedParams = context.PreparedParameters;
			var fingerprint = preparedParams.Get("fingerprint");
			var cms = preparedParams.Get("message");
			var expected = preparedParams.Get("expected");
			var container = server.Container;
			var caConfigProvider = container.Get<ICaConfigProvider>();
			if (caConfigProvider == null) {
				throw new Exception("Cannot get CA config");
			}
			var caConfig = caConfigProvider.GetConfig();
			if (caConfig == null || !caConfig.GetIsValid()) {
				throw new Exception("Not valid CA config");
			}
			var cmsDecryptor = new CmsDecryptor();
			cmsDecryptor.Initialize(caConfig);
			var cmsMessage = new CmsMessage {
				CertificateFingerprint = fingerprint,
				EncryptedMessage = cms
			};
			context.ContentType = MimeHelper.JSON;
			try {
				var message = cmsDecryptor.Descrypt(cmsMessage);
				if (message != expected) {
					context.Write("false");
					return;
				}
				context.Write("true");
			} catch {
				context.Write("false");
			}
		}
	}
}