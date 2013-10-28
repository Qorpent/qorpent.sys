using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Qorpent.Applications;
using Qorpent.IO.Resources;
using Qorpent.Security;
using Qorpent.Utils.Extensions;

namespace Qorpent.Mvc
{
	/// <summary>
	/// Клиентская консоль Qweb
	/// </summary>
	public class MvcClient
	{
		/// <summary>
		/// Создает клиента на приложение и пользователя
		/// </summary>
		public MvcClient(string appname, Func<ICredentials> credentials, IResourceProvider resources = null) {
			ApplicationRoot = appname;
			LogonFunction = credentials;
			Resources = resources ?? Application.Current.Resources;
		}
		/// <summary>
		/// Используемый сервис ресурсов
		/// </summary>
		protected IResourceProvider Resources { get; private set; }

		/// <summary>
		/// Учетные сведения пользователя
		/// </summary>
		public Func<ICredentials> LogonFunction { get;private set; }

	    private ICredentials _credentials;
        private ICredentials GetCredentials() {
            if (null == _credentials) {
                var uri = new Uri(ApplicationRoot);
                var host = uri.Host;
                var app = ApplicationRoot.Split('/').Last();
                var cs = Application.Current.Container.Get<ICredentialStorage>();
                ICredentials credentials = null;
                if (null != cs) {
                    credentials = cs.GetCredentials(host, app);
                    if (null == credentials) {
                        credentials = cs.GetCredentials(host);
                    }
                }
                if (null == credentials) {
                    if (null != LogonFunction) {
                        credentials = LogonFunction();
                    }
                    else {
                        throw new Exception("no credentials or logon function provided");
                    }
                }
                _credentials = credentials;
            }
            return _credentials;
        }

		/// <summary>
		/// Корневой URL приложения
		/// </summary>
		public string ApplicationRoot { get; private set; }

		/// <summary>
		/// Получить XML с сервера
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<XElement> GetXml(string command, object parameters = null) {
			const string format = "xml";
			var stream = GetStream(command, parameters, format);
			return XElement.Load(XmlReader.Create(await stream));
		}
		/// <summary>
		/// Получить XML с сервера
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public async Task<string> GetString(string command, object parameters = null)
		{
			const string format = "string";
			var stream = GetStream(command, parameters, format);
			return new StreamReader(await stream).ReadToEnd();
		}

		private  Task<Stream> GetStream(string command, object parameters, string format) {
			var url = ApplicationRoot + "/" + command.Replace(".","/") + "." + format + ".qweb";
			var resourceConfig = new ResourceConfig {
				Credentials = GetCredentials(),
				AcceptAllCeritficates = true,
				UseQwebAuthentication = true,
			};
			if (null != parameters) {
				var postdata = GeneratePostData(parameters);
				if (!string.IsNullOrWhiteSpace(postdata)) {
					resourceConfig.Method = "POST";
					resourceConfig.RequestFormString = postdata;
				}
			}

			var stream = Resources.GetStreamAsync(url, resourceConfig);
			return stream;
		}

		private string GeneratePostData(object parameters) {
			var dict = parameters.ToDict();
            return string.Join("&", dict.Select(_ => _.Key + "=" + Uri.EscapeDataString(_.Value.ToStr())));
		}
		/// <summary>
		/// Возвращает строку, полученную в указанном формате
		/// </summary>
		/// <param name="command"></param>
		/// <param name="parameters"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public async Task<string> GetString(string command, IDictionary<string, object> parameters, string format) {
			var stream = GetStream(command, parameters, format);
			return new StreamReader(await stream).ReadToEnd();
		}
	}
}