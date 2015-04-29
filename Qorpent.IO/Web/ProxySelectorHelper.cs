using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Qorpent.IO.Resources;
using Qorpent.Utils.Extensions;

namespace Qorpent.IO.Web {
	/// <summary>
	/// Вспомогательный класс конструирования Proxy из конфига
	/// </summary>
	public static class ProxySelectorHelper {
		/// <summary>
		/// Регулярное выражение для определения параметров прокси
		/// </summary>
		public const string PROXY_ADDRESS_DEFINITION_REGEX =
			@"(?<t>(\*)|(\w+))=(?<s>\w+)://((?<l>[^:]+):(?<p>[^@])+@)?(?<a>[^\s:]+(:\d+)?)";


		/// <summary>
		/// Вспомогательный метод загрузки прокси из конфигурации
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="config"></param>
		/// <returns></returns>
		public static WebProxy Select(Uri uri, IScope config = null) {
			config = config ?? ResourceConfig.Default;
			return Select(uri,
			              config.Get(ResourceConfig.PROXY_USAGE, ProxyUsage.Default),
			              config.Get(ResourceConfig.PROXY_ADDRESSES, ""),
			              config.Get(ResourceConfig.PROXY_EXCLUDES, "")
				);
		}
		/// <summary>
		/// Вспомогательный метод загрузки прокси при помощи строчных параметров
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="usage"></param>
		/// <param name="addrDefinition"></param>
		/// <param name="bypassDefinition"></param>
		/// <returns></returns>
		public static WebProxy Select(Uri uri, ProxyUsage usage, string addrDefinition = null, string bypassDefinition = null) {
			if (ProxyUsage.Undefined == usage) return null;
			if (ProxyUsage.NoProxy == usage) return null;
			if (ProxyUsage.System == usage) {
				return SelectSystemProxy(uri, bypassDefinition);
			}
			if (ProxyUsage.Custom == usage) {
				if (string.IsNullOrWhiteSpace(addrDefinition)) {
					return null;
				}
				return SelectCustomProxy(uri, addrDefinition, bypassDefinition);
			}
			throw new ResourceException("unknown proxy type " + usage);
		}
		/// <summary>
		/// Метод формирования нестандартного прокси из параметров
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="addrDefinition"></param>
		/// <param name="bypassDefinition"></param>
		/// <returns></returns>
		public static WebProxy SelectCustomProxy(Uri uri, string addrDefinition, string bypassDefinition) {
			var defs = ProxyRecord.Parse(addrDefinition);
			var def = defs.FirstOrDefault(_ => _.TargetScheme.ToUpperInvariant() == uri.Scheme.ToUpperInvariant());
			if (null == def) {
				def = defs.FirstOrDefault(_ => _.TargetScheme=="*");
			}
			if (null == def) return null;
			var proxy = new WebProxy(
				def.Uri,
				true, null,
				def.Credentials);
			if (!string.IsNullOrWhiteSpace(bypassDefinition))
			{
				proxy.BypassList = bypassDefinition.SmartSplit(false, true, ' ').ToArray();
			}
			if (proxy.IsBypassed(uri)) return null;
			return proxy;
		}

		/// <summary>
		/// Вспомогательный класс парсинга прокси
		/// </summary>
		public class ProxyRecord {
			/// <summary>
			/// Парсинг записи о прокси из параметра
			/// </summary>
			/// <param name="addrDefinition"></param>
			/// <returns></returns>
			public static ProxyRecord[] Parse(string addrDefinition) {
				return Regex.Matches(addrDefinition, PROXY_ADDRESS_DEFINITION_REGEX)
				            .OfType<Match>()
				            .Select(_ =>
				                    new ProxyRecord {
					                    ProxyScheme = _.Groups["s"].Value,
					                    ProxyAddress = _.Groups["a"].Value,
					                    UserName =  _.Groups["l"].Value,
					                    Password = _.Groups["p"].Value
				                    }
					).ToArray();
			}
			/// <summary>
			/// Адрес сервера
			/// </summary>
			public Uri Uri {
				get { return new Uri(ProxyScheme +"://"+ ProxyAddress); }
			}
			/// <summary>
			/// Учетная запись
			/// </summary>
			public ICredentials Credentials {
				get {
					if (string.IsNullOrWhiteSpace(UserName)) {
						return CredentialCache.DefaultCredentials;
					}
					return new NetworkCredential(UserName, Password);
				}
			}
			/// <summary>
			/// Схема в адресе
			/// </summary>
			public string ProxyScheme;
			/// <summary>
			/// Целевая схема
			/// </summary>
			public string TargetScheme;
			/// <summary>
			/// Имя пользователя
			/// </summary>
			public string UserName;
			/// <summary>
			/// Пароль
			/// </summary>
			public string Password;
			/// <summary>
			/// Адрес прокси
			/// </summary>
			public string ProxyAddress;

		}

		/// <summary>
		/// Метод формирования системного прокси
		/// </summary>
		/// <param name="uri"></param>
		/// <param name="bypassDefinition"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		public static WebProxy SelectSystemProxy(Uri uri, string bypassDefinition) {
			if (WebRequest.DefaultWebProxy.IsBypassed(uri)) return null;
			var proxy = new WebProxy(
				WebRequest.DefaultWebProxy.GetProxy(uri).ToString(),
				true,null,
				CredentialCache.DefaultCredentials);
			if (!string.IsNullOrWhiteSpace(bypassDefinition)) {
				proxy.BypassList = bypassDefinition.SmartSplit(false, true, ' ').ToArray();
			}
			//second check - bypass list updated
			if (proxy.IsBypassed(uri)) return null;
			return proxy;
		}
	}
}