using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HassClient.Core.Models.RegistryEntries.Modifiable
{
    /// <summary>
    /// Represents an observable set that can track pending changes.
    /// </summary>
    /// <typeparam name="T">The collection elements type.</typeparam>
    internal class ObservableHashSet<T> : ObservableCollection<T>
    {
        private readonly HashSet<T> _addedValues = new();

        private readonly HashSet<T> _removedValues = new();

        private readonly Action<T> _validationCallback;

        public bool HasPendingChanges => _addedValues.Count > 0 || _removedValues.Count > 0;

        public ObservableHashSet(Action<T> validationCallback)
        {
            _validationCallback = validationCallback;
        }

        /// <inheritdoc />
        protected override void InsertItem(int index, T item)
        {
            if (Contains(item))
            {
                return;
            }

            _validationCallback(item);
            if (!_removedValues.Remove(item))
            {
                _addedValues.Add(item);
            }

            base.InsertItem(index, item);
        }

        /// <inheritdoc />
        protected override void RemoveItem(int index)
        {
            var item = this[index];

            if (!_addedValues.Remove(item))
            {
                _removedValues.Add(item);
            }

            base.RemoveItem(index);
        }

        /// <inheritdoc />
        protected override void ClearItems()
        {
            foreach (var item in this.Except(_addedValues))
            {
                _removedValues.Add(item);
            }

            _addedValues.Clear();

            base.ClearItems();
        }

        /// <inheritdoc />
        protected override void SetItem(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        protected override void MoveItem(int oldIndex, int newIndex)
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            _addedValues.Clear();
            _removedValues.Clear();
        }

        public void DiscardPendingChanges()
        {
            foreach (var item in _removedValues)
            {
                base.InsertItem(Count, item);
            }

            foreach (var index in _addedValues.Select(IndexOf))
            {
                base.RemoveItem(index);
            }

            _addedValues.Clear();
            _removedValues.Clear();
        }
    }
}
