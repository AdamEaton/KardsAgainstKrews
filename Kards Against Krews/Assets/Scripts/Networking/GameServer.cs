using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LowLevelNetworking;

public class GameServer : CustomBehaviour
{
	[SerializeField]
	Game game;

	public UnityEvent onBeginCheckPoint;
	public UnityEvent onFinishCheckPoint;

	Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();
	Queue<PlayerData> czarQueue = new Queue<PlayerData>();

	Queue<PlayerData> limbo = new Queue<PlayerData>();

	NetServer server;

	#region Unity

	void Start()
	{
		var config = new ConnectionConfig();
		config.DisconnectTimeout = 30000;
		config.NetworkDropThreshold = 50;
		config.OverflowDropThreshold = 50;
		config.AddChannel(QosType.ReliableSequenced);
		NetManager.connectionConfig = config;
		NetManager.Init();

		server = NetManager.CreateServer(32, 7777);
		
		// Server Event Callbacks
		server.OnConnection = OnConnection;
		server.OnData = OnReceiveData;
		server.OnDisconnection = OnDisconnection;
		server.OnMessage = OnMessage;
	}
	void OnDestroy()
	{
		server.DisconnectAllClients();
		EventSystem.Dispatch(new DestroyServerEvent(server));
	}

	void OnEnable()
	{
		EventSystem.AddListener<DequeuePlayersEvent>(OnDequeuePlayers);
		EventSystem.AddListener<NewRoundEvent>(OnNewRound);
		EventSystem.AddListener<NewCallEvent>(OnNewCall);
		EventSystem.AddListener<DrawCardEvent>(OnDrawCard);
		EventSystem.AddListener<DiscardCardEvent>(OnDiscardCard);
		EventSystem.AddListener<ResponseBuiltEvent>(OnResponseBuilt);
		EventSystem.AddListener<GotAllResponsesEvent>(OnGotAllResponses);
		EventSystem.AddListener<Scoreboard.ClearedEvent>(OnScoreboardCleared);
		EventSystem.AddListener<KickPlayerEvent>(OnKickPlayer);
		EventSystem.AddListener<PlayerWonEvent>(OnPlayerWon);
		EventSystem.AddListener<NewGameEvent>(OnNewGame);
		EventSystem.AddListener<BeginCheckPointEvent>(OnBeginCheckPoint);
		EventSystem.AddListener<CheckPointCompleteEvent>(OnCheckPointComplete);
		EventSystem.AddListener<SendPingEvent>(OnSendPing);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<DequeuePlayersEvent>(OnDequeuePlayers);
		EventSystem.RemoveListener<NewRoundEvent>(OnNewRound);
		EventSystem.RemoveListener<NewCallEvent>(OnNewCall);
		EventSystem.RemoveListener<DrawCardEvent>(OnDrawCard);
		EventSystem.RemoveListener<DiscardCardEvent>(OnDiscardCard);
		EventSystem.RemoveListener<ResponseBuiltEvent>(OnResponseBuilt);
		EventSystem.RemoveListener<GotAllResponsesEvent>(OnGotAllResponses);
		EventSystem.RemoveListener<Scoreboard.ClearedEvent>(OnScoreboardCleared);
		EventSystem.RemoveListener<KickPlayerEvent>(OnKickPlayer);
		EventSystem.RemoveListener<PlayerWonEvent>(OnPlayerWon);
		EventSystem.RemoveListener<NewGameEvent>(OnNewGame);
		EventSystem.RemoveListener<BeginCheckPointEvent>(OnBeginCheckPoint);
		EventSystem.RemoveListener<CheckPointCompleteEvent>(OnCheckPointComplete);
		EventSystem.RemoveListener<SendPingEvent>(OnSendPing);
	}

	void Update()
	{
		NetManager.PollEvents();
	}

	#endregion

	#region Game Integration

