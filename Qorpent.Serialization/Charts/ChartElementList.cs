using System;
using System.Collections.Generic;

namespace Qorpent.Charts {
    /// <summary>
    /// Заготовка под перечислимый элемент чарта
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ChartElementList<T> : ChartElement, IChartElementList<T> where T  :IChartElement

    {
        /// <summary>
        /// Реальный хранимый список
        /// </summary>
        protected IList<T> RealList = new List<T>();
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<T> GetEnumerator() {
            return RealList.GetEnumerator();
        }
        /// <summary>
        /// 
        /// </summary>
        public IList<T> AsList {
            get { return RealList; }
        } 

       
    }
}