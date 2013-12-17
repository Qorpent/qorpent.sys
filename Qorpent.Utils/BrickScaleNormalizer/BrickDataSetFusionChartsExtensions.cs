using System;
using System.Globalization;
using System.Linq;

namespace Qorpent.Utils.BrickScaleNormalizer {
    /// <summary>
    ///     Набор расширений для интеграции BrickDataSet с FusionCharts
    /// </summary>
    public static class BrickDataSetFusionChartsExtensions {
        /// <summary>
        ///     Рендеринг датасета в JSON
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет</param>
        /// <returns>JSON-представление датасета</returns>
        public static string RenderDataset(this BrickDataSet brickDataSet) {
            var json = "\"dataset\" : [\r\n";
            foreach (var seria in brickDataSet.Series) {
                json += seria.RenderSeria() + ",";
            }
            json += "]";
            return json.Replace(",]", "]");
        }
        /// <summary>
        ///     Рендеринг серии в JSON
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет</param>
        /// <param name="serianumber">Номер отрисовываемомй серии</param>
        /// <returns>JSON-представление серии</returns>
        public static string RenderSeria(this BrickDataSet brickDataSet, int serianumber) {
            var seria = brickDataSet.Series.FirstOrDefault(_ => _.SeriaNumber.Equals(serianumber));
            if (seria == null) {
                throw new Exception("Seria not found");
            }

            var json = "\r\n\t{";
            foreach (var meta in seria.Meta) {
                json += "\r\n\t\t\"" + meta.Key + "\" : \"" + meta.Value.Replace("\"", "\\\"") + "\",";
            }
            json += "\r\n" + brickDataSet.RenderData(serianumber) + "\r\n\t}";
            return json;
        }
        /// <summary>
        ///     Рендеринг серии в JSON
        /// </summary>
        /// <param name="seria">Серия</param>
        /// <returns>JSON-представление серии</returns>
        public static string RenderSeria(this BrickDataSetSeria seria) {
            var json = "\r\n\t{";
            foreach (var meta in seria.Meta) {
                json += "\r\n\t\t\"" + meta.Key + "\" : \"" + meta.Value.Replace("\"", "\\\"") + "\",";
            }
            json += "\r\n" + seria.RenderData() + "\r\n\t}";
            return json;
        }
        /// <summary>
        ///     Отрисовка блока «data» по указанному номеру серии
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет</param>
        /// <param name="serianumber">Номер отрисовываемомй серии</param>
        /// <returns>JSON</returns>
        public static string RenderData(this BrickDataSet brickDataSet, int serianumber) {
            return brickDataSet.Series.FirstOrDefault(_ => _.SeriaNumber.Equals(serianumber)).RenderData();
        }
        /// <summary>
        ///     Отрисовка данных указанной серии
        /// </summary>
        /// <param name="seria">Серия</param>
        /// <returns>JSON-представление серии</returns>
        public static string RenderData(this BrickDataSetSeria seria) {
            var json = "\t\t\"data\" : [";
            foreach (var item in seria) {
                json += "\r\n\t\t\t{\"value\" : \"" + item.Value.ToString(CultureInfo.InvariantCulture) + "\", ";
                if (item.LabelPosition == LabelPosition.Hidden) {
                    json += "\"showValue\" : \"0\"";
                } else {
                    json += "\"valuePosition\" : \"" + item.LabelPosition.ToString() + "\"";
                }
                json += "},";
            }
            json += "]";
            return json.Replace("},]", "}\r\n\t\t]");
        }
        /// <summary>
        ///     Отрисовка категорий
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет</param>
        /// <returns>JSON массив categories</returns>
        public static string RenderCategories(this BrickDataSet brickDataSet) {
            var json = "\r\n\"categories\" : [\r\n\t{\"category\" : [";
            foreach (var _ in brickDataSet.Categories) {
                json += "\r\n\t\t{ \"label\" : \"" + _.Replace("\"", "\\\"") + "\"},";
            }
            json += "]}]";
            return json.Replace("},]}]", "}\r\n\t]}\r\n]\r\n");
        }
    }
}