	void QueuePlayer(int connectionID, string name)
	{
		PlayerData data = ScriptableObject.CreateInstance<PlayerData>();
		data.id = connectionID;
		data.playerName = name;

		if (game.inProgress)
			limbo.Enqueue(data);
		else
			AddPlayer(data);
	}
	void AddPlayer(PlayerData player)
	{
		Log("New player: " + player.name + "(" + player.id + ")");

		players[player.id] = player;
		czarQueue.Enqueue(player);
		game.AddPlayer(player);
		EventSystem.Dispatch(new Scoreboard.UpdateEvent());

		BroadcastMessage(NetworkingUtil.WriteIntStringMessage(NetworkMessageID.PlayerJoined, player.id, player.playerName));
		foreach (var p in players)
		{
			if (p.Value == player) continue;

			SendMessage(player.id, NetworkingUtil.WriteIntStringMessage(NetworkMessageID.PlayerJoined, p.Key, p.Value.playerName));
		}

		foreach (var p in game.scoreboard.GetScoringPlayers().Select(x => game.GetPlayerByName(x)).Where(x => x && players.ContainsKey(x.id)))
			SendMessage(player.id, NetworkingUtil.WriteIntIntMessage(NetworkMessageID.UpdateScore, p.id, game.scoreboard[p]));
	}
	void RemovePlayer(int connectionID)
	{
		Queue<PlayerData> newQueue = new Queue<PlayerData>();
		while (limbo.Count > 0)
		{
			if (limbo.Peek().id == connectionID)
			{
				limbo.Dequeue();
				continue;
			}

			newQueue.Enqueue(limbo.Dequeue());
		}
		limbo = newQueue;

		PlayerData data;
		if (!players.TryGetValue(connectionID, out data))
			return;

		players.Remove(connectionID);
		game.RemovePlayer(data);

		newQueue = new Queue<PlayerData>();
		while (czarQueue.Count > 0)
		{
			if (czarQueue.Peek().id == connectionID)
			{
				czarQueue.Dequeue();
				continue;
			}

			newQueue.Enqueue(czarQueue.Dequeue());
		}
		czarQueue = newQueue;

		EventSystem.Dispatch(new Scoreboard.UpdateEvent());
		BroadcastMessage(NetworkingUtil.WriteIntMessage(NetworkMessageID.PlayerLeft, connectionID));

		if (!game.checkPointActive && game.currentCzar && game.currentCzar.id == connectionID)
			ForceNextRound();
	}
	public void PingStragglers()
	{
		foreach (var player in game.GetStragglers())
			EventSystem.Dispatch(new SendPingEvent(player));
	}

	#endregion

	#region Messaging

	public void BroadcastMessage(byte[] bytes)
	{
		server.BroadcastStream(bytes, bytes.Length, 0);
	}
	public void SendMessage(int connectionID, byte[] bytes)
	{
		server.SendStream(bytes, bytes.Length, connectionID, 0);
	}

	#endregion

	#region Network Event Listeners

	public void OnConnection(int connectionID, int channelID, byte[] buffer, int datasize)
	{
		Log("Connection to " + connectionID.ToString());
	}

