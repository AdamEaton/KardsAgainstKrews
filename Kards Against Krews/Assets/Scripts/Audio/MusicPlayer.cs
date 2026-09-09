using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : CustomBehaviour
{
	public static MusicPlayer Instance;

	AudioSource _source;
	public AudioSource source
	{
		get
		{
			if (!_source) _source = GetComponent<AudioSource>();
			return _source;
		}
	}

	public static float currentTime
	{
		get
		{
			if (Instance)
				return Instance.source.time;
			return 0;
		}
		set
		{
			if (!Instance)
				return;
			Instance.source.time = value;
		}
	}

	public static void Play()
	{
		if (!Instance)
			return;

		Instance.source.Play();
	}

	void Awake()
	{
		Instance = this;
	}
}
