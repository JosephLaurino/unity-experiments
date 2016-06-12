using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class BallPlayer : NetworkBehaviour {

	const int nudgeAmount = 33;
	
	public enum NudgeDir
	{
		Up,
		Down,
		Left,
		Right,
		Jump
	}

	void Start()
	{
	}
	
	[ClientCallback]
	void Update ()
	{
		if (!isLocalPlayer)
			return;
			
		if (Input.GetKey(KeyCode.Space))
		{
			CmdNudge(NudgeDir.Jump);
		}
		
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			CmdNudge(NudgeDir.Left);
		}
		
		if (Input.GetKey(KeyCode.RightArrow))
		{
			CmdNudge(NudgeDir.Right);
		}
		
		if (Input.GetKey(KeyCode.UpArrow))
		{
			CmdNudge(NudgeDir.Up);
		}
		
		if (Input.GetKey(KeyCode.DownArrow))
		{
			CmdNudge(NudgeDir.Down);
		}

/*
		if (Input.GetKey(KeyCode.Q))
		{
			GetComponent<UNetView>().EnablePhysicsTracker(true);
		}
		
		if (Input.GetKey(KeyCode.W))
		{
			GetComponent<UNetView>().EnablePhysicsTracker(false);
		}		
		*/
	}
	
	[Command]
	public void CmdNudge(NudgeDir direction)
	{
		switch (direction)
		{
			case NudgeDir.Left:
				GetComponent<Rigidbody>().AddForce(new Vector3(-nudgeAmount,0,0));
				break;
		
			case NudgeDir.Right:
				GetComponent<Rigidbody>().AddForce(new Vector3(nudgeAmount,0,0));
				break;

			case NudgeDir.Up:
				GetComponent<Rigidbody>().AddForce(new Vector3(0,0,nudgeAmount));
				break;
				
			case NudgeDir.Down:
				GetComponent<Rigidbody>().AddForce(new Vector3(0,0,-nudgeAmount));
				break;
				
			case NudgeDir.Jump:
				GetComponent<Rigidbody>().AddForce(new Vector3(0,nudgeAmount,0));
				break;
		}
	}
	
	/*
	void FixedUpdate()
	{
		Vector3 diff = transform.position - oldPos;
		if (diff.magnitude > 0.3f)
		{
			Debug.Log ("Snapped: " + diff.magnitude);
			Snap snap = new Snap();
			snap.snapPos1 = oldPos;
			snap.snapPos2 = transform.position;
			snap.timer = Time.time + 2.0f;
			
			Color col = Color.yellow;
			if (diff.magnitude > 0.4f)
			{
				col = new Color(1, 0.5f ,0);
			}
			if (diff.magnitude > 0.5f)
			{
				col = Color.red;
			}
			snap.col = col;
			
			snaps.Add(snap);
		}
		else
		{
			//snapPos = transform.position;
		}
		oldPos = transform.position;
	}
	*/
		
		/*
		CreateLineMaterial();
		
		lineMaterial.SetPass( 0 );
		GL.Begin( GL.LINES );
		foreach (var snap in snaps)
		{
			GL.Color(snap.col);		
			GL.Vertex3(snap.snapPos1.x, snap.snapPos1.y, snap.snapPos1.z);			
			GL.Vertex3(snap.snapPos2.x, snap.snapPos2.y, snap.snapPos2.z);	
		}
		GL.End();
	}
	
	static Material lineMaterial;
	
	static void CreateLineMaterial() 
	{
		if( !lineMaterial ) {
			lineMaterial = new Material( "Shader \"Lines/Colored Blended\" {" +
			                            "SubShader { Pass { " +
			                            "    Blend SrcAlpha OneMinusSrcAlpha " +
			                            "    ZWrite Off Cull Off Fog { Mode Off } " +
			                            "    BindChannels {" +
			                            "      Bind \"vertex\", vertex Bind \"color\", color }" +
			                            "} } }" );
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		}
	}*/
}
