using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NumericControlDigit : ListDisplayItem<int>
{
	[SerializeField]
	Text digitDisplay;
	[SerializeField]
	Button upButton;
	[SerializeField]
	Button downButton;

	NumericControl mainControl { get { return owner as NumericControl; } }

	public override void Populate(ListDisplay<int> owner, int item)
	{
		base.Populate(owner, item);

		if (!mainControl)
			return;

		mainControl.onValueChanged.AddListener(OnValueChanged);
		UpdateLabel();
	}
	void OnDestroy()
	{
		if (!mainControl)
			return;

		mainControl.onValueChanged.RemoveListener(OnValueChanged);
	}

	void OnEnable()
	{
		upButton.onClick.AddListener(OnUpClicked);
		downButton.onClick.AddListener(OnDownClicked);
	}
	void OnDisable()
	{
		upButton.onClick.RemoveListener(OnUpClicked);
		downButton.onClick.RemoveListener(OnDownClicked);
	}

	void OnUpClicked()
	{
		if (!mainControl)
			return;

		mainControl.currentValue += currentItem;
	}
	void OnDownClicked()
	{
		if (!mainControl)
			return;

		mainControl.currentValue -= currentItem;
	}

	void OnValueChanged(int value)
	{
		UpdateLabel();
	}
	void UpdateLabel()
	{
		if (!mainControl)
			return;

		if (digitDisplay) digitDisplay.text = ((mainControl.currentValue % (currentItem * 10)) / currentItem).ToString();
	}
}
