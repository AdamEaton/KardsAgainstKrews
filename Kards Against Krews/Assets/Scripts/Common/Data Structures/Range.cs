using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Represents a range of float values.
/// </summary>
[System.Serializable]
public struct Range
{
	public float min;
	public float max;

	public float RandomValue { get { return Random.Range(min, max); } }

	public Range(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	public bool Contains(float value)
	{
		return min <= value && value <= max;
	}

	public float Lerp(float t)
	{
		return Mathf.Lerp(min, max, t);
	}
	public float LerpUnclamped(float t)
	{
		return Mathf.LerpUnclamped(min, max, t);
	}

	public float InverseLerp(float value)
	{
		return Mathf.InverseLerp(min, max, value);
	}
	public float InverseLerpUnclamped(float value)
	{
		return MathUtil.InverseLerpUnclamped(min, max, value);
	}
}
