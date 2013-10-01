using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Charts {
    /// <summary>
    ///     Представление категории
    /// </summary>
    public class ChartCategory : ChartElement {
        /// <summary>
        ///     Представление категории
        /// </summary>
        public ChartCategory() {
            SetName(ChartDefaults.CategoryElementName);
        }
        /// <summary>
        ///     Добавление метки
        /// </summary>
        /// <param name="value">Значение метки</param>
        /// <returns>Представление элемента</returns>
        public IChartElement AddLabel(string value) {
            var element = new ChartElement();

            element.SetName(ChartDefaults.CategoryValueName);
            element.AddAttribute(new ChartAttribute {
                Name = ChartDefaults.CategoryLabelAttributeName,
                ParentElement = element,
                Value = value
            });

            return AddChild(element);
        }
        /// <summary>
        ///     Возвращает метки
        /// </summary>
        /// <returns>Перечисление меток</returns>
        public IEnumerable<string> GetLabels() {
            foreach (var child in Childs) {
                if (child.Name != ChartDefaults.CategoryValueName) {
                    continue;
                }

                yield return child.Attributes.Where(
                    _ => _.Name == ChartDefaults.CategoryLabelAttributeName
                ).Select(
                    _ => _.Value
                ).First();
            }
        }
    }
}
