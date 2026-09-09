using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum NetworkMessageID : byte
{
	CheckGameVersion,
	GameVersionResult,
	SelectName,
	SelectedNameRejected,
	SelectedNameAccepted,
	SyncMusic,
	PlayerJoined,
	PlayerLeft,
	ClearHand,
	NewRound,
	NewCall,
	DeckOut,
	DrawCard,
	DiscardCard,
	PlayCard,
	ResponseSubmitted,
	GotAllResponses,
	RevealResponses,
	SelectResponse,
	UpdateScore,
	ClearScores,
	ContinueButtonClicked,
	BeginCheckPoint,
	FinishedCheckPoint,
	PlayerWon,
	Ping,
}
