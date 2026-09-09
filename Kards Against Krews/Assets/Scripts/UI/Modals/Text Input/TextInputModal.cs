using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TextInputModal : ListDisplay<TextInputModal.ButtonInfo>
{
	[SerializeField]
	Text headerText;
	[SerializeField]
	Text bodyText;
	[SerializeField]
	InputField inputField;

	List<ButtonInfo> buttons = new List<ButtonInfo>();

	void Start()
	{
		if (inputField)
			inputField.GetOrAddComponent<ListenForInputFieldSubmit>().onSubmit.AddListener(OnSubmit);
	}

	public void Show(ShowEvent e)
	{
		gameObject.SetActive(true);

		if (headerText)
			headerText.text = e.headerText;
		if (bodyText)
			bodyText.text = e.bodyText;
		if (inputField)
		{
			inputField.characterLimit = e.characterLimit;
			inputField.text = e.defaultInput;
		}

		CommonAudioLibrary.Instance.Play(e.audioID);

		this.buttons = e.buttons;

		Refresh();
	}
	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Execute(ButtonInfo button)
	{
		if (inputField)
			button.onClick(inputField.text);
	}

	protected override IEnumerable<ButtonInfo> GetUnfilteredItems()
	{
		foreach (var button in buttons)
			yield return button;
	}

	void OnSubmit(string text)
	{
		if (buttons.Count <= 0)
			return;

		CommonAudioLibrary.Instance.Play("Select");
		Execute(buttons.Last());
	}

	public class ButtonInfo
	{
		public string label;
		public Action<string> onClick;

		public ButtonInfo(string label, Action<string> onClick)
		{
			this.label = label;
			this.onClick = onClick;
		}
	}

	public class ShowEvent : EventInstance
	{
		public string headerText;
		public string bodyText;
		public string defaultInput;
		public int characterLimit;
		public string audioID;
		public List<TextInputModal.ButtonInfo> buttons;

		public ShowEvent(string headerText, string bodyText, string defaultInput, int characterLimit, string audioID, params TextInputModal.ButtonInfo[] buttons)
		{
			this.headerText = headerText;
			this.bodyText = bodyText;
			this.defaultInput = defaultInput;
			this.characterLimit = characterLimit;
			this.audioID = audioID;
			this.buttons = buttons.ToList();
		}
	}
	public class HideEvent : EventInstance { }
}