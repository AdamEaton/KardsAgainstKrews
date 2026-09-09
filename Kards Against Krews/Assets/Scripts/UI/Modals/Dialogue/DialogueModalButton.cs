using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class DialogueModalButton : ListDisplayItem<DialogueModal.ButtonInfo>
{
	[SerializeField]
	Text label;
	[SerializeField]
	Button button;

	DialogueModal modal { get { return owner as DialogueModal; } }

	public override void Populate(ListDisplay<DialogueModal.ButtonInfo> owner, DialogueModal.ButtonInfo item)
	{
		base.Populate(owner, item);

		if (label)
			label.text = item.label;
	}

	void OnEnable()
	{
		if (button)
			button.onClick.AddListener(OnClick);
	}
	void OnDisable()
	{
		if (button)
			button.onClick.RemoveListener(OnClick);
	}

	void OnClick()
	{
		modal.Execute(currentItem);
	}
}
