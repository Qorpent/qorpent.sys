﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Qorpent.IO.Resources;
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
		public MvcClient(string appname, ICredentials credentials, IResourceProvider resources = null) {
			ApplicationRoot = appname;
			Credentials = credentials;
			Resources = resources ?? Applications.Application.Current.Resources;
		}
		/// <summary>
		/// Используемый сервис ресурсов
		/// </summary>
		protected IResourceProvider Resources { get; private set; }

		/// <summary>
		/// Учетные сведения пользователя
		/// </summary>
		public ICredentials Credentials { get;private set; }
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
			var url = ApplicationRoot + "/" + command + ".xml.qweb";
			var resourceConfig = new ResourceConfig {
				Credentials = Credentials,
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

			var stream =  Resources.GetStreamAsync(url, resourceConfig);
			return XElement.Load(XmlReader.Create(await stream));
		}

		private string GeneratePostData(object parameters) {
			var dict = parameters.ToDict();
			return string.Join("&", dict.Select(_ => _.Key + "=" + _.Value));
		}
	}
}
