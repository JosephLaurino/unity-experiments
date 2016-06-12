using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AsteroidGame : MonoBehaviour {
	
	public Spawner spawner;
	public Texture minimap;
	
	public static string txtMessage = "";
	
	void OnGUI()
	{
		GUI.Label(new Rect((Screen.width/2)-40, 4, 150, 100), txtMessage);
		GUI.DrawTexture (new Rect(2, 2, Screen.width/8, Screen.width/8), minimap);
	}

}
