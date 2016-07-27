using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.Networking.NetworkManager networkManager = GetComponent<UnityEngine.Networking.NetworkManager>();
        networkManager.StartClient();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
