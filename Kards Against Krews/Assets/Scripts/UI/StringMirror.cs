using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class StringMirror : MonoBehaviour
{
	public Text source;

	Text _destination;
	Text destination
	{
		get
		{
			if (!_destination)
				_destination = GetComponent<Text>();
			return _destination;
		}
	}

	void Update()
	{
		if (!source)
			return;

		destination.text = source.text;
	}
}