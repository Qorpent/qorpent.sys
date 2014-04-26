
namespace Qorpent.Integration.BSharp.Builder {
    /// <summary>
    /// 
    /// </summary>
    public static class BSharpBuilderClassUtils {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string GetClassname(string fullname) {
            return fullname.Substring(
                fullname.LastIndexOf('.') + 1,
                fullname.Length - fullname.LastIndexOf('.') - 1
            );
        }
        /// <summary>
        ///     получение относительной директории по нэймспэсу класса
        ///     пример: из test.name.space.clazzz
        ///             получит {"test\name\space", "clazzz"}
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string GetRelativeDirByNamespace(string fullname) {
            return GetNamespace(fullname).Replace('.', '\\');
        }
        /// <summary>
        ///     получает нэймспэйс из полного имени.
        ///     Пример: из test.name.space.clazzz
        ///             получит test.name.space
        /// </summary>
        /// <param name="fullname"></param>
        /// <returns></returns>
        public static string GetNamespace(string fullname) {
            var exploded = fullname.Split('.');
            var plainns = "";

            for (var i = 0; i < (exploded.Length - 1); i++) {
                plainns += exploded[i];
                plainns += (i != (exploded.Length - 1) - 1) ? (".") : (""); // если следующий элемент — имя класса, точку не ставим
            }

            return plainns;
        }
    }
}
