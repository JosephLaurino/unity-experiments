using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Networking;
using System.IO;

public class Buf
{
	public enum BufType { Speed, Rotate, Triple, Double, Health, Energy, QuadDamage, Bounce, Last };
	public static Color[] bufColors = { Color.red, Color.blue, Color.cyan, Color.yellow, Color.green, Color.magenta, new Color(1,0.5f,0), new Color(0, 1, 0.5f)};
	public static Color GetColor(BufType bt)
	{
		return bufColors[(int)bt];
	}
};







public class ShipControl : NetworkBehaviour
{
	public GameObject bulletPrefab;
	public AudioSource fireSound;
	
	float rotateSpeed = 200f;
	float acceleration = 12f;
	float bulletLifetime = 2;
	float topSpeed = 7.0f;
	
	[SyncVar(hook="OnHealth")]
	public int health = 100;
	
	[SyncVar]
	public int energy = 100;
		
	//[ClientVar(hook="OnClientThrust")]
	public bool clientThrust;

	//[ClientVar]
	public int clientIntValue;

	//int kills = 0; 
	int deaths = 0;
	
	[SyncVar]
	public float speedBufTimer = 0;

	[SyncVar]
	public float rotateBufTimer = 0;

	[SyncVar]
	public float tripleshotTimer = 0;

	[SyncVar]
	public float doubleshotTimer = 0;

	[SyncVar]
	public float quadDamageTimer = 0;

	[SyncVar]
	public float bounceTimer = 0;
	
	float energyTimer = 0;

	[SyncVar]
	public string playerName = "SomeGuy";
	
	void OnClientThrust(bool value)
	{
		//Debug.Log("Thrust = " + value);
	}
	Texture box;
	public ParticleSystem friction;
	public ParticleSystem thrust;
	
	float frictionStopTimer = 0;
	
	// for client movement command throttling
	float oldMoveForce = 0;
	float oldSpin = 0;
		

	// server movement
	[SyncVar]
	float thrusting;
	float spin;

	public GameObject explosionParticle;

	void OnHealth(int h)
	{
		health = h;
	}

	void Start ()
	{
		box = (Texture)Resources.Load ("box");
		friction.Stop ();
		thrust.Stop ();
		
		DontDestroyOnLoad(gameObject);
	}







	public override void OnStartLocalPlayer()
	{
		GetComponent<AudioListener>().enabled = true;
	}	
	
	public void OnDamage(int amount, float direction)
	{
		//Debug.Log ("OnDamage " + amount + " " + direction);
	}
	
	void DoHandler(int amount)
	{
		//Debug.Log ("DoHandler " + amount);
	}
	
	public void TakeDamage(int amount)
	{
		health = health - amount;
		friction.Play();
		frictionStopTimer = Time.time + 1.0f;
		
		if (health <= 0) {
			health = 0;

			//todo: reset all bufs
			
			deaths += 1;
			health = 100;
			transform.position = Vector3.zero;
			GetComponent<Rigidbody2D>().velocity = Vector3.zero;
			GetComponent<Rigidbody2D>().angularVelocity = 0;
		}
	}
			
	void Fire(Vector3 direction)
	{
		if (fireSound != null)
		{
			fireSound.Play();
		}
		
		int damage = 5;
		if (quadDamageTimer > Time.time) {
			damage = 20;
		}
		
		bool bounce = false;
		if (bounceTimer > Time.time) {
			bounce = true;
		}
		
		//TODO: add owner? need object references!
		GameObject bullet = (GameObject)GameObject.Instantiate(bulletPrefab, transform.position+direction, Quaternion.identity);
		bullet.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;
		bullet.GetComponent<Rigidbody2D>().velocity += (Vector2)(direction) * 10; 
		bullet.GetComponent<Bullet>().Config(gameObject, damage, bounce, bulletLifetime);


		//ClientScene.SpawnClientObject(bullet, 0);
		NetworkServer.Spawn(bullet);
	}
	