	public void OnReceiveData(int connectionID, int channelID, byte[] buffer, int datasize)
	{
		Log("Data received from connection " + connectionID + ": " + ((NetworkMessageID)buffer[0]).ToString());

		switch ((NetworkMessageID)buffer[0])
		{
			#region CheckGameVersion
			case NetworkMessageID.CheckGameVersion:
				{
					NetworkMessageID id;
					string versionNumber;
					NetworkingUtil.ReadStringMessage(buffer, out id, out versionNumber);

					int theirMajor, theirMinor, theirPatch;
					VersionNumber.Extract(versionNumber, out theirMajor, out theirMinor, out theirPatch);
					int myMajor, myMinor, myPatch;
					VersionNumber.Extract(versionNumber, out myMajor, out myMinor, out myPatch);

					if (theirMajor > myMajor || theirMajor == myMajor && theirMinor > myMinor)
						SendMessage(connectionID, NetworkingUtil.WriteBoolStringMessage(NetworkMessageID.GameVersionResult, false,
							"Server version out of date. Tell your host to update."));
					else if (theirMajor < myMajor || theirMajor == myMajor && theirMinor < myMinor)
						SendMessage(connectionID, NetworkingUtil.WriteBoolStringMessage(NetworkMessageID.GameVersionResult, false,
							"You're out of date. Get yourself an update, son."));
					else
						SendMessage(connectionID, NetworkingUtil.WriteBoolStringMessage(NetworkMessageID.GameVersionResult, true, ""));
				}
				break;
			#endregion
			#region SelectName
			case NetworkMessageID.SelectName:
				{
					NetworkMessageID id;
					string name;
					NetworkingUtil.ReadStringMessage(buffer, out id, out name);

					name = name.Trim();
					string error = string.Empty;
					if (string.IsNullOrEmpty(name))
						error = "Name must not be empty.";
					else if (game.GetPlayers().Any(x => x.playerName.ToLower() == name.ToLower()))
						error = "Name is already taken.";
					else if (name.Contains('~') || name.Contains('_'))
						error = "Name cannot contain '~' or '_'.";

					if (string.IsNullOrEmpty(error))
					{
						SendMessage(connectionID, NetworkingUtil.WriteStringMessage(NetworkMessageID.SelectedNameAccepted, name));
						SendMessage(connectionID, NetworkingUtil.WriteFloatMessage(NetworkMessageID.SyncMusic, MusicPlayer.currentTime));
						QueuePlayer(connectionID, name);
					}
					else
					{
						SendMessage(connectionID, NetworkingUtil.WriteStringMessage(NetworkMessageID.SelectedNameRejected, error));
					}
				}	
				break;
			#endregion
			#region PlayCard
			case NetworkMessageID.PlayCard:
				{
					NetworkMessageID id;
					CardData cardData;
					NetworkingUtil.ReadCardMessage(buffer, out id, out cardData);

					foreach (var card in players[connectionID].GetHand())
						if (card.id == cardData.id)
						{
							card.text = cardData.text;
							EventSystem.Dispatch(new ConfirmHandSelectionEvent(players[connectionID], card, true));
							break;
						}
				}
				break;
			#endregion
			#region SelectResponse
			case NetworkMessageID.SelectResponse:
				{
					if (game.currentWinner)
						return;

					NetworkMessageID id;
					int playerID;
					CardData[] cards;
					NetworkingUtil.ReadIntResponseMessage(buffer, out id, out playerID, out cards);

					foreach (var response in game.GetResponses())
					{
						if (response.player.id != playerID)
							continue;
						if (response.cards.Length != cards.Length)
							continue;
						if (!response.cards.Length.Enumerate().All(x => response.cards[x].id == cards[x].id))
							continue;

						CommonAudioLibrary.Instance.Play("Point");
						BroadcastMessage(NetworkingUtil.WriteIntResponseMessage(NetworkMessageID.SelectResponse, playerID, cards));
						EventSystem.Dispatch(new ConfirmResponseSelectionEvent(response, true));
						EventSystem.Dispatch(new PlayerScoredEvent(players[playerID]));
						BroadcastMessage(NetworkingUtil.WriteIntIntMessage(NetworkMessageID.UpdateScore, playerID, game.scoreboard[players[playerID]]));
						break;
					}
				}
				break;
			#endregion
			#region ContinueButtonClicked
			case NetworkMessageID.ContinueButtonClicked:
				{
					game.OnContinueButtonClicked();
				}
				break;
			#endregion
			#region FinishedCheckPoint
			case NetworkMessageID.FinishedCheckPoint:
				{
					EventSystem.Dispatch(new PlayerFinishedCheckPointEvent(players[connectionID]));
				}
				break;
			#endregion
		}
	}

	public void OnDisconnection(int connectionID, int channelID, byte[] buffer, int datasize)
	{
		Log("User disconnected from server: " + connectionID);
		RemovePlayer(connectionID);
	}

