using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayCommonAudioToggleDecorator : UIToggleDecorator
{
	public string trueAudioID;
	public string falseAudioID;

	protected override void OnToggle(bool value)
	{
		CommonAudioLibrary.Instance.Play(value ? trueAudioID : falseAudioID);
	}
}
