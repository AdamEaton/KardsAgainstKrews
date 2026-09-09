using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandListDisplay : ListDisplay<CardData>
{
	public PlayerData player;

	protected override IEnumerable<CardData> GetUnfilteredItems()
	{
		foreach (var card in player.GetHand())
			yield return card;
	}

	void OnEnable()
	{
		EventSystem.AddListener<LocalPlayerDataCreatedEvent>(OnLocalPlayerDataCreated);
		EventSystem.AddListener<HandUpdatedEvent>(OnHandUpdated);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<LocalPlayerDataCreatedEvent>(OnLocalPlayerDataCreated);
		EventSystem.RemoveListener<HandUpdatedEvent>(OnHandUpdated);
	}

	public void UnhighlightAllItems()
	{
		foreach (var item in currentItems)
		{
			(item as HandDisplayItem).SetHighlight(false);
		}
	}

	void OnLocalPlayerDataCreated(LocalPlayerDataCreatedEvent e)
	{
		player = e.player;
	}
	void OnHandUpdated(HandUpdatedEvent e)
	{
		if (e.player != player)
			return;

		Refresh();
	}
}