	/*
	public override bool CanSpawnClientObject(GameObject obj, NetworkHash128 assetId)
	{
		Debug.Log("Server spawned client object:" + obj);
		if (assetId.Equals(bulletPrefab.GetComponent<NetworkIdentity>().assetId))
		{
			if (energy > 10)
			{
				energy -= 10;
				Destroy(obj, 2.0f);
				return true;
			}
			else
			{
				return false;
			}
		}

		return true;
	}*/

	void Update()
	{
		if (NetworkServer.active)
		{
			UpdateServer();
		}
		if (NetworkClient.active)
		{
			UpdateClient();
		}
	}
	
	void UpdateServer()
	{
		// energy regen
		if (energyTimer < Time.time) {
			if (energy < 100)
			{
				if (energy+20 > 100) {
					energy = 100;
				} else {
					energy = energy + 20;
				}
			}
			energyTimer = Time.time + 1;
		}
		
		// update rotation 
		float rotate = spin * rotateSpeed;
		if (rotateBufTimer > Time.time) {
			rotate *= 2;
		}
		GetComponent<Rigidbody2D>().angularVelocity = rotate;	
		
		// update thrust
		if (thrusting != 0)
		{
			float accel = acceleration;
			if (speedBufTimer > Time.time) {
				accel *= 2;
			}
			
			Vector3 thrustVec = transform.right * thrusting * accel;
			GetComponent<Rigidbody2D>().AddForce(thrustVec);

			// restrict max speed
			float top = topSpeed;
			if (speedBufTimer > Time.time) {
				top *= 1.5f;
			}

			if (GetComponent<Rigidbody2D>().velocity.magnitude > top)
			{
				GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity.normalized * top;			
			}
		}
	}
	





	void UpdateClient()
	{
		
		if (!isLocalPlayer) {
			return;
		}
		
		if (frictionStopTimer < Time.time) {
			friction.Stop ();
		}
	
		// movement
		int spin = 0;
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			spin += 1;
		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			spin -= 1;
		}
		
		int moveForce = 0;
		if (Input.GetKey(KeyCode.UpArrow))
		{
			moveForce += 1;
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			moveForce -= 1;
		}		

		if (oldMoveForce != moveForce || oldSpin != spin)
		{
			CmdThrust (moveForce, spin);
			oldMoveForce = moveForce;
			oldSpin = spin;
			clientIntValue += 1;
			//Debug.LogError("clientIntValue = " + clientIntValue);
		}
		

		// control thrust particles
		if (moveForce == 0.0f)
		{
			thrust.Stop ();
			GetComponent<AudioSource>().Pause();
			clientThrust = false;
		}
		else
		{
			thrust.Play ();
			GetComponent<AudioSource>().Play();
			clientThrust = true;
		}
					
		// fire
		if (Input.GetKeyDown(KeyCode.Space))
		{
			CmdFire();
		}

