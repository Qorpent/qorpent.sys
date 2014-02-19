using System.Collections;
using System.Collections.Generic;

namespace Qorpent.Config {
    /// <summary>
    ///     Абстрактный класс для организации примитивной односторонней иерархии
    /// </summary>
    /// <typeparam name="T">Типизация элементов иерархии</typeparam>
    public abstract class HierarchyElement<T> : IEnumerable<HierarchyElement<T>> where T : HierarchyElement<T> {
        /// <summary>
        ///     Внутренний список дочерних элементов
        /// </summary>
        private List<HierarchyElement<T>> _childs;
        /// <summary>
        ///     Список дочерних элементов
        /// </summary>
        private List<HierarchyElement<T>> Childs {
            get { return _childs ?? (_childs = new List<HierarchyElement<T>>()); }
        }
        /// <summary>
        ///     Родительский элемент
        /// </summary>
        public HierarchyElement<T> Parent { get; set; }
        /// <summary>
        ///     Признак того, что это родительский элемент
        /// </summary>
        public bool IsRoot { get; set; }
        /// <summary>
        ///     Получение родительского элемента
        /// </summary>
        /// <returns>Типизированный элемент</returns>
        public T GetParent() {
            return Parent as T;
        }
        /// <summary>
        ///     Добавление дочернего элемента
        /// </summary>
        /// <param name="child">Дочерний элемент</param>
        /// <returns>Замыкание на текущий экземпляр <see cref="HierarchyElement{T}"/></returns>
        public HierarchyElement<T> AddChild(HierarchyElement<T> child) {
            lock (Childs) {
                Childs.Add(child);
                child.Parent = this;
            }
            return this;
        }
        /// <summary>
        ///     Получение <see cref="IEnumerable"/> по <see cref="HierarchyElement{T}"/>
        /// </summary>
        /// <returns><see cref="IEnumerable"/> по <see cref="HierarchyElement{T}"/></returns>
        public IEnumerator<HierarchyElement<T>> GetEnumerator() {
            return Childs.GetEnumerator();
        }
        /// <summary>
        ///     Получение <see cref="IEnumerable"/> по <see cref="HierarchyElement{T}"/>
        /// </summary>
        /// <returns><see cref="IEnumerable"/> по <see cref="HierarchyElement{T}"/></returns>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
