using System;
using System.Collections;
using System.Collections.Generic;

namespace Simulation
{
	/// <summary>
	/// TODO: 
	/// </summary>
	public interface IDataResolver<in TKey, TValue>
	{
		bool Contains(TKey key);
		TValue Get(TKey key);
		bool TryGetValue(TKey key, out TValue value);

		void Add(TKey key, TValue value);
		void Set(TKey key, TValue value);
		void Remove(TKey key);
	}
	public class DataResolver<TKey, TValue> : IDataResolver<TKey, TValue>, IEnumerable<TKey>
	{
		private readonly IList<TKey> _keyList;
		private readonly IList<TValue> _valueList;

		private readonly IDictionary<TKey, int> _cache;
		public DataResolver(int capacity = 64)
		{
			_keyList = new List<TKey>(capacity);
			_valueList = new List<TValue>(capacity);
			_cache = new Dictionary<TKey, int>();
		}

		public bool Contains(TKey key)
		{
			if (GetIndexFromCache(key) >= 0)
			{
				return true;
			}
			return FindIndex(key) >= 0;
		}

		public void Add(TKey key, TValue value)
		{
			var index = GetIndexFromCache(key);
			if (index >= 0)
			{
				throw new Exception($"An item with the given key {key} is already in the list");
			}
			AddInternal(key, value);
		}

		public void Set(TKey key, TValue value)
		{
			var index = GetIndexFromCache(key);
			if (index >= 0)
			{
				_valueList[index] = value;
				return;
			}
			AddInternal(key, value);
		}
		private void AddInternal(TKey key, TValue value)
		{
			// use _keyList.Count before adding the new key and value
			_cache[key] = _keyList.Count;

			_keyList.Add(key);
			_valueList.Add(value);
		}

		public void Remove(TKey key)
		{
			var index = GetIndexFromCache(key);
			if (index < 0)
			{
				return;
			}
			
			_keyList.RemoveAt(index);
			_valueList.RemoveAt(index);
			_cache.Remove(key);
		}

		public TValue Get(TKey key)
		{
			if (!TryGetValue(key, out var value))
			{
				throw new Exception($"An element with key {key} could not be found.");
			}

			return value;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			value = default;

			var index = GetIndexFromCache(key);
			if (index >= 0)
			{
				value = _valueList[index];
				return true;
			}
			// Entry is not (yet) in the cache, check the dataList
			index = FindIndex(key);
			if (index < 0)
			{
				return false;
			}
			_cache.Add(key, index);
			value = _valueList[index];

			return true;
		}

		public IEnumerator<TKey> GetEnumerator()
		{
			return _keyList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		
		private int GetIndexFromCache(TKey key)
		{
			if(!_cache.TryGetValue(key, out var index))
			{
				index = -1;
			}

			return index;
		}

		private int FindIndex(TKey key)
		{
			for (var i = 0; i < _keyList.Count; i++)
			{
				if (key.Equals(_keyList[i]))
				{
					return i;
				}
			}

			return -1;
		}
	}
}