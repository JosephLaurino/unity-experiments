using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class AlienInvader : NetworkBehaviour {
	
	public bool alive = true;
	public float column = 0;
	public float row = 0;
	public int score = 0;
	public bool canShoot = false;
	
	public GameObject bulletPrefab;
	
	public override void OnStartClient()
	{
		if (score == 100)
			return;
		
		if (!InvadersGame.singleton.aliens.Contains(this.gameObject))
		{
			InvadersGame.singleton.aliens.Add(this.gameObject);
		}
	}
	
	public override void OnNetworkDestroy()
	{
		//InvadersGame.singleton.aliens.Remove(this.gameObject);
	}
	
	public void Setup(float column, float row)
	{
		this.column = column;
		this.row = row;
	}
	
	void FixedUpdate()
	{
		if (NetworkServer.active && canShoot)
		{
			if (Random.Range(0,1.0f) > 0.996f)
			{
				GameObject myBullet = (GameObject)GameObject.Instantiate(bulletPrefab, transform.position - Vector3.up, Quaternion.identity);
				NetworkServer.Spawn(myBullet);
			}
		}
	}
	
	[ServerCallback]
	void OnTriggerEnter2D(Collider2D collider)
	{
		Shield hitShield = collider.gameObject.GetComponent<Shield>();
		if (hitShield != null)
		{
			NetworkServer.Destroy(hitShield.gameObject);
		}
	}
}
		