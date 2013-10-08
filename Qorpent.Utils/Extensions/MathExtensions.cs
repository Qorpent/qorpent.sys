using System;

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
        public static double RoundToNearestOrder(this double number, int order) {
            return Math.Round(number/(order*10))*(order*10);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double GetNumberOfDigits(this double number) {
            return Math.Floor(Math.Log10(number) + 1);
        }
    }
}
