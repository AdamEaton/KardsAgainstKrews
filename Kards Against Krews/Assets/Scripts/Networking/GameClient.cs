using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using LowLevelNetworking;

public class GameClient : CustomBehaviour
{
	const string ServerAddressKey = "ServerAddress";
	const string PlayerNameKey = "PlayerName";

	[SerializeField]
	Game game;

	public UnityEvent onCzar;
	public UnityEvent onCzarComplete;
	public UnityEvent onNotCzar;
	public UnityEvent onNotCzarComplete;
	public UnityEvent onResponseSelected;
	public UnityEvent onBeginCheckPoint;
	public UnityEvent onFinishCheckPoint;

	NetClient client;

	public PlayerData me { get; private set; }
	Dictionary<int, PlayerData> players = new Dictionary<int, PlayerData>();

	bool? isPlaying = null;

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

		client = NetManager.CreateClient();

		client.OnConnection = OnConnection;
		client.OnData = OnReceiveData;
		client.OnDisconnection = OnDisconnection;
		client.OnMessage = OnMessage;

		EventSystem.Dispatch(GetConnectionModalEvent());
	}
	void OnDestroy()
	{
		if (client.isConnected)
			client.Disconnect();
		EventSystem.Dispatch(new DestroyClientEvent(client));
	}
	void Update()
	{
		NetManager.PollEvents();
	}

	void OnEnable()
	{
		EventSystem.AddListener<ConfirmHandSelectionEvent>(OnConfirmHandSelection);
		EventSystem.AddListener<ConfirmResponseSelectionEvent>(OnConfirmResponseSelection);
		EventSystem.AddListener<ContinueButtonClickedEvent>(OnContinueButtonClicked);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<ConfirmHandSelectionEvent>(OnConfirmHandSelection);
		EventSystem.RemoveListener<ConfirmResponseSelectionEvent>(OnConfirmResponseSelection);
		EventSystem.RemoveListener<ContinueButtonClickedEvent>(OnContinueButtonClicked);
	}

	#endregion

	#region Game Integration

	public void AddPlayer(int connectionID, string name)
	{
		Log("New player: " + name + "(" + connectionID + ")");

		PlayerData data = ScriptableObject.CreateInstance<PlayerData>();
		data.id = connectionID;
		data.playerName = name;
		players[connectionID] = data;
		game.AddPlayer(data);
		EventSystem.Dispatch(new Scoreboard.UpdateEvent());
	}
	public void RemovePlayer(int connectionID)
	{
		PlayerData data;
		if (!players.TryGetValue(connectionID, out data))
			return;

		players.Remove(connectionID);
		game.RemovePlayer(data);
		EventSystem.Dispatch(new Scoreboard.UpdateEvent());
	}

	#endregion

	#region Modals

	#region Connection Modal

	TextInputModal.ShowEvent GetConnectionModalEvent()
	{
		return new TextInputModal.ShowEvent(
			"WHERE TO?",
			"Enter the IP Address of the server you would like to connect to.",
			PlayerPrefs.GetString(ServerAddressKey, "127.0.0.1"),
			40,
			"Alert",
			new TextInputModal.ButtonInfo("Cancel", OnConnectionModalCancel),
			new TextInputModal.ButtonInfo("Connect", OnConnectionModalConnect)
			);
	}
	void OnConnectionModalCancel(string address)
	{
		EventSystem.Dispatch(new TextInputModal.HideEvent());
		game.LeaveGame();
	}
	void OnConnectionModalConnect(string address)
	{
		address = new string(address.Where(x => !char.IsWhiteSpace(x)).ToArray());

		PlayerPrefs.SetString(ServerAddressKey, address);
		client.Connect(address, 7777);
		EventSystem.Dispatch(new TextInputModal.HideEvent());
	}

	#endregion

	#region Name Entry Modal

	TextInputModal.ShowEvent GetNameEntryModalEvent()
	{
		return new TextInputModal.ShowEvent(
			"WHO ARE YOU?",
			"Choose a name to use as your identity.",
			PlayerPrefs.GetString(PlayerNameKey, ""),
			16,
			"Alert",
			new TextInputModal.ButtonInfo("Submit", OnNameEntryModalSubmit)
			);
	}
	TextInputModal.ShowEvent GetNameEntryErrorModalEvent(string error)
	{
		return new TextInputModal.ShowEvent(
			"SERIOUSLY, WHO ARE YOU?",
			"There was a problem with your previous nickname: " + error,
			PlayerPrefs.GetString(PlayerNameKey, ""),
			16,
			"Alert",
			new TextInputModal.ButtonInfo("Submit Better", OnNameEntryModalSubmit)
			);
	}
	void OnNameEntryModalSubmit(string name)
	{
		PlayerPrefs.SetString(PlayerNameKey, name);
		EventSystem.Dispatch(new TextInputModal.HideEvent());
		SendMessage(NetworkingUtil.WriteStringMessage(NetworkMessageID.SelectName, name));
	}

	#endregion

	#region Disconnect Modal

	DialogueModal.ShowEvent GetDisconnectModalEvent()
	{
		return new DialogueModal.ShowEvent(
			"DISCONNECTED",
			"The connection to the server has been lost. Bummer.",
			"Alert",
			new DialogueModal.ButtonInfo("Welp.", OnDisconnectModalConfirm)
			);
	}
	void OnDisconnectModalConfirm()
	{
		SceneManager.LoadScene("Main");
	}

	#endregion

	#region Version Mismatch Modal

	DialogueModal.ShowEvent GetVersionMismatchModalEvent(string message)
	{
		return new DialogueModal.ShowEvent(
			"VERSION'S ALL WACK, YO",
			"Version mismatch with server: " + message,
			"Alert",
			new DialogueModal.ButtonInfo("Welp.", OnVersionMismatchModalConfirm)
			);
	}
	void OnVersionMismatchModalConfirm()
	{
		if (client.isConnected)
			client.Disconnect();
	}

	#endregion

	#endregion

	#region Messaging

	public void SendMessage(byte[] bytes)
	{
		client.SendStream(bytes, bytes.Length, 0);
	}

	#endregion

	#region Network Event Listeners

	public void OnConnection(int connectionID, int channelID, byte[] buffer, int dataSize)
	{
		Log("Connection to " + connectionID.ToString());
		SendMessage(NetworkingUtil.WriteStringMessage(NetworkMessageID.CheckGameVersion, Application.version));
	}

	public void OnReceiveData(int connectionID, int channelID, byte[] buffer, int dataSize)
	{
		Log("Data received from connection " + connectionID + ": " + ((NetworkMessageID)buffer[0]).ToString());
		
		if (!isPlaying.HasValue)
		{
			switch ((NetworkMessageID)buffer[0])
			{
				case NetworkMessageID.SyncMusic:
				case NetworkMessageID.GameVersionResult:
				case NetworkMessageID.SelectedNameAccepted:
				case NetworkMessageID.SelectedNameRejected:
					break;
				default:
					return;
			}
		}
		else if (!isPlaying.Value)
		{
			switch ((NetworkMessageID)buffer[0])
			{
				case NetworkMessageID.SyncMusic:
				case NetworkMessageID.PlayerJoined:
					break;
				default:
					return;
			}
		}

		switch ((NetworkMessageID)buffer[0])
		{
			#region GameVersionResult
			case NetworkMessageID.GameVersionResult:
				{
					NetworkMessageID id;
					bool result;
					string message;
					NetworkingUtil.ReadBoolStringMessage(buffer, out id, out result, out message);

					if (result)
						EventSystem.Dispatch(GetNameEntryModalEvent());
					else
						EventSystem.Dispatch(GetVersionMismatchModalEvent(message));
				}
				break;
			#endregion
			#region SelectedNameAccepted
			case NetworkMessageID.SelectedNameAccepted:
				{
					NetworkMessageID id;
					string name;
					NetworkingUtil.ReadStringMessage(buffer, out id, out name);

					me = ScriptableObject.CreateInstance<PlayerData>();
					me.playerName = name;
					EventSystem.Dispatch(new LocalPlayerDataCreatedEvent(me));

					isPlaying = false;
					EventSystem.Dispatch(new DialogueModal.ShowEvent(
						"YOU MADE IT",
						"Name has been registered. Waiting in limbo...",
						"",
						new DialogueModal.ButtonInfo("Quit Waiting and Leave", () => { game.LeaveGame(); })
						));
				}
				break;
			#endregion
			#region SelectedNameRejected
			case NetworkMessageID.SelectedNameRejected:
				{
					NetworkMessageID id;
					string error;
					NetworkingUtil.ReadStringMessage(buffer, out id, out error);

					EventSystem.Dispatch(GetNameEntryErrorModalEvent(error));
				}
				break;
			#endregion
			#region SyncMusic
			case NetworkMessageID.SyncMusic:
				{
					NetworkMessageID id;
					float time;
					NetworkingUtil.ReadFloatMessage(buffer, out id, out time);

					MusicPlayer.Play();
					MusicPlayer.currentTime = time;
				}
				break;
			#endregion
			#region PlayerJoined
			case NetworkMessageID.PlayerJoined:
				{
					NetworkMessageID id;
					int playerID;
					string name;
					NetworkingUtil.ReadIntStringMessage(buffer, out id, out playerID, out name);

					if (!isPlaying.Value)
					{
						if (name != me.playerName) return;

						isPlaying = true;
						EventSystem.Dispatch(new DialogueModal.HideEvent());
					}

					if (name == me.playerName) me.id = playerID;
					AddPlayer(playerID, name);
				}
				break;
			#endregion
			#region PlayerLeft
			case NetworkMessageID.PlayerLeft:
				{
					NetworkMessageID id;
					int playerID;
					NetworkingUtil.ReadIntMessage(buffer, out id, out playerID);

					RemovePlayer(playerID);

					if (client.isConnected && playerID == me.id)
						client.Disconnect();
				}
				break;
			#endregion
			#region ClearHand
			case NetworkMessageID.ClearHand:
				{
					me.ClearHand();
				}
				break;
			#endregion
			#region NewRound
			case NetworkMessageID.NewRound:
				{
					NetworkMessageID id;
					int playerID;
					NetworkingUtil.ReadIntMessage(buffer, out id, out playerID);

					game.ClearResponses();
					game.HideContinueButton();

					if (playerID == me.id)
					{
						onCzar.TryInvoke();
					}
					else
					{
						onNotCzar.TryInvoke();
						CommonAudioLibrary.Instance.Play("Alert");
					}

					EventSystem.Dispatch(new NewCzarEvent(players[playerID]));
					EventSystem.Dispatch(new DialogueModal.HideEvent());
				}
				break;
			#endregion
			#region NewCall
			case NetworkMessageID.NewCall:
				{
					NetworkMessageID id;
					CardData card;
					NetworkingUtil.ReadCardMessage(buffer, out id, out card);

					EventSystem.Dispatch(new NewCallEvent(card));
				}
				break;
			#endregion
			#region DeckOut
			case NetworkMessageID.DeckOut:
				{
					EventSystem.Dispatch(new NewCallEvent(null));
				}
				break;
			#endregion
			#region DrawCard
			case NetworkMessageID.DrawCard:
				{
					NetworkMessageID id;
					CardData card;
					NetworkingUtil.ReadCardMessage(buffer, out id, out card);

					me.Draw(card);
				}
				break;
			#endregion
			#region DiscardCard
			case NetworkMessageID.DiscardCard:
				{
					NetworkMessageID id;
					int cardID;
					NetworkingUtil.ReadIntMessage(buffer, out id, out cardID);

					me.Discard(cardID);
				}
				break;
			#endregion
			#region ResponseSubmitted
			case NetworkMessageID.ResponseSubmitted:
				{
					NetworkMessageID id;
					int playerID;
					CardData[] cards;
					NetworkingUtil.ReadIntResponseMessage(buffer, out id, out playerID, out cards);

					if (playerID == me.id)
						onNotCzarComplete.TryInvoke();
					else
						CommonAudioLibrary.Instance.Play("Submit");
					EventSystem.Dispatch(new ResponseBuiltEvent(players[playerID], cards));
				}
				break;
			#endregion
			#region GotAllResponses
			case NetworkMessageID.GotAllResponses:
				{
					NetworkMessageID id;
					int[] cardIDs;
					NetworkingUtil.ReadIntListMessage(buffer, out id, out cardIDs);

					game.SetResponseOrder(cardIDs.ToList());
					game.hasAllResponses = true;

					if (game.currentCzar.id == me.id)
						CommonAudioLibrary.Instance.Play("Alert");
				}
				break;
			#endregion
			#region RevealResponses
			case NetworkMessageID.RevealResponses:
				{
					game.ForceReveal();

					if (game.currentCzar.id == me.id)
						CommonAudioLibrary.Instance.Play("Alert");
				}
				break;
			#endregion
			#region SelectResponse
			case NetworkMessageID.SelectResponse:
				{
					NetworkMessageID id;
					int playerID;
					CardData[] cards;
					NetworkingUtil.ReadIntResponseMessage(buffer, out id, out playerID, out cards);

					foreach (var response in game.GetResponses())
					{
						if (response.player.id != playerID) continue;
						if (response.cards.Length != cards.Length) continue;
						if (!response.cards.Length.Enumerate().All(x => response.cards[x].id == cards[x].id)) continue;

						if (game.currentCzar.id == me.id)
							onCzarComplete.TryInvoke();
						EventSystem.Dispatch(new ConfirmResponseSelectionEvent(response, true));
						CommonAudioLibrary.Instance.Play("Point");
						onResponseSelected.TryInvoke();
						break;
					}
				}
				break;
			#endregion
			#region UpdateScore
			case NetworkMessageID.UpdateScore:
				{
					NetworkMessageID id;
					int playerID;
					int score;
					NetworkingUtil.ReadIntIntMessage(buffer, out id, out playerID, out score);

					game.scoreboard[players[playerID].playerName] = score;
				}
				break;
			#endregion
			#region Clear Scores
			case NetworkMessageID.ClearScores:
				{
					game.scoreboard.Clear();
				}
				break;
			#endregion
			#region PlayerWon
			case NetworkMessageID.PlayerWon:
				{
					NetworkMessageID id;
					int playerID;
					NetworkingUtil.ReadIntMessage(buffer, out id, out playerID);

					EventSystem.Dispatch(new DialogueModal.ShowEvent(
						"GAME OVER",
						players[playerID].playerName + " has reached the target score! Waiting for the host to start a new game.",
						"Win",
						new DialogueModal.ButtonInfo("Quit Waiting and Leave", () => { game.LeaveGame(); })
						));
				}
				break;
			#endregion
			#region BeginCheckPoint
			case NetworkMessageID.BeginCheckPoint:
				{
					BeginCheckPoint();
				}
				break;
			#endregion
			#region FinishedCheckPoint
			case NetworkMessageID.FinishedCheckPoint:
				{
					onFinishCheckPoint.TryInvoke();
				}
				break;
			#endregion
			#region Ping
			case NetworkMessageID.Ping:
				{
					EventSystem.Dispatch(new PingReceivedEvent());
				}
				break;
			#endregion
		}
	}

	public void OnDisconnection(int connectionID, int channelID, byte[] buffer, int dataSize)
	{
		Log("Disconnected from server: " + connectionID);
		EventSystem.Dispatch(GetDisconnectModalEvent());
	}

	public void OnMessage(NetworkEventType eventType, int connectionID, int channelID, byte[] buffer, int dataSize)
	{
		if (eventType == NetworkEventType.Nothing)
			return;
	}

	#endregion

	#region Game Event Listeners

	void OnConfirmHandSelection(ConfirmHandSelectionEvent e)
	{
		if (e.fromServer)
			return;

		SendMessage(NetworkingUtil.WriteCardMessage(NetworkMessageID.PlayCard, e.card));
	}
	void OnConfirmResponseSelection(ConfirmResponseSelectionEvent e)
	{
		if (e.fromServer)
			return;

		SendMessage(NetworkingUtil.WriteIntResponseMessage(NetworkMessageID.SelectResponse, e.response.player.id, e.response.cards));
	}
	void OnContinueButtonClicked(ContinueButtonClickedEvent e)
	{
		SendMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.ContinueButtonClicked));
	}

	#endregion

	#region	Public API

	public void BeginCheckPoint()
	{
		onBeginCheckPoint.TryInvoke();
		game.ClearResponses();
		CommonAudioLibrary.Instance.Play("Alert");
	}
	public void FinishCheckPoint()
	{
		onFinishCheckPoint.TryInvoke();
		SendMessage(NetworkingUtil.WriteBasicMessage(NetworkMessageID.FinishedCheckPoint));
	}

	#endregion
}

public class LocalPlayerDataCreatedEvent : EventInstance
{
	public PlayerData player;

	public LocalPlayerDataCreatedEvent(PlayerData player)
	{
		this.player = player;
	}
}