using System;
using System.Collections.Generic;
using System.Diagnostics;

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
        public static int RoundDown(this double number, int order) {
            var t = Math.Pow(10, order);
            var f = Math.Floor(number.ToInt()/t);
            var y = Math.Round(number%t);
            var b = (order > 1) ? Math.Floor(y / Math.Pow(10, order - 1)) : (0);
            return (f*Math.Pow(10, order) + b * (Math.Pow(10, order - 1))).ToInt();
        }
        /// <summary>
        ///     Округляет в большую сторону относительно указанного порядка
        /// </summary>
        /// <param name="number">Исходное число</param>
        /// <param name="order">Порядок, по которому округлять</param>
        /// <returns>Округлённое число</returns>
        public static int RoundUp(this double number, int order) {
            return number.RoundDown(order) + ((order == 1) ? (10) : (Math.Pow(10, order - 1).ToInt()));
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
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetNumberOfDigits(this double number) {
            return Math.Floor(Math.Log10(number) + 1).ToInt(true);
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
    }
}
