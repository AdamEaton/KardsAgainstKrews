using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Graphic))]
public class UIGraphicColorizer : CustomBehaviour
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
	Color[] colors;
	[SerializeField]
	float transitionTime = 0f;

	int? targetIndex;

	void OnEnable()
	{
		if (targetIndex.HasValue)
			StartCoroutine(DoSetColorIndex(targetIndex.Value));
	}

	public void SetColorIndex(int index)
	{
		StopAllCoroutines();

		if (transitionTime <= 0)
			graphic.color = colors[index];
		else if (isActiveAndEnabled)
			StartCoroutine(DoSetColorIndex(index));
		else
			targetIndex = index;
	}

	IEnumerator DoSetColorIndex(int index)
	{
		float startTime = Time.time;
		Color startColor = graphic.color;

		while (Time.time < startTime + transitionTime)
		{
			graphic.color = Color.Lerp(startColor, colors[index], Mathf.InverseLerp(startTime, startTime + transitionTime, Time.time));
			yield return null;
		}
		graphic.color = colors[index];
		targetIndex = null;
	}
}