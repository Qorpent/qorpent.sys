using System.Globalization;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Набор расширений для интеграции BrickDataSet с FusionCharts
    /// </summary>
    public static class BrickDataSetFusionChartsExtensions {
        /// <summary>
        ///     Отрисовка блока «data» по указанному номеру серии
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет</param>
        /// <param name="serianumber">Номер отрисовываемомй серии</param>
        /// <returns>JSON</returns>
        public static string RenderData(this BrickDataSet brickDataSet, int serianumber) {
            var colons = brickDataSet.BuildColons();
            var json = "'data' : [";
            foreach (var item in colons.Select(_ => _.Items[serianumber])) {
                json += "\r\n\t\t\t{'value' : '" + item.Value.ToString(CultureInfo.InvariantCulture)+ "', ";
                if (item.LabelPosition == LabelPosition.Hidden) {
                    json += "'showValue' : '0'";
                } else {
                    json += "'valuePosition' : '" + item.LabelPosition.ToString() + "'";
                }
                json += "},";
            }
            json += "]";
            return json.Replace("},]", "}]");
        }
        /// <summary>
        ///     Отрисовка категорий
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет</param>
        /// <returns>JSON массив categories</returns>
        public static string RenderCategories(this BrickDataSet brickDataSet) {
            var json = "\r\n'categories' : [\r\n\t{'category': [";
            foreach (var _ in brickDataSet.Categories) {
                json += "\r\n\t\t{ 'label' : '" + _ + "'},";
            }
            json += "]}]";
            return json.Replace("},]}]", "}\r\n\t]}\r\n]\r\n");
        }
    }
}
