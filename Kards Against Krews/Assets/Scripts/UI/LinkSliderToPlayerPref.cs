using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Slider))]
public class LinkSliderToPlayerPref : CustomBehaviour
{
	Slider _slider;
	public Slider slider
	{
		get
		{
			if (!_slider) _slider = GetComponent<Slider>();
			return _slider;
		}
	}

	public string key;
	public float defaultValue;

	void OnEnable()
	{
		slider.value = PlayerPrefs.GetFloat(key, defaultValue);

		slider.onValueChanged.AddListener(OnSliderValueChanged);
	}
	void OnDisable()
	{
		slider.onValueChanged.RemoveListener(OnSliderValueChanged);
	}

	void OnSliderValueChanged(float newValue)
	{
		PlayerPrefs.SetFloat(key, newValue);
	}
}
