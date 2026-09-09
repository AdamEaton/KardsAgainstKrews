using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class PingDisplay : CustomBehaviour
{
	public UnityEvent onPing;

	void OnEnable()
	{
		EventSystem.AddListener<PingReceivedEvent>(OnPingReceived);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<PingReceivedEvent>(OnPingReceived);
	}

	void OnPingReceived(PingReceivedEvent e)
	{
		onPing.TryInvoke();
	}
}

public class SendPingEvent : EventInstance
{
	public PlayerData player;

	public SendPingEvent(PlayerData player)
	{
		this.player = player;
	}
}
public class PingReceivedEvent : EventInstance { }