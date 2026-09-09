using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Extension class for Vector2.
/// </summary>
public static class Vector2Ext
{
	/// <summary>
	/// Determines the shortest angle, in degrees, from the Vector2 to the x-axis.
	/// </summary>
	/// <param name="vec">The Vector2 to determine the angle of.</param>
	/// <returns>The angle of the Vector2, in degrees, between -180 and 180.</returns>
	public static float Direction(this Vector2 vec)
	{
		return Mathf.Atan2(vec.y, vec.x) * Mathf.Rad2Deg;
	}
}