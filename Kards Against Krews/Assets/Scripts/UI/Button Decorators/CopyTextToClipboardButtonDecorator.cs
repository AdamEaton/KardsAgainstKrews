using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CopyTextToClipboardButtonDecorator : UIButtonDecorator
{
	[SerializeField]
	Text textToCopy;

	public override void OnButtonClicked()
	{
		ClipboardUtil.Value = textToCopy.text;
	}
}