using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PartialLoopingAudio : CustomBehaviour
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

	[SerializeField]
	float loopStart;
	[SerializeField]
	float loopEnd;

	void Update()
	{
		if (source.time >= loopEnd)
			source.time -= loopEnd - loopStart;
	}
}
