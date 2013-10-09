using System;
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
        public static int RoundToNearestOrder(this double number, int order) {
            var floor = Math.Floor(number / Math.Pow(10, order)).ToInt();
            var fract = Math.Round(number%(Math.Pow(10, order)));

            if (order == 1) {
                var f = ((fract < 5) ? (0) : (10));
                return floor*10 + f;
            }

            var t = floor*Math.Pow(10, order).ToInt();
            var g = ((order > 1) ? fract.RoundToNearestOrder(order - 1) : (fract.ToInt()));
            return t + g;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static int GetNumberOfDigits(this double number) {
            return Math.Floor(Math.Log10(number) + 1).ToInt();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double ToDouble(this int n) {
            return n*1.0;
        }
    }
}
