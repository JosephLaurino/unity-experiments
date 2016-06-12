using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public partial class Asteroid : NetworkBehaviour {

	public int numCreates = 3;


	public GameObject prefab;

	public static int numAsteroids = 0;
	
	[SyncVar]
	public int size = 4;

	// Use this for initialization
	void Start () {
		numAsteroids += 1;
		//Debug.Log("Asteroid start");
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		//if (UServer.active) {	
		//	isDirty = true;
		//}
	}
	
	public void Update()
	{
	}

	public void Explode()
	{
		if (size >= 1) {
			int num = Random.Range(1,numCreates+1);
			
			for (int i=0; i < num; i++)
			{
				int dx = Random.Range(0,4)-2;
				int dy = Random.Range(0,4)-2;
				Vector3 diff = new Vector3(dx*0.3f, dy*0.3f, 0);

				GameObject a = (GameObject)GameObject.Instantiate(this.gameObject, transform.position+diff, Quaternion.identity);
				a.transform.localScale = new Vector3(size-1,size-1,size-1);
				a.GetComponent<Asteroid>().size = size - 1;
				NetworkServer.Spawn(a);
			}
		}
		
		numAsteroids -= 1;
		Destroy (gameObject);
	}
}
