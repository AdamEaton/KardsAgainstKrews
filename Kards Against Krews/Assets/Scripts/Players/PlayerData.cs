using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
	public int id;
	public string playerName;
	List<CardData> hand = new List<CardData>();

	public int handCount { get { return hand.Count; } }
	public IEnumerable<CardData> GetHand()
	{
		foreach (var card in hand)
			yield return card;
	}

	public void ClearHand()
	{
		hand.Clear();

		EventSystem.Dispatch(new HandUpdatedEvent(this));
	}

	public bool Draw(CardData card)
	{
		if (!card) return false;

		hand.Add(card);
		EventSystem.Dispatch(new DrawCardEvent(this, card));
		EventSystem.Dispatch(new HandUpdatedEvent(this));
		return true;
	}
	public bool Discard(CardData card)
	{
		var output = hand.Remove(card);
		EventSystem.Dispatch(new DiscardCardEvent(this, card));
		EventSystem.Dispatch(new HandUpdatedEvent(this));
		return output;
	}
	public bool Discard(int cardID)
	{
		foreach (var card in hand)
			if (card.id == cardID)
			{
				return Discard(card);
			}
		return false;
	}
}

public class DrawCardEvent : EventInstance
{
	public PlayerData player;
	public CardData card;

	public DrawCardEvent(PlayerData player, CardData card)
	{
		this.player = player;
		this.card = card;
	}
}
public class DiscardCardEvent : EventInstance
{
	public PlayerData player;
	public CardData card;

	public DiscardCardEvent(PlayerData player, CardData card)
	{
		this.player = player;
		this.card = card;
	}
}
public class HandUpdatedEvent : EventInstance
{
	public PlayerData player;

	public HandUpdatedEvent(PlayerData player)
	{
		this.player = player;
	}
}