using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class VectorUtil
{
	/// <summary>
	/// Calculates a Vector2 from polar coordinates.
	/// </summary>
	/// <param name="magnitude">The length of the vector.</param>
	/// <param name="angle">The angle, in degrees, counterclockwise from the x-axis.</param>
	/// <returns>The vector defined by the supplied polar coordinates.</returns>
	public static Vector2 FromPolar(float magnitude, float angle)
	{
		return new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * magnitude;
	}

	/// <summary>
	/// Calculates a Vector3 from spherical coordinates.
	/// </summary>
	/// <param name="magnitude">The length of the vector.</param>
	/// <param name="azimuth">The angle, in degrees, counterclockwise around the y-axis, starting from the x-axis.</param>
	/// <param name="inclination">The angle, in degrees, elevated up from the xz-plane.</param>
	/// <returns>The vector defined by the supplied spherical coordinates.</returns>
	public static Vector3 FromSpherical(float magnitude, float azimuth, float inclination)
	{
		var horizontal = FromPolar(1, azimuth);
		return new Vector3(
			Mathf.Cos(inclination * Mathf.Deg2Rad) * horizontal.x,
			Mathf.Sin(inclination * Mathf.Deg2Rad),
			Mathf.Cos(inclination * Mathf.Deg2Rad) * horizontal.y) * magnitude;
	}
}