using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerControl : NetworkBehaviour
{
	[SyncVar]
	public int moveX = 0;
	
	
	[SyncVar(hook="OnLives")] 
	public int lives = 3;
	
	[SyncVar]
	public int score = 0;
	
	[SyncVar]
	public bool alive = true;
	
	public GameObject bulletPrefab;
	
	GameObject myBullet;
	float moveSpeed = 0.2f;
	int oldDx;
	
	void OnLives(int lives)
	{
		Debug.Log("lives = " + lives);
		this.lives = lives;
	}
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
	public override void OnStartLocalPlayer()
	{
		GetComponent<SpriteRenderer>().material.color = Color.green;
	}


	void Update()
	{
		if (!isLocalPlayer)
		{
			return;
		}
		if (!alive)
		{
			return;
		}
		
		int dx = 0;
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			dx -= 1;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			dx += 1;
		}

		if (dx != oldDx)
		{
			CmdMove(dx);
			oldDx = dx;
		}
		
		if (Input.GetKey(KeyCode.Space))
		{
			CmdShoot();
		}
		
		if (InvadersGame.singleton != null)
		{
			InvadersGame.singleton.SetScore(score);
			InvadersGame.singleton.SetLives(lives);
		}
	}

	void FixedUpdate()
	{
		transform.Translate(moveX*moveSpeed, 0, 0);
	}
	
	[Command]
	void CmdMove(int dx)
	{
		if (!alive)
			return;
			
		moveX = dx;
		GetComponent<NetworkTransform>().SetDirtyBit(1);
	}
	
	[Command]
	void CmdShoot()
	{
		if (!alive)
			return;
			
		if (myBullet == null)
		{
			myBullet = (GameObject)GameObject.Instantiate(bulletPrefab, transform.position + Vector3.up, Quaternion.identity);
			myBullet.GetComponent<PlayerBullet>().owner = this;
			NetworkServer.Spawn(myBullet);
		}
	}

	[Server]	
	public void HitByBullet()
	{
		lives -= 1;
		transform.position = NetworkManager.singleton.GetStartPosition().position;
		GetComponent<NetworkTransform>().SetDirtyBit(1);
		
		if (lives == -1)
		{
			// gameover!
			alive = false;
			moveX = 0;
			lives = 0;
			RpcDead();
		}
	}
	
	[ClientRpc]
	public void RpcDead()
	{
		InvadersGame.singleton.SetMessage("You Are Dead!");
	}
	
}
