using System.Text;
using Qorpent.Config;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Интерфейс метаданных ресурса
	/// </summary>
	public interface IResourceConfig:IConfig {
		/// <summary>
		/// Данные для отсылки конечной точке(для вебоподобных ресурсов)
		/// </summary>
		byte[] RequestPostData { get; set; }
		/// <summary>
		/// Метод запроса (для вебоподобных ресурсов)
		/// </summary>
		string Method { get; set; }
		/// <summary>
		/// Кодировка закаченных данных
		/// </summary>
		Encoding ResponseEncoding { get; set; }
		/// <summary>
		/// 
		/// </summary>
		ProxyUsage ProxyUsage { get; set; }

		/// <summary>
		/// Адреса прокси в формате 
		/// TYPE=scheme://[USER:PASSWORD@]url, разделенные пробелами, вместо TYPE может использоваться 
		/// звездочка, а имя/пароль могут опускаться (будут использоваться DefaultCredentials)
		/// например 
		/// *=http://myproxy.com
		/// </summary>
		string ProxyAddress { get; set; }

		/// <summary>
		/// Space-delimited list of proxy excludes
		/// </summary>
		string ProxyExcludes { get; set; }

		/// <summary>
		/// True, чтобы принимать самоподписные или недоверенные сертификаты
		/// </summary>
		bool AcceptAllCeritficates { get; set; }
	}
}