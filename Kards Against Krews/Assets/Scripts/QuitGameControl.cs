using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class QuitGameControl : CustomBehaviour
{
	[SerializeField]
	Game game;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
			ShowModal();
	}

	void ShowModal()
	{
		if (ModalController.Instance.isDialogueModalShowing)
			return;

		EventSystem.Dispatch(new DialogueModal.ShowEvent(
			"WHAT'S HAPPENING?",
			"Select an option below.",
			"Submit",
			new DialogueModal.ButtonInfo("Quit Game", () => { game.LeaveGame(); }),
			new DialogueModal.ButtonInfo("Settings", () =>
				{
					EventSystem.Dispatch(new ShowSettingsMenuEvent());
					EventSystem.Dispatch(new DialogueModal.HideEvent());
				}),
			new DialogueModal.ButtonInfo("Resume Game", () => { EventSystem.Dispatch(new DialogueModal.HideEvent()); })
			));
	}
}
