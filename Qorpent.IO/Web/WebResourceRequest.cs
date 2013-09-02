using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Qorpent.IO.Resources;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Web {
	/// <summary>
	///     Обертка над Web-запросом
	/// </summary>
	public class WebResourceRequest : IResourceRequest {
		/// <summary>
		///     Конфигурация
		/// </summary>
		protected IResourceConfig Config;

		/// <summary>
		///     Сохраненный результат запроса
		/// </summary>
		protected IResourceResponse Response;

		/// <summary>
		///     Целевой адрес
		/// </summary>
		protected Uri Uri;

		/// <summary>
		///     Стандартный конструктор
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		public WebResourceRequest(Uri uri, IResourceConfig config) {
			State = ResourceRequestState.Init;
			Config = config;
			Uri = uri;
		}

		/// <summary>
		///     Акцессор к последней ошибке
		/// </summary>
		public Exception LastError { get; protected set; }

		/// <summary>
		///     Текущее состояние запроса
		/// </summary>
		public ResourceRequestState State { get; private set; }

		/// <summary>
		///     Выполняет синхронный запрос ресурса
		/// </summary>
		/// <returns></returns>
		public async Task<IResourceResponse> GetResponse(IResourceConfig config = null) {
			if (CheckCurrentCanBeReturned()) return Response;
			config = config ?? Config ?? ResourceConfig.Default;
			if (ResourceRequestState.Init == State) {
				if (null == Response) {
					Response = await InternalGetResponse(config);					
				}
			}
			if (ResourceRequestState.Error == State) {
				if (null == LastError) {
					LastError = new ResourceException("some error in request");
				}
				throw LastError;
			}

			return Response;
		}

		/// <summary>
		///     Получить параметр из конфигурации
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="code"></param>
		/// <param name="def"></param>
		/// <returns></returns>
		public T GetParameter<T>(string code, T def = default(T)) {
			if (null == Config) return def;
			return Config.Get(code, def);
		}

		private bool CheckCurrentCanBeReturned() {
			lock (this) {
				if (ResourceRequestState.Init == State) {
					return false;
				}
				if (ResourceRequestState.Error == State) {
					if (null == LastError) {
						LastError = new ResourceException("some error in request");
					}
					throw LastError;
				}
				if (ResourceRequestState.Finished == State) {
					{
						return true;
					}
				}
				throw new ResourceException("insuficient state for GetResponse call " + State);
			}
		}

		private async Task<IResourceResponse> InternalGetResponse(IResourceConfig config) {
			try {
				if (config.UseQwebAuthentication) {
					PerformQwebAuthentiacation(config);
				}
				config = config ?? ResourceConfig.Default;
				State = ResourceRequestState.Creating;
				var nativeRequest = WebRequest.Create(Uri);
				SetupNativeRequest(nativeRequest, config);
				State = ResourceRequestState.Created;
				await PostDataToServer(config, nativeRequest);
				State = ResourceRequestState.Get;
				nativeRequest.Headers["ALLOW_ALL_CERTIFICATES"] = "1";
				var nativeResponse = await nativeRequest.GetResponseAsync();
				State = ResourceRequestState.Finished;
				return new WebResourceResponse(nativeResponse, nativeRequest, config);
			}
			catch (Exception ex) {
				State= ResourceRequestState.Error;
				LastError = new ResourceException("erorr in get response", ex);
			}
			return null;
		}

		private void PerformQwebAuthentiacation(IResourceConfig config) {
			var root = GetApplicationPath(Uri);
			var whoami = root + "/_sys/whoami.xml.qweb";
			var nr = (HttpWebRequest)WebRequest.Create(whoami);
			nr.CookieContainer = config.Cookies ?? MainContainer;
			if (config.AcceptAllCeritficates) {
				nr.Headers["ALLOW_ALL_CERTIFICATES"] = "1";
			}
			var r = nr.GetResponse();
			var x = XElement.Load(XmlReader.Create(r.GetResponseStream()));
			var logonname = x.Element("result").Attr("logonname");
			if (string.IsNullOrWhiteSpace(logonname)) {
				if (null == config.Credentials) throw new ResourceException("cannot perform qweb auth without credentials");
				var login = root + "/_sys/login.string.qweb";
				var nl = ((NetworkCredential) config.Credentials);
				var postdata = string.Format("_l_o_g_i_n_={0}&_p_a_s_s_={1}", nl.UserName, nl.Password);
				var lr = (HttpWebRequest)WebRequest.Create(login);
				lr.CookieContainer = config.Cookies ?? MainContainer;
				lr.Method = "POST";
				lr.ContentType =  "application/x-www-form-urlencoded";
				using (var s = new StreamWriter( lr.GetRequestStream())) {
					s.Write(postdata);
					s.Flush();
					s.Close();
				}
				lr.GetResponse();
			}

		}

		private string GetApplicationPath(Uri uri) {
			var basis = uri.Scheme+"://" + uri.Host + ":" + uri.Port;
			var path = uri.AbsolutePath.Split('/')[1];
			return basis + "/" + path;
		}

	
		private async Task PostDataToServer(IResourceConfig config, WebRequest nativeRequest) {
			if (config.Method == "POST") {
				if (null != config.RequestPostData) {
					State = ResourceRequestState.Post;
					using (var stream = new BinaryWriter(await nativeRequest.GetRequestStreamAsync())) {
						stream.Write(config.RequestPostData, 0, config.RequestPostData.Length);
					}
				}else if (!string.IsNullOrWhiteSpace(config.RequestFormString)) {
					nativeRequest.ContentType = "application/x-www-form-urlencoded";
					State = ResourceRequestState.Post;
					using (var stream = new StreamWriter(await nativeRequest.GetRequestStreamAsync()))
					{
						stream.Write(config.RequestFormString);
					}
				}
			}
		}

		private static CookieContainer MainContainer = new CookieContainer();
		/// <summary>
		///     Донастройка веб-запроса
		/// </summary>
		/// <param name="nativeRequest"></param>
		/// <param name="config"></param>
		private  void SetupNativeRequest(WebRequest nativeRequest, IResourceConfig config) {
			SetupHttpMethod(nativeRequest, config);
			if (null != config.Credentials) {
				nativeRequest.Credentials = config.Credentials;
			}
			else {
				nativeRequest.UseDefaultCredentials = true;
			}
			nativeRequest.Proxy = ProxySelectorHelper.Select(Uri, config);
			((HttpWebRequest)nativeRequest).UserAgent =
				"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/28.0.1500.95 Safari/537.36";
			((HttpWebRequest) nativeRequest).CookieContainer = config.Cookies ?? MainContainer;
		}

		private static void SetupHttpMethod(WebRequest nativeRequest, IResourceConfig config) {
			var method = config.Method;
			if (string.IsNullOrWhiteSpace(method)) {
				method = null != config.RequestPostData ? "POST" : "GET";
			}
			nativeRequest.Method = method;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose() {
			Config = null;
			Response = null;
		}

		static WebResourceRequest() {
			ServicePointManager.ServerCertificateValidationCallback = ValidateServerCertficate;

		}

		private static bool ValidateServerCertficate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslpolicyerrors) {
			if (sslpolicyerrors == SslPolicyErrors.None) return true;
			var req = (HttpWebRequest) sender;
			if (req.Headers.AllKeys.Contains("ALLOW_ALL_CERTIFICATES")) {
				return true;
			}
			return false;
		}
	}
}