using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Slider))]
public abstract class UISliderDecorator : CustomBehaviour
{
	public bool activateOnEnable;
	Slider _slider;
	protected Slider slider
	{
		get
		{
			if (!_slider) _slider = GetComponent<Slider>();
			return _slider;
		}
	}

	protected virtual void OnEnable()
	{
		if (activateOnEnable)
			OnValueChanged(slider.value);

		slider.onValueChanged.AddListener(OnValueChanged);
	}
	protected virtual void OnDisable()
	{
		slider.onValueChanged.RemoveListener(OnValueChanged);
	}

	protected abstract void OnValueChanged(float value);
}