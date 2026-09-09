using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for Array.
/// </summary>
public static class ArrayExt
{
	/// <summary>
	/// Returns a copy of this Array with the first n elements removed.
	/// </summary>
	/// <typeparam name="T">The type of the Array.</typeparam>
	/// <param name="array">The Array to truncate.</param>
	/// <param name="n">The number of elements to skip.</param>
	/// <returns>A new Array, whose length is n less than this Array, with the first n elements omitted (or null, if this Array is not longer than n).</returns>
	public static T[] Skip<T>(this T[] array, int n)
	{
		if (n >= array.Length)
			return null;

		T[] output = new T[array.Length - n];
		for (int i = 0; i < output.Length; i++)
			output[i] = array[i + n];
		return output;
	}
	/// <summary>
	/// Returns a copy of this Array with the last n elements removed.
	/// </summary>
	/// <typeparam name="T">The type of the Array.</typeparam>
	/// <param name="array">The Array to truncate.</param>
	/// <param name="n">The number of elements to crop.</param>
	/// <returns>A new array, whose length is n less than this Array, with the last n elements omitted (or null, if this Array is not longer than n).</returns>
	public static T[] Crop<T>(this T[] array, int n)
	{
		if (n >= array.Length)
			return null;

		T[] output = new T[array.Length - n];
		for (int i = 0; i < output.Length; i++)
			output[i] = array[i];
		return output;
	}
	/// <summary>
	/// Returns a new array with the specified elements appended.
	/// </summary>
	/// <typeparam name="T">The type of the Array.</typeparam>
	/// <param name="array">The array to append to.</param>
	/// <param name="elements">The elements to append.</param>
	/// <returns>A new Array, containing</returns>
	public static T[] Push<T>(this T[] array, params T[] elements)
	{
		T[] output = new T[array.Length + elements.Length];
		for (int i = 0; i < array.Length; i++)
			output[i] = array[i];
		for (int i = 0; i < elements.Length; i++)
			output[i + array.Length] = elements[i];
		return output;
	}
}