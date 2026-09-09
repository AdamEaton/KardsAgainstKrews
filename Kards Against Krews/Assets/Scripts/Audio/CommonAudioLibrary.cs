using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CommonAudioLibrary : Singleton<CommonAudioLibrary>
{
	const float MinimumWaitTime = 0.05f;

	[SerializeField]
	string volumePref;

	[SerializeField]
	List<AudioData> data;

	public static float MasterVolume { get { return PlayerPrefs.GetFloat(Instance.volumePref, 0); } }

	protected override bool dontDestroyOnLoad { get { return true; } }

	List<AudioSource> sourcePool = new List<AudioSource>();
	AudioSource GetSource()
	{
		return sourcePool.FirstOrDefault(x => !x.isPlaying) ?? CreateSource();
	}
	AudioSource CreateSource()
	{
		var output = new GameObject("Common Audio Source").AddComponent<AudioSource>();
		output.transform.parent = transform;

		output.playOnAwake = false;
		
		sourcePool.Add(output);
		return output;
	}

	public void Play(string name)
	{
		if (string.IsNullOrEmpty(name))
			return;

		foreach (var sound in data)
		{
			if (sound.name == name)
			{
				sound.Play(GetSource());
			}
		}
	}

	[System.Serializable]
	class AudioData
	{
		public string name = "";
		public AudioClip[] clips = new AudioClip[0];
		public Range volumeRange = new Range(1, 1);
		public Range pitchRange = new Range(1, 1);

		public float lastPlayTime { get; private set; }

		public void Play(AudioSource source)
		{
			if (lastPlayTime + MinimumWaitTime > Time.time) return;

			source.volume = volumeRange.RandomValue * CommonAudioLibrary.MasterVolume;
			source.pitch = pitchRange.RandomValue;
			source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
			lastPlayTime = Time.time;
		}
	}
}