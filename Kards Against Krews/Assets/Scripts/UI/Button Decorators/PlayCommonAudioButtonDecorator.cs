using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayCommonAudioButtonDecorator : UIButtonDecorator
{
	public string audioID;

	public override void OnButtonClicked()
	{
		CommonAudioLibrary.Instance.Play(audioID);
	}
}
