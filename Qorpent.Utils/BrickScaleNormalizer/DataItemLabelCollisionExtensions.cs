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
        public static decimal VeryHotBorderline = 15;
        /// <summary>
        ///     Поис «ОЧЕНЬ ГОРЯЧИХ» коллизий в переданном представлении коллизии
        /// </summary>
        /// <param name="collision">Представление коллизии</param>
        /// <returns>Перечисление «ОЧЕНЬ ГОРЯЧИХ» коллизий</returns>
        public static IEnumerable<DataItem> SelectVeryHot(this DataItemLabelCollision collision) {
            return collision.Where(_ => collision.DataItem.Temperature(_) > VeryHotBorderline);
        }
    }
}