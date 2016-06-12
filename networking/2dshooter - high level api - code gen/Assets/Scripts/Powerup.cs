using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class Powerup : NetworkBehaviour {

	[SyncVar]
	public Buf.BufType mbuf;
	
	[SyncVar]
	public int testMe;
	
	static public int numPowerups = 0;
	
	public override void OnStartClient ()
	{
		//Debug.Log ("StartClient " + gameObject + " mbuf:" + mbuf + " testMe:" + testMe);
	
		float dir = 170.0f;
		transform.rotation = Quaternion.Euler(0, 180, dir);
		GetComponent<Rigidbody2D>().angularVelocity = dir;

		Color c = Buf.bufColors[(int)mbuf];
		GetComponent<Renderer>().material.color = c;

		if (!isServer) {
			numPowerups += 1;
		}
	}
	
	public override void OnStartServer()
	{
		numPowerups += 1;
	}

	void OnGUI()
	{
		GUI.color = Buf.bufColors[(int)mbuf];
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		GUI.Label(new Rect(pos.x-20, Screen.height - pos.y - 30, 100, 30), mbuf.ToString());
	}
	
	public override void OnNetworkDestroy()
	{
		AudioSource.PlayClipAtPoint(GetComponent<AudioSource>().clip, transform.position);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!NetworkServer.active) {
			return;
		}
			
		ShipControl s = other.gameObject.GetComponent<ShipControl>();
		if (s != null)
		{
			s.AddBuf(mbuf);
			Destroy (gameObject);
		}
		
	}
	
	void OnDestroy()
	{
		numPowerups -= 1;
	}
}
