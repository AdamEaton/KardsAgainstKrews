using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Graphic))]
public class UIGraphicColorFlash : CustomBehaviour
{
	Graphic _graphic;
	Graphic graphic
	{
		get
		{
			if (!_graphic) _graphic = GetComponent<Graphic>();
			return _graphic;
		}
	}

	[SerializeField]
	Color baseColor;
	[SerializeField]
	Color[] colors;
	[SerializeField]
	float flashTime = 0f;

	public void Flash(int index)
	{
		StopAllCoroutines();

		if (flashTime <= 0)
			return;

		StartCoroutine(DoSetColorIndex(index));
	}

	IEnumerator DoSetColorIndex(int index)
	{
		float startTime = Time.time;
		Color startColor = colors[index];

		while (Time.time < startTime + flashTime)
		{
			graphic.color = Color.Lerp(startColor, baseColor, Mathf.InverseLerp(startTime, startTime + flashTime, Time.time));
			yield return null;
		}
		graphic.color = baseColor;
	}
}