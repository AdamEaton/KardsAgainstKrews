using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModalController : Singleton<ModalController>
{
	[SerializeField]
	DialogueModal dialogue;
	[SerializeField]
	TextInputModal textInput;

	protected override bool dontDestroyOnLoad { get { return false; } }

	public bool isDialogueModalShowing { get { return dialogue.isActiveAndEnabled; } }
	public bool isTextInputModalShowing { get { return textInput.isActiveAndEnabled; } }

	void OnEnable()
	{
		EventSystem.AddListener<DialogueModal.ShowEvent>(OnShowDialogueModal);
		EventSystem.AddListener<DialogueModal.HideEvent>(OnHideDialogueModal);

		EventSystem.AddListener<TextInputModal.ShowEvent>(OnShowTextInputModal);
		EventSystem.AddListener<TextInputModal.HideEvent>(OnHideTextInputModal);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<DialogueModal.ShowEvent>(OnShowDialogueModal);
		EventSystem.RemoveListener<DialogueModal.HideEvent>(OnHideDialogueModal);

		EventSystem.RemoveListener<TextInputModal.ShowEvent>(OnShowTextInputModal);
		EventSystem.RemoveListener<TextInputModal.HideEvent>(OnHideTextInputModal);
	}

	void OnShowDialogueModal(DialogueModal.ShowEvent e)
	{
		dialogue.transform.SetAsLastSibling();
		dialogue.Show(e);
	}
	void OnHideDialogueModal(DialogueModal.HideEvent e)
	{
		dialogue.Hide();
	}

	void OnShowTextInputModal(TextInputModal.ShowEvent e)
	{
		textInput.transform.SetAsLastSibling();
		textInput.Show(e);
	}
	void OnHideTextInputModal(TextInputModal.HideEvent e)
	{
		textInput.Hide();
	}
}