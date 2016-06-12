using System;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public partial class Other : NetworkBehaviour {
	
	public delegate void TakeDamageDelegate(int amount, float dir);

	[SyncEvent]
	public event TakeDamageDelegate EventTakeDamage;
		
	[SyncVar]
	public int otherValue = 0;
	
	public struct Foo
	{
		public string ouch;
	};





	//static bool one = true;


	//[SyncVar]
	Foo myFoo;
	
	

	/*
	static protected void InvokeRPCDoOnClient(UNetBehaviour obj, UReader reader)
	{
		((Other)obj).DoOnClient((System.Int32)reader.UReadUInt32());
	}
	
	static protected void InvokeEventTakeDamage(UNetBehaviour obj, UReader reader)
	{
		((Other)obj).TakeDamage((System.Int32)reader.UReadUInt32(), reader.ReadSingle());
	}
		
	public override bool UNetAwake()
	{
		RegisterRpcDelegate(this.GetType(), "Other:DoOnClient", InvokeRPCDoOnClient);
		RegisterEventDelegate(this.GetType(), "Other:TakeDamageDelegate", InvokeEventTakeDamage);
		return false;
	}
*/


	void Start()
	{
	}

	// Update is called once per frame
	void Update () {
		if (NetworkServer.active) {
			//OtherValue += 1;
		}
	}

	public struct Inner
	{
		public double innerOne;
		public string innerTwo;
	}
	
	public struct Custom
	{
		public int one;
		public float two;
		public Vector3 three;
		public Inner inner;
	}
	
	[Command]
	public void CmdDoMe(int val, Custom c, Transform me)
	{
		//Debug.Log("DoMe other value: " + val + " me:" + me);
		//Debug.Log("Custom :" + c.one + " " + c.two + " "  + c.three + " "  + c.inner);
		
		RpcDoOnClient((int)Time.time);
		
		//Debug.Log ("SDKHF");
		EventTakeDamage(102, 1.0f);
	}


	
	[ClientRpc]
	public void RpcDoOnClient(int foo)
	{
		//Debug.Log("DoOnClient " + foo);
	}

	static protected void InvokeSyncEventTakeDamageDelegateOld(NetworkBehaviour obj, NetworkReader reader)
	{
		if (!NetworkClient.active) {
			return;
		}
		
		if (((Other)obj).EventTakeDamage == null) {
			return;
		}
		((Other)obj).EventTakeDamage((System.Int32)reader.ReadPackedUInt32(), (System.Single)reader.ReadSingle());
	}
	
}