	public void OnMessage(NetworkEventType eventType, int connectionID, int channelID, byte[] buffer, int dataSize)
	{
		if (eventType == NetworkEventType.Nothing)
			return;

		//Log("Message received from connection " + connectionID + "(" + eventType.ToString() + "): " + ((dataSize <= 0) ? "<EMPTY>" : (buffer.Take(dataSize).Select(x => ((char)x).ToString()).Aggregate((x, y) => x.ToString()))));
	}

	#endregion

	#region Game Event Listeners

	void OnDequeuePlayers(DequeuePlayersEvent e)
	{
		while (limbo.Count > 0)
			AddPlayer(limbo.Dequeue());
	}
	void OnNewRound(NewRoundEvent e)
	{
		PlayerData czar = null;
		if (czarQueue.Count > 0)
		{
			czar = czarQueue.Dequeue();
			czarQueue.Enqueue(czar);
		}

		EventSystem.Dispatch(new NewCzarEvent(czar));
		BroadcastMessage(NetworkingUtil.WriteIntMessage(NetworkMessageID.NewRound, (czar ? czar.id : -1)));
	}
	void OnDrawCard(DrawCardEvent e)
	{
		SendMessage(e.player.id, NetworkingUtil.WriteCardMessage(NetworkMessageID.DrawCard, e.card.id, e.card.isBlank, e.card.text, e.card.pack));
	}
	void OnDiscardCard(DiscardCardEvent e)
	{
		SendMessage(e.player.id, NetworkingUtil.WriteIntMessage(NetworkMessageID.DiscardCard, e.card.id));
	}
	void OnNewCall(NewCallEvent e)
	{
		if (e.newCall)
			BroadcastMessage(NetworkingUtil.WriteCardMessage(NetworkMessageID.NewCall, e.newCall.id, e.newCall.isBlank, e.newCall.text, e.newCall.pack));
		else
			BroadcastMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.DeckOut));
	}
	void OnResponseBuilt(ResponseBuiltEvent e)
	{
		BroadcastMessage(NetworkingUtil.WriteIntResponseMessage(NetworkMessageID.ResponseSubmitted, e.player.id, e.cards));
	}
	void OnGotAllResponses(GotAllResponsesEvent e)
	{
		BroadcastMessage(NetworkingUtil.WriteIntListMessage(NetworkMessageID.GotAllResponses, game.GetResponses().Select(x => x.cards[0].id).ToArray()));
	}
	void OnScoreboardCleared(Scoreboard.ClearedEvent e)
	{
		BroadcastMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.ClearScores));
	}
	void OnKickPlayer(KickPlayerEvent e)
	{
		server.DisconnectClient(e.player.id);
	}
	void OnPlayerWon(PlayerWonEvent e)
	{
		BroadcastMessage(NetworkingUtil.WriteIntMessage(NetworkMessageID.PlayerWon, e.winner.id));
	}
	void OnNewGame(NewGameEvent e)
	{
		BroadcastMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.ClearHand));
	}
	void OnBeginCheckPoint(BeginCheckPointEvent e)
	{
		BroadcastMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.BeginCheckPoint));
		onBeginCheckPoint.TryInvoke();
	}
	void OnCheckPointComplete(CheckPointCompleteEvent e)
	{
		BroadcastMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.FinishedCheckPoint));
		onFinishCheckPoint.TryInvoke();
	}
	void OnSendPing(SendPingEvent e)
	{
		SendMessage(e.player.id, NetworkingUtil.WriteBasicMessage(NetworkMessageID.Ping));
	}

	#endregion

	#region Public API

	public void ForceRevealResponses()
	{
		if (game.Reveal())
			BroadcastMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.RevealResponses));
	}
	public void ForceNextRound()
	{
		game.ForceNextRound();
	}

	#endregion
}

public class NewCzarEvent : EventInstance
{
	public PlayerData czar;

	public NewCzarEvent(PlayerData czar)
	{
		this.czar = czar;
	}
}
public class KickPlayerEvent : EventInstance
{
	public PlayerData player;

	public KickPlayerEvent(PlayerData player)
	{
		this.player = player;
	}
}