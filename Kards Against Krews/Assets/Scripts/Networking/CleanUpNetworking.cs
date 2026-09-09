using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LowLevelNetworking;

public class CleanUpNetworking : CustomBehaviour
{
	void OnEnable()
	{
		EventSystem.AddListener<DestroyServerEvent>(OnDestroyServer);
		EventSystem.AddListener<DestroyClientEvent>(OnDestroyClient);
	}
	void OnDisable()
	{
		EventSystem.RemoveListener<DestroyServerEvent>(OnDestroyServer);
		EventSystem.RemoveListener<DestroyClientEvent>(OnDestroyClient);
	}

	void OnDestroyServer(DestroyServerEvent e)
	{
		StartCoroutine(DoDestroyServer(e.server));
	}
	void OnDestroyClient(DestroyClientEvent e)
	{
		StartCoroutine(DoDestroyClient(e.client));
	}

	IEnumerator DoDestroyServer(NetServer server)
	{
		yield return null;
		NetManager.DestroyServer(server);
		NetManager.Shutdown();
	}
	IEnumerator DoDestroyClient(NetClient client)
	{
		yield return null;
		NetManager.DestroyClient(client);
		NetManager.Shutdown();
	}
}

public class DestroyServerEvent : EventInstance
{
	public NetServer server;

	public DestroyServerEvent(NetServer server)
	{
		this.server = server;
	}
}
public class DestroyClientEvent : EventInstance
{
	public NetClient client;

	public DestroyClientEvent(NetClient client)
	{
		this.client = client;
	}
}