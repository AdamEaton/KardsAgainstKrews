using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DeckManager : CustomBehaviour
{
	[SerializeField]
	Game game;

	List<CardData> callCards = new List<CardData>();
	List<CardData> responseCards = new List<CardData>();

	List<CardData> currentCalls = new List<CardData>();
	List<CardData> currentResponses = new List<CardData>();

	int callIndex;
	int responseIndex;

	int cardIndex;

	public void InitializeWithPacks(IEnumerable<CardPack> packs, GameMetrics metrics)
	{
		cardIndex = 0;
		callCards.Clear();
		responseCards.Clear();

		foreach (var pack in packs)
		{
			callCards.AddRange(pack.callCards);
			if (!metrics.disablePackResponses) responseCards.AddRange(pack.responseCards);
		}

		responseCards.AddRange(CardData.GetBlankCards(metrics.blankCardCount));
		foreach (var player in game.GetPlayers())
		{
			responseCards.AddRange(CardData.GetCards(player.playerName + ".", metrics.personalCardCount));
		}

		FlipCalls();
		FlipResponses();
	}

	public void FlipCalls()
	{
		currentCalls = callCards.Shuffled().ToList();
		callIndex = 0;
	}
	public void FlipResponses()
	{
		currentResponses = responseCards.Shuffled().Select(x => x.Clone()).ToList();
		responseIndex = 0;
	}

	public CardData GetNextCall()
	{
		if (game.metrics.maximumResponseCards > 0
			&& currentCalls.All(
				x => x.responseCount > 1
					&& x.responseCount * (game.GetPlayers().Count() - 1) > game.metrics.maximumResponseCards))
			return null;

		if (callIndex >= currentCalls.Count) FlipCalls();
		if (callIndex >= currentCalls.Count) return null;

		currentCalls[callIndex].id = cardIndex++;
		if (game.metrics.maximumResponseCards > 0
			&& currentCalls[callIndex].responseCount > 1
			&& currentCalls[callIndex].responseCount * (game.GetPlayers().Count() - 1) > game.metrics.maximumResponseCards)
		{
			callIndex++;
			return GetNextCall();
		}
		else return currentCalls[callIndex++];
	}
	public CardData GetNextResponse()
	{
		if (responseIndex >= currentResponses.Count) FlipResponses();
		if (responseIndex >= currentResponses.Count) return null;

		currentResponses[responseIndex].id = cardIndex++;
		return currentResponses[responseIndex++];
	}
}
