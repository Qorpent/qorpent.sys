namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Эпликейшен
    /// </summary>
    public class BrickChartApplication {
        /// <summary>
        ///     Объект, к которому применяется стиль
        /// </summary>
        public string ToObject { get; set; }
        /// <summary>
        ///     Стили
        /// </summary>
        public string Styles { get; set; }
        /// <summary>
        ///     Приведение <see cref="BrickChartApplication"/> к <see cref="string"/>
        /// </summary>
        /// <returns>Строковое представление <see cref="BrickChartApplication"/></returns>
        public override string ToString() {
            return "{\r\n\t\"toobject\" : \"" + ToObject + "\",\r\n\t\"styles\" : \"" + Styles + "\"\r\n}";
        }
    }
}