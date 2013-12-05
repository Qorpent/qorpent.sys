using System;
using System.Collections.Generic;
using System.Linq;
using Qorpent.Utils.BrickScaleNormalizer;
using Qorpent.Utils.Extensions;
using Qorpent.Charts.FusionCharts;

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
        public static IChart Add(this IChart chart, IChartElement element) {
            if (element is IChartDatasets) {
                if (chart is Chart) {
                    ((Chart) chart).Datasets = (IChartDatasets) element;
                }
            } else if (element is IChartDataset) {
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
        ///     Добавление элемента к элементу
        /// </summary>
        /// <param name="baseElement">Исходное представление элемента</param>
        /// <param name="element">Элемент для добавления</param>
        /// <returns>Замыкание на исходный элемент</returns>
        public static IChartElement Add(this IChartElement baseElement, IChartElement element) {
            if (baseElement is IChart) {
                (baseElement as IChart).Add(element);
            } else if (baseElement is IChartDataset) {
                if (element is IChartDataItem) {
                    (baseElement as IChartDataset).Add(element as IChartDataItem);
                }
            }

            return baseElement;
        }
        /// <summary>
        ///     Добавление элемента к чарту
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <param name="element">Элемент для добавления</param>
        /// <param name="postInit">Действие пост-инициализации</param>
        /// <returns>Замыкание на чарт</returns>
        public static IChart Add(this IChart chart, IChartElement element, Action<IChartElement> postInit) {
            postInit(chart.Add(element));
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
                chart.Add(element);
            }

            return chart;
        }
        /// <summary>
        ///     Установка конфига для <see cref="Chart"/>
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
        ///     Установка родительского элемента элементу. Фасад над <see cref="IChartElement.SetParentElement"/>
        /// </summary>
        /// <param name="element">Исходный элемент</param>
        /// <param name="parent">Родительский элемент</param>
        private static void SetParent(IChartElement element, IChartElement parent) {
            element.SetParentElement(parent);
        }
        /// <summary>
        ///     Возвращает типизированный класс-родитель
        /// </summary>
        /// <param name="element">Исходный элемент</param>
        /// <returns>Типизированный класс-родитель</returns>
        public static T ParentOf<T>(IChartElement element) {
            return element.Get<T>(ChartDefaults.ChartElementParentProperty);
        }
        /// <summary>
        ///     Установка атрибута
        /// </summary>
        /// <param name="element">Исходный элемент</param>
        /// <param name="name">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        /// <returns>Замыкание на элемент</returns>
        public static IChartElement Set(this IChartElement element, string name, object value) {
            element.Set(name, value);
            return element;
        }
        /// <summary>
        ///     Получение значения атрибута
        /// </summary>
        /// <param name="element">Исходный элемент</param>
        /// <param name="name">Имя атрибута</param>
        /// <returns>Значение атрибута</returns>
        public static object Get(this IChartElement element, string name) {
            return element.Get(name, typeof (object));
        }
        /// <summary>
        ///     Получение значения атрибута
        /// </summary>
        /// <typeparam name="T">Типизация</typeparam>
        /// <param name="element">Исходный элемент</param>
        /// <param name="name">Имя атрибута</param>
        /// <returns>Значение атрибута</returns>
        public static T Get<T>(this IChartElement element, string name) {
            return (T)element.Get(name);
        }
        /// <summary>
        ///     Установка атрибута
        /// </summary>
        /// <typeparam name="T">Типизация элемента</typeparam>
        /// <param name="element">Исходный элемент</param>
        /// <param name="name">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        /// <returns>Замыкание на типизированный элемент</returns>
        public static T Set<T>(this IChartElement element, string name, object value) {
            return (T)Set(element, name, value);
        }
        /// <summary>
        ///     Собирает из указанного представления чарта в виде <see cref="IChart"/> экземпляр <see cref="BrickDataSet"/>
        /// </summary>
        /// <param name="chart">Исходное представление чарта</param>
        /// <returns>Заполненное представление датасета в виде <see cref="BrickDataSet"/></returns>
        public static BrickDataSet ToBrickDataset(this IChart chart) {
            var ds = new BrickDataSet();
            var seria = 0;
            chart.Datasets.Children.ForEach(_ => {
                seria++;
                _.Children.DoForEach(__ => ds.Add(seria, 0, __.GetValue()));
            });
            return ds;
        }
        /// <summary>
        ///     Преобразует данные из <see cref="BrickDataSet"/> в <see cref="IChart"/>
        /// </summary>
        /// <param name="brickDataSet">Исходный датасет в виде <see cref="BrickDataSet"/></param>
        /// <returns>Эквивалентный экземпляр <see cref="IChart"/></returns>
        public static IChart ToChart(this BrickDataSet brickDataSet) {
            var chart = new Chart();
            brickDataSet.GetSeries().DoForEach(_ => chart.Add(new ChartDataset(_.Select(__ => new ChartSet().SetValue(__.Value).SetLabelPosition(__.LabelPosition)))));
            for (var i = 0; i < chart.Datasets.Children.Select(_ => _.Children.Count()).Max(); i++) {
                chart.Add(new ChartCategory().SetLabelValue(""));
            }
            return chart;
        }
    }
}
