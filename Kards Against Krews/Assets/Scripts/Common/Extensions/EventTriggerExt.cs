using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public static class EventTriggerExt
{
	public static EventTrigger.TriggerEvent GetTriggerEvent(this EventTrigger eventTrigger, EventTriggerType type)
	{
		foreach (var trigger in eventTrigger.triggers)
		{
			if (trigger.eventID == type)
			{
				return trigger.callback;
			}
		}
		EventTrigger.Entry newEntry = new EventTrigger.Entry();
		newEntry.eventID = type;
		eventTrigger.triggers.Add(newEntry);
		return newEntry.callback;
	}
}