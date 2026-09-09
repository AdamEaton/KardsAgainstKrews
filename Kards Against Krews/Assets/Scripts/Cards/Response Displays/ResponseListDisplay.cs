using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResponseListDisplay : ListDisplay<ResponseData>
{
	[SerializeField]
	Game game;

	public bool showing { get { return game.forceReveal || game.hasAllResponses; } }

	protected override IEnumerable<ResponseData> GetUnfilteredItems()
	{
		foreach (var response in game.GetResponses())
			yield return response;
	}

	void OnEnable()
	{
		EventSystem.AddListener<ResponsesUpdatedEvent>(OnResponsesUpdated);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<ResponsesUpdatedEvent>(OnResponsesUpdated);
	}

	public void UnhighlightAllOptions()
	{
		foreach (var item in currentItems)
		{
			(item as ResponseDisplayItem).SetHighlight(false);
		}
	}

	void OnResponsesUpdated(ResponsesUpdatedEvent e)
	{
		Refresh();
	}
}
