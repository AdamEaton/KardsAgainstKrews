using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for string.
/// </summary>
public static class StringExt
{
	/// <summary>
	/// Returns whether this string represents an integer.
	/// </summary>
	/// <param name="s">The string to test.</param>
	/// <returns>True if the string consists of only digits with an optional minus sign, otherwise false.</returns>
	public static bool IsIntegral(this string s)
	{
		for (int i = 0; i < s.Length; i++)
			if ((s[i] < '0' || s[i] > '9') && (s[i] != '-' || i != 0))
				return false;
		return true;
	}

	public static string Repeat(this string s, int count)
	{
		if (s == null)
			return null;

		string output = string.Empty;

		for (int i = 0; i < count; i++)
			output += s;

		return output;
	}
}