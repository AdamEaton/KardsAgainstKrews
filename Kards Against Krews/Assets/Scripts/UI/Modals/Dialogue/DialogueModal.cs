using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DialogueModal : ListDisplay<DialogueModal.ButtonInfo>
{
	[SerializeField]
	Text headerText;
	[SerializeField]
	Text bodyText;

	List<ButtonInfo> buttons = new List<ButtonInfo>();

	public void Show(ShowEvent e)
	{
		gameObject.SetActive(true);

		if (headerText)
			headerText.text = e.headerText;
		if (bodyText)
			bodyText.text = e.bodyText;

		this.buttons = e.buttons;

		CommonAudioLibrary.Instance.Play(e.audioID);

		Refresh();
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Execute(ButtonInfo button)
	{
		button.onClick();
	}

	protected override IEnumerable<ButtonInfo> GetUnfilteredItems()
	{
		foreach (var button in buttons)
			yield return button;
	}

	public class ButtonInfo
	{
		public string label;
		public Action onClick;

		public ButtonInfo(string label, Action onClick)
		{
			this.label = label;
			this.onClick = onClick;
		}
	}

	public class ShowEvent : EventInstance
	{
		public string headerText;
		public string bodyText;
		public string audioID;
		public List<DialogueModal.ButtonInfo> buttons;

		public ShowEvent(string headerText, string bodyText, string audioID, params DialogueModal.ButtonInfo[] buttons)
		{
			this.headerText = headerText;
			this.bodyText = bodyText;
			this.audioID = audioID;
			this.buttons = buttons.ToList();
		}
	}
	public class HideEvent : EventInstance { }
}