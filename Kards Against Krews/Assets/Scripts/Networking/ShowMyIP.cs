using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ShowMyIP : CustomBehaviour
{
	[SerializeField]
	Text text;
	
	IEnumerator Start()
	{
		text.text = "Retrieving...";

		var myExtIPWWW = new WWW("http://checkip.dyndns.org");
		if (myExtIPWWW == null) yield break;
		yield return myExtIPWWW;
		var data = myExtIPWWW.text;
		data = data.Substring(data.IndexOf(": ") + 2);
		data = data.Substring(0, data.IndexOf("<"));
		text.text = data;
	}
}
