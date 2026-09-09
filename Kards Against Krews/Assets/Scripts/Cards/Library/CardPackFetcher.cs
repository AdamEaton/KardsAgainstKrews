using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public abstract class CardPackFetcher : CustomBehaviour
{
	[SerializeField]
	protected Game game;

	public abstract IEnumerable<string> GetPackNames();
	public abstract CardPack FetchPack(string name);

	public string AdjustCardText(string text)
	{
		if (string.IsNullOrEmpty(text))
			return null;

		int replaceIndex = text.IndexOf('~');
		if (replaceIndex < 0)
			return text;

		HashSet<string> availableNames = new HashSet<string>();
		foreach (var name in game.GetPlayers().Select(x => x.playerName))
			availableNames.Add(name);

		while (replaceIndex >= 0)
		{
			var name = availableNames.RandomElementOrDefault();
			if (string.IsNullOrEmpty(name))
				return null;
			availableNames.Remove(name);

			text = text.Substring(0, replaceIndex) + name + text.Substring(replaceIndex + 1);
			replaceIndex = text.IndexOf('~');
		}

		return text;
	}
}
