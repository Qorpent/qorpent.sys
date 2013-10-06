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
    }
}
