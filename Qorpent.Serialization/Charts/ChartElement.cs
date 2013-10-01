using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Qorpent.Config;

namespace Qorpent.Charts {
    /// <summary>
    ///     Элемент чарта
    /// </summary>
    public class ChartElement : ConfigBase, IChartElement {
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public IChartElement Parent {
            get { return Get<IChartElement>(ChartDefaults.ChartElementParentProperty, this); }
            private set { Set(ChartDefaults.ChartElementParentProperty, value); }
        }
        /// <summary>
        ///     Имя элемента
        /// </summary>
        public string Name {
            get { return Get(ChartDefaults.ChartElementNameProperty, "ChartElement"); }
            private set { Set(ChartDefaults.ChartElementNameProperty, value); }
        }
        /// <summary>
        ///     Атрибуты
        /// </summary>
        public IEnumerable<IChartAttribute> Attributes {
            get { return Get<IEnumerable<IChartAttribute>>(ChartDefaults.ChartElementChilds, new List<IChartAttribute>()); }
        }
        /// <summary>
        ///     Дочерние элементы
        /// </summary>
        public IEnumerable<IChartElement> Childs {
            get { return Get<IEnumerable<IChartElement>>(ChartDefaults.ChartElementChilds, new List<IChartElement>()); }
        }
        /// <summary>
        ///     Устанавливает родительский элемент
        /// </summary>
        /// <param name="parent">Родительский элемент</param>
        public void SetParent(IChartElement parent) {
            Parent = parent;
        }
        /// <summary>
        ///     Устанавливает имя элемента
        /// </summary>
        /// <param name="name">Имя элемента</param>
        public void SetName(string name) {
            Name = name;
        }
        /// <summary>
        ///     Добавляет атрибут во внутреннюю коллекцию
        /// </summary>
        /// <param name="chartAttribute">Представление атрибута</param>
        public IChartAttribute AddAttribute(IChartAttribute chartAttribute) {
            if (!HasAttribute(chartAttribute.Name)) {
                ((List<IChartAttribute>)Attributes).Add(chartAttribute);
            }

            return chartAttribute;
        }
        /// <summary>
        ///     Добавляет атрибут во внутреннюю коллекцию
        /// </summary>
        /// <param name="name">Имя атрибута</param>
        /// <param name="value">Значение атрибута</param>
        public IChartAttribute AddAttribute(string name, string value) {
            var attribute = new ChartAttribute {
                Name = name,
                ParentElement = this,
                Value = value
            };

            AddAttribute(attribute);

            return attribute;
        }
        /// <summary>
        ///     Добавляет дочерний элемент во внутреннюю коллекцию
        /// </summary>
        /// <param name="chartElement">Представление элемента</param>
        public IChartElement AddChild(IChartElement chartElement) {
            chartElement.SetParent(this);
            ((List<IChartElement>)Childs).Add(chartElement);
            return chartElement;
        }
        /// <summary>
        ///     Добавляет дочерний элемент во внутреннюю коллекцию
        /// </summary>
        /// <param name="name">Имя элемента</param>
        public IChartElement AddChild(string name) {
            var chartElement = new ChartElement {
                Name = name
            };

            chartElement.SetParent(this);
            ((List<IChartElement>)Childs).Add(chartElement);

            return chartElement;
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
        /// <summary>
        ///     Разрисовка структуры
        /// </summary>
        /// <returns>XML-представление элемента</returns>
        public XElement ToXml() {
            var xml = new XElement(Name);

            foreach (var chartAttribute in Attributes) {
                xml.SetAttributeValue(chartAttribute.Name, chartAttribute.Value);
            }

            foreach (var chartElement in Childs) {
                xml.Add(((ChartElement)chartElement).ToXml());
            }

            return xml;
        }
    }
}
