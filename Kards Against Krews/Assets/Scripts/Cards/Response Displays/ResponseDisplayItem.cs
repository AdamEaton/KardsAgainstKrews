using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResponseDisplayItem : ListDisplayItem<ResponseData>
{
	[SerializeField]
	Game game;
	[SerializeField]
	GameClient client;

	[SerializeField]
	Transform cardHolder;
	[SerializeField]
	ResponseCardDisplay cardPrefab;

	ResponseCardDisplay firstCard;

	public UnityEvent onHidden;
	public UnityEvent onShowing;

	public UnityEvent onUnhighlight;
	public UnityEvent onHighlight;

	public UnityEvent onLost;
	public UnityEvent onWon;

	bool showing { get { return (owner as ResponseListDisplay).showing; } }
	bool highlighted;

	public bool selectionDisabled { get; set; }

	public override void Populate(ListDisplay<ResponseData> owner, ResponseData item)
	{
		base.Populate(owner, item);

		Transform holder = cardHolder;
		ResponseCardDisplay currentCard = null;
		foreach (var card in item.cards)
		{
			if (currentCard)
			{
				currentCard.nextCard = Instantiate(cardPrefab);
				currentCard = currentCard.nextCard;
			}
			else
			{
				firstCard = Instantiate(cardPrefab);
				currentCard = firstCard;
			}
			currentCard.gameObject.SetActive(true);
			holder.gameObject.SetActive(true);
			currentCard.transform.SetParent(holder, false);
			currentCard.transform.SetAsFirstSibling();
			currentCard.transform.localScale = Vector3.one;
			currentCard.Populate(item.player, card);
			holder = currentCard.nextCardHolder;
		}

		CheckStatus();
	}

	public void HighlightOrConfirm()
	{
		if (!highlighted)
		{
			(owner as ResponseListDisplay).UnhighlightAllOptions();
			SetHighlight(true);
		}
		else
		{
			Confirm();
		}
	}

	public void SetHighlight(bool highlighted)
	{
		if (!showing)
			return;

		if (!client || game.currentWinner || client.me.id != game.currentCzar.id)
			return;

		if (selectionDisabled && highlighted)
			return;

		this.highlighted = highlighted;
		(highlighted ? onHighlight : onUnhighlight).TryInvoke();

		if (firstCard)
			firstCard.SetHighlight(highlighted);
	}

	public void Confirm()
	{
		if (selectionDisabled)
			return;

		EventSystem.Dispatch(new ConfirmResponseSelectionEvent(currentItem, false));
	}

	void OnEnable()
	{
		EventSystem.AddListener<ResponsesUpdatedEvent>(OnResponsesUpdated);
		EventSystem.AddListener<ResponseWonEvent>(OnResponseWon);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<ResponsesUpdatedEvent>(OnResponsesUpdated);
		EventSystem.RemoveListener<ResponseWonEvent>(OnResponseWon);
	}

	void OnResponsesUpdated(ResponsesUpdatedEvent e)
	{
		CheckStatus();
	}
	void OnResponseWon(ResponseWonEvent e)
	{
		(e.response == currentItem ? onWon : onLost).TryInvoke();

		if (firstCard)
			firstCard.SetVictoryStatus(e.response == currentItem);
	}

	void CheckStatus()
	{
		(showing ? onShowing : onHidden).TryInvoke();

		if (firstCard)
			firstCard.SetVisibility(showing);
	}
}

public class ConfirmResponseSelectionEvent : EventInstance
{
	public ResponseData response;
	public bool fromServer;

	public ConfirmResponseSelectionEvent(ResponseData response, bool fromServer)
	{
		this.response = response;
		this.fromServer = fromServer;
	}
}
public class ResponseWonEvent : EventInstance
{
	public ResponseData response;

	public ResponseWonEvent(ResponseData response)
	{
		this.response = response;
	}
}