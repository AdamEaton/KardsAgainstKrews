using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class UnityEventButtonDecorator : UIButtonDecorator
{
	public UnityEvent onClick;

	public override void OnButtonClicked()
	{
		onClick.TryInvoke();
	}
}