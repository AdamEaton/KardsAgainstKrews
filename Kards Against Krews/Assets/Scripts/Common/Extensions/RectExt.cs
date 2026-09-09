using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class RectExt
{
	public static Vector2 LerpUnclamped(this Rect rect, float tX, float tY)
	{
		return new Vector2(Mathf.LerpUnclamped(rect.xMin, rect.xMax, tX), Mathf.LerpUnclamped(rect.yMin, rect.yMax, tY));
	}
	public static Vector2 LerpUnclamped(this Rect rect, Vector2 tXY)
	{
		return rect.LerpUnclamped(tXY.x, tXY.y);
	}
	public static Vector2 LerpUnclamped(this Rect rect, float tXY)
	{
		return rect.LerpUnclamped(tXY, tXY);
	}

	public static Vector2 Lerp(this Rect rect, float tX, float tY)
	{
		return rect.LerpUnclamped(tX, tY).Select(Mathf.Clamp01);
	}
	public static Vector2 Lerp(this Rect rect, Vector2 tXY)
	{
		return rect.LerpUnclamped(tXY).Select(Mathf.Clamp01);
	}
	public static Vector2 Lerp(this Rect rect, float tXY)
	{
		return rect.LerpUnclamped(tXY).Select(Mathf.Clamp01);
	}

	public static Vector2 InverseLerpUnclamped(this Rect rect, float valueX, float valueY)
	{
		return new Vector2(MathUtil.InverseLerpUnclamped(rect.xMin, rect.xMax, valueX), MathUtil.InverseLerpUnclamped(rect.yMin, rect.yMax, valueY));
	}
	public static Vector2 InverseLerpUnclamped(this Rect rect, Vector2 valueXY)
	{
		return rect.InverseLerpUnclamped(valueXY).Select(Mathf.Clamp01);
	}

	public static Vector2 InverseLerp(this Rect rect, float valueX, float valueY)
	{
		return rect.InverseLerpUnclamped(valueX, valueY).Select(Mathf.Clamp01);
	}
	public static Vector2 InverseLerp(this Rect rect, Vector2 valueXY)
	{
		return rect.InverseLerpUnclamped(valueXY).Select(Mathf.Clamp01);
	}

	public static Rect EqualPartition(this Rect rect, int rows, int rowIndex, int cols, int colIndex)
	{
		if (rows <= 0) throw new System.ArgumentException("Row count must be positive!", "rows");
		if (cols <= 0) throw new System.ArgumentException("Column count must be positive!", "cols");

		Vector2 partitionSize = rect.size;
		partitionSize.x /= cols;
		partitionSize.y /= rows;

		Rect output = new Rect();
		output.position = partitionSize;
		output.x *= colIndex;
		output.y *= rowIndex;
		output.position += rect.position;

		output.size = partitionSize;
		return output;
	}
}