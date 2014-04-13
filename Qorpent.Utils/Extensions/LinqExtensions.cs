using System;
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
        /// <summary>
        ///     Вставляет в типизированный список элемент первым
        /// </summary>
        /// <typeparam name="T">Типизация списка</typeparam>
        /// <param name="list">Исходный список</param>
        /// <param name="value">Значение для вставки</param>
        public static void InsertFirst<T>(this IList<T> list, T value) {
            list.Remove(value);
            list.Insert(0, value);
        }
        /// <summary>
        ///     Обёртка надо foreach
        /// </summary>
        /// <typeparam name="T">Типизация перечисляемого значения</typeparam>
        /// <param name="enumerable">Исходной перечисление</param>
        /// <param name="action">Дейстие</param>
        public static void DoForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (var _ in enumerable) {
                action(_);
            }
        }
        /// <summary>
        ///     Обёртка надо foreach
        /// </summary>
        /// <typeparam name="T">Типизация перечисляемого значения</typeparam>
        /// <typeparam name="TResult">Типизация возвращаемого перечисления</typeparam>
        /// <param name="enumerable">Исходное перечисление</param>
        /// <param name="func">Функция</param>
        /// <returns>Результирующее перечисление</returns>
        public static IEnumerable<TResult> DoForEach<T, TResult>(IEnumerable<T> enumerable, Func<T, TResult> func) {
            return enumerable.Select(func);
        }
    }
}
