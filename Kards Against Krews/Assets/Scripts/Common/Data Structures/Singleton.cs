using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Forces a derived class to be a lazy-loaded singleton.
/// </summary>
/// <typeparam name="T">Must be populated with the type of the derived class.</typeparam>
public abstract class Singleton<T> : CustomBehaviour where T : Singleton<T>
{
	/// <summary>
	/// Internal reference to the singleton instance.
	/// </summary>
	static T _Instance;
	/// <summary>
	/// Returns the current singleton instance, and creates one if it doesn't exist.
	/// </summary>
	public static T Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = new GameObject(typeof(T).ToString()).AddComponent<T>();

				if (_Instance.dontDestroyOnLoad)
					DontDestroyOnLoad(_Instance);
			}

			return _Instance;
		}
	}

	protected abstract bool dontDestroyOnLoad { get; }

	protected virtual void Awake()
	{
		if (_Instance == null)
		{
			_Instance = this as T;
			if (dontDestroyOnLoad)
				DontDestroyOnLoad(this);
		}

		if (_Instance != this)
			Destroy(gameObject);
	}
}