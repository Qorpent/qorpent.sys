using System;
using System.Collections;
using System.Collections.Generic;

namespace Qorpent.Utils.Scaling {
    /// <summary>
    ///     Класс представления трёхмерной таблицы
    /// </summary>
    public class Table<TKey, TSubKey, TValue> : IEnumerable<KeyValuePair<TKey, Table<TSubKey, TValue>>> {
        /// <summary>
        ///     Внутреннее представление таблицы
        /// </summary>
        private readonly Table<TKey, Table<TSubKey, TValue>> _table;
        /// <summary>
        /// 
        /// </summary>
        public Table() {
            _table = new Table<TKey, Table<TSubKey, TValue>>();
        }
        /// <summary>
        ///     Класс представления трёхмерной таблицы
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns>Двухмерная таблица, поставленная в соответствие ключу</returns>
        public Table<TSubKey, TValue> this[TKey key] {
            get { return _table[key]; }
            set { _table[key] = value; }
        }
        /// <summary>
        ///     Вставка секции в таблицу
        /// </summary>
        /// <param name="section"></param>
        public void Add(TKey section) {
            if (ContainsSection(section)) {
                throw new Exception("There is a key ");
            }

            _table.Add(section, new Table<TSubKey, TValue>());
        }
        /// <summary>
        ///     Вставляет секцию с готовым набором данных
        /// </summary>
        /// <param name="section"></param>
        /// <param name="subtable"></param>
        public void Add(TKey section, Table<TSubKey, TValue> subtable) {
            Add(section);
            _table[section] = subtable;
        }
        /// <summary>
        ///     Удаление секции из таблицы
        /// </summary>
        /// <param name="section">Представление секции</param>
        public void Remove(TKey section) {
            if (_table.ContainsKey(section)) {
                _table.Remove(section);
            }
        }
        /// <summary>
        ///     Определяет признак наличия секции в таблице
        /// </summary>
        /// <param name="section">Представление секции</param>
        /// <returns>Признак наличия секции в таблице</returns>
        public bool ContainsSection(TKey section) {
            return _table.ContainsKey(section);
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public IEnumerator<KeyValuePair<TKey, Table<TSubKey, TValue>>> GetEnumerator() {
            return _table.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
    /// <summary>
    ///     Представление плоской таблицы типа ключ:значение с возможностью блокировки
    /// </summary>
    /// <typeparam name="TEndKey">Типизация ключа</typeparam>
    /// <typeparam name="TEndValue">Типизация значения</typeparam>
    public class Table<TEndKey, TEndValue> : IDictionary<TEndKey, TEndValue> {
        /// <summary>
        ///     Внутренний экземпляр таблицы
        /// </summary>
        private readonly IDictionary<TEndKey, TEndValue> _table = new Dictionary<TEndKey, TEndValue>();
        /// <summary>
        ///     Внутренний признак того, что таблица защищена от записи
        /// </summary>
        private bool _isHolden;
        /// <summary>
        ///     Признак того, что таблица защищена от записи
        /// </summary>
        public bool IsHolden {
            get { return _isHolden; }
        }
        /// <summary>
        ///     Защитить таблицу от записи (необратимая операция)
        /// </summary>
        public void Hold() {
            _isHolden = true;
        }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<KeyValuePair<TEndKey, TEndValue>> GetEnumerator() {
            return _table.GetEnumerator();
        }
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public void Add(KeyValuePair<TEndKey, TEndValue> item) {
            if (_isHolden) {
                throw new Exception("Table is hold");
            }

            _table.Add(item);
        }
        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
        public void Clear() {
            if (_isHolden) {
                throw new Exception("Table is hold");
            }

            _table.Clear();
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public bool Contains(KeyValuePair<TEndKey, TEndValue> item) {
            return _table.Contains(item);
        }
        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param><param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param><exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception><exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception><exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.</exception>
        public void CopyTo(KeyValuePair<TEndKey, TEndValue>[] array, int arrayIndex) {
            _table.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
        public bool Remove(KeyValuePair<TEndKey, TEndValue> item) {
            if (!_isHolden) {
                throw new Exception("Table is hold");
            }

            return _table.Remove(item);
        }
        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count {
            get { return _table.Count; }
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly {
            get { return _isHolden && _table.IsReadOnly; }
        }
        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key.
        /// </summary>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the key; otherwise, false.
        /// </returns>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool ContainsKey(TEndKey key) {
            return _table.ContainsKey(key);
        }
        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param><param name="value">The object to use as the value of the element to add.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public void Add(TEndKey key, TEndValue value) {
            if (_isHolden) {
                throw new Exception("Table is hold");
            }

            _table.Add(key, value);
        }
        /// <summary>
        /// Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        /// <param name="key">The key of the element to remove.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public bool Remove(TEndKey key) {
            if (_isHolden) {
                throw new Exception("Table is hold");
            }

            return _table.Remove(key);
        }
        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <returns>
        /// true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <param name="key">The key whose value to get.</param><param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
        public bool TryGetValue(TEndKey key, out TEndValue value) {
            return _table.TryGetValue(key, out value);
        }
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        /// The element with the specified key.
        /// </returns>
        /// <param name="key">The key of the element to get or set.</param><exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception><exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception><exception cref="T:System.NotSupportedException">The property is set and the <see cref="T:System.Collections.Generic.IDictionary`2"/> is read-only.</exception>
        public TEndValue this[TEndKey key] {
            get { return _table[key]; }
            set {
                if (_isHolden) {
                    throw new Exception("Table is hold");
                }

                _table[key] = value;
            }
        }
        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TEndKey> Keys {
            get { return _table.Keys; }
        }
        /// <summary>
        /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the object that implements <see cref="T:System.Collections.Generic.IDictionary`2"/>.
        /// </returns>
        public ICollection<TEndValue> Values {
            get { return _table.Values; }
        }
    }
}