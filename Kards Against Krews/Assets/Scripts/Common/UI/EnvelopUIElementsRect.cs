using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class EnvelopUIElementsRect : CustomBehaviour
{
	RectTransform rt { get { return transform as RectTransform; } }
	RectTransform parent { get { return transform.parent as RectTransform; } }

	[SerializeField]
	RectTransform[] elements;

	Vector3[] parentCorners = new Vector3[4];
	Vector3[] corners = new Vector3[4];

	float left;
	float right;
	float bottom;
	float top;

	public bool update = true;

	public void Readjust()
	{
		if (elements == null || elements.Length <= 0)
		{
			return;
		}

		left = Mathf.Infinity;
		right = Mathf.NegativeInfinity;
		bottom = Mathf.Infinity;
		top = Mathf.NegativeInfinity;

		parent.GetWorldCorners(parentCorners);
		foreach (var element in elements)
		{
			element.GetWorldCorners(corners);

			left = Mathf.Min(
				left,
				MathUtil.InverseLerpUnclamped((parent.worldToLocalMatrix * parentCorners[1]).x,
					(parent.worldToLocalMatrix * parentCorners[2]).x,
					(parent.worldToLocalMatrix * corners[1]).x)
				);
			right = Mathf.Max(
				right,
				MathUtil.InverseLerpUnclamped((parent.worldToLocalMatrix * parentCorners[1]).x,
					(parent.worldToLocalMatrix * parentCorners[2]).x,
					(parent.worldToLocalMatrix * corners[2]).x)
				);
			bottom = Mathf.Min(
				bottom,
				MathUtil.InverseLerpUnclamped((parent.worldToLocalMatrix * parentCorners[0]).y,
					(parent.worldToLocalMatrix * parentCorners[1]).y,
					(parent.worldToLocalMatrix * corners[0]).y)
				);
			top = Mathf.Max(
				top,
				MathUtil.InverseLerpUnclamped((parent.worldToLocalMatrix * parentCorners[0]).y,
					(parent.worldToLocalMatrix * parentCorners[1]).y,
					(parent.worldToLocalMatrix * corners[1]).y)
				);
		}

		rt.anchorMin = new Vector2(left, bottom);
		rt.anchorMax = new Vector2(right, top);
		rt.offsetMin = Vector2.zero;
		rt.offsetMax = Vector2.zero;
	}

	void OnEnable()
	{
		Readjust();
	}
	void Update()
	{
		if (update)
			Readjust();
	}

	[System.Serializable]
	public struct Anchor
	{
		public RectTransform referenceObject;
		public float normalizedAnchorPoint;
		public float offset;
	}
}