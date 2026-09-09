using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public static class VectorExt
{
	public static Vector2 Select(this Vector2 v, Func<float, float> selector)
	{
		return new Vector2(selector(v.x), selector(v.y));
	}
	public static Vector2 Select(this Vector2 v, Func<float, int, float> selector)
	{
		return new Vector2(selector(v.x, 0), selector(v.y, 1));
	}

	public static Vector3 Select(this Vector3 v, Func<float, float> selector)
	{
		return new Vector3(selector(v.x), selector(v.y), selector(v.z));
	}
	public static Vector3 Select(this Vector3 v, Func<float, int, float> selector)
	{
		return new Vector3(selector(v.x, 0), selector(v.y, 1), selector(v.z, 2));
	}

	public static Vector4 Select(this Vector4 v, Func<float, float> selector)
	{
		return new Vector4(selector(v.x), selector(v.y), selector(v.z), selector(v.w));
	}
	public static Vector4 Select(this Vector4 v, Func<float, int, float> selector)
	{
		return new Vector4(selector(v.x, 0), selector(v.y, 1), selector(v.z, 2), selector(v.w, 3));
	}
}