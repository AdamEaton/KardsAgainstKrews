using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

namespace LowLevelNetworking
{
	/// <summary>
	/// The NetManager is responsible for creating our client and server sockets and housing our messaging queue/delegate system.
	/// </summary>
	public static class NetManager
	{
		// Connection config vars
		public static ConnectionConfig connectionConfig = new ConnectionConfig();
		public static GlobalConfig globalConfig = new GlobalConfig();

		public static byte channelReliable;
		public static byte channelUnreliable;

		// True if Init has ran.
		public static bool isInitialized = false;

		// Lists to hold our clients
		public static List<NetServer> servers = new List<NetServer>();
		public static List<NetClient> clients = new List<NetClient>();

		/// <summary>
		/// Initialize our low level network APIs.
		/// </summary>
		public static void Init()
		{

			NetworkTransport.Init(globalConfig);

			isInitialized = true;

		}

		public static void Shutdown()
		{

			// Kill all clients
			for (int i = 0; i < clients.Count; i++)
			{
				DestroyClient(clients[i]);
			}

			// Disconnect and destroy all servers
			for (int i = 0; i < servers.Count; i++)
			{
				DestroyServer(servers[i]);
			}

			NetworkTransport.Shutdown();
			isInitialized = false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetServer"/> class. We can simulate real world network conditions by using the simMinTimeout/simMaxTimeout params
		/// to simulate connection lag.
		/// </summary>
		/// <returns>The server object.</returns>
		/// <param name="maxConnections">Max connections.</param>
		/// <param name="port">Port.</param>
		/// <param name="simMinLatency">Minimum latency to simulate on the server.</param>
		/// <param name="simMaxLatency">Maximum latency to simulate on the server.</param> 
		public static NetServer CreateServer(int maxConnections, int port, int simMinLatency = 0, int simMaxLatency = 0)
		{

			NetServer s = new NetServer(maxConnections, port, simMinLatency, simMaxLatency);
			

			// If we were successful in creating our server and it is unique
			if (s.isRunning && servers.Contains(s) != true)
			{
				servers.Add(s);
			}

			return s;
		}

		/// <summary>
		/// Destroys the server.
		/// </summary>
		/// <returns><c>true</c>, if server was destroyed, <c>false</c> otherwise.</returns>
		/// <param name="s">NetServer to destroy</param>
		public static bool DestroyServer(NetServer s)
		{

			if (servers.Contains(s) == false)
			{
				Debug.Log("NetManager::DestroyServer( " + s.socket.ToString() + ") - Server does not exist!");
				return false;
			}

			s.DisconnectAllClients();

			NetworkTransport.RemoveHost(s.socket);

			servers.Remove(s);

			return true;
		}


		/// <summary>
		/// Create a client that is ready to connect with a server.
		/// </summary>
		/// <returns>The client.</returns>
		public static NetClient CreateClient()
		{

			if (!isInitialized)
			{
				Debug.Log("NetManager::CreateServer( ... ) - NetManager was not initialized. Did you forget to call NetManager.Init()?");
				return null;
			}

			NetClient c = new NetClient();

			if (clients.Contains(c) != true)
			{
				clients.Add(c);
			}

			return c;
		}

		/// <summary>
		/// Destroys specified client.
		/// </summary>
		/// <returns><c>true</c>, if client was destroyed, <c>false</c> otherwise.</returns>
		/// <param name="c">NetClient object to destroy</param>
		public static bool DestroyClient(NetClient c)
		{

			if (clients.Contains(c) == false)
			{
				Debug.Log("NetManager::DestroyClient( " + c.socket.ToString() + ") - Client does not exist!");
				return false;
			}

			if (c.isConnected) c.Disconnect();

			NetworkTransport.RemoveHost(c.socket);

			clients.Remove(c);

			return true;
		}

		/// <summary>
		/// Reads network events and delegates how they are used
		/// </summary>
		public static void PollEvents()
		{

			// If nothing is running, why bother
			if (servers.Count < 1 && clients.Count < 1)
			{
				return;
			}

			int recHostId;
			int connectionId;
			int channelId;
			int dataSize;
			byte[] buffer = new byte[1024];
			byte error;

			NetworkEventType networkEvent = NetworkEventType.DataEvent;

			// Process network events for n clients and n servers
			while (isInitialized && networkEvent != NetworkEventType.Nothing)
			{
				int i = -1; // Index for netserver in mservers

				networkEvent = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, buffer, 1024, out dataSize, out error);

				// Route message to our server delegate
				i = servers.FindIndex(x => x.socket == recHostId);
				if (i != -1)
				{
					if (servers[i].OnMessage != null) servers[i].OnMessage(networkEvent, connectionId, channelId, buffer, dataSize);
				}

				// Route message to our client delegate
				// Client Connect Event
				i = clients.FindIndex(c => c.socket.Equals(recHostId));
				if (i != -1)
				{
					if (clients[i].OnMessage != null) clients[i].OnMessage(networkEvent, connectionId, channelId, buffer, dataSize);
				}

				switch (networkEvent)
				{

					// Nothing
					case NetworkEventType.Nothing:
						break;

					// Connect
					case NetworkEventType.ConnectEvent:

						// Server Connect Event
						i = servers.FindIndex(s => s.socket.Equals(recHostId));

						if (i != -1)
						{
							servers[i].AddClient(connectionId);
							if (servers[i].OnConnection != null) servers[i].OnConnection(connectionId, channelId, buffer, dataSize);
						}

						// Client Connect Event
						i = clients.FindIndex(c => c.socket.Equals(recHostId));
						if (i != -1)
						{
							clients[i].isConnected = true; // Set client connected to true
							if (clients[i].OnConnection != null) clients[i].OnConnection(connectionId, channelId, buffer, dataSize);
						}

						break;

					// Data 
					case NetworkEventType.DataEvent:

						// Server received data
						i = servers.FindIndex(x => x.socket == recHostId);
						if (i != -1)
						{
							if (servers[i].OnData != null) servers[i].OnData(connectionId, channelId, buffer, dataSize);

						}

						// Client Received Data
						i = clients.FindIndex(c => c.socket.Equals(recHostId));
						if (i != -1)
						{
							if (clients[i].OnData != null) clients[i].OnData(connectionId, channelId, buffer, dataSize);
						}
						break;

					// Disconnect
					case NetworkEventType.DisconnectEvent:

						// Server Disconnect Event
						i = servers.FindIndex(x => x.socket == recHostId);
						if (i != -1)
						{
							servers[i].RemoveClient(connectionId);
							if (servers[i].OnDisconnection != null) servers[i].OnDisconnection(connectionId, channelId, buffer, dataSize);
						}

						// Client Disconnect Event
						i = clients.FindIndex(c => c.socket.Equals(recHostId));
						if (i != -1)
						{
							clients[i].isConnected = false; // Set client connected to true
							if (clients[i].OnDisconnection != null) clients[i].OnDisconnection(connectionId, channelId, buffer, dataSize);
						}


						break;
				}

			}
		}

	}
}
