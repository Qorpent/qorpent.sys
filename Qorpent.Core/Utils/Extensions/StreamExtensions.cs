﻿using System.Collections.Generic;
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
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadToAsByte(this Stream stream) {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream()) {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<byte[]> ReadToEndAsByteAsync(this Stream stream) {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream()) {
                int read;
                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0) {
                    await ms.WriteAsync(buffer, 0, read);
                }
                return ms.ToArray();
            }
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
        /// <summary>
        ///     
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static Stream SeekBegin(this Stream stream) {
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
    }
}
