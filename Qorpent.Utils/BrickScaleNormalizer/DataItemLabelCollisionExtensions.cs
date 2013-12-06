using System;
using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Набор расширений для <see cref="DataItemLabelCollision"/>
    /// </summary>
    internal static class DataItemLabelCollisionExtensions {
        /// <summary>
        ///     Температура коллизии, начиная с которой она считается «ОЧЕНЬ ГОРЯЧЕЙ»
        /// </summary>
        public static decimal VeryHotBorderline = 13;
        /// <summary>
        ///     Поис «ОЧЕНЬ ГОРЯЧИХ» коллизий в переданном представлении коллизии
        /// </summary>
        /// <param name="collision">Представление коллизии</param>
        /// <returns>Перечисление «ОЧЕНЬ ГОРЯЧИХ» коллизий</returns>
        public static IEnumerable<DataItem> SelectVeryHot(this DataItemLabelCollision collision) {
            return collision.Where(_ => collision.DataItem.Temperature(_) > VeryHotBorderline);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collision"></param>
        /// <returns></returns>
        public static IEnumerable<DataItem> SelectSimilar(this DataItemLabelCollision collision) {
            return collision.Conflicts.Where(_ => Math.Abs(collision.DataItem.NormalizedLabelMax - _.NormalizedLabelMax) <= 2);
        }
    }
}