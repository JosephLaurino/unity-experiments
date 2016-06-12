using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Stealth : NetworkBehaviour
{
	void Update ()
	{
		if (!isLocalPlayer)
			return;

		if (Input.GetKeyDown(KeyCode.Return))
		{
			CmdToggleStealth();
		}
	}

	[Command]
	void CmdToggleStealth()
	{
		bool hidden = GetComponent<NetworkProximityChecker>().forceHidden;
		GetComponent<NetworkProximityChecker>().forceHidden = !hidden;
	}
}
