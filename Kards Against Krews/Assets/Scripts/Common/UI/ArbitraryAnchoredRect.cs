using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class ArbitraryAnchoredRect : CustomBehaviour
{
	RectTransform rt { get { return transform as RectTransform; } }
	RectTransform parent { get { return transform.parent as RectTransform; } }

	public Anchor leftAnchor;
	public Anchor rightAnchor;
	public Anchor topAnchor;
	public Anchor bottomAnchor;

	Vector3[] parentCorners = new Vector3[4];
	Vector3[] corners = new Vector3[4];

	float left
	{
		get
		{
			leftAnchor.referenceObject.GetWorldCorners(corners);
			return MathUtil.InverseLerpUnclamped(
				(parent.worldToLocalMatrix * parentCorners[1]).x,
				(parent.worldToLocalMatrix * parentCorners[2]).x,
				Mathf.LerpUnclamped(
					(parent.worldToLocalMatrix * corners[1]).x,
					(parent.worldToLocalMatrix * corners[2]).x,
					leftAnchor.normalizedAnchorPoint)
				);
		}
	}
	float right
	{
		get
		{
			rightAnchor.referenceObject.GetWorldCorners(corners);
			return MathUtil.InverseLerpUnclamped(
				(parent.worldToLocalMatrix * parentCorners[1]).x,
				(parent.worldToLocalMatrix * parentCorners[2]).x,
				Mathf.LerpUnclamped(
					(parent.worldToLocalMatrix * corners[1]).x,
					(parent.worldToLocalMatrix * corners[2]).x,
					rightAnchor.normalizedAnchorPoint)
				);
		}
	}
	float top
	{
		get
		{
			topAnchor.referenceObject.GetWorldCorners(corners);
			return MathUtil.InverseLerpUnclamped(
				(parent.worldToLocalMatrix * parentCorners[0]).y,
				(parent.worldToLocalMatrix * parentCorners[1]).y,
				Mathf.LerpUnclamped(
					(parent.worldToLocalMatrix * corners[0]).y,
					(parent.worldToLocalMatrix * corners[1]).y,
					topAnchor.normalizedAnchorPoint)
				);
		}
	}
	float bottom
	{
		get
		{
			bottomAnchor.referenceObject.GetWorldCorners(corners);
			return MathUtil.InverseLerpUnclamped(
				(parent.worldToLocalMatrix * parentCorners[0]).y,
				(parent.worldToLocalMatrix * parentCorners[1]).y,
				Mathf.LerpUnclamped(
					(parent.worldToLocalMatrix * corners[0]).y,
					(parent.worldToLocalMatrix * corners[1]).y,
					bottomAnchor.normalizedAnchorPoint)
				);
		}
	}

	public bool update = true;

	public void Readjust()
	{
		if (!parent)
			return;

		if (parent.rect.width == 0 || parent.rect.height == 0)
			return;

		if (leftAnchor.referenceObject == null
			|| rightAnchor.referenceObject == null
			|| topAnchor.referenceObject == null
			|| bottomAnchor.referenceObject == null)
			return;

		parent.GetWorldCorners(parentCorners);

		rt.anchorMin = new Vector2(left, bottom);
		rt.anchorMax = new Vector2(right, top);

		rt.offsetMin = new Vector2(leftAnchor.offset, bottomAnchor.offset);
		rt.offsetMax = new Vector2(-rightAnchor.offset, -topAnchor.offset);
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

		public Anchor(RectTransform referenceObject, float normalizedAnchorPoint, float offset)
		{
			this.referenceObject = referenceObject;
			this.normalizedAnchorPoint = normalizedAnchorPoint;
			this.offset = offset;
		}
	}
}