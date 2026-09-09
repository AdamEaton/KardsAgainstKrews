using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnityEventToggleDecorator : UIToggleDecorator
{
	public BoolEvent onValueChanged;
	public UnityEvent onBecameTrue;
	public UnityEvent onBecameFalse;

	protected override void OnToggle(bool value)
	{
		onValueChanged.TryInvoke(value);

		(value ? onBecameTrue : onBecameFalse).TryInvoke();
	}
}