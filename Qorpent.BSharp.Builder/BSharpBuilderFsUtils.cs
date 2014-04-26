using System;
using System.IO;
using System.Threading;

namespace Qorpent.Integration.BSharp.Builder {
    /// <summary>
    ///     Утилиты для работы с файловой системой
    /// </summary>
    public static class BSharpBuilderFsUtils {
        /// <summary>
        ///     Производит реальное удаление директории
        ///     через рекурсинвый обход во избежаине 
        ///     экзепшенов при Directory.Delete(target, true)
        /// </summary>
        /// <param name="target">Целевая директория</param>
        /// <param name="repeat"></param>
        public static void DeleteDirectory(string target , bool repeat = true) {
            try {
                if (!Directory.Exists(target)) {
                    return;
                }

                foreach (string file in Directory.GetFiles(target)) {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                foreach (string dir in Directory.GetDirectories(target)) {
                    DeleteDirectory(dir);
                }

                Directory.Delete(target, false);
            }
            catch (Exception) {
                if (repeat) {
                    Thread.Sleep(100);
                    DeleteDirectory(target, false);
                }
                else {
                    throw;
                }
            }
        }
    }
}
