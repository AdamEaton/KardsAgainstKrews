using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class LocalCardPackListDisplay : ListDisplay<string>
{
	[SerializeField]
	Game game;
	[SerializeField]
	GameMetrics targetMetrics;
	[SerializeField]
	DeckManager deck;
	[SerializeField]
	LocalCardPackFetcher fetcher;

	public UnityEvent onConfirm;

	void OnEnable()
	{
		Refresh();
	}

	public void Confirm()
	{
		if (!game || game.GetPlayers().Count() <= 0)
			return;

		if (GetSelectedPacks().Count() <= 0 || ((targetMetrics.blankCardCount + targetMetrics.personalCardCount) <= 0 && targetMetrics.disablePackResponses))
			return;

		if (deck)
			deck.InitializeWithPacks(GetSelectedPacks(), targetMetrics);

		onConfirm.TryInvoke();
	}

	public void SelectAll()
	{
		foreach (var item in currentItems)
			(item as LocalCardPackDisplayItem).isIncluded = true;
	}
	public void DeselectAll()
	{
		foreach (var item in currentItems)
			(item as LocalCardPackDisplayItem).isIncluded = false;
	}

	protected override IEnumerable<string> GetUnfilteredItems()
	{
		foreach (var item in fetcher.GetPackNames())
			yield return item;
	}

	public IEnumerable<CardPack> GetSelectedPacks()
	{
		foreach (var pack in currentItems.OfType<LocalCardPackDisplayItem>().Where(x => x.isIncluded).Select(x => fetcher.FetchPack(x.currentItem)))
			yield return pack;
	}
}
