using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VersionNumber
{
	public static void Extract(string versionNumberString, out int major, out int minor, out int patch)
	{
		major = 0;
		minor = 0;
		patch = 0;
		
		var numbers = versionNumberString.Split('.');
		try
		{
			major = int.Parse(numbers[0]);
			minor = int.Parse(numbers[1]);
			patch = int.Parse(numbers[2]);
		}
		catch (System.IndexOutOfRangeException)
		{
			return;
		}
	}
}