using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class ShowVersionNumber : CustomBehaviour
{
	public string format;

	void OnEnable()
	{
		GetComponent<Text>().text = string.Format(format, Application.version);
	}
}
