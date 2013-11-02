using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.Extensions {
    /// <summary>
    /// 
    /// </summary>
    public static class MathExtensions {
        /// <summary>
        ///     Округляет число до ближайшей сотни
        /// </summary>
        /// <returns>Округлённое значение</returns>
        public static double RoundToNearestHundred(this double number) {
            return Math.Round(number.ToInt()/100.0)*100;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static int Round(this double number, int order) {
            var floor = Math.Floor(number / Math.Pow(10, order)).ToInt();
            var fract = Math.Round(number%(Math.Pow(10, order)));

            if (order == 1) {
                var f = ((fract < 5) ? (0) : (10));
                return floor*10 + f;
            }

            var t = floor*Math.Pow(10, order).ToInt();
            var g = ((order > 1) ? fract.Round(order - 1) : (fract.ToInt()));
            return t + g;
        }
        /// <summary>
        ///     Округляет в меньшую сторону относительно указанного порядка
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="order">Порядок, по которому округлять</param>
        /// <returns>Округлённое число</returns>
        public static double RoundDown(this double number, int order) {
            var pow = Math.Pow(10, order);
            return Math.Floor(number.ToInt() / pow) * pow.ToInt();
        }
        /// <summary>
        ///     Суммирование ряда чисел
        /// </summary>
        /// <param name="numbers">Перечисление чисел для суммирования</param>
        /// <returns>Результат суммирования</returns>
        public static double Sigma(this IEnumerable<double> numbers) {
            var result = 0.0;
            numbers.DoForEach(_ => result += _);
            return result;
        }
        /// <summary>
        ///     Округляет в большую сторону относительно указанного порядка
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="order">Порядок, по которому округлять</param>
        /// <returns>Округлённое число</returns>
        public static double RoundUp(this double number, int order) {
            return number.RoundDown(order).ToInt() + Math.Pow(10, order).ToInt();
        }
        /// <summary>
        ///     Чётное
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsEven(this int number) {
            return number%2 == 0;
        }
        /// <summary>
        ///     Нечётное
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsOdd(this int number) {
            return !number.IsEven();
        }
        /// <summary>
        ///     Определяет количество знаков целой части числа
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <returns>Количество знаков целой части числа</returns>
        public static int GetNumberOfDigits(this double number) {
            var intNum = number.ToInt();
            var digits = 1;
            while (intNum/10 != 0) {
                intNum /= 10;
                digits++;
            }

            return digits;
        }
        /// <summary>
        ///     Определяет признак того, что число по модулю больше или рано единице
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <returns>Признак того, что число по модулю больше или рано единице</returns>
        public static bool GreaterOrEqualOneByAbs(this double number) {
            return Math.Abs(number) > 1;
        }
        /// <summary>
        ///     Выполняет действие каждый раз, когда исходное число делится нацело на элемент из перечисления делителей
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="dividers">Перечисление делителей</param>
        /// <param name="action">Действие</param>
        public static void IfDivisible(this double number, IEnumerable<int> dividers, Action<double> action) {
            dividers.DoForEach(_ => {
                if ((number%_).ToInt() == 0) {
                    action(_);
                }
            });
        }
        /// <summary>
        ///     Выполняет переданное действие, если исходное число делится нацело на делитель
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="divider">Делитель</param>
        /// <param name="action">Действие</param>
        public static void IfDivisible(this double number, int divider, Action<double> action) {
            if ((number % divider).ToInt() == 0) {
                action(divider);
            }
        }
        /// <summary>
        ///     Удаляет из перечисления отрицательные значения
        /// </summary>
        /// <param name="numbers">Исходное перечисление чисел</param>
        /// <returns>Очищенное перечисление чисел</returns>
        public static IEnumerable<double> RemoveNegativeValues(this IEnumerable<double> numbers) {
            return numbers.Where(_ => _ >= 0);
        }
        /// <summary>
        ///     Удаляет из перечисления положительные значения
        /// </summary>
        /// <param name="numbers">Исходное перечисление чисел</param>
        /// <returns>Очищенное перечисление чисел</returns>
        public static IEnumerable<double> RemovePositiveValues(this IEnumerable<double> numbers) {
            return numbers.Where(_ => _ <= 0);
        }
        /// <summary>
        ///     Возвращает перечисление без дробной части
        /// </summary>
        /// <param name="numbers">Исходное перечисление</param>
        /// <returns>Перечисление без дробной части</returns>
        public static IEnumerable<double> Floor(this IEnumerable<double> numbers) {
            return numbers.Select(Math.Floor);
        }
        /// <summary>
        ///     Определяет признак того, что числа имеют одинаковый порядок
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="another">Число для сравнения</param>
        /// <returns>Признак того, что числа имеют одинаковый порядок</returns>
        public static bool IsOrderEquals(this double number, double another) {
            return number.GetNumberOfDigits() == another.GetNumberOfDigits();
        }
        /// <summary>
        ///     Определяет признак того, что исходное число больше, чем второе
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="another">Число для сравнения</param>
        /// <returns>Признак того, что исходное число больше, чем второе</returns>
        public static bool IsGreaterThan(this double number, double another) {
            return number > another;
        }
        /// <summary>
        ///     Определяет признак того, что исходное число больше или равно, чем второе
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="another">Число для сравнения</param>
        /// <returns>Признак того, что исходное число больше или равно, чем второе</returns>
        public static bool IsGreaterOrEqual(this double number, double another) {
            return number >= another;
        }
        /// <summary>
        ///     Аппроксимация вверх
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <returns>Аппроксимированное число</returns>
        public static double ApproximateUp(this double number) {
            return number.RoundUp(number.OrderEstimation());
        }
        /// <summary>
        ///     Аппроксимация вниз
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <returns>Аппроксимированное число</returns>
        public static double ApproximateDown(this double number) {
            return number.RoundDown(number.OrderEstimation());
        }
        /// <summary>
        ///     Признак того, что это круглое число
        /// </summary>
        /// <param name="number"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool IsRoundNumber(this double number, int order) {
            return (number%Math.Pow(10, order)).ToInt() == 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double ToDouble(this int n) {
            return n*1.0;
        }
        /// <summary>
        ///     Возвращает наибольшее из двух
        /// </summary>
        /// <param name="f">Исходное число</param>
        /// <param name="s">Второе число</param>
        /// <returns>Наибольшее из двух чисел</returns>
        public static double Maximal(this double f, double s) {
            return (f > s) ? (f) : (s);
        }
        /// <summary>
        ///     Возвращает наименьшее из двух
        /// </summary>
        /// <param name="f">Исходное число</param>
        /// <param name="s">Второе число</param>
        /// <returns>Наибольшее из двух чисел</returns>
        public static double Minimal(this double f, double s) {
            return (f < s) ? (f) : (s);
        }
        /// <summary>
        ///     Возвращает наибольшее из двух
        /// </summary>
        /// <param name="f">Исходное число</param>
        /// <param name="s">Второе число</param>
        /// <returns>Наибольшее из двух чисел</returns>
        public static int Maximal(this int f, int s) {
            return (f*1.0).Maximal(s*1.0).ToInt();
        }
        /// <summary>
        ///     Возвращает наименьшее из двух
        /// </summary>
        /// <param name="f">Исходное число</param>
        /// <param name="s">Второе число</param>
        /// <returns>Наибольшее из двух чисел</returns>
        public static int Minimal(this int f, int s) {
            return (f * 1.0).Minimal(s * 1.0).ToInt();
        }
        /// <summary>
        ///     Определяет порядок приближения числа, по которому можно округлить число, не потеряв семантики
        /// </summary>
        /// <param name="number">Число</param>
        public static int OrderEstimation(this double number) {
            var digits = number.GetNumberOfDigits();

            if (digits < 0) {
                throw new Exception("There are no number with negative count of digits");
            }
            
            if (digits >= 7) {
                return digits - 1;
            }
            
            if (digits >= 4) {
                return digits - 2;
            }
            
            if (digits >= 2) {
                return 2;
            }

            return 1;
        }
    }
}
