using System.Collections;
using System.Collections.Generic;

namespace Qorpent.Config {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class TreeConfigBaseElementsList<T> : IList<T> where T :TreeConfigBase<T> {
        private T _parent;
        private List<T> _list;

        public TreeConfigBaseElementsList(T parent) {
            this._parent = parent;
            this._list = new List<T>();
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return _list.GetEnumerator();
        }

        void ICollection<T>.Add(T item) {
            if(_list.Contains(item))return;
            Register(item);
            _list.Add(item);
        }

        private void Register(T item) {
            item.ParentElement = this._parent;
        }

        void ICollection<T>.Clear() {
            _list.Clear();
        }

        bool ICollection<T>.Contains(T item) {
            return _list.Contains(item);
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex) {
            _list.CopyTo(array,arrayIndex);
        }

        bool ICollection<T>.Remove(T item) {
            
            var removed = _list.Remove(item);
            if (removed) {
                if (item.ParentElement == _parent) {
                    item.ParentElement = null;
                }
            }
            return removed;
        }

        int ICollection<T>.Count {
            get { return _list.Count; }
          
        }

        bool ICollection<T>.IsReadOnly { get { return false; }}

        int IList<T>.IndexOf(T item) {
            return _list.IndexOf(item);
        }

        void IList<T>.Insert(int index, T item) {
            if(_list.Contains(item))return;
            Register(item);
            _list.Insert(index,item);
        }

        void IList<T>.RemoveAt(int index) {
            _list.RemoveAt(index);
        }

        T IList<T>.this[int index] {
            get { return _list[index]; }
            set {
                if (!_list.Contains(value)) {
                    Register(value);
                    _list[index] = value;
                }
            }
        }
    }
}