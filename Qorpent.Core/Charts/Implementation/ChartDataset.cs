using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Qorpent.Charts.Implementation {
    /// <summary>
    ///     Представление датасета
    /// </summary>
    public class ChartDataset : ChartElement {
        /// <summary>
        ///     Представление датасета
        /// </summary>
        public ChartDataset() {
            Name = ChartDefaults.DatasetElementName;
        }
        /// <summary>
        ///     Добавление значения в датасет
        /// </summary>
        /// <returns>Элемент, при помощи которого было представлено значение</returns>
        public IChartElement AddValue(int value) {
            var element = new ChartElement {
                Name = ChartDefaults.DatasetValueName
            };

            element.AddAttribute(new ChartAttribute {
                Name = "value",
                ParentElement = element,
                Value = value.ToString(CultureInfo.InvariantCulture)
            });


            AddChild(element);

            return element;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<int> GetValues() {
            foreach (var child in Childs) {
                if (child.Name != ChartDefaults.DatasetValueName) {
                    continue;
                }

                yield return Convert.ToInt32(child.Attributes.Where(_ => _.Name == "value").Select(_ => _.Value).First());
            }
        }
    }
}
