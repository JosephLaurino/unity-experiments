using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//
/* 
public class PhysicsTracker3D : UNetBehaviour 
{
	Vector3		m_syncPosition;
	Vector3 	m_syncVelocity;
	float 		m_lastSyncTime;
	Rigidbody 	rb;
	
	public float snapThreshold = 5.0f;
	public GameObject visualizerPrefab;
	GameObject visualizer;
	
	public override void OnStartClient() 
	{
		CreateLineMaterial();
		if (visualizerPrefab != null)
		{
			visualizer = (GameObject)Instantiate(visualizerPrefab, transform.position, Quaternion.identity);
		}
		
		rb = GetComponent<Rigidbody>();
		GetComponent<UNetView>().RegisterNetworkTransformUpdate3D(HandleTransform);
	}
	
	public void HandleTransform(Vector3 newPosition, Vector3 newVelocity, Quaternion newRotation, Vector3 newAngularVelocity)
	{
		float dist = (transform.position - newPosition).magnitude;
		if (dist > snapThreshold)
		{
			transform.position = newPosition;
		}
			
		m_syncPosition = newPosition;
		m_syncVelocity = newVelocity;
		m_lastSyncTime = Time.time;
		
		rb.rotation = newRotation;
		rb.angularVelocity = newAngularVelocity;
	}
	
	[ClientCallback]
	public void FixedUpdate()
	{
		// dont run this on a local server.
		if (isServer)
			return;
				
		float sendInterval = 1.0f / GetComponent<UNetView>().sendRate;

		if (m_lastSyncTime < Time.time - sendInterval)
		{
			// sync data is more than one "sendInterval" old, ignore it
			rb.velocity = m_syncVelocity;
			return;
		}
		
		rb.velocity = (m_syncPosition - transform.position) * GetComponent<UNetView>().sendRate;
		
		// move sync position slightly in the position of velocity
		m_syncPosition += (m_syncVelocity * sendInterval * 0.1f);	
				
		if (visualizer != null)
			visualizer.transform.position = m_syncPosition;
	}
	
	void OnRenderObject()
	{
		if (m_syncPosition == Vector3.zero)
			return;
				
		lineMaterial.SetPass(0);
		GL.Begin(GL.LINES);
		GL.Color(Color.white);
		GL.Vertex3(transform.position.x, transform.position.y, transform.position.z);			
		GL.Vertex3(m_syncPosition.x, m_syncPosition.y, m_syncPosition.z);	
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
	}
	
}
*/
