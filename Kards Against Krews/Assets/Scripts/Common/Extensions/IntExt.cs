using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for int.
/// </summary>
public static class IntExt
{
	/// <summary>
	/// Determines the next number the specified increment after n, ascending in a modular cycle of the specified size.
	/// </summary>
	/// <param name="n">The integer to increment.</param>
	/// <param name="cycleSize">The number to modularly divide by.</param>
	/// <param name="increment">The amount to increment by.</param>
	/// <returns>The smallest nonnegative integer that is n + increment more than a multiple of cycleSize.</returns>
	public static int CyclicalNext(this int n, int cycleSize, int increment = 1)
	{
		if (cycleSize <= 0)
			throw new System.ArgumentException("Cycle size cannot be less than 1!");

		n += increment;
		while (n >= cycleSize)
			n -= cycleSize;
		while (n < 0)
			n += cycleSize;
		return n;
	}
	/// <summary>
	/// Determines the previous number the specified decrement before n, descending in a modular cycle of the specified size.
	/// </summary>
	/// <param name="n">The integer to decrement.</param>
	/// <param name="cycleSize">The number to modularly divide by.</param>
	/// <param name="decrement">The amount to decrement by.</param>
	/// <returns>The smallest nonnegative integer that is n - decrement more than a multiple of cycleSize.</returns>
	public static int CyclicalPrevious(this int n, int cycleSize, int decrement = 1)
	{
		if (cycleSize <= 0)
			throw new System.ArgumentException("Cycle size cannot be less than 1!");

		return n.CyclicalNext(cycleSize, -decrement);
	}
	/// <summary>
	/// Yields all non-negative integers less than n.
	/// </summary>
	/// <param name="n">The number to iterate up to.</param>
	/// <returns>The sequence of numbers starting at 0 that are less than n.</returns>
	public static IEnumerable<int> Enumerate(this int n)
	{
		for (int i = 0; i < n; i++)
			yield return i;
	}
}