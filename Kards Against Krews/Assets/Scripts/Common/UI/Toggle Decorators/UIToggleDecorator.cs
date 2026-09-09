using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Toggle))]
public abstract class UIToggleDecorator : CustomBehaviour
{
	public bool activateOnEnable;

	Toggle _toggle;
	protected Toggle toggle
	{
		get
		{
			if (!_toggle) _toggle = GetComponent<Toggle>();
			return _toggle;
		}
	}

	protected virtual void OnEnable()
	{
		if (activateOnEnable)
			OnToggle(toggle.isOn);

		toggle.onValueChanged.AddListener(OnToggle);
	}
	protected virtual void OnDisable()
	{
		toggle.onValueChanged.RemoveListener(OnToggle);
	}
	
	protected abstract void OnToggle(bool value);
}