		// center camera.. only if this is MY player!
		Vector3 pos = transform.position;
		pos.z = -50;
		Camera.main.transform.position = pos;
	}
	
	public void AddBuf(Buf.BufType buf)
	{
		if (buf == Buf.BufType.Speed) {
			speedBufTimer = Time.time + 10;
		}
		if (buf == Buf.BufType.Rotate) {
			rotateBufTimer = Time.time + 10;
		}
		if (buf == Buf.BufType.Triple) {
			tripleshotTimer = Time.time + 10;
		}
		if (buf == Buf.BufType.Double) {
			doubleshotTimer = Time.time + 10;
		}
		if (buf == Buf.BufType.Health) {
			health = health + 20;
			if (health >= 100)
			{
				health = 100;
			}
		}
		if (buf == Buf.BufType.Energy) {
			energy = energy + 50;
			if (energy >= 100)
			{
				energy = 100;
			}
		}	
		if (buf == Buf.BufType.QuadDamage) {
			quadDamageTimer = Time.time + 10;
		}
		if (buf == Buf.BufType.Bounce) {
			bounceTimer = Time.time + 10;
		}
	}
	
	void OnCollision2DEnter(Collision2D other)
	{
		if (NetworkServer.active) {
			return;
		}
		
		Asteroid a = other.gameObject.GetComponent<Asteroid>();
		if (a != null)
		{
			TakeDamage(5);
		}	
	}
		
	// --- commands ---


	[Command]
	public void CmdThrust(float thrusting, int spin)
	{
		this.thrusting = thrusting;
		this.spin = spin;
	}
	
	[Command]
	public void CmdFire()
	{
		if (energy >= 10)
		{			
			if (tripleshotTimer > Time.time)
			{
				Fire(Quaternion.Euler(0, 0, 20) * transform.right);
				Fire(Quaternion.Euler(0, 0, -20) * transform.right);
				Fire(transform.right);
			}
			else if (doubleshotTimer > Time.time)
			{
				Fire(Quaternion.Euler(0, 0, -10) * transform.right);
				Fire(Quaternion.Euler(0, 0, 10) * transform.right);
			}
			else
			{
				Fire(transform.right);
			}
			energy -= 10;
			if (energy <= 0) {
				energy = 0;
			}
		}	
	}
	
	[Command]
	public void CmdSetName(string name)
	{
		this.playerName = name;
	}

	void OnGUI()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		
		// draw the name with a shadow (colored for buf)	
		GUI.color = Color.black;
		GUI.Label(new Rect(pos.x-20, Screen.height - pos.y - 30, 100, 30), playerName);
		
		GUI.color = Color.white;
		if (speedBufTimer > Time.time) { GUI.color = Buf.GetColor(Buf.BufType.Speed); }
		if (rotateBufTimer > Time.time) { GUI.color = Buf.GetColor(Buf.BufType.Rotate); }
		if (tripleshotTimer > Time.time) { GUI.color = Buf.GetColor(Buf.BufType.Triple); }
		if (doubleshotTimer > Time.time) { GUI.color = Buf.GetColor(Buf.BufType.Double); }
		if (quadDamageTimer> Time.time) { GUI.color = Buf.GetColor(Buf.BufType.QuadDamage); }
		if (bounceTimer> Time.time) { GUI.color = Buf.GetColor(Buf.BufType.Bounce); }		
		
		GUI.Label(new Rect(pos.x-21, Screen.height - pos.y - 31, 100, 30), playerName);		
		
		// draw health bar background
		GUI.color = Color.grey;
		GUI.DrawTexture (new Rect(pos.x-26, Screen.height - pos.y + 20, 52, 7), box);
		
		// draw health bar amount
		GUI.color = Color.green;
		GUI.DrawTexture (new Rect(pos.x-25, Screen.height - pos.y + 21, health/2, 5), box);
		
		// draw energy bar background
		GUI.color = Color.grey;
		GUI.DrawTexture (new Rect(pos.x-26, Screen.height - pos.y + 27, 52, 7), box);
		
		// draw energy bar amount
		GUI.color = Color.magenta;
		GUI.DrawTexture (new Rect(pos.x-25, Screen.height - pos.y + 28, energy/2, 5), box);		
	}



	// ShipControl
