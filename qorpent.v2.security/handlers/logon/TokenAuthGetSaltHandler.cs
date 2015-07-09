using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Qorpent.Host;
using Qorpent.IO;
using Qorpent.IO.Http;
using Qorpent.IoC;
using Qorpent.Log;
using Qorpent.Utils.Extensions;
using qorpent.v2.security.authorization;

namespace qorpent.v2.security.handlers.logon {
	/// <summary>
	///		Сервис получения и токена 
	/// </summary>
	[UserOp("tokenauthgetsalt", SuccessLevel = LogLevel.Info, Secure = false)]
	[ContainerComponent(Lifestyle.Singleton, "handler.tokenauthgetsalt", ServiceType = typeof(ITokenAuthGetSaltHandler))]
	public class TokenAuthGetSaltHandler : TokenAuthHandlerBase, ITokenAuthGetSaltHandler {
		/// <summary>
		///		Соль
		/// </summary>
		public class Salt {
			/// <summary>
			///		Значение соли
			/// </summary>
			public string Value { get; set; }
			/// <summary>
			///		Дата окончания действия соли
			/// </summary>
			public DateTime Expire { get; set; }
		}
		/// <summary>
		///		Объект синхронизации
		/// </summary>
		public static readonly object Sync = new object();
		/// <summary>
		///		Набор солей
		/// </summary>
		public static Dictionary<string, Salt> Salts = new Dictionary<string, Salt>();
		public override void Run(IHostServer server, WebContext context, string callbackEndPoint, CancellationToken cancel) {
			var container = server.Container;
			if (container == null) {
				throw new Exception("Cannot access container");
			}
			if (context.PreparedParameters == null) {
				context.PreparedParameters = RequestParameters.Create(context);
			}
			var certId = context.PreparedParameters.Get("cert");
			if (string.IsNullOrWhiteSpace(certId)) {
				throw new ArgumentException("Empty certificate fingerprint");
			}
			var hostConfigProvider = container.Get<IHostConfigProvider>();
			if (hostConfigProvider == null) {
				throw new Exception("Cannot resolve server role");
			}
			var hostConfig = hostConfigProvider.GetConfig();
			if (hostConfig == null) {
				throw new Exception("Cannot resolve server role");
			}
			var definition = hostConfig.Definition;
			if (definition == null) {
				throw new Exception("Cannot resolve server role");
			}
			var caAttr = definition.Attr("ca");
			if (!string.IsNullOrWhiteSpace(caAttr) && caAttr.To<bool>()) {
				lock (Sync) {
					Salt saltObj;
					if (Salts.ContainsKey(certId)) {
						saltObj = Salts[certId];
						if (saltObj.Expire <= DateTime.UtcNow) {
							saltObj = new Salt {
								Value = Guid.NewGuid().ToString(),
								Expire = DateTime.UtcNow.AddHours(1)
							};
							Salts[certId] = saltObj;
						}
					} else {
						saltObj = new Salt {
							Value = Guid.NewGuid().ToString(),
							Expire = DateTime.UtcNow.AddHours(1)
						};
						Salts[certId] = saltObj;
					}
					context.Finish("\"" + saltObj.Value + "\"");
					CleanUpExpiredSaltsInternal();
					return;
				}
			}
			var caProxy = container.Get<ICaWrapper>();
			if (caProxy == null) {
				throw new Exception("Cannot access CA proxy");
			}
			context.ContentType = MimeHelper.JSON;
			var salt = caProxy.GetSalt(certId);
			context.Finish(salt);
		}
		private void CleanUpExpiredSaltsInternal() {
			var keys = Salts.Where(_ => _.Value.Expire <= DateTime.UtcNow).Select(_ => _.Key).ToArray();
			foreach (var key in keys) {
				Salts.Remove(key);
			}
		}
	}
}