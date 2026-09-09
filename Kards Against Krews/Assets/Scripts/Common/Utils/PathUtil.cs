using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Utility class for handling directory/Catalog paths.
/// </summary>
public class PathUtil
{
	/// <summary>
	/// All characters that are assumed to be directory separators.
	/// </summary>
	const string SeparatorChars = "/\\";

	/// <summary>
	/// Combines the specified series of relative paths into a single path.
	/// </summary>
	/// <param name="paths">The relative paths to combine.</param>
	/// <returns>A single path that represents the specified series of paths.</returns>
	public static string Combine(params string[] paths)
	{
		if (paths.Length == 0)
			return "";
		else if (paths.Length == 1)
			return FixSeparators(paths[0]);

		string output = FixSeparators(paths[0]);

		for (int i = 1; i < paths.Length; i++)
			output = Path.Combine(output, FixSeparators(paths[i]));

		return output;
	}

	/// <summary>
	/// Splits the specified path into a series of relative paths.
	/// </summary>
	/// <param name="path">The path to split.</param>
	/// <returns>A series of paths that represents each step in the specified path.</returns>
	public static string[] Split(string path)
	{
		return FixSeparators(path).Split(Path.DirectorySeparatorChar);
	}

	/// <summary>
	/// Returns the specified path with all directory separators replaced with Path.DirectorySeparatorChar.
	/// 
	/// Separators are any of '/', '\'.
	/// </summary>
	/// <param name="path">The path to fix.</param>
	/// <returns>A new string with the specified path, where all separators are replaced with Path.DirectorySeparatorChar.</returns>
	public static string FixSeparators(string path)
	{
		foreach (var separator in SeparatorChars)
			path = path.Replace(separator, Path.DirectorySeparatorChar);
		return path;
	}

	/// <summary>
	/// Returns the name of the root of the specified path.
	/// </summary>
	/// <param name="path">The path to determine the root of.</param>
	/// <returns>The name of the topmost element of the specified path.</returns>
	public static string GetRoot(string path)
	{
		path = FixSeparators(path);
		var index = path.IndexOf(Path.DirectorySeparatorChar);

		if (index < 0)
			return path;
		return path.Substring(0, index);
	}
	/// <summary>
	/// Returns the specified path with the root removed.
	/// </summary>
	/// <param name="path">The path to remove the root from.</param>
	/// <returns>The specified path without its topmost element.</returns>
	public static string PopRoot(string path)
	{
		path = FixSeparators(path);
		var index = path.IndexOf(Path.DirectorySeparatorChar);

		if (index < 0)
			return "";
		return path.Substring(index + 1);
	}
	/// <summary>
	/// Returns the name of the object referenced by the specified path.
	/// </summary>
	/// <param name="path">The path to determine the name of.</param>
	/// <returns>The name of the bottommost element of the specified path.</returns>
	public static string GetName(string path)
	{
		path = FixSeparators(path);
		var index = path.LastIndexOf(Path.DirectorySeparatorChar);

		if (index < 0)
			return path;
		return path.Substring(index + 1);
	}
	/// <summary>
	/// Returns the containing path for the specified path.
	/// </summary>
	/// <param name="path">The path to determine the containing path of.</param>
	/// <returns>The specified path without its bottommost element.</returns>
	public static string PopName(string path)
	{
		path = FixSeparators(path);
		var index = path.LastIndexOf(Path.DirectorySeparatorChar);

		if (index < 0)
			return "";
		return path.Substring(0, index);
	}

	/// <summary>
	/// Exception-safe implementation of GetRelativePath.
	/// 
	/// Outputs to the specified string the path that must be followed from the specified source path to the specified destination path.
	/// </summary>
	/// <param name="source">The path to start at.</param>
	/// <param name="destination">The path to end at.</param>
	/// <param name="path">If successful (returned true), outputs the path to follow from source to destination, otherwise outputs an empty string.</param>
	/// <returns>True if the source path contains the destination path, otherwise false.</returns>
	public static bool TryGetRelativePath(string source, string destination, out string path)
	{
		try
		{
			path = GetRelativePath(source, destination);
			return true;
		}
		catch (CatalogNotFoundException)
		{
			path = "";
			return false;
		}
	}
	/// <summary>
	/// Returns the path that must be followed from the specified source path to the specified destination path.
	/// 
	/// Will throw a CatalogNotFoundException if the source path does not contain the destination path.
	/// </summary>
	/// <param name="source">The path to start at.</param>
	/// <param name="destination">The path to end at.</param>
	/// <returns>The path to follow from source to destination.</returns>
	public static string GetRelativePath(string source, string destination)
	{
		source = FixSeparators(source);
		destination = FixSeparators(destination);
		if (Contains(source, destination))
			return destination.Substring(source.Length + 1);

		Debug.Log(source);
		Debug.Log(destination);

		throw new CatalogNotFoundException("The destination path is not contained by the source path.");
	}

	/// <summary>
	/// Does the specified rhs path represent a subpath of the specified lhs path?
	/// </summary>
	/// <param name="lhs">The containing path.</param>
	/// <param name="rhs">The contained path.</param>
	/// <returns>True if the rhs path starts with the lhs path, otherwise false.</returns>
	public static bool Contains(string lhs, string rhs)
	{
		lhs = FixSeparators(lhs);
		rhs = FixSeparators(rhs);
		return rhs.StartsWith(lhs);
	}
	/// <summary>
	/// Does the specified lhs path represent the same path as the specified rhs path?
	/// </summary>
	/// <param name="lhs">A path to compare.</param>
	/// <param name="rhs">A path to compare.</param>
	/// <returns>True if the two paths are the same ignoring their separators, otherwise false.</returns>
	public static bool IsEqual(string lhs, string rhs)
	{
		return FixSeparators(lhs) == FixSeparators(rhs);
	}
}