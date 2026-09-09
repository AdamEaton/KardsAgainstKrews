using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ResponseBuilder : CustomBehaviour
{
	Dictionary<PlayerData, List<CardData>> responses = new Dictionary<PlayerData,List<CardData>>();

	bool checkPointActive;

	CardData call;

	void OnEnable()
	{
		EventSystem.AddListener<NewCallEvent>(OnNewCall);
		EventSystem.AddListener<ConfirmHandSelectionEvent>(OnConfirmHandSelection);
		EventSystem.AddListener<BeginCheckPointEvent>(OnBeginCheckPoint);
		EventSystem.AddListener<CheckPointCompleteEvent>(OnCheckPointComplete);
		EventSystem.AddListener<PlayerFinishedCheckPointEvent>(OnPlayerFinishedCheckPoint);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<NewCallEvent>(OnNewCall);
		EventSystem.RemoveListener<ConfirmHandSelectionEvent>(OnConfirmHandSelection);
		EventSystem.RemoveListener<BeginCheckPointEvent>(OnBeginCheckPoint);
		EventSystem.RemoveListener<CheckPointCompleteEvent>(OnCheckPointComplete);
		EventSystem.RemoveListener<PlayerFinishedCheckPointEvent>(OnPlayerFinishedCheckPoint);
	}

	void OnNewCall(NewCallEvent e)
	{
		responses.Clear();
		call = e.newCall;
	}
	void OnConfirmHandSelection(ConfirmHandSelectionEvent e)
	{
		if (!e.fromServer)
			return;

		List<CardData> targetList = GetResponse(e.player);

		if (!call || !checkPointActive && (targetList.Count >= call.responseCount || targetList.Any(x => x.id == e.card.id)))
			return;

		PlayCard(targetList, e.player, e.card);
	}
	void OnBeginCheckPoint(BeginCheckPointEvent e)
	{
		responses.Clear();
		checkPointActive = true;
	}
	void OnCheckPointComplete(CheckPointCompleteEvent e)
	{
		checkPointActive = false;
	}
	void OnPlayerFinishedCheckPoint(PlayerFinishedCheckPointEvent e)
	{
		var response = GetResponse(e.player);

		if (response.Count <= 0)
			response.Add(new CardData());

		EventSystem.Dispatch(new ResponseBuiltEvent(e.player, response));
	}
	void PlayCard(List<CardData> response, PlayerData player, CardData card)
	{
		player.Discard(card);
		response.Add(card);
		if (!checkPointActive && response.Count >= call.responseCount)
		{
			EventSystem.Dispatch(new ResponseBuiltEvent(player, response));
		}
	}

	List<CardData> GetResponse(PlayerData player)
	{
		List<CardData> output;
		if (responses.TryGetValue(player, out output))
			return output;

		output = new List<CardData>();
		responses[player] = output;

		return output;
	}
}

public class ResponseBuiltEvent : EventInstance
{
	public PlayerData player;
	public CardData[] cards;

	public ResponseBuiltEvent(PlayerData player, IEnumerable<CardData> cards)
	{
		this.player = player;
		this.cards = cards.ToArray();
	}
}