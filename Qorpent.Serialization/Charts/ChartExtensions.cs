using System;
using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// 
    /// </summary>
    public static class ChartExtensions {
        /// <summary>
        ///     Добавление элемента к чарту
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <param name="element">Элемент для добавления</param>
        /// <returns>Замыкание на чарт</returns>
        public static IChart AddElement(this IChart chart, IChartElement element) {
            if (element is IChartDataset) {
                chart.Datasets.Add(element as IChartDataset);
                SetParent(element, chart.Datasets);
            } else if (element is IChartCategory) {
                chart.Categories.Add(element as IChartCategory);
                SetParent(element, chart.Categories);
            } else if (element is IChartDataItem) {
                var p = ParentOf<IChartDataset>(element);
                
                if (p != null) {
                    p.Children.Add(element as IChartDataItem);
                    SetParent(element, p);
                }
            } else if (element is IChartTrendLine) {
                var p = ParentOf<IChartTrendLines>(element);

                if (p == null) {
                    chart.TrendLines.Add(element as IChartTrendLine);
                    SetParent(element, chart.TrendLines);
                } else {
                    p.Add(element as IChartTrendLine);
                    SetParent(element, chart.TrendLines);
                }
            }

            return chart;
        }
        /// <summary>
        ///     Добавление элемента к чарту
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <param name="element">Элемент для добавления</param>
        /// <param name="postInit">Действие пост-инициализации</param>
        /// <returns>Замыкание на чарт</returns>
        public static IChart AddElement(this IChart chart, IChartElement element, Action<IChartElement> postInit) {
            postInit(chart.AddElement(element));
            return chart;
        }
        /// <summary>
        ///     Добавление элементов к чарту
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <param name="elements">Элементы для добавления</param>
        /// <returns>Замыкание на чарт</returns>
        public static IChart AddElements(this IChart chart, IEnumerable<IChartElement> elements) {
            foreach (var element in elements) {
                chart.AddElement(element);
            }

            return chart;
        }
        /// <summary>
        ///     Установка нонфига для <see cref="Chart"/>
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <param name="chartConfig">Конфиг чарта</param>
        /// <returns>Замыкание на чарт</returns>
        public static IChart SetConfig(this IChart chart, IChartConfig chartConfig) {
            if (chart is Chart) {
                (chart as Chart).Config = chartConfig;
            }

            return chart;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="parent"></param>
        private static void SetParent(IChartElement element, IChartElement parent) {
            element.SetParentElement(parent);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static T ParentOf<T>(IChartElement element) {
            return element.Get<T>(ChartDefaults.ChartElementParentProperty);
        }
        /// <summary>
        ///     Установка атрибута
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IChartElement SetAttribute(this IChartElement element, string name, object value) {
            element.Set(name, value);
            return element;
        }
    }
}
