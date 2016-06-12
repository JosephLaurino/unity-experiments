using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;

/*
public class UnetDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
	
		int posY = 10;
		int yDiff = 15;
		
		GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 100, 20), "netId:" + GetComponent<UNetView>().netId);	
		posY += yDiff;
		GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 200, 20), "assetId:" + GetComponent<UNetView>().assetId);	
		posY += yDiff;
		GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 200, 20), "pos: (" + transform.position.x + "," + transform.position.y + ")");
		posY += yDiff;
				
		if (GetComponent<UNetView>().isLocalPlayer) {
			GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 200, 20), "IsLocalPlayer");
			posY += yDiff;
		}
		if (GetComponent<UNetView>().isServer) {
			GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 200, 20), "IsServer");
			posY += yDiff;
		}
		
		if (GetComponent<UNetView>().isClient) {
			GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 200, 20), "IsClient");
			posY += yDiff;
		}
		
		foreach (UNetBehaviour beh in GetComponents<UNetBehaviour>())
		{
			GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 200, 20), "beh: " + beh.GetType().Name);
			posY += yDiff;
			foreach (FieldInfo field in beh.GetType ().GetFields())
			{
				System.Attribute[] markers = (System.Attribute[])field.GetCustomAttributes(typeof(SyncVar), true);
				if (markers.Length > 0)
				{
					GUI.Label(new Rect(pos.x-20, Screen.height - pos.y + posY, 200, 20), "  Var " + field.Name + "=" + field.GetValue(beh));
					posY += yDiff;
				}
			}
		}
	}
}

	
	
	*/
	