using System;
using System.Collections.Generic;

namespace BerichtManager.Extensions
{
	/// <summary>
	/// Holds extension methods for <see cref="Dictionary{TKey, TValue}"/>
	/// </summary>
	public static class DictionaryExtensions
	{
		/// <summary>
		/// Compares keys and values of <paramref name="dict"/> and <paramref name="other"/> and checks if the <see cref="KeyValuePair{TKey, TValue}"/>s are contained in both <see cref="Dictionary{TKey, TValue}"/>s
		/// </summary>
		/// <typeparam name="K"><see cref="Type"/> of the key</typeparam>
		/// <typeparam name="V"><see cref="Type"/> of the value</typeparam>
		/// <param name="dict">First <see cref="Dictionary{TKey, TValue}"/> to compare</param>
		/// <param name="other">Second <see cref="Dictionary{TKey, TValue}"/> to compare</param>
		/// <returns><see langword="true"/> if both <see cref="Dictionary{TKey, TValue}"/>s have the same collection of <see cref="KeyValuePair{TKey, TValue}"/>s even if out of order and <see langword="false"/> otherwise</returns>
		public static bool KeyValuePairsEqualNoSequence<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> other) where V : IEquatable<V>
		{
			if (dict == other)
				return true;
			if (dict == null || other == null || dict.Count != other.Count)
				return false;
			foreach (KeyValuePair<K, V> pair in dict)
			{
				if (!other.TryGetValue(pair.Key, out V value) || !EqualityComparer<V>.Default.Equals(pair.Value, value))
					return false;
			}
			return true;
		}
	}
}
