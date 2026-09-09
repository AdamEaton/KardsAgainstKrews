using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ResponseData
{
	public PlayerData player;
	public CardData[] cards;

	public ResponseData(PlayerData player, IEnumerable<CardData> cards)
	{
		this.player = player;
		this.cards = cards.ToArray();
	}
}