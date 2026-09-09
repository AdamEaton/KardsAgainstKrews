using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayCommonAudio : CustomBehaviour
{
	public void Play(string audioID)
	{
		CommonAudioLibrary.Instance.Play(audioID);
	}
}
