using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DontDestroyOnLoad : CustomBehaviour
{
	void Awake()
	{
		DontDestroyOnLoad(this);
	}
}
