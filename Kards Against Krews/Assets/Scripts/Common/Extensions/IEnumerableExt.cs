using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Extension class for IEnumerable
/// </summary>
public static class IEnumerableExt
{
	/// <summary>
	/// Returns a random element from the IEnumerable.
	/// </summary>
	/// <typeparam name="T">The type of the IEnumerable.</typeparam>
	/// <param name="t">The IEnumerable in which to find the element.</param>
	/// <returns>Any one random element contained by the IEnumerable.</returns>
	public static T RandomElement<T>(this IEnumerable<T> t)
	{
		return t.ElementAt(Random.Range(0, t.Count()));
	}
	public static T RandomElementOrDefault<T>(this IEnumerable<T> t)
	{
		try { return RandomElement(t); }
		catch (System.Exception) { return default(T); }
	}

	public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> t)
	{
		T[] data = t.ToArray();
		int index;
		T swap;

		for (int i = 0; i < data.Length; i++)
		{
			index = Random.Range(i, data.Length);
			swap = data[index];
			data[index] = data[i];
			data[i] = swap;
		}

		foreach (var output in data)
			yield return output;
	}
}