using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Better.Interactor.Runtime.Models
{
    public class StackDictionary<TKey, TValue> : IEnumerable<TValue> where TKey : class
    {
        private Stack<TKey> _keyStack;
        private readonly Dictionary<TKey, TValue> _dictionary;

        public StackDictionary()
        {
            _keyStack = new Stack<TKey>();
            _dictionary = new Dictionary<TKey, TValue>();
        }

        public int Count => _keyStack.Count;

        public void Push(TKey key, TValue value)
        {
            _keyStack.Push(key);
            _dictionary[key] = value;
        }

        public TValue Pop()
        {
            if (_keyStack.Count == 0)
            {
                throw new InvalidOperationException("The StackDictionary is empty.");
            }

            var key = _keyStack.Pop();
            var value = _dictionary[key];
            _dictionary.Remove(key);

            return value;
        }

        public bool Remove(TKey key)
        {
            if (_dictionary.ContainsKey(key))
            {
                _keyStack = new Stack<TKey>(_keyStack.Where(k => !ReferenceEquals(k, key)));
                _dictionary.Remove(key);
                return true;
            }

            return false;
        }

        public TValue Peek()
        {
            if (_keyStack.Count == 0)
            {
                throw new InvalidOperationException("The StackDictionary is empty.");
            }

            var key = _keyStack.Peek();
            return _dictionary[key];
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public TValue this[TKey key]
        {
            get => _dictionary[key];
            set => _dictionary[key] = value;
        }

        public IEnumerator<TValue> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}