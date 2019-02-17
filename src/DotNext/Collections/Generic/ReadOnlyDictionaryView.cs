﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DotNext.Collections.Generic
{
    /// <summary>
    /// Represents read-only view of the mutable dictionary.
    /// </summary>
    /// <typeparam name="K">Type of dictionary keys.</typeparam>
    /// <typeparam name="V">Type of dictionary values.</typeparam>
    /// <remarks>
    /// Any changes in the original dictionary are visible from the read-only view.
    /// </remarks>
    public readonly struct ReadOnlyDictionaryView<K, V>: IReadOnlyDictionary<K, V>, IEquatable<ReadOnlyDictionaryView<K, V>>
	{
		private readonly IDictionary<K, V> source;

        /// <summary>
        /// Initializes a new read-only view for the mutable dictionary.
        /// </summary>
        /// <param name="dictionary">A dictionary to wrap.</param>
		public ReadOnlyDictionaryView(IDictionary<K, V> dictionary)
			=> source = dictionary ?? throw new ArgumentNullException(nameof(dictionary));

        /// <summary>
        /// Gets value associated with the key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The value associated with the key.</returns>
		public V this[K key] => source[key];

        /// <summary>
        /// All dictionary keys.
        /// </summary>
		public IEnumerable<K> Keys => source.Keys;

        /// <summary>
        /// All dictionary values.
        /// </summary>
		public IEnumerable<V> Values => source.Values;

        /// <summary>
        /// Count of key/value pairs in the dictionary.
        /// </summary>
		public int Count => source.Count;

        /// <summary>
        /// Determines whether the wrapped dictionary contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns><see langword="true"/> if the key exists in the wrapped dictionary; otherwise, <see langword="false"/>.</returns>
		public bool ContainsKey(K key) => source.ContainsKey(key);

        /// <summary>
        /// Gets enumerator over all key/value pairs in the dictionary.
        /// </summary>
        /// <returns>The enumerator over all key/value pairs in the dictionary.</returns>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
			=> source.GetEnumerator();

        /// <summary>
        /// Returns the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">The value associated with the specified key, if the
        /// key is found; otherwise, the <see langword="default"/> value for the type of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns><see langword="true"/>, if the dictionary contains the specified key; otherwise, <see langword="false"/>.</returns>
		public bool TryGetValue(K key, out V value)
			=> source.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Determines whether the current view and the specified view points
        /// to the same dictionary.
        /// </summary>
        /// <param name="other">Other view to compare.</param>
        /// <returns><see langword="true"/> if the current view points to the same dictionary as other view; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Comparison between two wrapped dictionaries is 
        /// performed using method <see cref="object.ReferenceEquals(object, object)"/>.
        /// </remarks>
        public bool Equals(ReadOnlyDictionaryView<K, V> other) => ReferenceEquals(source, other.source);

        /// <summary>
        /// Returns identity hash code of the wrapped collection.
        /// </summary>
        /// <returns>Identity hash code of the wrapped collection.</returns>
        public override int GetHashCode() => RuntimeHelpers.GetHashCode(source);

        /// <summary>
        /// Determines whether wrapped dictionary and the specified object 
        /// are equal by reference.
        /// </summary>
        /// <param name="other">Other dictionary to compare.</param>
        /// <returns><see langword="true"/>, if wrapped dictionary and the specified object are equal by reference; otherwise, <see lngword="false"/>.</returns>
        public override bool Equals(object other)
			=> other is ReadOnlyDictionaryView<K, V> view ? Equals(view) : Equals(source, other);

        /// <summary>
        /// Determines whether two views point to the same dictionary.
        /// </summary>
        /// <param name="first">The first view to compare.</param>
        /// <param name="second">The second view to compare.</param>
        /// <returns><see langword="true"/> if both views point to the same collection; otherwise, <see langword="false"/>.</returns>
        public static bool operator ==(ReadOnlyDictionaryView<K, V> first, ReadOnlyDictionaryView<K, V> second)
			=> first.Equals(second);

        /// <summary>
        /// Determines whether two views point to the different dictionaries.
        /// </summary>
        /// <param name="first">The first view to compare.</param>
        /// <param name="second">The second view to compare.</param>
        /// <returns><see langword="true"/> if both views point to the different dictionaries; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(ReadOnlyDictionaryView<K, V> first, ReadOnlyDictionaryView<K, V> second)
			=> !first.Equals(second);
	}

    /// <summary>
    /// Represents lazily converted read-only dictionary.
    /// </summary>
    /// <typeparam name="K">Type of dictionary keys.</typeparam>
    /// <typeparam name="I">Type of values in the source dictionary.</typeparam>
    /// <typeparam name="O">Type of values in the converted dictionary.</typeparam>
	public readonly struct ReadOnlyDictionaryView<K, I, O> : IReadOnlyDictionary<K, O>, IEquatable<ReadOnlyDictionaryView<K, I, O>>
	{
		private readonly IReadOnlyDictionary<K, I> source;
		private readonly Converter<I, O> mapper;

        /// <summary>
        /// Initializes a new lazily converted view.
        /// </summary>
        /// <param name="dictionary">Read-only dictionary to convert.</param>
        /// <param name="mapper">Value converter.</param>
        public ReadOnlyDictionaryView(IReadOnlyDictionary<K, I> dictionary, Converter<I, O> mapper)
		{
			source = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
			this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}

        /// <summary>
        /// Gets value associated with the key and convert it.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>The converted value associated with the key.</returns>
		public O this[K key] => mapper(source[key]);

        /// <summary>
        /// All dictionary keys.
        /// </summary>
		public IEnumerable<K> Keys => source.Keys;

        /// <summary>
        /// All converted dictionary values.
        /// </summary>
        public IEnumerable<O> Values => source.Values.Select(mapper.AsFunc());

        /// <summary>
        /// Count of key/value pairs.
        /// </summary>
		public int Count => source.Count;

        /// <summary>
        /// Determines whether the wrapped dictionary contains an element
        /// with the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the dictionary.</param>
        /// <returns><see langword="true"/> if the key exists in the wrapped dictionary; otherwise, <see langword="false"/>.</returns>
        public bool ContainsKey(K key) => source.ContainsKey(key);

        /// <summary>
        /// Returns enumerator over key/value pairs in the wrapped dictionary
        /// and performs conversion for each value in the pair.
        /// </summary>
        /// <returns>The enumerator over key/value pairs.</returns>
		public IEnumerator<KeyValuePair<K, O>> GetEnumerator()
		{
			var mapper = this.mapper;
			return source
				.Select(entry => new KeyValuePair<K, O>(entry.Key, mapper(entry.Value)))
				.GetEnumerator();
		}

        /// <summary>
        /// Returns the converted value associated with the specified key.
        /// </summary>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="value">The converted value associated with the specified key, if the
        /// key is found; otherwise, the <see langword="default"/> value for the type of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns><see langword="true"/>, if the dictionary contains the specified key; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValue(K key, out O value)
		{
			if (source.TryGetValue(key, out var sourceVal))
			{
				value = mapper(sourceVal);
				return true;
			}
			else
			{
				value = default;
				return false;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
			=> GetEnumerator();

        /// <summary>
        /// Determines whether two converted dictionaries are same.
        /// </summary>
        /// <param name="other">Other dictionary to compare.</param>
        /// <returns><see langword="true"/> if this view wraps the same source dictionary and contains the same converter as other view; otherwise, <see langword="false"/>.</returns>
        public bool Equals(ReadOnlyDictionaryView<K, I, O> other)
			=> ReferenceEquals(source, other.source) && Equals(mapper, other.mapper);

        /// <summary>
        /// Returns hash code for the this view.
        /// </summary>
        /// <returns>The hash code of this view.</returns>
        public override int GetHashCode()
			=> source is null || mapper is null ? 0 : RuntimeHelpers.GetHashCode(source) ^ mapper.GetHashCode();

        /// <summary>
        /// Determines whether two converted dictionaries are same.
        /// </summary>
        /// <param name="other">Other dictionary to compare.</param>
        /// <returns><see langword="true"/> if this view wraps the same source dictionary and contains the same converter as other view; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object other)
			=> other is ReadOnlyDictionaryView<K, I, O> view ? Equals(view) : Equals(source, other);

        /// <summary>
        /// Determines whether two views are same.
        /// </summary>
        /// <param name="first">The first dictionary to compare.</param>
        /// <param name="second">The second dictionary to compare.</param>
        /// <returns><see langword="true"/> if the first view wraps the same source dictionary and contains the same converter as the second view; otherwise, <see langword="false"/>.</returns>
		public static bool operator ==(ReadOnlyDictionaryView<K, I, O> first, ReadOnlyDictionaryView<K, I, O> second)
            => first.Equals(second);

        /// <summary>
        /// Determines whether two views are not same.
        /// </summary>
        /// <param name="first">The first dictionary to compare.</param>
        /// <param name="second">The second collection to compare.</param>
        /// <returns><see langword="true"/> if the first view wraps the diferent source dictionary and contains the different converter as the second view; otherwise, <see langword="false"/>.</returns>
        public static bool operator !=(ReadOnlyDictionaryView<K, I, O> first, ReadOnlyDictionaryView<K, I, O> second)
            => !first.Equals(second);
    }
}