public void FakeOnUnserializeVars(NetworkReader reader, bool initialState)
{
	int num = (int)reader.ReadPackedUInt32();
	if ((num & 1) != 0)
	{
		if (initialState)
		{
			this.health = (int)reader.ReadPackedUInt32();
		}
		else
		{
			this.OnHealth((int)reader.ReadPackedUInt32());
		}
	}
	if ((num & 2) != 0)
	{
		this.energy = (int)reader.ReadPackedUInt32();
	}
	if ((num & 4) != 0)
	{
		this.speedBufTimer = reader.ReadSingle();
	}
	if ((num & 8) != 0)
	{
		this.rotateBufTimer = reader.ReadSingle();
	}
	if ((num & 16) != 0)
	{
		this.tripleshotTimer = reader.ReadSingle();
	}
	if ((num & 32) != 0)
	{
		this.doubleshotTimer = reader.ReadSingle();
	}
	if ((num & 64) != 0)
	{
		this.quadDamageTimer = reader.ReadSingle();
	}
	if ((num & 128) != 0)
	{
		this.bounceTimer = reader.ReadSingle();
	}
	if ((num & 256) != 0)
	{
		this.playerName = reader.ReadString();
	}
	if ((num & 512) != 0)
	{
		this.thrusting = reader.ReadSingle();
	}
}


	// ShipControl
	/*
public bool xxOnSerializeVars(NetworkWriter writer, int channelId, bool forceAll)
{
	if (forceAll)
	{
		this.m_DirtyBits = 4294967295u;
	}

	bool dirty = false;
	if (channelId == 0 || forceAll)
	{
		if (!dirty)
		{
			writer.WritePackedUInt32(this.m_DirtyBits);
			dirty = true;
		}

		if ((this.m_DirtyBits & 1u) != 0u)
		{
			writer.WritePackedUInt32((uint)this.health);
		}
		if ((this.m_DirtyBits & 2u) != 0u)
		{
			writer.WritePackedUInt32((uint)this.energy);
		}
		if ((this.m_DirtyBits & 4u) != 0u)
		{
			writer.Write(this.speedBufTimer);
		}
		if ((this.m_DirtyBits & 8u) != 0u)
		{
			writer.Write(this.rotateBufTimer);
		}
		if ((this.m_DirtyBits & 16u) != 0u)
		{
			writer.Write(this.tripleshotTimer);
		}
		if ((this.m_DirtyBits & 32u) != 0u)
		{
			writer.Write(this.doubleshotTimer);
		}
		
	}

	if (channelId == 1 || forceAll)
	{
		if (!dirty)
		{
			writer.WritePackedUInt32(this.m_DirtyBits);
			dirty = true;
		}

		if ((this.m_DirtyBits & 64u) != 0u)
		{
			writer.Write(this.quadDamageTimer);
		}
		if ((this.m_DirtyBits & 128u) != 0u)
		{
			writer.Write(this.bounceTimer);
		}
		if ((this.m_DirtyBits & 256u) != 0u)
		{
			writer.Write(this.playerName);
		}
		if ((this.m_DirtyBits & 512u) != 0u)
		{
			writer.Write(this.thrusting);
		}
	}

	if (!dirty)
	{
		writer.WritePackedUInt32(this.m_DirtyBits);
		dirty = true;
	}

	this.m_DirtyBits = 0u;
	return dirty;
}
	 * */

	/*
	public bool xxOnSerializeClientVars(NetworkWriter writer, int channelId, bool forceAll)
	{
		bool flag = false;
		if (channelId == 0)
		{
			if ((this.m_DirtyClientBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(this.m_DirtyClientBits);
					flag = true;
				}
				writer.Write(this.clientThrust);
			}
			if ((this.m_DirtyClientBits & 2u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(this.m_DirtyClientBits);
					flag = true;
				}
				writer.WritePackedUInt32((uint)this.clientIntValue);
			}
		}
		if (!flag)
		{
			writer.WritePackedUInt32(this.m_DirtyClientBits);
		}
		return flag;
	}



	public void xxOnUnserializeClientVars(NetworkReader reader, int channelId, bool initialState)
	{
		int num = (int)reader.ReadPackedUInt32();
		if (channelId == 0)
		{
			if ((num & 1) != 0)
			{
				this.clientThrust = reader.ReadBoolean();
			}
			if ((num & 2) != 0)
			{
				this.clientIntValue = (int)reader.ReadPackedUInt32();
			}
		}
	}
	*/

	/*
	// ShipControl
	public override void OnUnserializeClientVars(NetworkReader reader, int channelId, bool initialState)
	{
		if (initialState)
		{
			this.clientThrust = reader.ReadBoolean();
			this.clientIntValue = (int)reader.ReadPackedUInt32();
			return;
		}
		int num = (int)reader.ReadPackedUInt32();
		if (channelId == 0)
		{
			if ((num & 1) != 0)
			{
				this.clientThrust = reader.ReadBoolean();
				SetSyncVar(this.clientThrust, ref this.clientThrust, 1u, 4u, 1u);
			}
			if ((num & 2) != 0)
			{
				this.clientIntValue = (int)reader.ReadPackedUInt32();
				SetSyncVar(this.clientIntValue, ref this.clientIntValue, 2u, 8u, 1u);
			}
		}
		if (channelId == 1)
		{
		}
	}
	*/
}

