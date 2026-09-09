using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UpdateAudioVolumeSliderDecorator : UISliderDecorator
{
	bool dirty = false;

	void Update()
	{
		if (dirty)
		{
			EventSystem.Dispatch(new UpdateVolumeEvent());
			dirty = false;
		}
	}

	protected override void OnValueChanged(float value)
	{
		dirty = true;
	}
}
