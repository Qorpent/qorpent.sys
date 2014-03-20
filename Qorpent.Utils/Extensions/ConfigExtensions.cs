using System.Xml.Linq;
using Qorpent.Config;

namespace Qorpent.Utils.Extensions {
	/// <summary>
	///		Набор расширений для <see cref="IConfig"/>
	/// </summary>
	public static class ConfigExtensions {
		/// <summary>
		///		Читает атрибуты переданного <see cref="XElement"/> и записывает их в реализацию <see cref="IConfig"/>
		/// </summary>
		/// <param name="xElement">Исходный конфиг в виде <see cref="XElement"/></param>
		/// <param name="toLowerCase">Признак того, что нужно приводить имена параметров к нижнему регистру</param>
		/// <returns>Конфиг в виде реализации <see cref="IConfig"/></returns>
		public static IConfig ToConfig(this XElement xElement, bool toLowerCase = true) {
			var config = new ConfigBase();
			foreach (var attribute in xElement.Attributes()) {
				var name = toLowerCase ? attribute.Name.LocalName.ToLowerInvariant() : attribute.Name.LocalName;
                config.Set(name, attribute.Value);
            }
			return config;
		}
		/// <summary>
		///		Приводит экземпляра класса, реализующего <see cref="IConfig"/> к эквивалентному <see cref="XElement"/>
		/// </summary>
		/// <param name="config">Исходный конфиг</param>
		/// <param name="name">Имя элемента</param>
		/// <returns>Собранный <see cref="XElement"/></returns>
		public static XElement ToXElement(this IConfig config, string name = "config") {
			var xElement = new XElement(name);
			foreach (var item in config) {
				xElement.SetAttributeValue(item.Key, item.Value);
			}
			return xElement;
		}
	}
}
