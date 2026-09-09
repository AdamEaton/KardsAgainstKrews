using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// Utility class for handling File I/O.
/// </summary>
public static class FileUtil
{
	/// <summary>
	/// Generates a list of every file within the specified directory that matches the specified search pattern.
	/// </summary>
	/// <param name="relativePath">The relative path from Application.persistentDataPath to the directory to scan. If omitted, the entirety of Application.persistentDataPath will be scanned.</param>
	/// <param name="pattern">The pattern to match. If omitted, all filenames will be matched.</param>
	/// <returns>A list of every file within the specified subdirectory of Application.persistentDataPath that matches the specified search pattern.</returns>
	public static List<FileInfo> FindAllFiles(string relativePath = "", string pattern = "*")
	{
		return FindAllFiles(new DirectoryInfo(PathUtil.Combine(Application.persistentDataPath, relativePath)), pattern);
	}
	static List<FileInfo> FindAllFiles(DirectoryInfo dir, string pattern)
	{
		var output = new List<FileInfo>();

		try
		{
			foreach (var file in dir.GetFiles(pattern))
				output.Add(file);
		}
		catch
		{
			Debug.Log("Error accessing " + dir.FullName);
		}

		foreach (var directory in dir.GetDirectories())
			output.AddRange(FindAllFiles(directory, pattern));

		return output;
	}
}