using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

/*
public class Manager : MonoBehaviour {

	bool isAtStartup = true;
	
	NetworkClient myClient;
	NetworkServer myServer;
	
	public GameObject playerPrefab;
	public GameObject cylinderPrefab;
	
	static Manager singleton;
	
	void Awake()
	{
		singleton = this;
	}
	
	void StartServer()
	{
		Tuner t = new Tuner("manyServer");
		t.awakeTimeout = 1;
		t.minUpdateTimeout = 1;
		
		NetworkServer.Configure(t,4);
		
		NetworkServer.Listen(4444);	
		NetworkServer.RegisterHandler(MsgType.SYSTEM_READY, OnPlayerReadyMessage);
		NetworkServer.RegisterHandler(MsgType.SYSTEM_DISCONNECT, OnServerDisconnected);
	}
	
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.S))
		{
			StartServer();
			isAtStartup = false;
		}
		
		if (Input.GetKeyDown(KeyCode.C))
		{
			Tuner t = new Tuner("manyClient");
			t.awakeTimeout = 1;
			t.minUpdateTimeout = 1;
			
			myClient = new NetworkClient();
			myClient.Configure(t,4);
			
			myClient.Connect("127.0.0.1", 4444);	
			myClient.RegisterHandler(MsgType.SYSTEM_CONNECT, OnConnected);
			ClientManager.RegisterPrefab(playerPrefab);
			ClientManager.RegisterPrefab(cylinderPrefab);

			isAtStartup = false;
		}
		
		
		if (Input.GetKeyDown(KeyCode.B))
		{
			StartServer();
			
			myClient = ClientManager.ConnectLocalServer();
				
			ClientManager.RegisterHandler(MsgType.SYSTEM_CONNECT, OnConnected);
			ClientManager.RegisterPrefab(playerPrefab);
			ClientManager.RegisterPrefab(cylinderPrefab);
			
			isAtStartup = false;
		}
	}
	
	void OnGUI()
	{
		if (isAtStartup)
		{
			GUI.Label(new Rect(2, 10, 150, 100), "Press S for server");		
			GUI.Label(new Rect(2, 30, 150, 100), "Press B for both");		
			GUI.Label(new Rect(2, 50, 150, 100), "Press C for client");
		}
	}
	
	// client function
	public void OnConnected(NetworkConnection conn, NetworkReader reader)
	{
		ClientManager.Ready(conn);
		myClient.SetMaxDelay(0.0f);
	}
	
	// server function
	public void OnPlayerReadyMessage(NetworkConnection conn, NetworkReader reader)
	{
		GameObject thePlayer = (GameObject)Instantiate(singleton.playerPrefab, new Vector3(4,4,4), Quaternion.identity);
		NetworkServer.SetClientReady(conn, thePlayer);
		conn.SetMaxDelay(0);
	}
	
	public void OnServerDisconnected(NetworkConnection conn, NetworkReader reader)
	{
		NetworkServer.Destroy(conn.player);
		conn.player = null;
	}
	
}



*/
