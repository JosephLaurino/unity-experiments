using UnityEngine;
using System.Collections;
using System.Reflection;
using UnityEngine.Networking;

public class UnetDebug : MonoBehaviour {

	public int posX = 40;
	public int m_PosY = -50;
	bool m_Show = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		if (!m_Show) {
			if (GUI.Button(new Rect(pos.x+posX, Screen.height - pos.y + m_PosY, 80, 18), "Unet Info")) {
				m_Show = true;
			}
		}
		if (!m_Show) {
			return;
		}
	
		int posY = m_PosY;
		int yDiff = 15;
		
		GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 100, 20), "netId:" + GetComponent<NetworkIdentity>().netId);	
		posY += yDiff;
		GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), "assetId:" + GetComponent<NetworkIdentity>().assetId);	
		posY += yDiff;
		
		GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), 
		          System.String.Format("pos: ({0:F2}, {1:F2}, {2:F2})" , transform.position.x, transform.position.y, transform.position.z));
		posY += yDiff;
		
		if (GetComponent<Rigidbody>())
		{
			GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), 
			          System.String.Format("vel: ({0:F2}, {1:F2}, {2:F2})" , GetComponent<Rigidbody>().velocity.x,GetComponent<Rigidbody>().velocity.y,GetComponent<Rigidbody>().velocity.z));
			          posY += yDiff;
		}
		
		if (GetComponent<Rigidbody2D>())
		{
			GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), 
			          System.String.Format("vel: ({0:F2}, {1:F2})" , GetComponent<Rigidbody2D>().velocity.x,GetComponent<Rigidbody2D>().velocity.y));
			posY += yDiff;
		}		
				
		if (GetComponent<NetworkIdentity>().isLocalPlayer) {
			GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), "IsLocalPlayer");
			posY += yDiff;
		}
		if (GetComponent<NetworkIdentity>().isServer) {
			GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), "IsServer");
			posY += yDiff;
		}
		
		if (GetComponent<NetworkIdentity>().isClient) {
			GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), "IsClient");
			posY += yDiff;
		}
		


		if (GetComponents<NetworkBehaviour>().Length > 0) {
		
			if (!m_ShowBehaviours) {
				if (GUI.Button(new Rect(pos.x+posX, Screen.height - pos.y + posY, 80, 18), "behaviours")) {
					m_ShowBehaviours = true;
				}
			}
			else
			{
				foreach (var beh in GetComponents<NetworkBehaviour>())
				{
					GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), "beh: " + beh.GetType().Name);
					posY += yDiff;
					foreach (FieldInfo field in beh.GetType ().GetFields())
					{
						System.Attribute[] markers = (System.Attribute[])field.GetCustomAttributes(typeof(SyncVarAttribute), true);
						if (markers.Length > 0)
						{
							GUI.Label(new Rect(pos.x+posX, Screen.height - pos.y + posY, 200, 20), "  Var " + field.Name + "=" + field.GetValue(beh));
							posY += yDiff;
						}
					}
				}
			}
		}
	}
	
	bool m_ShowBehaviours = false;
}

	
	