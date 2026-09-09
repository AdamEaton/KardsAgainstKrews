using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class LinkAudioVolumeToPlayerPref : CustomBehaviour
{
	AudioSource _source;
	public AudioSource source
	{
		get
		{
			if (!_source) _source = GetComponent<AudioSource>();
			return _source;
		}
	}

	public string key;
	public float defaultValue;

	void Refresh()
	{
		source.volume = PlayerPrefs.GetFloat(key, defaultValue);
	}

	void OnEnable()
	{
		Refresh();
		EventSystem.AddListener<UpdateVolumeEvent>(OnUpdateVolume);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<UpdateVolumeEvent>(OnUpdateVolume);
	}

	void OnUpdateVolume(UpdateVolumeEvent e)
	{
		Refresh();
	}
}

public class UpdateVolumeEvent : EventInstance { }