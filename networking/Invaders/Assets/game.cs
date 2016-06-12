using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class game : MonoBehaviour {

/*
	public List<GameObject> aliens = new List<GameObject>();
	public List<GameObject> players = new List<GameObject>();
	public List<GameObject> shields = new List<GameObject>();
	public List<GameObject> alienbullets = new List<GameObject>();
	public GameObject saucer;
	
	public float next_tick = 0.0f;
	public float tick_length = 0.2f; 
	public float alien_dir = 2.0f;
	public int score = 0;
	public int lives = 3;
	float death_timer = 0.0f;
	float score_timer = 0.0f;
	Vector2 scorePos;
	int scoreValue;
	public bool finished = false;

	public void RemoveSaucer()
	{
		if (saucer == null)
		{
			return;
		}
		Destroy (saucer);
		saucer = null;
	}
	
	void CreateAlien(Object source, int posX, int posY, int id)
	{
		//Debug.Log("CreateAlien " + posX + " " + posY);
		GameObject a1 = (GameObject)Instantiate(source);
		a1.transform.position = new Vector3(posX,posY,0.0f);
		a1.GetComponent<alien>().alive = true;
		a1.GetComponent<alien>().id = id;
		a1.GetComponent<alien>().column = posX;
		a1.GetComponent<alien>().row = posY;
		aliens.Add(a1);
	}
	
	void CreateAliens()
	{
		// Create Aliens
		
		int offset = 0;
		int startx = -80;
		for (int i=0; i < 10; i++)
		{
			CreateAlien(Resources.Load("alien1"), startx, 120, offset);
			startx += 16;
			offset +=1;
		}
		
		startx = -80;
		for (int i=0; i < 10; i++)
		{
			CreateAlien(Resources.Load("alien2"), startx, 100, offset);
			startx += 16;
			offset +=1;
		}
		
		startx = -80;
		for (int i=0; i < 10; i++)
		{
			CreateAlien(Resources.Load("alien3"), startx, 80, offset);
			startx += 16;
			offset +=1;
		}
	}
	
	void CreateShield(Object source, int posX, int posY)
	{
		for (int y=posY; y < posY+10; y += 2) {
			for (int x=posX-5; x < posX+5; x += 2) 
			{
				if (y == posY+8 && x == posX-5 || y == posY+8 && x == posX+3) {
					continue;
				}
				GameObject shield = (GameObject)Instantiate(source, new Vector3(x, y, 0), Quaternion.identity);
				shields.Add(shield);
			}
		}
	}
	
	void CreateShields()
	{
		// Create Shields
		Object source = Resources.Load("shield");
		CreateShield (source, -60, 10);
		CreateShield (source, 0, 10);
		CreateShield (source, 60, 10);
	}
	
	void RemoveShields()
	{
		// remove all shields
		int numShields = shields.Count;
		for (int i=numShields-1; i >= 0; i--)
		{
			GameObject shield = shields[i];
			Destroy (shield);
			shields.RemoveAt (i);
		}		
	}
	
	void RemoveAlienBullets()
	{
		// remove all shields
		int numAlienBullets = alienbullets.Count;
		for (int i=numAlienBullets-1; i >= 0; i--)
		{
			GameObject alienbullet = alienbullets[i];
			Destroy (alienbullet);
			alienbullets.RemoveAt (i);
		}		
	}
	
	
	void RecreateShields()
	{
		RemoveShields();
		CreateShields();
	}
	
	void NewWave()
	{
		RecreateShields();
		RemoveAlienBullets();
		CreateAliens();
		alien_dir = 2.0f;	// TODO: make this faster? start at lower position?
		tick_length = 0.2f;
	}

	// Use this for initialization
	void Start() 
	{
		alien_dir = 2.0f;
		tick_length = 0.2f;
		score = 0;
		lives = 3;
		
		CreateAliens ();
		CreateShields();
	}
	
	void OnGUI()
	{
		GUI.Label(new Rect(20, 4, 150, 100), "Score: " + score);
		GUI.Label(new Rect(Screen.width-100, 4, 150, 100), "Lives: " + lives);
		if (Time.time < death_timer)
		{
			GUI.Label(new Rect((Screen.width/2)-40, 20 , 150, 100), "You Died");
		}
		if (Time.time < score_timer)
		{
			GUI.Label(new Rect(scorePos.x-5, Screen.height - scorePos.y-5 , 150, 100), "[" + scoreValue + "]");
		}				   
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (finished) {
			return;
		}
		
		// update aliens
		if (Time.time >= next_tick) {
			next_tick = Time.time + tick_length;
			
			bool foundEdge = false;
			foreach (GameObject alien in aliens)
			{
				alien.transform.position += new Vector3(alien_dir,0,0);
				if (alien.transform.position.x > 100 || alien.transform.position.x < -100)
				{
					foundEdge = true;
				}
				
				// can fire if the lowest in my column
				bool canShoot = true;
				int column = alien.GetComponent<alien>().column;
				int row = alien.GetComponent<alien>().row;
				foreach (GameObject other in aliens)
				{
					if (other.GetComponent<alien>().column == column) {
						if (other.GetComponent<alien>().row < row) {
							canShoot = false;
							break;
						}
					}
				}
				if (canShoot)
				{
					float r = Random.value;
					if (r > 0.9f)
					{
						//Debug.Log ("fire bullet" + r);
						GameObject abullet = (GameObject)Instantiate(Resources.Load("alienbullet"), alien.transform.position + new Vector3(0,-12,0),Quaternion.identity);
						abullet.rigidbody.velocity = new Vector3(0,-40,0);
						alienbullets.Add(abullet);
					}
				}
						
			}
			
			if (foundEdge) {
				alien_dir = -alien_dir;
				//alien_dir *= 1.2f;
				tick_length = tick_length * 0.9f;
				
				foreach (GameObject alien in aliens)
				{
					alien.transform.Translate(0,-8,0); 
				}
			}
			
			
			if (saucer == null)
			{
				float r = Random.value;
				if (r > 0.97f)
				{
					saucer = (GameObject)Instantiate(Resources.Load("saucer"), new Vector3(-100, 140, 0), Quaternion.identity);
					saucer.GetComponent<saucer>().alive = true;
				}
			}
		}
		if (saucer != null)
		{
			saucer.transform.Translate(0.8f,0,0);
		}
			
	}

	public void RemoveAlien(GameObject alien)
	{
		//Debug.Log("RemoveAlien");
		int value = alien.GetComponent<alien>().score;
		Score (value, alien.transform.position);
		aliens.Remove(alien);
		Destroy (alien);
		if (aliens.Count == 0)
		{
			NewWave();
		}
	}
	
	public bool PlayerHit(GameObject player)
	{
		if (lives == 0)
		{
			finished = true;
			GameObject.Find("Loader").GetComponent<Loader>().EndGame(score);
			return false;
		}
		lives -= 1;
		player.transform.position = new Vector3(0,0,0);
		death_timer = Time.time + 2.0f;
		return true;
	}
	
	public void ShieldHit(GameObject shield)
	{
		shields.Remove(shield);
		Destroy (shield);
	}
	
	public void SaucerHit(GameObject saucer)
	{
		Score (300, saucer.transform.position);
		RemoveSaucer();
	}
	
	public void AlienBulletRemove(GameObject alienbullet)
	{
		alienbullets.Remove(alienbullet);
		Destroy (alienbullet);
	}
	
	void Score(int value, Vector3 pos)
	{
		score_timer = Time.time + 0.5f;
		scoreValue = value; 
		scorePos = Camera.main.WorldToScreenPoint(pos);
		score += value;
	}
	*/
}

