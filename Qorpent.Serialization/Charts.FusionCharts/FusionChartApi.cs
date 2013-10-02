using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Serialization;
using Qorpent.Utils.Extensions;

namespace Qorpent.Charts.FusionCharts {
    public static partial  class FusionChartApi {
        /// <summary>
        /// Проверяет доступность атрибута для данного элемента в конфигурации
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static bool IsMatch(string element, string name, IChartConfig config) {
            var elementType = element.To<FusionChartElementType>();
            var chartType = config.Type.To<FusionChartType>();
            return IsMatch(name, chartType, elementType, config);
        }
        /// <summary>
        /// Вычисляет укрупненную группу графика
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static FusionChartGroupedType GetChartGroup(this IChartConfig config) {
            return GetChartGroup(config.Type.To<FusionChartType>());
        }
        /// <summary>
        /// Вычисляет укрупненную группу графика
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static FusionChartGroupedType GetChartGroup(this FusionChartType type) {
            foreach (long g in Enum.GetValues(typeof(FusionChartGroupedType))) {
                if (0 != (g & (long) type)) {
                    return (FusionChartGroupedType)g;
                }
            }
            throw new Exception("group not found");
        }

        /// <summary>
        /// Проверяет доступность атрибута для данного элемента в конфигурации
        /// </summary>
        /// <param name="name"></param>
        /// <param name="chartType"></param>
        /// <param name="elementType"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private static bool IsMatch(string name, FusionChartType chartType, FusionChartElementType elementType = FusionChartElementType.Chart, IChartConfig config = null) {
            var key = (elementType.ToString() + "_" + name).ToLower();
            if (Attributes.ContainsKey(key)) {
                return 0 != (Attributes[key].Charts & chartType);
            }
            //custom attribute
            return true;
        }

        /// <summary>
        /// Находит все атрибуты по указанному критерию
        /// </summary>
        /// <param name="chartType"></param>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerable<FusionChartAttributeDescriptor> FindAttributes(FusionChartType chartType = FusionChartType.None, FusionChartElementType element = FusionChartElementType.None, string name = null) {
            return Attributes.Values.Where(
                _ => {
                    if (chartType != FusionChartType.None)
                    {
                        if (0 == (_.Charts & chartType)) return false;
                    }
                    if (element != FusionChartElementType.None)
                    {
                        if (_.Element != element) return false;
                    }
                    if (!string.IsNullOrWhiteSpace(name)) {
                        if (name.ToLower() != _.Name.ToLower()) return false;
                    }
                    
                    return true;
                }
                );
        }

        /// <summary>
        /// Возвращает адаптер для работы с графиком в API FusionChart
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static FusionChartWrapper AsFusion(this IChartElement chart, IChartConfig config=null) {
            config = config ?? new ChartConfig(){Type = FusionChartType.Column2D.ToString()};
            return new FusionChartWrapper(chart,config); 

        }
        
    }
}