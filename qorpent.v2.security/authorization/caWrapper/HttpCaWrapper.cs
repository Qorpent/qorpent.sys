using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Qorpent;
using Qorpent.Host;
using Qorpent.IoC;
using qorpent.v2.security.user;

namespace qorpent.v2.security.authorization.caWrapper {
	/// <summary>
	///		Враппер к CA на HTTP
	/// </summary>
	[ContainerComponent(Lifestyle.Singleton, "httpcawrapper", ServiceType = typeof(ICaWrapper))]
	public class HttpCaWrapper : ServiceBase, ICaWrapper {
		/// <summary>
		///		Получение сессионной соли относительно идентификатора сертификата
		/// </summary>
		/// <param name="certId">Идентификатор сертификата</param>
		/// <returns>Сессионная соль</returns>
		public string GetSalt(string certId) {
			var uri = GetCaServerUri();
			using (var webClient = new WebClient()) {
				var requestUri = new Uri(uri, "tokenauthgetsalt?cert=" + certId);
				var requestStream = webClient.OpenRead(requestUri);
				if (requestStream == null) {
					throw new Exception("Cannot access CA server");
				}
				var responseReader = new StreamReader(requestStream);
				var responseString = responseReader.ReadToEnd();
				responseReader.Close();
				requestStream.Close();
				return responseString;
			}
		}
		/// <summary>
		///		Проведение процесса авторизации
		/// </summary>
		/// <param name="certId">Идентификатор сертификата</param>
		/// <param name="encryptedSalt">Зашифрованная соль</param>
		/// <returns>Авторизованный пользователь</returns>
		public IUser ProcessAuth(string certId, string encryptedSalt) {
			var uri = GetCaServerUri();
			bool success;
			using (var webClient = new WebClient()) {
				var requestUri = new Uri(uri, "tokenauthverifycms");
				var requestParams = new NameValueCollection {{"cert", certId}, {"message", encryptedSalt}};
				var responseBytes = webClient.UploadValues(requestUri, requestParams);
				var responseText = Encoding.UTF8.GetString(responseBytes);
				success = responseText == "true";
			}
			if (success) {
				var user = GetUserByCertId(certId);
				return user;
			}
			return null;
		}
		/// <summary>
		///		Определяет учётную запись пользователя по идентификатору сопоставленного ему сертификата
		/// </summary>
		/// <param name="certId"></param>
		/// <returns>Представление пользователя</returns>
		private IUser GetUserByCertId(string certId) {
			throw new NotImplementedException();
		}
		/// <summary>
		///		Определяет из текщего контекста адрес сервера CA
		/// </summary>
		/// <returns>Адрес сервера CA</returns>
		private Uri GetCaServerUri() {
			var hostConfigProvider = Container.Get<IHostConfigProvider>();
			if (hostConfigProvider == null) {
				throw new Exception("Cannot resolved CA server uri");
			}
			var hostConfig = hostConfigProvider.GetConfig();
			if (hostConfig == null) {
				throw new Exception("Cannot resolved CA server uri");
			}
			var definition = hostConfig.Definition;
			if (definition == null) {
				throw new Exception("Cannot resolved CA server uri");
			}
			var caServerUriElement = definition.Element("ca-uri");
			if (caServerUriElement == null) {
				throw new Exception("Cannot resolved CA server uri");
			}
			if (caServerUriElement.Attribute("code") != null) {
				var strUri = caServerUriElement.Attribute("code").Value;
				var uri = new Uri(strUri);
				return uri;
			}
			if (!string.IsNullOrWhiteSpace(caServerUriElement.Value)) {
				var strUri = caServerUriElement.Value;
				var uri = new Uri(strUri);
				return uri;
			}
			throw new Exception("Cannot resolved CA server uri");
		}
	}
}
