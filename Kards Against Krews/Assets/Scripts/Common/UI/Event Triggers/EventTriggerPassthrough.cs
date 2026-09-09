using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(EventTrigger))]
public class EventTriggerPassthrough : CustomBehaviour
{
	EventTrigger myTrigger;

	[SerializeField]
	bool pointerEnterHandler;
	[SerializeField]
	bool pointerExitHandler;
	[SerializeField]
	bool pointerDownHandler;
	[SerializeField]
	bool pointerUpHandler;
	[SerializeField]
	bool pointerClickHandler;
	[SerializeField]
	bool dragHandler;
	[SerializeField]
	bool dropHandler;
	[SerializeField]
	bool scrollHandler;
	[SerializeField]
	bool updateSelectedHandler;
	[SerializeField]
	bool selectHandler;
	[SerializeField]
	bool deselectHandler;
	[SerializeField]
	bool moveHandler;
	[SerializeField]
	bool initializePotentialDragHandler;
	[SerializeField]
	bool beginDragHandler;
	[SerializeField]
	bool endDragHandler;
	[SerializeField]
	bool submitHandler;
	[SerializeField]
	bool cancelHandler;

	void OnEnable()
	{
		myTrigger = GetComponent<EventTrigger>();
		if (myTrigger == null)
			return;

		if (pointerEnterHandler)
			AddListener(EventTriggerType.PointerEnter, OnPointerEnter);
		if (pointerExitHandler)
			AddListener(EventTriggerType.PointerExit, OnPointerExit);
		if (pointerDownHandler)
			AddListener(EventTriggerType.PointerDown, OnPointerDown);
		if (pointerUpHandler)
			AddListener(EventTriggerType.PointerUp, OnPointerUp);
		if (pointerClickHandler)
			AddListener(EventTriggerType.PointerClick, OnPointerClick);
		if (dragHandler)
			AddListener(EventTriggerType.Drag, OnDrag);
		if (dropHandler)
			AddListener(EventTriggerType.Drop, OnDrop);
		if (scrollHandler)
			AddListener(EventTriggerType.Scroll, OnScroll);
		if (updateSelectedHandler)
			AddListener(EventTriggerType.UpdateSelected, OnUpdateSelected);
		if (selectHandler)
			AddListener(EventTriggerType.Select, OnSelect);
		if (deselectHandler)
			AddListener(EventTriggerType.Deselect, OnDeselect);
		if (initializePotentialDragHandler)
			AddListener(EventTriggerType.InitializePotentialDrag, OnInitializePotentialDrag);
		if (beginDragHandler)
			AddListener(EventTriggerType.BeginDrag, OnBeginDrag);
		if (endDragHandler)
			AddListener(EventTriggerType.EndDrag, OnEndDrag);
		if (submitHandler)
			AddListener(EventTriggerType.Submit, OnSubmit);
		if (cancelHandler)
			AddListener(EventTriggerType.Cancel, OnCancel);
	}
	void OnDisable()
	{
		if (myTrigger == null)
			return;

		if (pointerEnterHandler)
			RemoveListener(EventTriggerType.PointerEnter, OnPointerEnter);
		if (pointerExitHandler)
			RemoveListener(EventTriggerType.PointerExit, OnPointerExit);
		if (pointerDownHandler)
			RemoveListener(EventTriggerType.PointerDown, OnPointerDown);
		if (pointerUpHandler)
			RemoveListener(EventTriggerType.PointerUp, OnPointerUp);
		if (pointerClickHandler)
			RemoveListener(EventTriggerType.PointerClick, OnPointerClick);
		if (dragHandler)
			RemoveListener(EventTriggerType.Drag, OnDrag);
		if (dropHandler)
			RemoveListener(EventTriggerType.Drop, OnDrop);
		if (scrollHandler)
			RemoveListener(EventTriggerType.Scroll, OnScroll);
		if (updateSelectedHandler)
			RemoveListener(EventTriggerType.UpdateSelected, OnUpdateSelected);
		if (selectHandler)
			RemoveListener(EventTriggerType.Select, OnSelect);
		if (deselectHandler)
			RemoveListener(EventTriggerType.Deselect, OnDeselect);
		if (initializePotentialDragHandler)
			RemoveListener(EventTriggerType.InitializePotentialDrag, OnInitializePotentialDrag);
		if (beginDragHandler)
			RemoveListener(EventTriggerType.BeginDrag, OnBeginDrag);
		if (endDragHandler)
			RemoveListener(EventTriggerType.EndDrag, OnEndDrag);
		if (submitHandler)
			RemoveListener(EventTriggerType.Submit, OnSubmit);
		if (cancelHandler)
			RemoveListener(EventTriggerType.Cancel, OnCancel);
	}

