using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TextInputModalButton : ListDisplayItem<TextInputModal.ButtonInfo>
{
	[SerializeField]
	Text label;
	[SerializeField]
	Button button;

	TextInputModal modal { get { return owner as TextInputModal; } }

	public override void Populate(ListDisplay<TextInputModal.ButtonInfo> owner, TextInputModal.ButtonInfo item)
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
