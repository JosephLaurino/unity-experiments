using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class Bullet : NetworkBehaviour {

	public bool dead = false;
	
	bool bounce = false;
	int damage = 5;
	GameObject owner;
	
	public GameObject explosionParticle;

	public void Config(GameObject owner, int damage, bool bounce, float lifetime)
	{
		this.owner = owner;
		this.damage = damage;
		this.bounce = bounce;
		
		if (GetComponent<NetworkIdentity>().isServer) {
			Destroy(gameObject, lifetime);
		}
	}
	
	public override void OnNetworkDestroy()
	{
		// create explosion on client
		Vector3 pos = transform.position;
		pos.z = -2;
		GameObject ex = (GameObject)Instantiate(explosionParticle, pos, Quaternion.identity);
		GameObject.Destroy(ex, 0.5f);
	}

	void OnCollisionEnter2D (Collision2D other)
	{
		if (!NetworkServer.active) {
			return;
		}

		if (dead) {
			return;
		}

		Asteroid a = other.gameObject.GetComponent<Asteroid>();
		if (a != null)
		{
			dead = true;
			a.Explode();
			
			Destroy(gameObject);
		}
		
		Wall w = other.gameObject.GetComponent<Wall>();
		Obstacle o = other.gameObject.GetComponent<Obstacle>();
		if (bounce == false && (w != null || o != null))
		{
			Destroy(gameObject);
		}
		
		ShipControl s = other.gameObject.GetComponent<ShipControl>();
		if (s != null)
		{
			if (s != owner) {
				s.TakeDamage(damage);
				Destroy(gameObject);
			}
		}
	}

	IEnumerator Fade() {
    for (float f = 1f; f >= 0; f -= 0.1f) {
        Color c = GetComponent<Renderer>().material.color;
        c.a = f;
        GetComponent<Renderer>().material.color = c;
        yield return null;
    }
}


}
