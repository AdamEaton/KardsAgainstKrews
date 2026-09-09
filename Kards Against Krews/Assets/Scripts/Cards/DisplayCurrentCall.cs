using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DisplayCurrentCall : CustomBehaviour
{
	[SerializeField]
	Text cardTextDisplay;
	[SerializeField]
	Text packTextDisplay;

	public int blankCount;

	void OnEnable()
	{
		EventSystem.AddListener<NewCallEvent>(OnNewCall);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<NewCallEvent>(OnNewCall);
	}

	void OnNewCall(NewCallEvent e)
	{
		if (e.newCall)
		{
			cardTextDisplay.text = e.newCall.text.Replace("_", "_".Repeat(blankCount));
			packTextDisplay.text = string.Format("<i>{0}</i>", e.newCall.pack);
		}
		else
		{
			cardTextDisplay.text = "[DECK OUT]";
			packTextDisplay.text = string.Format("<i>{0}</i>", "[N/A]");
		}
	}
}
