using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Scoreboard : CustomBehaviour
{
	Dictionary<string, int> scores = new Dictionary<string, int>();

	public IEnumerable<string> GetScoringPlayers()
	{
		foreach (var player in scores.Keys)
			yield return player;
	}

	public int this[PlayerData player]
	{
		get { return this[player.playerName]; }
		set { this[player.playerName] = value; }
	}
	public int this[string name]
	{
		get
		{
			if (!scores.ContainsKey(name)) return 0;
			return scores[name];
		}
		set
		{
			scores[name] = value;
			EventSystem.Dispatch(new UpdateEvent());
		}
	}

	public void Clear()
	{
		scores.Clear();
		EventSystem.Dispatch(new ClearedEvent());
	}

	void OnEnable()
	{
		EventSystem.AddListener<PlayerScoredEvent>(OnPlayerScored);
		EventSystem.AddListener<NewRoundEvent>(OnNewRound);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<PlayerScoredEvent>(OnPlayerScored);
		EventSystem.RemoveListener<NewRoundEvent>(OnNewRound);
	}

	void OnPlayerScored(PlayerScoredEvent e)
	{
		this[e.player]++;
	}
	void OnNewRound(NewRoundEvent e)
	{
		EventSystem.Dispatch(new UpdateEvent());
	}

	public class UpdateEvent : EventInstance { }
	public class ClearedEvent : EventInstance { }
}

public class PlayerScoredEvent : EventInstance
{
	public PlayerData player;

	public PlayerScoredEvent(PlayerData player)
	{
		this.player = player;
	}
}
