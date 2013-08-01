using System.Text;
using Qorpent.Config;

namespace Qorpent.IO.Resources {
	/// <summary>
	/// Базовая реализация конфгурации запроса и самого ресурса
	/// </summary>
	public class ResourceConfig : ConfigBase, IResourceConfig {
		/// <summary>
		/// Имя параметра данных для закачки POST
		/// </summary>
		public const string REQUEST_POST_DATA = "request_post_data";
		/// <summary>
		/// Метод веб для запроса
		/// </summary>
		public const string REQUEST_METHOD = "request_method";

		/// <summary>
		/// Кодировка результатов загрузки
		/// </summary>
		public const string RESPONSE_ENCODING = "response_encoding";

		/// <summary>
		/// Данные для отсылки конечной точке(для вебоподобных ресурсов)
		/// </summary>
		public byte[] RequestPostData {
			get { return Get<byte[]>(REQUEST_POST_DATA); }
			set { Set(REQUEST_POST_DATA, value); }
		}

		/// <summary>
		/// Метод запроса (для вебоподобных ресурсов)
		/// </summary>
		public string Method {
			get { return Get<string>(REQUEST_METHOD); }
			set { Set(REQUEST_METHOD, value); }
		}

		/// <summary>
		/// Метод запроса (для вебоподобных ресурсов)
		/// </summary>
		public Encoding ResponseEncoding
		{
			get { return Get<Encoding>(RESPONSE_ENCODING); }
			set { Set(RESPONSE_ENCODING, value); }
		}
	}
}