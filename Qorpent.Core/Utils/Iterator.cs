using System;
using System.Collections.Generic;

namespace Qorpent.Utils {
    /// <summary>
    ///     Итератор для каждого из <see cref="Masters"/> каждый из <see cref="Slaves"/>
    /// </summary>
    /// <typeparam name="TMaster">Типизация ведущей коллекции</typeparam>
    /// <typeparam name="TSlave">Типизация ведомой коллекции</typeparam>
    public class Iterator<TMaster, TSlave> {
        /// <summary>
        ///     Мастер коллекция
        /// </summary>
        public IEnumerable<TMaster> Masters { get; private set; }
        /// <summary>
        ///     Ведомая коллекция
        /// </summary>
        public IEnumerable<TSlave> Slaves { get; private set; }
        /// <summary>
        ///     Инициализация итератора коллекциями
        /// </summary>
        /// <param name="masters">Мастер коллекция</param>
        /// <param name="slaves">Ведомая коллекция</param>
        /// <returns>Замыкание на текущий экземпляр <see cref="Iterator{TMaster,TSlave}"/></returns>
        public Iterator<TMaster, TSlave> Initialize(IEnumerable<TMaster> masters, IEnumerable<TSlave> slaves) {
            Masters = masters;
            Slaves = slaves;
            return this;
        }
        /// <summary>
        ///     Последовательно выполнение итерации для каждого из <see cref="Masters"/> применить каждый из <see cref="Slaves"/>
        /// </summary>
        /// <param name="action">Действие</param>
        public void Iterate(Action<TMaster, TSlave> action) {
            foreach (var master in Masters) {
                foreach (var slave in Slaves) {
                    action(master, slave);
                }
            }
        }
    }
}
