using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

// Allows the direct operation of an EventTrigger through UnityEvents or any other script.
[RequireComponent(typeof(EventTrigger))]
public class EventTriggerOperator : CustomBehaviour
{
	EventTrigger _trigger;
	EventTrigger trigger
	{
		get
		{
			if (_trigger == null)
				_trigger = GetComponent<EventTrigger>();

			return _trigger;
		}
	}

	public void OnPointerEnter()
	{
		trigger.OnPointerEnter(null);
	}
	public void OnPointerExit()
	{
		trigger.OnPointerExit(null);
	}
	public void OnPointerDown()
	{
		trigger.OnPointerDown(null);
	}
	public void OnPointerUp()
	{
		trigger.OnPointerUp(null);
	}
	public void OnPointerClick()
	{
		trigger.OnPointerClick(null);
	}
}