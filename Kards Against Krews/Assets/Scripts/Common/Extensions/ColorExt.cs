using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for Color.
/// </summary>
public static class ColorExt
{
	/// <summary>
	/// The 16 hexadecimal digits, for ease of conversion in the HexCode method.
	/// </summary>
	const string HexDigits = "0123456789ABCDEF";

	/// <summary>
	/// Returns the Color with the opposite R, G, and B values (and same alpha).
	/// </summary>
	/// <param name="color">The Color to invert.</param>
	/// <returns>The Color that, when added to this Color, yields white. The alpha of the result will match the alpha of this Color.</returns>
	public static Color Inverse(this Color color)
	{
		return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
	}

	/// <summary>
	/// Returns the hex code for this Color.
	/// </summary>
	/// <param name="color">The Color to convert to hex code.</param>
	/// <returns>The hex code of this Color, in the form 'RRGGBB'.</returns>
	public static string HexCode(this Color color)
	{
		string output = "";
		int channel;
		for (int i = 0; i < 3; i++)
		{
			channel = Mathf.FloorToInt(color[i] * 255);
			output += HexDigits[channel / 16].ToString() + HexDigits[channel % 16].ToString();
		}
		return output;
	}
}