	void AddListener(EventTriggerType eventType, UnityAction<BaseEventData> listener)
	{
		myTrigger.GetTriggerEvent(eventType).AddListener(listener);
	}
	void RemoveListener(EventTriggerType eventType, UnityAction<BaseEventData> listener)
	{
		myTrigger.GetTriggerEvent(eventType).RemoveListener(listener);
	}

	void OnPointerEnter(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IPointerEnterHandler>();
		if (parentHandler == null)
			return;
		
		foreach (var comp in (parentHandler as Component).GetComponents<IPointerEnterHandler>())
		{
			comp.OnPointerEnter(eventData as PointerEventData);
		}
	}
	void OnPointerExit(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IPointerExitHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IPointerExitHandler>())
		{
			parentHandler.OnPointerExit(eventData as PointerEventData);
		}
	}
	void OnPointerDown(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IPointerDownHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IPointerDownHandler>())
		{
			parentHandler.OnPointerDown(eventData as PointerEventData);
		}
	}
	void OnPointerUp(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IPointerUpHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IPointerUpHandler>())
		{
			parentHandler.OnPointerUp(eventData as PointerEventData);
		}
	}
	void OnPointerClick(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IPointerClickHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IPointerClickHandler>())
		{
			parentHandler.OnPointerClick(eventData as PointerEventData);
		}
	}
	void OnDrag(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IDragHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IDragHandler>())
		{
			parentHandler.OnDrag(eventData as PointerEventData);
		}
	}
	void OnDrop(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IDropHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IDropHandler>())
		{
			parentHandler.OnDrop(eventData as PointerEventData);
		}
	}
	void OnScroll(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IScrollHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IScrollHandler>())
		{
			parentHandler.OnScroll(eventData as PointerEventData);
		}
	}
	void OnUpdateSelected(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IUpdateSelectedHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IUpdateSelectedHandler>())
		{
			parentHandler.OnUpdateSelected(eventData as PointerEventData);
		}
	}
	void OnSelect(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<ISelectHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<ISelectHandler>())
		{
			parentHandler.OnSelect(eventData as PointerEventData);
		}
	}
	void OnDeselect(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IDeselectHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IDeselectHandler>())
		{
			parentHandler.OnDeselect(eventData as PointerEventData);
		}
	}
	void OnInitializePotentialDrag(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IInitializePotentialDragHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IInitializePotentialDragHandler>())
		{
			parentHandler.OnInitializePotentialDrag(eventData as PointerEventData);
		}
	}
	void OnBeginDrag(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IBeginDragHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IBeginDragHandler>())
		{
			parentHandler.OnBeginDrag(eventData as PointerEventData);
		}
	}
	void OnEndDrag(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<IEndDragHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<IEndDragHandler>())
		{
			parentHandler.OnEndDrag(eventData as PointerEventData);
		}
	}
	void OnSubmit(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<ISubmitHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<ISubmitHandler>())
		{
			parentHandler.OnSubmit(eventData as PointerEventData);
		}
	}
	void OnCancel(BaseEventData eventData)
	{
		if (transform.parent == null)
			return;

		var parentHandler = transform.parent.GetComponentInParent<ICancelHandler>();
		if (parentHandler == null)
			return;

		foreach (var comp in (parentHandler as Component).GetComponents<ICancelHandler>())
		{
			parentHandler.OnCancel(eventData as PointerEventData);
		}
	}
}