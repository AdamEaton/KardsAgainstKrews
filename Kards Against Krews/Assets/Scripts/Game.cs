using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Game : CustomBehaviour
{
	public GameMetrics metrics;
	GameMetrics activeMetrics;
	public DeckManager deck;
	public Scoreboard scoreboard;

	List<PlayerData> players = new List<PlayerData>();
	public IEnumerable<PlayerData> GetPlayers()
	{
		foreach (var player in players)
			yield return player;
	}
	public PlayerData currentCzar { get; private set; }
	public PlayerData currentWinner { get; private set; }
	public IEnumerable<PlayerData> GetStragglers()
	{
		if (!inProgress || currentWinner != null)
			yield break;

		if (hasAllResponses)
			yield return currentCzar;

		if (!hasAllResponses)
			foreach (var player in players.Where(x => (x != currentCzar || checkPointActive) && !responses.Any(y => y.player == x)))
				yield return player;
	}

	CardData _call;
	public CardData call
	{
		get { return _call; }
		set
		{
			_call = value;
			EventSystem.Dispatch(new NewCallEvent(value));
		}
	}
	
	bool _hasAllResponses;
	public bool hasAllResponses
	{
		get { return _hasAllResponses; }
		set
		{
			_hasAllResponses = value;
			EventSystem.Dispatch(new ResponsesUpdatedEvent());
		}
	}
	List<ResponseData> responses = new List<ResponseData>();
	public IEnumerable<ResponseData> GetResponses()
	{
		foreach (var response in responses)
			yield return response;
	}

	public PlayerData winner { get { return players.FirstOrDefault(x => scoreboard[x] >= activeMetrics.targetScore); } }

	[SerializeField]
	GameObject newGameMenu;
	[SerializeField]
	GameObject settingsMenu;
	[SerializeField]
	Button continueButton;

	public bool forceReveal { get; set; }

	public bool inProgress { get; private set; }

	HashSet<PlayerData> readyPlayers = new HashSet<PlayerData>();

	bool advance;

	int checkPointCounter;
	public bool checkPointActive { get; private set; }

	public PlayerData GetPlayerByName(string name)
	{
		foreach (var player in players)
		{
			if (player.playerName == name)
				return player;
		}
		return null;
	}

	public void Play()
	{
		activeMetrics = metrics.Clone();

		scoreboard.Clear();
		foreach (var player in players)
		{
			player.ClearHand();
		}
		if (players.Count <= 0)
			return;

		checkPointCounter = activeMetrics.checkPointFrequency;

		EventSystem.Dispatch(new NewGameEvent());
		StartCoroutine(MainLoop());
	}
	IEnumerator MainLoop()
	{
		inProgress = true;

		while (true)
		{
			if (activeMetrics.checkPointFrequency <= 0 || checkPointCounter > 0)
			{
				EventSystem.Dispatch(new DequeuePlayersEvent());

				NextRound();

				while (!call)
					yield return null;

				advance = false;

				while (!forceReveal && responses.Count < players.Count - 1)
				{
					yield return new WaitFor.Any(
						new WaitFor.Event<ResponsesUpdatedEvent>(),
						new WaitFor.Condition(() => advance)
						);
				}

				yield return new WaitFor.Any(
					new WaitFor.Event<ConfirmResponseSelectionEvent>(),
					new WaitFor.Condition(() => advance)
					);

				ShowContinueButton();
				yield return new WaitFor.Condition(() => advance);
			}

			if (CheckGameOver(winner))
				break;
			else if (activeMetrics.checkPointFrequency > 0 && --checkPointCounter <= 0)
			{
				checkPointActive = true;
				checkPointCounter = activeMetrics.checkPointFrequency;

				ClearResponses();

				EventSystem.Dispatch(new BeginCheckPointEvent());

				advance = false;

				while (!advance)
				{
					yield return new WaitFor.Any(
						new WaitFor.Event<PlayerFinishedCheckPointEvent>(),
						new WaitFor.Condition(() => advance)
						);

					if (readyPlayers.Count >= players.Count)
						break;
				}

				checkPointActive = false;
				EventSystem.Dispatch(new CheckPointCompleteEvent());
			}
		}

		inProgress = false;
	}

	void SubmitResponse(PlayerData player, IEnumerable<CardData> cards)
	{
		var response = new ResponseData(player, cards);
		responses.Add(response);
		if (!checkPointActive && responses.Count >= players.Count - 1)
		{
			responses = responses.Shuffled().ToList();
			if (inProgress)
			{
				hasAllResponses = true;
				EventSystem.Dispatch(new GotAllResponsesEvent());
			}
		}
		EventSystem.Dispatch(new ResponsesUpdatedEvent());
	}
	public void SelectWinner(ResponseData response)
	{
		EventSystem.Dispatch(new ResponseWonEvent(response));
	}
	void NextRound()
	{
		FillPlayerHands();
		ClearResponses();
		EventSystem.Dispatch(new NewRoundEvent());
		GetNextCall();
	}
	public void ShowNewGameWindow()
	{
		newGameMenu.SetActive(true);
	}
	bool CheckGameOver(PlayerData player)
	{
		if (!player)
			return false;

		EventSystem.Dispatch(new PlayerWonEvent(player));
		EventSystem.Dispatch(new DialogueModal.ShowEvent(
			"GAME OVER",
			player.playerName + " has reached the target score!",
			"Win",
			new DialogueModal.ButtonInfo("Way to be!", () =>
				{
					EventSystem.Dispatch(new DialogueModal.HideEvent());
				})
			));
		return true;
	}

	public void FillPlayerHands()
	{
		while (players.Any(x => x.handCount < activeMetrics.handSize))
		{
			foreach (var player in players)
			{
				if (player.handCount >= activeMetrics.handSize)
					continue;

				if (!player.Draw(deck.GetNextResponse()))
					return;
			}
		}
	}
	public void ClearResponses()
	{
		responses.Clear();
		currentWinner = null;
		forceReveal = false;
		hasAllResponses = false;
	}
	public void GetNextCall()
	{
		call = deck.GetNextCall();
		if (!call)
			return;

		for (int i = 0; i < call.responseCount - 1; i++)
		{
			foreach (var player in players)
			{
				if (player == currentCzar)
					continue;

				player.Draw(deck.GetNextResponse());
			}
		}
	}

	public bool Reveal()
	{
		if (!inProgress || checkPointActive || hasAllResponses)
			return false;

		ForceReveal();
		return true;
	}
	public void ForceReveal()
	{
		responses = responses.Shuffled().ToList();
		forceReveal = true;
		EventSystem.Dispatch(new ResponsesUpdatedEvent());
	}
	public void ForceNextRound()
	{
		if (!inProgress)
			return;

		if (activeMetrics.checkPointFrequency > 0)
		{
			if (checkPointActive)
			{
				checkPointActive = false;
				EventSystem.Dispatch(new CheckPointCompleteEvent());
			}
			else
				checkPointCounter--;
		}

		StopAllCoroutines();
		StartCoroutine(MainLoop());
	}

	void Awake()
	{
		foreach (var player in players)
		{
			player.ClearHand();
		}
	}

	void OnEnable()
	{
		EventSystem.AddListener<ResponseBuiltEvent>(OnResponseBuilt);
		EventSystem.AddListener<ConfirmResponseSelectionEvent>(OnConfirmResponseSelection);
		EventSystem.AddListener<NewCzarEvent>(OnNewCzar);
		EventSystem.AddListener<ShowSettingsMenuEvent>(OnShowSettingsMenu);
		EventSystem.AddListener<BeginCheckPointEvent>(OnBeginCheckPoint);
		EventSystem.AddListener<PlayerFinishedCheckPointEvent>(OnPlayerFinishedCheckPoint);

		if (continueButton)
			continueButton.onClick.AddListener(OnContinueButtonClicked);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<ResponseBuiltEvent>(OnResponseBuilt);
		EventSystem.RemoveListener<ConfirmResponseSelectionEvent>(OnConfirmResponseSelection);
		EventSystem.RemoveListener<NewCzarEvent>(OnNewCzar);
		EventSystem.RemoveListener<ShowSettingsMenuEvent>(OnShowSettingsMenu);
		EventSystem.RemoveListener<BeginCheckPointEvent>(OnBeginCheckPoint);
		EventSystem.RemoveListener<PlayerFinishedCheckPointEvent>(OnPlayerFinishedCheckPoint);

		if (continueButton)
			continueButton.onClick.RemoveListener(OnContinueButtonClicked);
	}

	public void AddPlayer(PlayerData player)
	{
		players.Add(player);
	}
	public void RemovePlayer(PlayerData player)
	{
		players.Remove(player);
		readyPlayers.Remove(player);
		if (responses.RemoveAll(x => x.player == player) > 0)
			EventSystem.Dispatch(new ResponsesUpdatedEvent());

		if (checkPointActive)
		{
			if (responses.Count >= players.Count)
				ForceNextRound();
		}
		else if (responses.Count >= players.Count - 1)
		{
			if (!hasAllResponses)
			{
				responses = responses.Shuffled().ToList();
				if (inProgress)
				{
					hasAllResponses = true;
					EventSystem.Dispatch(new GotAllResponsesEvent());
				}
			}
		}
	}

	public void SetResponseOrder(List<int> order)
	{
		responses = responses.OrderBy(x => order.IndexOf(x.cards[0].id)).ToList();
	}

	void OnResponseBuilt(ResponseBuiltEvent e)
	{
		SubmitResponse(e.player, e.cards);
	}
	void OnConfirmResponseSelection(ConfirmResponseSelectionEvent e)
	{
		currentWinner = e.response.player;
		EventSystem.Dispatch(new ResponseWonEvent(e.response));
	}
	void OnNewCzar(NewCzarEvent e)
	{
		currentCzar = e.czar;
	}
	void OnShowSettingsMenu(ShowSettingsMenuEvent e)
	{
		CommonAudioLibrary.Instance.Play("Select");
		if (settingsMenu)
			settingsMenu.gameObject.SetActive(true);
	}
	void OnBeginCheckPoint(BeginCheckPointEvent e)
	{
		readyPlayers.Clear();
	}
	void OnPlayerFinishedCheckPoint(PlayerFinishedCheckPointEvent e)
	{
		readyPlayers.Add(e.player);
	}

	public void OnContinueButtonClicked()
	{
		advance = true;
		EventSystem.Dispatch(new ContinueButtonClickedEvent());
	}
	public void ShowContinueButton()
	{
		if (continueButton)
			continueButton.gameObject.SetActive(true);
	}
	public void HideContinueButton()
	{
		if (continueButton)
			continueButton.gameObject.SetActive(false);
	}

	public void LeaveGame()
	{
		SceneManager.LoadScene("Main");
	}
}

public class DequeuePlayersEvent : EventInstance { }
public class NewCallEvent : EventInstance
{
	public CardData newCall;

	public NewCallEvent(CardData newCall)
	{
		this.newCall = newCall;
	}
}
public class ResponsesUpdatedEvent : EventInstance { }
public class GotAllResponsesEvent : EventInstance { }
public class ContinueButtonClickedEvent : EventInstance { }
public class PlayerWonEvent : EventInstance
{
	public PlayerData winner;

	public PlayerWonEvent(PlayerData winner)
	{
		this.winner = winner;
	}
}
public class NewRoundEvent : EventInstance { }
public class NewGameEvent : EventInstance { }
public class ShowSettingsMenuEvent : EventInstance { }
public class BeginCheckPointEvent : EventInstance { }
public class PlayerFinishedCheckPointEvent : EventInstance
{
	public PlayerData player;

	public PlayerFinishedCheckPointEvent(PlayerData player)
	{
		this.player = player;
	}
}
public class CheckPointCompleteEvent : EventInstance { }