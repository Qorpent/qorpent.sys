using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Qorpent.Utils.Extensions {
    /// <summary>
    ///     Набор расширений для потоков
    /// </summary>
    public static class StreamExtensions {
        /// <summary>
        ///     Читает поток до конца как строку
        /// </summary>
        /// <param name="stream">Исходный поток</param>
        /// <returns>Строка</returns>
        public static string ReadToEndAsString(this Stream stream) {
            return new StreamReader(stream).ReadToEnd();
        }
        /// <summary>
        ///     Читает поток до конца как строку в асинхронном режиме
        /// </summary>
        /// <param name="stream">Исходный поток</param>
        /// <returns>Строка</returns>
        public static async Task<string> ReadToEndAsStringAsync(this Stream stream) {
            return await new StreamReader(stream).ReadToEndAsync();
        }
        /// <summary>
        ///     Читает поток до конца как XML
        /// </summary>
        /// <param name="stream">Исходный поток</param>
        /// <returns>XML</returns>
        public static XElement ReadToEndAsXml(this Stream stream) {
            return XElement.Parse(new StreamReader(stream).ReadToEnd());
        }
        /// <summary>
        ///     Читает поток до конца как XML в асинхронном режиме
        /// </summary>
        /// <param name="stream">Исходный поток</param>
        /// <returns>XML</returns>
        public static async Task<XElement> ReadToEndAsXmlAsync(this Stream stream) {
            var str = await stream.ReadToEndAsStringAsync();
            return XElement.Parse(str);
        }
    }
}
