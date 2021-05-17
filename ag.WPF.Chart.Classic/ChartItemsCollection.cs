using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ag.WPF.Chart
{
    /// <summary>
    /// Represents a dynamic data collection that provides notifications when items get
    /// added, removed, or when the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class ChartItemsCollection<T> : INotifyCollectionChanged, IList<T>
    {
        private ObservableCollection<T> _innerCollection;
        private bool _useSource;

        /// <inheritdoc />
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        internal ChartItemsCollection()
        {
            if (_innerCollection != null)
                _innerCollection.CollectionChanged -= _innerCollection_CollectionChanged;
            _useSource = false;
            _innerCollection = new ObservableCollection<T>();
            _innerCollection.CollectionChanged += _innerCollection_CollectionChanged;
        }

        internal ChartItemsCollection(IEnumerable<T> source)
        {
            if (_innerCollection != null)
                _innerCollection.CollectionChanged -= _innerCollection_CollectionChanged;
            _useSource = true;
            _innerCollection = new ObservableCollection<T>(source);
            _innerCollection.CollectionChanged += _innerCollection_CollectionChanged;
        }

        private void _innerCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_useSource) return;
            CollectionChanged?.Invoke(sender, e);
        }

        /// <inheritdoc />
        public int Count => _innerCollection.Count;

        /// <inheritdoc />
        public bool IsReadOnly => ((ICollection<T>)_innerCollection).IsReadOnly;

        /// <inheritdoc />
        public T this[int index] { get => ((IList<T>)_innerCollection)[index]; set => ((IList<T>)_innerCollection)[index] = value; }

        /// <inheritdoc />
        public void Add(T item)
        {
            if (_useSource) return;
            _innerCollection.Add(item);
        }

        /// <inheritdoc />
        public bool Remove(T item)
        {
            if (_useSource) return false;
            return _innerCollection.Remove(item);
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            if (_useSource) return;
            _innerCollection.RemoveAt(index);
        }

        /// <summary>
        /// Moves the item at the specified index to a new location in the collection.
        /// </summary>
        /// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
        /// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
        public void Move(int oldIndex, int newIndex)
        {
            if (_useSource) return;
            _innerCollection.Move(oldIndex, newIndex); ;
        }

        /// <inheritdoc />
        public void Clear()
        {
            if (_useSource) return;
            _innerCollection.Clear();
        }

        internal void SetSource(IEnumerable<T> source)
        {
            _useSource = true;
            _innerCollection.Clear();
        }

        internal void ClearSource()
        {
            _useSource = false;
            _innerCollection = new ObservableCollection<T>();
        }

        /// <inheritdoc />
        public int IndexOf(T item)
        {
            return _innerCollection.IndexOf(item);
        }

        /// <inheritdoc />
        public void Insert(int index, T item)
        {
            _innerCollection.Insert(index, item);
        }

        /// <inheritdoc />
        public bool Contains(T item)
        {
            return _innerCollection.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex)
        {
            _innerCollection.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_innerCollection).GetEnumerator();
        }
    }
}
