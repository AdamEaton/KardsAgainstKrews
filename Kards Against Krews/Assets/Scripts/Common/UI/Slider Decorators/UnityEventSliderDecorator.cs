using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventSliderDecorator : UISliderDecorator
{
	public FloatEvent onValueChanged;

	protected override void OnValueChanged(float value)
	{
		onValueChanged.TryInvoke(value);
	}
}