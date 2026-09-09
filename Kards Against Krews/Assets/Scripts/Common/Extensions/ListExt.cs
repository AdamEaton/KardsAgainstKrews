using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for List.
/// </summary>
public static class ListExt
{
	/// <summary>
	/// Performs a shallow copy of this List.
	/// </summary>
	/// <typeparam name="T">The type of the List.</typeparam>
	/// <param name="list">The List to copy.</param>
	/// <returns>A new List with the same elements as this List.</returns>
	public static List<T> Clone<T>(this List<T> list)
	{
		List<T> output = new List<T>();
		output.AddRange(list);
		return output;
	}
}