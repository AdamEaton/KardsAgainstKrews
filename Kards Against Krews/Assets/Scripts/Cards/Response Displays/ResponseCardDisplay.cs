using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResponseCardDisplay : CustomBehaviour
{
	[SerializeField]
	Text cardTextDisplay;
	[SerializeField]
	Text packTextDisplay;
	[SerializeField]
	Text playerNameDisplay;

	public Transform nextCardHolder;

	public ResponseCardDisplay nextCard { get; set; }

	public UnityEvent onHidden;
	public UnityEvent onShowing;

	public UnityEvent onUnhighlight;
	public UnityEvent onHighlight;

	public UnityEvent onLost;
	public UnityEvent onWon;

	public void Populate(PlayerData player, CardData card)
	{
		if (cardTextDisplay)
			cardTextDisplay.text = card.text;
		if (packTextDisplay)
			packTextDisplay.text = string.Format("<i>{0}</i>", card.pack);
		if (playerNameDisplay)
			playerNameDisplay.text = player.playerName;
	}

	public void SetVisibility(bool visible)
	{
		(visible ? onShowing : onHidden).TryInvoke();
		if (nextCard)
			nextCard.SetVisibility(visible);
	}
	public void SetVictoryStatus(bool victory)
	{
		(victory ? onWon : onLost).TryInvoke();
		if (nextCard)
			nextCard.SetVictoryStatus(victory);
	}
	public void SetHighlight(bool highlighted)
	{
		(highlighted ? onHighlight : onUnhighlight).TryInvoke();
		if (nextCard)
			nextCard.SetHighlight(highlighted);
	}
}