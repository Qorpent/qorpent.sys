using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.FuzzyLogic {
    /// <summary>
    ///     Представление нечёткого множества
    /// </summary>
    /// <typeparam name="T">Типизация нечёткого множества</typeparam>
    public interface IFuzzySet<T> : IEnumerable<T> {
        /// <summary>
        ///     Мю-функция для определения принадлежности элемента к множеству
        /// </summary>
        Func<T, double> MuFunc { get; set; }
        /// <summary>
        ///     Возвращает перечисление пар типа {[степень_принадлежности]:[элемент]}
        /// </summary>
        IEnumerable<KeyValuePair<double, T>> AsCountedPairs { get; }
        /// <summary>
        ///     Определяет степень принадлежности элемента к множеству
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        double Mu(T element);
        /// <summary>
        ///     Вставляет элемет во множество
        /// </summary>
        /// <param name="element">Элемент множества</param>
        void Insert(T element);
        /// <summary>
        ///     Очистка сета от всех элементов
        /// </summary>
        void Clear();
        /// <summary>
        ///     Вставка перечисления элементов
        /// </summary>
        /// <param name="elements">Перечисление элементов</param>
        void InsertRange(IEnumerable<T> elements);
        /// <summary>
        ///     Удаление элемента из множества
        /// </summary>
        /// <param name="element">Элемент множества</param>
        void Remove(T element);
    }
    /// <summary>
    ///     Представление нечёткого множества
    /// </summary>
    /// <typeparam name="T">Типизация нечёткого множества</typeparam>
    public class FuzzySet<T> : IFuzzySet<T> {
        /// <summary>
        ///     Внутренний экземпляр функции Мю
        /// </summary>
        private Func<T, double> _muFunc;
        /// <summary>
        ///     Внутреннее представление множества
        /// </summary>
        private readonly List<T> _fuzzySet;
        /// <summary>
        ///     Представление нечёткого множества
        /// </summary>
        public FuzzySet() {
            _fuzzySet = new List<T>();
            _muFunc = _ => 0;   //  по умолчанию реализуем единственно возможное в универсуме U пустое множество
        }
        /// <summary>
        ///     Мю-функция для определения принадлежности элемента к множеству
        /// </summary>
        public Func<T, double> MuFunc {
            get { return _muFunc; }
            set {
                if (value == null) {
                    throw new Exception("Mu func can not be null");
                }

                _muFunc = value;
            }
        }
        /// <summary>
        ///     Возвращает перечисление пар типа {[степень_принадлежности]:[элемент]}
        /// </summary>
        public IEnumerable<KeyValuePair<double, T>> AsCountedPairs {
            get { return _fuzzySet.Select(_ => new KeyValuePair<double, T>(Mu(_), _)); }
        }
        /// <summary>
        ///     Определяет степень принадлежности элемента к множеству
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public double Mu(T element) {
            if (MuFunc == null) {
                throw new Exception("Mu is not set!");
            }

            return MuFunc(element);
        }
        /// <summary>
        ///     Вставляет элемет во множество
        /// </summary>
        /// <param name="element">Элемент множества</param>
        public void Insert(T element) {
            _fuzzySet.Add(element);
        }
        /// <summary>
        ///     Очистка сета от всех элементов
        /// </summary>
        public void Clear() {
            _fuzzySet.Clear();
        }
        /// <summary>
        ///     Вставка перечисления элементов
        /// </summary>
        /// <param name="elements">Перечисление элементов</param>
        public void InsertRange(IEnumerable<T> elements) {
            _fuzzySet.AddRange(elements);
        }
        /// <summary>
        ///     Удаление элемента из множества
        /// </summary>
        /// <param name="element">Элемент множества</param>
        public void Remove(T element) {
            _fuzzySet.Remove(element);
        }
        /// <summary>
        ///     Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator() {
            return _fuzzySet.GetEnumerator();
        }
        /// <summary>
        ///     Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
    /// <summary>
    ///     Расширения для удобной работы с нечёткими множествами
    /// </summary>
    public static class FuzzyExtensions {
        /// <summary>
        ///     Возвращает перечисление элементов, у которых значение Мю эквивалентно указанному
        /// </summary>
        /// <param name="fuzzySet">Представление сета</param>
        /// <param name="muResult">Ожидаемый результат мю</param>
        /// <returns>Перечисление элементов, у которых значение Мю эквивалентно указанному</returns>
        public static IEnumerable<T> WhereMuEquals<T>(this IFuzzySet<T> fuzzySet, double muResult) {
            return fuzzySet.Where(_ => fuzzySet.Mu(_).Equals(muResult));
        }
        /// <summary>
        ///     Мерж нескольких нечётких множеств между собой
        /// </summary>
        /// <typeparam name="T">Типизация сетов</typeparam>
        /// <param name="fuzzySet">Исходный сет</param>
        /// <param name="sets">Массив сетов</param>
        /// <returns>Замыкание на исходный сет</returns>
        public static IFuzzySet<T> Merge<T>(this IFuzzySet<T> fuzzySet, params IFuzzySet<T>[] sets) {
            EnumerableExtensions.DoForEach(sets, _ => fuzzySet.Merge(_));
            return fuzzySet;
        }
        /// <summary>
        ///     Мерж двух нечётких множеств между собой
        /// </summary>
        /// <typeparam name="T">Типизация сетов</typeparam>
        /// <param name="fuzzySet">Исходный сет</param>
        /// <param name="toMegeFuzzySet">Сет для для мержа</param>
        /// <returns>Замыкание на исходный сет</returns>
        public static IFuzzySet<T> Merge<T>(this IFuzzySet<T> fuzzySet, IFuzzySet<T> toMegeFuzzySet) {
            EnumerableExtensions.DoForEach(toMegeFuzzySet.Where(_ => !fuzzySet.Any(__ => __.Equals(_))), fuzzySet.Insert);
            return fuzzySet;
        }
        /// <summary>
        ///     Вставляет в сет несколько пречислений массивов элементов
        /// </summary>
        /// <typeparam name="T">Типизация сетов</typeparam>
        /// <param name="fuzzySet">Исходный сет</param>
        /// <param name="values">Перечисление массивов элементов</param>
        /// <returns>Замыкание на исходный сет</returns>
        public static IFuzzySet<T> InsertRange<T>(this IFuzzySet<T> fuzzySet, params T[] values) {
            EnumerableExtensions.DoForEach(values, _ => fuzzySet.InsertRange(_));
            return fuzzySet;
        }
    }
}
