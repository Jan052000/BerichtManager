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
		public static bool KeyValuePairsEqualNoSequence<K, V>(this Dictionary<K, V?>? dict, Dictionary<K, V?>? other) where V : IEquatable<V> where K : notnull
		{
			if (dict == other)
				return true;
			if (dict == null || other == null || dict.Count != other.Count)
				return false;
			foreach (KeyValuePair<K, V?> pair in dict)
			{
				if (!other.TryGetValue(pair.Key, out V? value) || !EqualityComparer<V>.Default.Equals(pair.Value, value))
					return false;
			}
			return true;
		}

		/// <summary>
		/// Tries to overwrite a the value at <paramref name="key"/> with <paramref name="value"/>
		/// </summary>
		/// <typeparam name="K"><see cref="Type"/> of the key</typeparam>
		/// <typeparam name="V"><see cref="Type"/> of the value</typeparam>
		/// <param name="dict"><see cref="Dictionary{TKey, TValue}"/> to overwrite in</param>
		/// <param name="key">Key to overwrite value of</param>
		/// <param name="value">Value to overwrite current value with</param>
		/// <returns><see langword="true"/> if the item was</returns>
		public static void AddOrOverwrite<K, V>(this Dictionary<K, V?> dict, K key, V? value) where V : class where K : notnull
		{
			if (!dict.TryAdd(key, value))
				dict[key] = value;
		}
	}
}
