using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Button))]
public abstract class UIButtonDecorator : CustomBehaviour
{
	Button _button;
	protected Button button
	{
		get
		{
			if (!_button) _button = GetComponent<Button>();
			return _button;
		}
	}

	protected virtual void OnEnable()
	{
		button.onClick.AddListener(OnButtonClicked);
	}
	protected virtual void OnDisable()
	{
		button.onClick.RemoveListener(OnButtonClicked);
	}

	public abstract void OnButtonClicked();
}