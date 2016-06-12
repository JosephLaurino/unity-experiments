using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerBullet : MonoBehaviour
{
	const float moveSpeed = 0.5f;
	public PlayerControl owner;
	
	void FixedUpdate()
	{
		transform.Translate(0,moveSpeed,0);
		
		if (transform.position.y  > 15.0f)
		{
			NetworkServer.Destroy(gameObject);
		}
	}
	
	void OnTriggerEnter2D(Collider2D collider)
	{
		if (!NetworkServer.active)
			return;

		AlienInvader hitAlien = collider.gameObject.GetComponent<AlienInvader>();
		if (hitAlien != null)
		{
			owner.score += hitAlien.score;		
			NetworkServer.Destroy(hitAlien.gameObject);
			NetworkServer.Destroy(gameObject);
		}
		
		Shield hitShield = collider.gameObject.GetComponent<Shield>();
		if (hitShield != null)
		{
			NetworkServer.Destroy(hitShield.gameObject);
			NetworkServer.Destroy(gameObject);
		}
	}
}
