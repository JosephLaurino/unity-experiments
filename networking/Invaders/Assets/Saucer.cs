using UnityEngine;
using System.Collections;

public class Saucer : MonoBehaviour 
{
	float moveSpeed = 0.12f;
		
	void FixedUpdate()
	{
		if (transform.position.x > 14)
		{
			Destroy(gameObject);
			return;
		}
	
		transform.Translate(moveSpeed, 0, 0);
	}
	
}
