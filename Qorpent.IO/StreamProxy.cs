using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qorpent.IO {
    /// <summary>
    ///     Класс, проксирующий один поток на чтение в несколько потоков на запись
    /// </summary>
    public class StreamProxy : IStreamProxy {
        /// <summary>
        ///     Размер буффера
        /// </summary>
        private int _bufferSize;
        /// <summary>
        ///     Размер буффера
        /// </summary>
        public int BufferSize {
            get { return _bufferSize; }
            set { _bufferSize = (value > 0) ? (value) : (_bufferSize); }
        }
        /// <summary>
        ///     Класс, проксирующий один поток на чтение в несколько потоков на запись
        /// </summary>
        public StreamProxy() {
            BufferSize = 1024;
        }
        /// <summary>
        ///     Проксирование
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="targets">Целевые потоки</param>
        /// <returns>Количество проксированных байт</returns>
        public int Proxy(Stream source, params Stream[] targets) {
            return RollProxySync(source, targets);
        }
        /// <summary>
        ///     Проксирование потока в файлы
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="paths">Полные пути до целевых файлов</param>
        /// <returns>Количество проксированных байт</returns>
        public int Proxy(Stream source, params string[] paths) {
            var streams = paths.Select(File.OpenWrite).ToList();
            var proxied = Proxy(source, streams.ToArray());
            foreach (var fileStream in streams) {
                fileStream.Close();
            }

            return proxied;
        }
        /// <summary>
        ///     Асинхронное проксирование
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="targets">Целевые потоки</param>
        /// <returns>Количество проксированных байт</returns>
        public Task<int> ProxyAsync(Stream source, params Stream[] targets) {
            return Task.Factory.StartNew(() => RollProxySync(source, targets));
        }
        /// <summary>
        ///     Асинхронное проксирование потока в файлы
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="paths">Полные пути до целевых файлов</param>
        /// <returns>Количество проксированных байт</returns>
        public Task<int> ProxyAsync(Stream source, params string[] paths) {
            return Task.Factory.StartNew(() => Proxy(source, paths));
        }
        /// <summary>
        ///     Прокатывает синхронную операцию проксирования потоков
        /// </summary>
        /// <param name="source">Поток-источник</param>
        /// <param name="targets">Целевые потоки</param>
        private int RollProxySync(Stream source, params Stream[] targets) {
            var buffer = new byte[BufferSize];
            var offset = 0;

            while (true) {
                var read = source.Read(buffer, offset, BufferSize);
                offset += read;

                foreach (var target in targets) {
                    target.Write(buffer, offset - read, read);
                }

                if ((read == 0) || (offset == source.Length)) {
                    break;
                }
            }

			foreach (var target in targets) {
				target.Position = 0;
			}

            return offset;
        }
    }
}
