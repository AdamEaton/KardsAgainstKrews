using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScoreboardListDisplay : ListDisplay<PlayerData>
{
	[SerializeField]
	Game game;
	[SerializeField]
	Scoreboard scoreboard;

	protected override IEnumerable<PlayerData> GetUnfilteredItems()
	{
		foreach (var player in game.GetPlayers().OrderByDescending(x => scoreboard[x]))
			yield return player;
	}

	void OnEnable()
	{
		EventSystem.AddListener<Scoreboard.UpdateEvent>(OnScoreboardUpdate);
		EventSystem.AddListener<NewCzarEvent>(OnNewCzar);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<Scoreboard.UpdateEvent>(OnScoreboardUpdate);
		EventSystem.RemoveListener<NewCzarEvent>(OnNewCzar);
	}

	void OnScoreboardUpdate(Scoreboard.UpdateEvent e)
	{
		Refresh();
	}
	void OnNewCzar(NewCzarEvent e)
	{
		Refresh();
	}
}
