using System.Collections.Generic;
using System.Linq;

namespace Qorpent.Charts.Implementation {
    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public class ChartElement : IChartElement {
        /// <summary>
        ///     Внутренний список атрибутов
        /// </summary>
        private readonly IList<IChartAttribute> _attributes;
        /// <summary>
        ///     Внутренний список дочерних элементов
        /// </summary>
        private readonly IList<IChartElement> _childs;
        /// <summary>
        ///     Имя элемента
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     Атрибуты
        /// </summary>
        public IEnumerable<IChartAttribute> Attributes {
            get { return _attributes; }
        }
        /// <summary>
        ///     Дочерние элементы
        /// </summary>
        public IEnumerable<IChartElement> Childs {
            get { return _childs; }
        }
        /// <summary>
        ///     Элемент чарта
        /// </summary>
        public ChartElement() {
            _attributes = new List<IChartAttribute>();
            _childs = new List<IChartElement>();
        }
        /// <summary>
        ///     Добавляет атрибут во внутреннюю коллекцию
        /// </summary>
        /// <param name="chartAttribute">Представление атрибута</param>
        public void AddAttribute(IChartAttribute chartAttribute) {
            if (!HasAttribute(chartAttribute.Name)) {
                _attributes.Add(chartAttribute);
            }
        }
        /// <summary>
        ///     Добавляет атрибут во внутреннюю коллекцию
        /// </summary>
        /// <param name="name">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        public void AddAttribute(string name, string value) {
            AddAttribute(new ChartAttribute {
                Name = name,
                ParentElement = this,
                Value = value
            });
        }
        /// <summary>
        ///     Добавляет дочерний элемент во внутреннюю коллекцию
        /// </summary>
        /// <param name="chartElement">Представление элемента</param>
        public void AddChild(IChartElement chartElement) {
            _childs.Add(chartElement);
        }
        /// <summary>
        ///     Проверяет наличие атрибута
        /// </summary>
        /// <param name="name">Имя атрибута</param>
        /// <returns></returns>
        public bool HasAttribute(string name) {
            return Attributes.Any(_ => _.Name == name);
        }
        /// <summary>
        ///     Возвращает значение атрибута по имени
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Значение атрибута или null</returns>
        public string GetAttributeValue(string name) {
            if (!HasAttribute(name)) {
                return null;
            }

            return GetAttribute(name).Value;
        }
        /// <summary>
        ///     Возвращает представление атрибута по его имени
        /// </summary>
        /// <param name="name">Имя атрибута</param>
        /// <returns>Представление атрибута</returns>
        public IChartAttribute GetAttribute(string name) {
            var attrs = Attributes.Where(_ => _.Name == name);

            if (!attrs.Any()) {
                return null;
            }

            return attrs.FirstOrDefault();
        }
    }
}
