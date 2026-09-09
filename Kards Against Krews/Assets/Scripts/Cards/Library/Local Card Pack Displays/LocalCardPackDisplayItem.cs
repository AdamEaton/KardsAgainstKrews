using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LocalCardPackDisplayItem : ListDisplayItem<string>
{
	[SerializeField]
	Text textDisplay;
	[SerializeField]
	Toggle toggle;

	public bool isIncluded
	{
		get { return toggle && toggle.isOn; }
		set { if (toggle) toggle.isOn = value; }
	}

	public override void Populate(ListDisplay<string> owner, string item)
	{
		base.Populate(owner, item);

		if (textDisplay)
			textDisplay.text = Path.GetFileNameWithoutExtension(item);
	}
}
