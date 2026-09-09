using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Utility class for handling enums as options and flags.
/// </summary>
public static class EnumUtil
{
	public static int Count<T>() where T : struct
	{
		CheckType<T>("Count");

		return System.Enum.GetValues(typeof(T)).Length;
	}

	/// <summary>
	/// Iterates over all individual enum values of the specified enum type.
	/// </summary>
	/// <typeparam name="T">The type of enum to iterate over.</typeparam>
	/// <returns></returns>
	public static IEnumerable<T> GetValues<T>() where T : struct
	{
		CheckType<T>("GetValues");

		foreach (var t in System.Enum.GetValues(typeof(T)))
			yield return (T)(object)t;
	}

	/// <summary>
	/// Determines whether the specified enum mask is exactly equal to each of the other specified enum masks.
	/// 
	/// Will throw a TypeArgumentException if the specified masks are not an enum type.
	/// </summary>
	/// <typeparam name="T">The type of the enum masks.</typeparam>
	/// <param name="mask">An enum mask to compare.</param>
	/// <param name="masks">Additional enum masks to compare.</param>
	/// <returns>True if all the specified enum masks have exactly the same enum flags, otherwise false.</returns>
	public static bool IsFlagsEqual<T>(T mask, params T[] masks) where T : struct
	{
		CheckType<T>("IsFlagsEqual");

		foreach (var m in masks)
			if (!(HasFlags(mask, m) && HasFlags(m, mask)))
				return false;

		return true;
	}
	/// <summary>
	/// Determines whether the specified enum mask contains all the specified enum flags.
	/// 
	/// Will throw a TypeArgumentException if the specified mask is not an enum type.
	/// </summary>
	/// <typeparam name="T">The type of the enum mask.</typeparam>
	/// <param name="mask">The enum mask to test.</param>
	/// <param name="flags">The enum flags (or enum masks) to check for in the enum mask.</param>
	/// <returns>True if the specified enum mask contains all the enum flags that are active in at least one of the specified enum flags.</returns>
	public static bool HasFlags<T>(T mask, params T[] flags) where T : struct
	{
		CheckType<T>("HasFlags");

		foreach (var t in flags)
			foreach (var flag in GetActiveFlags(t))
				if (((int)(object)flag & (int)(object)mask) == 0)
					return false;

		return true;
	}
	/// <summary>
	/// Iterates over all individual enum flags contained in the specified enum mask.
	/// 
	/// Will throw a TypeArgumentException if the specified masks are not an enum type.
	/// </summary>
	/// <typeparam name="T">The type of the enum mask.</typeparam>
	/// <param name="mask">The enum mask to explode.</param>
	/// <returns>Each enum flag that is active in the specified mask.</returns>
	public static IEnumerable<T> GetActiveFlags<T>(T mask) where T : struct
	{
		CheckType<T>("GetActiveFlags");

		foreach (var t in GetValues<T>())
			if (((int)(object)mask & (int)(object)t) != 0)
				yield return t;
	}

	/// <summary>
	/// Generates the union of the specified masks.
	/// 
	/// Will throw a TypeArgumentException if the specified masks are not an enum type.
	/// </summary>
	/// <typeparam name="T">The type of the enum masks.</typeparam>
	/// <param name="mask">An enum mask to form a union with.</param>
	/// <param name="masks">Additional enum masks to form a union with.</param>
	/// <returns>An enum mask containing only the enum flags that appear in at least one of the specified masks.</returns>
	public static T FlagsUnion<T>(T mask, params T[] masks) where T : struct
	{
		CheckType<T>("FlagsUnion");

		foreach (var m in masks)
			mask = (T)(object)(((int)(object)mask | (int)(object)m));

		return mask;
	}
	/// <summary>
	/// Generates the intersection of the specified enum masks.
	/// 
	/// Will throw a TypeArgumentException if the specified masks are not an enum type.
	/// </summary>
	/// <typeparam name="T">The type of the enum masks.</typeparam>
	/// <param name="mask">An enum mask to form an intersection with.</param>
	/// <param name="masks">Additional enum masks to form an intersection with.</param>
	/// <returns>An enum mask containing only the enum flags common to all the specified masks.</returns>
	public static T FlagsIntersection<T>(T mask, params T[] masks) where T : struct
	{
		CheckType<T>("FlagsIntersection");

		foreach (var m in masks)
			mask = (T)(object)(((int)(object)mask & (int)(object)m));

		return mask;
	}
	/// <summary>
	/// Generates the inversion of the specified enum mask.
	/// 
	/// Will throw a TypeArgumentException if the specified mask is not an enum type.
	/// </summary>
	/// <typeparam name="T">The type of the enum mask.</typeparam>
	/// <param name="mask">The existing mask to invert.</param>
	/// <returns>An enum mask containing only the enum flags not active in the specified mask.</returns>
	public static T FlagsInversion<T>(T mask) where T : struct
	{
		CheckType<T>("FlagsInversion");

		return (T)(object)(~(int)(object)mask);
	}

	/// <summary>
	/// Throws a TypeArgumentException if the specified type parameter is not an enum.
	/// </summary>
	/// <typeparam name="T">The type to check is an enum.</typeparam>
	/// <param name="context">The function that required the check. Used to output a friendly error message.</param>
	static void CheckType<T>(string context) where T : struct
	{
		if (!typeof(T).IsEnum)
			throw new TypeArgumentException("EnumUtil." + context + " is only usable with enum types. " + typeof(T).ToString() + " is not an enum type.");
	}
}