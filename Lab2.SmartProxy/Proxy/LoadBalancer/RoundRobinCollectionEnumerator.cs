using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab2.SmartProxy.Proxy.LoadBalancer {
    public class RoundRobinCollectionEnumerator<T> {
        private readonly object _lock = new object();
        private readonly T[]    _array;

        private int _index = 0;

        public RoundRobinCollectionEnumerator(IReadOnlyCollection<T> collection) {
            if (collection.Count == 0)
                throw new ArgumentException("Collection should not be empty");

            _array = collection.ToArray();
        }

        public T GetNext() {
            T item;
            lock (_lock) {
                item   = _array[_index];
                _index = (_index + 1) % _array.Length;
            }

            return item;
        }
    }
}
