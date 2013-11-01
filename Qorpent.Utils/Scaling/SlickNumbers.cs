using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.Extensions;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    /// 
    /// </summary>
    public static class SlickNumbers {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Gcd(double a, double b) {
            return b == 0.0 ? a : Gcd(b, a % b);
        }
        /// <summary>
        ///     Генерирует ряд из чисел с шагом
        /// </summary>
        /// <param name="from">Начало ряда (включительно)</param>
        /// <param name="to">Предел ряда ряда</param>
        /// <param name="step">Шаг</param>
        /// <returns></returns>
        public static IEnumerable<double> GenerateLine(double from, double to, double step) {
            if (Math.Abs(step - (double.MinValue - double.MaxValue)) <= 0) {
                throw new Exception("Infinity loop detected");
            }

            if (to < from) {
                throw new Exception("Infinity loop detected");
            }

            var current = from;
            while (current <= to) {
                yield return current;
                current += step;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public static IEnumerable<double> Dispersion(IEnumerable<double> numbers) {
            var array = numbers.ToArray();
            for (var i = 1; i < array.Length; i++) {
                yield return array[i] - array[i - 1];
            }
        }
        /// <summary>
        ///     Поиск максимального разброса между значениями
        /// </summary>
        /// <param name="numbers">Пречисление чисел</param>
        /// <returns>Максимальный разброс между двумя соседними числами</returns>
        public static double MaxDispersion(IEnumerable<double> numbers) {
            return Dispersion(numbers.OrderBy(_ => _)).Max();
        }
        /// <summary>
        ///     Поиск минимального разброса между значениями
        /// </summary>
        /// <param name="numbers">Пречисление чисел</param>
        /// <returns>Минимальный разброс между двумя соседними числами</returns>
        public static double MinDispersion(IEnumerable<double> numbers) {
            return Dispersion(numbers.OrderBy(_ => _)).Min();
        }
        /// <summary>
        ///     Сделать охуенно
        /// </summary>
        /// <param name="notSlick"></param>
        /// <returns></returns>
        public static IEnumerable<double> SlickSort(IEnumerable<double> notSlick) {
            notSlick = notSlick.OrderByDescending(_ => _);
            var t = new List<double>();
            for (var i = notSlick.Max().GetNumberOfDigits() - 1; i > 0; i--) {
                foreach (var d in notSlick.Where(_ => _%Math.Pow(10, i) == 0 && !t.Contains(_))) {
                    t.Add(d);
                }
            }

            var sublist = notSlick.Where(_ => !t.Contains(_));

            if (!sublist.Any()) return t;

            for (var i = sublist.Max().GetNumberOfDigits() - 1; i >= 0; i--) {
                foreach (var d in sublist.Where(_ => {
                    if (t.Contains(_)) {
                        return false;
                    }
                    var divider = Math.Pow(10, i)*2;
                    var result = _%divider;
                    var res = result == 0.0;
                    return res;
                })) {
                    t.Add(d);
                }
            }

            sublist = notSlick.Where(_ => !t.Contains(_));

            return t.Union(sublist);
        }
    }
}