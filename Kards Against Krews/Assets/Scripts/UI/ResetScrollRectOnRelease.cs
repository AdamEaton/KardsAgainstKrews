using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class ResetScrollRectOnRelease : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
	public float smoothTime;

	bool dragging;
	Vector2 currentVelocity;
	Vector2 lastPosition;

	Vector2 currentPosition
	{
		get { return scrollRect.normalizedPosition; }
		set { scrollRect.normalizedPosition = value; }
	}

	ScrollRect _scrollRect;
	ScrollRect scrollRect
	{
		get
		{
			if (!_scrollRect)
				_scrollRect = GetComponent<ScrollRect>();
			return _scrollRect;
		}
	}

	void OnEnable()
	{
		currentPosition = Vector2.zero;
	}

	void Update()
	{
		if (dragging)
		{
			if (Time.deltaTime > 0)
				currentVelocity = (currentPosition - lastPosition) / Time.deltaTime;
			lastPosition = currentPosition;
		}
		else
		{
			currentPosition = Vector2.SmoothDamp(currentPosition, Vector2.zero, ref currentVelocity, smoothTime, Mathf.Infinity, Time.deltaTime);
		}
	}

	public void OnBeginDrag(PointerEventData data)
	{
		dragging = true;
		currentVelocity = Vector2.zero;
		lastPosition = currentPosition;
	}
	public void OnEndDrag(PointerEventData data)
	{
		dragging = false;
	}
}