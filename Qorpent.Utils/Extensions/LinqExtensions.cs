using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Extensions {
    /// <summary>
    ///     Расширения для Linq
    /// </summary>
    public static class LinqExtensions {
        /// <summary>
        ///     Позволяет получить следующий элемент текущей коллекции относительно текущего
        /// </summary>
        /// <typeparam name="T">Типизация коллекции</typeparam>
        /// <param name="s">Исходная коллекция</param>
        /// <param name="c">Текущий элемент</param>
        /// <returns>Следующий элемент</returns>
        public static T Next<T>(this IEnumerable<T> s, T c) {
            return s.SkipWhile(_ => !_.Equals(c)).Skip(1).First();
        }
        /// <summary>
        ///     Позволяет предыдущий следующий элемент текущей коллекции относительно текущего
        /// </summary>
        /// <typeparam name="T">Типизация коллекции</typeparam>
        /// <param name="s">Исходная коллекция</param>
        /// <param name="c">Текущий элемент</param>
        /// <returns>Предыдущий элемент</returns>
        public static T Previous<T>(this IEnumerable<T> s, T c) {
            return s.TakeWhile(_ => !_.Equals(c)).Last();
        }
    }
}
