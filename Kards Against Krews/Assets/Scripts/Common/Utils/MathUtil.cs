using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class MathUtil
{
	public static float InverseLerpUnclamped(float a, float b, float value)
	{
		return (value - a) / (b - a);
	}
}
