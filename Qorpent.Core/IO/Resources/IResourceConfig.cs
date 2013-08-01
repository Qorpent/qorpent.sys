using System.Text;
using Qorpent.Utils.Config;

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
	}
}