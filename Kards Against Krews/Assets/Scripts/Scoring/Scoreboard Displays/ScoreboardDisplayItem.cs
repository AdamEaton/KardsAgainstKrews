using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class ScoreboardDisplayItem : ListDisplayItem<PlayerData>
{
	[SerializeField]
	Game game;
	[SerializeField]
	Scoreboard scoreboard;

	[SerializeField]
	Text nameDisplay;
	[SerializeField]
	Text scoreDisplay;

	[SerializeField]
	[Tooltip("{0} for score")]
	string scoreFormat;
	[SerializeField]
	[Tooltip("{0} for score")]
	string singularScoreFormat;

	public UnityEvent onCzar;
	public UnityEvent onNotCzar;
	public UnityEvent onWon;
	public UnityEvent onLost;

	public override void Populate(ListDisplay<PlayerData> owner, PlayerData item)
	{
		base.Populate(owner, item);

		nameDisplay.text = item.playerName;
		scoreDisplay.text = string.Format(scoreboard[item] == 1 ? singularScoreFormat : scoreFormat, scoreboard[item]);

		(game.currentCzar == item ? onCzar : onNotCzar).TryInvoke();
		(game.currentWinner == item ? onWon : onLost).TryInvoke();
	}

	public void Ping()
	{
		EventSystem.Dispatch(new SendPingEvent(currentItem));
	}
	public void Kick()
	{
		EventSystem.Dispatch(new DialogueModal.ShowEvent(
			"THIS GUY BOTHERING YOU?",
			"Kick player '" + currentItem.playerName + "'?",
			"Submit",
			new DialogueModal.ButtonInfo("Cancel", OnCancelKick),
			new DialogueModal.ButtonInfo("Kick", OnConfirmKick)
			));
	}

	void OnCancelKick()
	{
		EventSystem.Dispatch(new DialogueModal.HideEvent());
	}
	void OnConfirmKick()
	{
		EventSystem.Dispatch(new DialogueModal.HideEvent());
		EventSystem.Dispatch(new KickPlayerEvent(currentItem));
	}
}
