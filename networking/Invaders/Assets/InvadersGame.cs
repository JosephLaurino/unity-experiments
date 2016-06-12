using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


public class InvadersGame : NetworkBehaviour
{
	public GameObject alien1Prefab;
	public GameObject alien2Prefab;
	public GameObject alien3Prefab;
	public GameObject playerPrefab;
	public GameObject saucerPrefab;
	public GameObject shieldPrefab;
	
	public UnityEngine.UI.Text scoreUI;
	public UnityEngine.UI.Text livesUI;
	public UnityEngine.UI.Text messageUI;
	
	public Transform saucerSpawnPoint;
	
	public List<GameObject> aliens = new List<GameObject>();
	public List<GameObject> shields = new List<GameObject>();
	
	static public InvadersGame singleton;

	float nextTick = 0.0f;
	
	[SyncVar]
	float tickLength = 0.2f; 
	
	[SyncVar]
	float alienDir = 0.2f;
	
	[SyncVar]
	bool gameOver = false;
	
	public GameObject saucer;
	
	void Awake()
	{
		singleton = this;
		aliens = new List<GameObject>();
	}
	
	public void SetScore(int score)
	{
		scoreUI.text = "0" + score;
	}
	
	public void SetLives(int lives)
	{
		livesUI.text = "0" + lives;
	}
	
	public void SetMessage(string message)
	{
		messageUI.text = message;
	}
	
	public void ExitGame()
	{
		if (NetworkServer.active)
		{
			NetworkManager.singleton.StopServer();
		}
		if (NetworkClient.active)
		{
			NetworkManager.singleton.StopClient();
		}
	}
	
	void CreateShield(GameObject prefab, int posX, int posY)
	{
		float dy = 0.41f;
		float dx = 0.41f;
		
		int ycount = 0;
		for (float y=posY; y < posY+2; y += dy)
		{
			int xcount = 0;
			for (float x=posX; x < posX+2; x += dx) 
			{
				if (ycount == 4 && (xcount == 0 || xcount == 4))
				{
					xcount += 1;
					continue;
				}
				GameObject shield = (GameObject)Instantiate(prefab, new Vector3(x-1, y, 0), Quaternion.identity);
				shields.Add(shield);
				NetworkServer.Spawn(shield);
				xcount += 1;
			}
			ycount += 1;
		}
	}
	
	void CreateShields()
	{
		// Create Shields
		CreateShield (shieldPrefab, -7, -1);
		CreateShield (shieldPrefab, 0, -1);
		CreateShield (shieldPrefab, 7, -1);
	}
	
	void CreateSaucer()
	{
		saucer = (GameObject)GameObject.Instantiate(saucerPrefab, saucerSpawnPoint.position, Quaternion.identity);
		NetworkServer.Spawn(saucer);
	}
		
		void CreateAlien(GameObject prefab, float posX, float posY)
	{
		GameObject a1 = (GameObject)Instantiate(prefab);
		a1.transform.position = new Vector3(posX,posY,0.0f);
		a1.GetComponent<AlienInvader>().Setup(posX, posY);
		aliens.Add(a1);
		
		NetworkServer.Spawn(a1);
	}
	
	public void CreateAliens()
	{
		float startx = -8;
		for (int i=0; i < 10; i++)
		{
			CreateAlien(alien1Prefab, startx, 12);
			startx += 1.6f;
		}
		
		startx = -8;
		for (int i=0; i < 10; i++)
		{
			CreateAlien(alien2Prefab, startx, 10);
			startx += 1.6f;
		}
		
		startx = -8;
		for (int i=0; i < 10; i++)
		{
			CreateAlien(alien3Prefab, startx, 8);
			startx += 1.6f;
		}
	}
	
	public override void OnStartServer()
	{
		CreateAliens();
		CreateShields();
		CreateSaucer();
	}
	
	[ServerCallback]
	void Update()
	{
		if (gameOver) {
			return;
		}
		
		// update aliens
		if (Time.time >= nextTick) {
			nextTick = Time.time + tickLength;

			bool foundAlien = false;			
			bool foundEdge = false;
			foreach (GameObject alienObj in aliens)
			{
				if (alienObj == null)
					continue;
					
				AlienInvader ai = alienObj.GetComponent<AlienInvader>();					
				if (ai.score > 100)
					continue;
				
				foundAlien = true;
				alienObj.transform.position += new Vector3(alienDir,0,0);
				alienObj.GetComponent<NetworkTransform>().SetDirtyBit(1);
				
				if (alienObj.transform.position.x > 10 || alienObj.transform.position.x < -10)
				{
					foundEdge = true;
				}
				
				// can shoot if the lowest in my column
				bool canShoot = true;
				float column = ai.column;
				float row = ai.row;
				foreach (GameObject other in aliens)
				{
					if (other == null)
						continue;
						
					if (other.GetComponent<AlienInvader>().column == column) {
						if (other.GetComponent<AlienInvader>().row < row) {
							canShoot = false;
							break;
						}
					}
				}
				ai.canShoot = canShoot;
			}
			
			if (!foundAlien)
			{
				CreateAliens();
				tickLength = 0.2f; 
			}
			
			if (foundEdge) {
				alienDir = -alienDir;
				tickLength = tickLength * 0.9f; // get faster
				
				foreach (GameObject alien in aliens)
				{
					if (alien == null)
						continue;
						
					alien.transform.Translate(0,-0.8f,0); 
					alien.GetComponent<NetworkTransform>().SetDirtyBit(1);
				}
			}
			
			if (saucer == null)
			{
				if (Random.Range(0,1.0f) > 0.92f)
				{
					CreateSaucer();
				}
			}
			
		}
	}
}
