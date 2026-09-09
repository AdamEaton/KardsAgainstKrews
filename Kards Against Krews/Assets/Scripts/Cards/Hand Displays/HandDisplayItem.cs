using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class HandDisplayItem : ListDisplayItem<CardData>
{
	public Text textDisplay;
	public Text packDisplay;
	public string blankText;

	public UnityEvent onUnhighlight;
	public UnityEvent onHighlight;

	bool highlighted;

	public bool selectionDisabled { get; set; }
	
	public override void Populate(ListDisplay<CardData> owner, CardData item)
	{
		base.Populate(owner, item);

		if (textDisplay)
			textDisplay.text = item.isBlank ? blankText : item.text;
		if (packDisplay)
			packDisplay.text = string.Format("<i>{0}</i>", item.pack);
	}

	public void HighlightOrConfirm()
	{
		if (!highlighted)
		{
			(owner as HandListDisplay).UnhighlightAllItems();
			SetHighlight(true);
		}
		else
		{
			Confirm();
		}
	}

	public void SetHighlight(bool highlighted)
	{
		if (selectionDisabled && highlighted)
			return;

		this.highlighted = highlighted;
		(highlighted ? onHighlight : onUnhighlight).TryInvoke();
	}

	public void Confirm()
	{
		if (selectionDisabled)
			return;

		if (currentItem.isBlank)
		{
			EventSystem.Dispatch(GetCustomResponseModalEvent());
		}
		else
		{
			EventSystem.Dispatch(new ConfirmHandSelectionEvent((owner as HandListDisplay).player, currentItem, false));
		}
	}
	
	TextInputModal.ShowEvent GetCustomResponseModalEvent()
	{
		return new TextInputModal.ShowEvent(
			"WHAT'S THAT?",
			"Fill out a custom response for this card.",
			"",
			128,
			"Alert",
			new TextInputModal.ButtonInfo("Cancel", x =>
				{
					EventSystem.Dispatch(new TextInputModal.HideEvent());
				}),
			new TextInputModal.ButtonInfo("Submit", OnCustomResponseModalSubmit)
			);
	}
	TextInputModal.ShowEvent GetCustomResponseErrorModalEvent(string response, string error)
	{
		return new TextInputModal.ShowEvent(
			"SERIOUSLY, WHAT'S THAT?",
			"There was a problem with your previous response: " + error,
			response,
			128,
			"Alert",
			new TextInputModal.ButtonInfo("Cancel", x =>
				{
					EventSystem.Dispatch(new TextInputModal.HideEvent());
				}),
			new TextInputModal.ButtonInfo("Submit Better", OnCustomResponseModalSubmit)
			);
	}
	void OnCustomResponseModalSubmit(string response)
	{
		EventSystem.Dispatch(new TextInputModal.HideEvent());
		CommonAudioLibrary.Instance.Play("Submit");

		string error;
		if (!ValidateResponse(response, out error))
		{
			EventSystem.Dispatch(GetCustomResponseErrorModalEvent(response, error));
			return;
		}

		currentItem.text = response;
		EventSystem.Dispatch(new ConfirmHandSelectionEvent((owner as HandListDisplay).player, currentItem, false));
	}

	bool ValidateResponse(string response, out string error)
	{
		#region Response is blank
		if (string.IsNullOrEmpty(response.Trim()))
		{
			error = "You forgot to write it.";
			return false;
		}
		#endregion
		#region Disallowed tags
		{
			int openTagIndex = -1;
			int closeTagIndex = 0;
			string tagContent;
			while (openTagIndex < response.Length - 1)
			{
				openTagIndex = response.IndexOf('<', openTagIndex + 1);
				if (openTagIndex < 0)
					break;

				closeTagIndex = response.IndexOf('>', openTagIndex + 1);
				if (closeTagIndex < 0)
					break;

				tagContent = response.Substring(openTagIndex + 1, closeTagIndex - (openTagIndex + 1)).Trim().ToLowerInvariant();

				if (tagContent.Contains("size") || tagContent.Contains("material") || tagContent.Contains("quad"))
				{
					error = "A little too creative with the <tags>, wise guy.";
					return false;
				}
			}
		}
		#endregion

		error = null;
		return true;
	}
}

public class ConfirmHandSelectionEvent : EventInstance
{
	public PlayerData player;
	public CardData card;
	public bool fromServer;

	public ConfirmHandSelectionEvent(PlayerData player, CardData card, bool fromServer)
	{
		this.player = player;
		this.card = card;
		this.fromServer = fromServer;
	}
}