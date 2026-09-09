using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class UIFormatTextInt : CustomBehaviour
{
	[SerializeField]
	string format = "{0}";

	Text text;

	void Awake()
	{
		text = GetComponent<Text>();
	}

	public void SetValue(int value)
	{
		text.text = string.Format(format, value);
	}
}